using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BattleState { Start, ActionSelection, SpellSelection, PerformSpell, Busy}

public enum Stat{
    Constitution,
    Energie,
    Force,
    Defense,
    Critique,
    Erudition
}

public enum Status{
    None,
    Brulure,
    Poison,
    SpecialQueue
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] public Player player;
    [SerializeField] public EnemyBase enemy;
    [SerializeField] BattleHUD playerHud;
    [SerializeField] BattleHUD enemyHud;
    [SerializeField] BattleDialogBox dialogBox;
    public event Action<bool> OnBattleOver;

    bool isPlayerTurn;

    BattleState state;
    int currentAction;
    int currentSpell;

    public class StatusDetails
    {
        public Status status;
        public int turns;
        public Queue<int> dmg;

        public StatusDetails(Status status, int turns, int dmg)
        {
            this.status = status;
            this.turns = turns;
            this.dmg = new Queue<int>();
            if (dmg != 0)
            {
                this.dmg.Enqueue(dmg);
            }
        }
    }

    public Dictionary<Stat, int> Stats;
    public StatusDetails playerStatus;
    public StatusDetails enemyStatus;

    public class TypeChart
    {
        static float [][] chart =
        {   //                       Sque,Vamp,Pret
            /*Tran*/    new float[] { 1f , 1f ,0.5f},
            /*Glac*/    new float[] {0.5f,0.5f, 1f },
            /*Empo*/    new float[] {0.5f,0.5f, 1f },
            /*Enfl*/    new float[] {0.5f, 2f , 1f }
        };

        public static float GetEffectiveness(SpellType attackType, EnemyType defenseType)
        {
            if (attackType == SpellType.None || defenseType == EnemyType.None)
            {
                return 1;
            }
            int row = (int)attackType - 1;
            int col = (int)defenseType - 1;

            return chart[row][col];
        }
    }



    public void StartBattle() 
   {
        ResetStats();
        StartCoroutine(SetupBattle());
    }

   private void ResetStats()
   {
        player.Hp = player.MaxHp;
        player.Mana = player.MaxMana;
        enemy.Hp = enemy.MaxHp;
        enemy.Mana = enemy.MaxMana;

        CalculateStats();
        enemyStatus = new StatusDetails(Status.None, 0, 0);
        playerStatus = new StatusDetails(Status.None, 0, 0);

        playerHud.UnSetStatus();
        enemyHud.UnSetStatus();
   }

   private void CalculateStats()
   {
        Stats = new Dictionary<Stat, int>
        {
            { Stat.Constitution,    player.Constitution },
            { Stat.Energie,         player.Energie },
            { Stat.Force,           player.Force },
            { Stat.Defense,         player.Defense },
            { Stat.Critique,        player.Critique },
            { Stat.Erudition,       player.Erudition }
        };
   }

    // ------------------------ Gerer la selection et l'affichage des spells ------------------------

    public IEnumerator SetupBattle()
    {
        // Met à jour l'ui des barres du combats
        playerHud.SetPlayerData(player);
        enemyHud.SetEnemyData(enemy);

        // J'affiche les spells de mon arme
        dialogBox.SetSpellNames(player.EquippedWeapon.Spells);

        yield return dialogBox.TypeDialog(enemy.name + " vous attaque.");

        yield return ActionSelection();
    }

    IEnumerator ActionSelection()
    {
        dialogBox.EnableActionSelector(true);
        yield return dialogBox.TypeDialog("Que souhaitez vous faire ?");

        state = BattleState.ActionSelection;
    }

    void SpellSelection()
    {
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableSpellSelector(true);

        state = BattleState.SpellSelection;
    }

    IEnumerator PlayerSpell()
    {
        state = BattleState.PerformSpell;

        isPlayerTurn = true;
        var spell = player.EquippedWeapon.Spells[currentSpell];
        yield return dialogBox.TypeDialog($"{player.Pseudo} utilise {spell.Base.Name}.");

        var damageDetails = UseSpell(spell.Base);

        StartCoroutine(playerHud.UpdatePlayerMana(player));
        yield return enemyHud.UpdateEnemyHp(enemy);
        
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return EnemyFainted();
        }
        else
        {
            isPlayerTurn = false;
            yield return EnemySpell();
        }
    }

    public IEnumerator EnemySpell()
    {
        state = BattleState.PerformSpell;
        isPlayerTurn = false;

        var spell = enemy.GetRandomSpell();
        while (spell.ManaCost > enemy.Mana)
        {
            // Tant que le spell selectionné a un coup en mana trop elevé, on en choisis un autre
            spell = enemy.GetRandomSpell();
        }

        yield return dialogBox.TypeDialog($"{enemy.name} utilise {spell.Name}.");

        var damageDetails = UseSpell(spell);

         // Update des barres HP et mana
        StartCoroutine(enemyHud.UpdateEnemyMana(enemy));
        yield return playerHud.UpdatePlayerHp(player);
        
        yield return ShowDamageDetails(damageDetails); // Les détails du combat (critique etc)

        if (damageDetails.Fainted)
        {
            yield return PlayerFainted();
        }
        else 
        {
            yield return EndOfTurn();
        }
    }

    public IEnumerator EndOfTurn()
    {
        player.Mana +=5;
        enemy.Mana +=5;

        yield return CheckStatus();

        yield return new WaitForSeconds(1f);

        if (player.Mana > player.MaxMana)
            player.Mana = player.MaxMana;

        if (enemy.Mana > enemy.MaxMana)
            enemy.Mana = enemy.MaxMana;

        StartCoroutine(enemyHud.UpdateEnemyMana(enemy));
        yield return playerHud.UpdatePlayerMana(player);

        yield return ActionSelection();
    }

    public IEnumerator CheckStatus()
    {
        
        // Check les status du joueur 
        if (playerStatus.status == Status.Brulure && playerStatus.turns > 0 && playerStatus.dmg.Count >= 0)
        {
            --playerStatus.turns; // Un tour de moins
            yield return dialogBox.TypeDialog($"Vous etes sous l'effet de la brulure, il reste {playerStatus.turns} tours avant votre guerison.");

            player.Hp -= (int)playerStatus.dmg.Dequeue();
            if (player.Hp <= 0)
            {
                player.Hp = 0;
                yield return playerHud.UpdatePlayerHp(player);
                yield return PlayerFainted();
            }
            else
                yield return playerHud.UpdatePlayerHp(player);

            if (playerStatus.turns == 0)
            {
                yield return dialogBox.TypeDialog($"Vous n'etes plus sous l'effet de la brulure.");
                playerStatus.status = Status.None;
            }
        }
        else if (playerStatus.status == Status.Poison && playerStatus.turns > 0 && playerStatus.dmg.Count >= 0)
        {
            --playerStatus.turns; // Un tour de moins
            yield return dialogBox.TypeDialog($"Vous etes sous l'effet du poison, il reste {playerStatus.turns} tours avant votre guerison.");

            player.Hp -= (int)playerStatus.dmg.Dequeue();
            if (player.Hp <= 0)
            {
                player.Hp = 0;
                yield return playerHud.UpdatePlayerHp(player);
                yield return PlayerFainted();
            }
            else
                yield return playerHud.UpdatePlayerHp(player);

            if (playerStatus.turns == 0)
            {
                yield return dialogBox.TypeDialog($"Vous n'etes plus sous l'effet du poison.");
                playerStatus.status = Status.None;
                enemyHud.UnSetStatus();
            }
        }
        // Check les status de l'ennemie
        if (enemyStatus.status == Status.Brulure && enemyStatus.turns > 0 && enemyStatus.dmg.Count >= 0)
        {
            --enemyStatus.turns; // Un tour de moins
            yield return dialogBox.TypeDialog($"{enemy.name} est sous l'effet de la brulure, il reste {enemyStatus.turns} tours avant sa guerison.");
            Debug.Log("Queue count before Dequeue: " + enemyStatus.dmg.Count);
            enemy.Hp -= (int)enemyStatus.dmg.Dequeue();

            if (enemy.Hp <= 0)
            {
                enemy.Hp = 0;
                yield return enemyHud.UpdateEnemyHp(enemy);
                yield return EnemyFainted();
            }
            else
                yield return enemyHud.UpdateEnemyHp(enemy);

            if (enemyStatus.turns == 0)
            {
                yield return dialogBox.TypeDialog($"{enemy.name} n'est plus sous l'effet de la brulure.");
                enemyStatus.status = Status.None;
            }
        }
        else if (enemyStatus.status == Status.Poison && enemyStatus.turns > 0 && enemyStatus.dmg.Count >= 0)
        {
            --enemyStatus.turns; // Un tour de moins
            yield return dialogBox.TypeDialog($"{enemy.name} est sous l'effet du poison, il reste {enemyStatus.turns} tours avant sa guerison.");

            enemy.Hp -= (int)enemyStatus.dmg.Dequeue();

            if (enemy.Hp <= 0)
            {
                enemy.Hp = 0;
                yield return enemyHud.UpdateEnemyHp(enemy);
                yield return EnemyFainted();
            }
            else
                yield return enemyHud.UpdateEnemyHp(enemy);

            if (enemyStatus.turns == 0)
            {
                yield return dialogBox.TypeDialog($"{enemy.name} n'est plus sous l'effet du poison.");
                enemyStatus.status = Status.None;
            }
        }
    }

    private IEnumerator EnemyFainted()
    {
        yield return dialogBox.TypeDialog($"Vous avez vaincu {enemy.name}.");

        yield return new WaitForSeconds(2f);
        OnBattleOver(true);
    }
    private IEnumerator PlayerFainted()
    {
        yield return dialogBox.TypeDialog($"Vous avez ete vaincu par {enemy.name}.");

            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("C'est un coup critique !");
        }
        if (damageDetails.Effectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("C'est très efficace !");
        }
        else if (damageDetails.Effectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("Ce n'est pas très efficace...");
        }
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.SpellSelection)
        {
            StartCoroutine(HandleSpellSelection());
        }
    }

    void HandleActionSelection()
    {
        // Je séléctionne Attaquer (pour le moment seulement cette option existe)
        currentAction = 0;
        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyUp(KeyCode.E))
        {
            if (currentAction == 0)
            {
                // Attaquer
                SpellSelection();
            }
        }
    }

    IEnumerator HandleSpellSelection()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (currentSpell < player.EquippedWeapon.Spells.Count - 1)
            {
                ++currentSpell;
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (currentSpell > 0)
            {
                --currentSpell;
            }
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            if (currentSpell < player.EquippedWeapon.Spells.Count - 3)
            {
                currentSpell +=3;
            }
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (currentSpell > 1)
            {
                currentSpell -= 2;
            }
        }

        dialogBox.UpdateSpellSelection(currentSpell);
        
        if (Input.GetKeyUp(KeyCode.E))
        {
            dialogBox.EnableSpellSelector(false);
            dialogBox.EnableDialogText(true);
            if (player.Mana - player.EquippedWeapon.Spells[currentSpell].Base.ManaCost < 0)
            {
                state = BattleState.Busy;
                yield return dialogBox.TypeDialog($"Vous n'avez pas assez de mana pour utiliser {player.EquippedWeapon.Spells[currentSpell].Base.Name}");

                dialogBox.EnableDialogText(false);
                dialogBox.EnableSpellSelector(true);

                state = BattleState.SpellSelection;
            }
            else
            {
                yield return StartCoroutine(PlayerSpell());
            }
            
            
        }
    }

    public class DamageDetails
    {
        public bool Fainted { get; set; }
        public float Critical { get; set; }
        public float Effectiveness { get; set; }
    }

    // Fonctions pour le calcul des critiques
    private float PlayerCritical()
    {
        int r = UnityEngine.Random.Range(0, 100);
        int criticalChance = Stats[Stat.Critique] * 2;
        if (r < criticalChance)
        {
            return 1.5f;
        }
        else
        {
            return 1f;
        }
    }
    private float EnemyCritical()
    {
        int r = UnityEngine.Random.Range(0, 100);
        int criticalChance = enemy.Critique * 2;
        if (r < criticalChance)
        {
            return 1.5f;
        }
        else
        {
            return 1f;
        }
    }

    // ----------------------------------- Fonctions de spell -----------------------------------

    public DamageDetails UseSpell(SpellBase spell)
    {
        float effectiveness = TypeChart.GetEffectiveness(spell.SpellType, enemy.type);
        if (spell.Name == "Tourbillon")
            return Tourbillon(isPlayerTurn, effectiveness);

        else if (spell.Name == "Taillade")
            return Taillade(isPlayerTurn, effectiveness);

        else if (spell.Name == "Renforcement")
            return Renforcement(isPlayerTurn, effectiveness);
        
        else if (spell.Name == "Boule de feu")
            return BouleDeFeu(isPlayerTurn, effectiveness);

        else if (spell.Name == "Lame vive")
            return LameVive(isPlayerTurn, effectiveness);

        else if (spell.Name == "Estocade")
            return Estocade(isPlayerTurn, effectiveness);

        else if (spell.Name == "Lame ardente")
            return LameArdente(isPlayerTurn, effectiveness);
        else
        {
            return null;
        }
    }

//  ---------------------------- Epee ----------------------------
    public DamageDetails Tourbillon(bool isPlayerTurn, float effectiveness)
    {
        var damageDetails = new DamageDetails();

        if (isPlayerTurn)
        {
            // Si c'est le joueur qui lance l'attaque

            float critical = 1f; // Ne scale pas sur le crit
            float modifiers = effectiveness * critical;

            int damage = (int) (modifiers * (Stats[Stat.Constitution] + Stats[Stat.Force] * 2));
            int damageTaken = (int)(damage * (1f - (float)enemy.Defense/100));

            Debug.Log($"Constitution joueur : {Stats[Stat.Constitution]}, Force joueur : {Stats[Stat.Force]}, enemy defense : {enemy.Defense}");
            Debug.Log($"Effectiveness : {effectiveness}, modifiers : {modifiers}");
            Debug.Log($"damage : {damage}, damageTaken : {damageTaken}");

            damageDetails = new DamageDetails()
            {
                Effectiveness = effectiveness,
                Critical = critical,
                Fainted = false,
            };

            enemy.Hp -= damageTaken; // L'ennemie perd des Hp en fonction des dégats
            player.Mana -= 10;

            if (player.Mana <=0)
            {
                player.Mana = 0;
            }
            // Si l'enemi est mort
            if (enemy.Hp <= 0)
            {
                // Si l'enemi est vaincu
                enemy.Hp = 0;
                damageDetails.Fainted = true;
            }
            return damageDetails;
        }
        else
        {
            // Si c'est l'ennemie qui lance l'attaque

            effectiveness = 1f;
            float critical = 1f; // Ne scale pas sur le crit

            float modifiers = effectiveness * critical;
            int damage = (int) (modifiers * (enemy.Constitution + enemy.Force * 2));
            int damageTaken = (int)(damage * (1f - (float) Stats[Stat.Defense]/100));
            Debug.Log($"Constitution enemy : {enemy.Constitution}, Force enemy : {enemy.Force},  defense player : {Stats[Stat.Defense]}");
            Debug.Log($"modifiers : {modifiers}");
            Debug.Log($"damage : {damage}, damageTaken : {damageTaken}");

            damageDetails = new DamageDetails()
            {
                Effectiveness = effectiveness,
                Critical = critical,
                Fainted = false,        
            };

            enemy.Mana -= 10; // On met à jour son mana
            player.Hp -= damageTaken;
            if (player.Hp <= 0)
            {
                player.Hp = 0;
                damageDetails.Fainted = true;
            }
            return damageDetails;
        }
    }

    public DamageDetails Taillade(bool isPlayerTurn, float effectiveness)
    {
        var damageDetails = new DamageDetails();

        if (isPlayerTurn)
        {
            // Si c'est le joueur qui lance l'attaque

            float critical = PlayerCritical();
            float modifiers = effectiveness * critical;

            int damage = (int) (modifiers * Stats[Stat.Force] * 2);
            int damageTaken = (int)(damage * (1f - (float)enemy.Defense/100));

            Debug.Log($"Force joueur : {Stats[Stat.Force]}, critique joueur : {Stats[Stat.Critique]}, enemy defense : {enemy.Defense}");
            Debug.Log($"critical : {critical}, effectiveness : {effectiveness}, modifiers : {modifiers}");
            Debug.Log($"damage : {damage}, damageTaken : {damageTaken}");

            damageDetails = new DamageDetails()
            {
                Effectiveness = effectiveness,
                Critical = critical,
                Fainted = false,
            };

            enemy.Hp -= damageTaken; // L'ennemie perd des Hp en fonction des dégats
            player.Mana -= 5;

            if (player.Mana <=0)
            {
                player.Mana = 0;
            }
            // Si l'enemi est mort
            if (enemy.Hp <= 0)
            {
                // Si l'enemi est vaincu
                enemy.Hp = 0;
                damageDetails.Fainted = true;
            }
            return damageDetails;
        }
        else
        {
            // Si c'est l'ennemie qui lance l'attaque

            effectiveness = 1f;
            float critical = EnemyCritical();
            float modifiers = effectiveness * critical;

            int damage = (int) (modifiers * enemy.Force * 2);
            int damageTaken = (int)(damage * (1f - (float)Stats[Stat.Defense]/100));
            Debug.Log($"Force enemy : {enemy.Force}, defense player : {Stats[Stat.Defense]}");
            Debug.Log($"critical : {critical}, modifiers : {modifiers}");
            Debug.Log($"damage : {damage}, damageTaken : {damageTaken}");

            damageDetails = new DamageDetails()
            {
                Effectiveness = effectiveness,
                Critical = critical,
                Fainted = false,        
            };

            enemy.Mana -= 5; // On met à jour son mana
            player.Hp -= damageTaken;
            if (player.Hp <= 0)
            {
                player.Hp = 0;
                damageDetails.Fainted = true;
            }
            return damageDetails;
        }
    }

    public DamageDetails Renforcement(bool isPlayerTurn, float effectiveness)
    {
        var damageDetails = new DamageDetails();

        if (isPlayerTurn)
        {
            // Si c'est le joueur qui lance l'attaque
            Stats[Stat.Defense] += 2;
            Stats[Stat.Force] += 2;

            damageDetails = new DamageDetails()
            {
                Effectiveness = effectiveness,
                Critical = 1f,
                Fainted = false,
            };

            return damageDetails;
        }

        else
        {
            // Si c'est l'ennemie qui lance l'attaque
            enemy.Defense += 2;
            enemy.Force += 2;

            damageDetails = new DamageDetails()
            {
                Effectiveness = effectiveness,
                Critical = 1f,
                Fainted = false,
            };
            return damageDetails;
        }
    }

    public DamageDetails LameVive(bool isPlayerTurn, float effectiveness)
    {
        var damageDetails = new DamageDetails();

        if (isPlayerTurn)
        {
            // Si c'est le joueur qui lance l'attaque

            float critical = PlayerCritical();
            float modifiers = effectiveness * critical;

            int damage = (int) (modifiers * Stats[Stat.Force] * 3);
            int damageTaken = (int)(damage * (1f - (float)enemy.Defense/100));

            damageDetails = new DamageDetails()
            {
                Effectiveness = effectiveness,
                Critical = critical,
                Fainted = false,
            };

            enemy.Hp -= damageTaken; // L'ennemie perd des Hp en fonction des dégats
            player.Mana -= 20;

            if (player.Mana <=0)
            {
                player.Mana = 0;
            }
            // Si l'enemi est mort
            if (enemy.Hp <= 0)
            {
                // Si l'enemi est vaincu
                enemy.Hp = 0;
                damageDetails.Fainted = true;
            }
            return damageDetails;
        }
        else
        {
            // Si c'est l'ennemie qui lance l'attaque

            effectiveness = 1f;
            float critical = EnemyCritical();
            float modifiers = effectiveness * critical;

            int damage = (int) (modifiers * enemy.Force * 3);
            int damageTaken = (int)(damage * (1f - (float)Stats[Stat.Defense]/100));
            
            damageDetails = new DamageDetails()
            {
                Effectiveness = effectiveness,
                Critical = critical,
                Fainted = false,        
            };

            enemy.Mana -= 20; // On met à jour son mana
            player.Hp -= damageTaken; // On enlève ses pv
            if (player.Hp <= 0)
            {
                player.Hp = 0;
                damageDetails.Fainted = true;
            }
            return damageDetails;
        }
    }

    public DamageDetails Estocade(bool isPlayerTurn, float effectiveness)
    {
        var damageDetails = new DamageDetails();

        if (isPlayerTurn)
        {
            // Si c'est le joueur qui lance l'attaque

            int missingHealth = enemy.MaxHp - enemy.Hp;
            Debug.Log($"missing health : {missingHealth}");
            float pourcentage = (float)Stats[Stat.Force]/100;
            Debug.Log($"Player Turn: Percentage = {pourcentage}");

            int damage = (int)(pourcentage * missingHealth);
            int damageTaken = (int)(damage * (1f - (float)enemy.Defense/100));

            Debug.Log($"damage : {damage}, damageTaken : {damageTaken}");

            damageDetails = new DamageDetails()
            {
                Effectiveness = 1f,
                Critical = 1f,
                Fainted = false,
            };

            enemy.Hp -= damageTaken; // L'ennemie perd des Hp en fonction des dégats
            player.Mana -= 25;

            if (player.Mana <=0)
            {
                player.Mana = 0;
            }
            // Si l'enemi est mort
            if (enemy.Hp <= 0)
            {
                // Si l'enemi est vaincu
                enemy.Hp = 0;
                damageDetails.Fainted = true;
            }
            return damageDetails;
        }
        else
        {
            // Si c'est l'ennemie qui lance l'attaque

            int missingHealth = player.MaxHp - player.Hp;
            float pourcentage = (float)Stats[Stat.Force]/100;

            int damage = (int)(pourcentage * missingHealth);
            int damageTaken = (int)(damage * (1f - (float)Stats[Stat.Defense]/100));

            damageDetails = new DamageDetails()
            {
                Effectiveness = 1f,
                Critical = 1f,
                Fainted = false,        
            };

            enemy.Mana -= 25; // On met à jour son mana
            player.Hp -= damageTaken;

            if (player.Hp <= 0)
            {
                player.Hp = 0;
                damageDetails.Fainted = true;
            }
            return damageDetails;
        }
    }

    public DamageDetails LameArdente(bool isPlayerTurn, float effectiveness)
    {
        var damageDetails = new DamageDetails();

        if (isPlayerTurn)
        {
            float modifiers = effectiveness;

            int damage = (int) (modifiers * Stats[Stat.Force] * 3);
            int damageTaken = (int)(damage * (1f - (float)enemy.Defense/100));

            damageDetails = new DamageDetails()
            {
                Effectiveness = effectiveness,
                Critical = 1f,
                Fainted = false,
            };

            enemy.Hp -= damageTaken;
            player.Mana -= 40;

            if (player.Mana <=0)
            {
                player.Mana = 0;
            }
            // Si l'enemi est mort
            if (enemy.Hp <= 0)
            {
                // Si l'enemi est vaincu
                enemy.Hp = 0;
                damageDetails.Fainted = true;
            }

            if (enemyStatus.status != Status.Brulure)
            {
                enemyStatus = new StatusDetails(Status.Brulure, 2, Stats[Stat.Force]);
                enemyStatus.dmg.Enqueue(Stats[Stat.Force]);
                enemyHud.SetStatus(Status.Brulure);
            }
            else 
            {
                enemyStatus.turns += 2;
                enemyStatus.dmg.Enqueue(Stats[Stat.Force]);
                enemyStatus.dmg.Enqueue(Stats[Stat.Force]);
            }
            return damageDetails;
        }
        else
        {
            effectiveness = 1f;

            int damage = (int) (enemy.Force * 3);
            int damageTaken = (int)(damage * (1f - (float)Stats[Stat.Defense]/100));

            damageDetails = new DamageDetails()
            {
                Effectiveness = effectiveness,
                Critical = 1f,
                Fainted = false,
            };

            player.Hp -= damageTaken;
            enemy.Mana -= 40;

            if (player.Hp <= 0)
            {
                player.Hp = 0;
                damageDetails.Fainted = true;
            }
            if (player.Mana <=0)
            {
                player.Mana = 0;
            }

            if (playerStatus.status != Status.Brulure)
            {
                playerStatus = new StatusDetails(Status.Brulure, 2, enemy.Force);
                playerStatus.dmg.Enqueue(enemy.Force);
                playerHud.SetStatus(Status.Brulure);
            }
            else
            {
                playerStatus.turns += 2;
                playerStatus.dmg.Enqueue(enemy.Force);
                playerStatus.dmg.Enqueue(enemy.Force);
            }

            return damageDetails;
        }
    }


//  ---------------------------- Sceptre ----------------------------
    public DamageDetails BouleDeFeu(bool isPlayerTurn, float effectiveness)
    {
        var damageDetails = new DamageDetails();

        if (isPlayerTurn)
        {
            // Si c'est le joueur qui lance l'attaque
            
            float critical = PlayerCritical();
            float modifiers = effectiveness * critical;

            int damage = (int) (modifiers * Stats[Stat.Erudition] * 2);
            int damageTaken = (int)(damage * (1f - (float)enemy.Defense/100));

            damageDetails = new DamageDetails()
            {
                Effectiveness = effectiveness,
                Critical = critical,
                Fainted = false,
            };

            enemy.Hp -= damageTaken; // L'ennemie perd des Hp en fonction des dégats
            player.Mana -= 10;

            if (player.Mana <=0)
            {
                player.Mana = 0;
            }
            // Si l'enemi est mort
            if (enemy.Hp <= 0)
            {
                // Si l'enemi est vaincu
                enemy.Hp = 0;
                damageDetails.Fainted = true;
            }
            if (enemyStatus.status != Status.Brulure)
            {
                enemyStatus = new StatusDetails(Status.Brulure, 3, Stats[Stat.Erudition]);
                enemyStatus.dmg.Enqueue(Stats[Stat.Erudition]);
                enemyStatus.dmg.Enqueue(Stats[Stat.Erudition]);
                enemyHud.SetStatus(Status.Brulure);
            }
            else
            {
                enemyStatus.turns += 3;
                enemyStatus.dmg.Enqueue(Stats[Stat.Erudition]);
                enemyStatus.dmg.Enqueue(Stats[Stat.Erudition]);
                enemyStatus.dmg.Enqueue(Stats[Stat.Erudition]);
            }
            
            return damageDetails;
        }

        else
        {
            // Si c'est l'ennemie qui lance l'attaque
            effectiveness = 1f;
            float critical = EnemyCritical();
            float modifiers = critical * effectiveness;

            int damage = (int) (modifiers * enemy.Erudition * 2);
            int damageTaken = (int)(damage * (1f - (float)Stats[Stat.Defense]/100));

            damageDetails = new DamageDetails()
            {
                Effectiveness = effectiveness,
                Critical = 1f,
                Fainted = false,
            };

            player.Hp -= damageTaken;
            player.Mana -= 15;

            if (enemy.Mana <=0)
            {
                enemy.Mana = 0;
            }
            // Si l'enemi est mort
            if (player.Hp <= 0)
            {
                // Si l'enemi est vaincu
                player.Hp = 0;
                damageDetails.Fainted = true;
            }

            if (playerStatus.status != Status.Brulure)
            {
                playerStatus = new StatusDetails(Status.Brulure, 3, enemy.Erudition);
                playerStatus.dmg.Enqueue(enemy.Erudition);
                playerStatus.dmg.Enqueue(enemy.Erudition);
                playerHud.SetStatus(Status.Brulure);
            }
            else
            {
                playerStatus.turns += 3;
                playerStatus.dmg.Enqueue(enemy.Erudition);
                playerStatus.dmg.Enqueue(enemy.Erudition);
                playerStatus.dmg.Enqueue(enemy.Erudition);
            }
            return damageDetails;
        }
    }
}


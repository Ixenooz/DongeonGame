using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerSpell, EnemySpell, Busy}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    [SerializeField] BattleHUD playerHud;
    [SerializeField] BattleHUD enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    bool isPlayerTurn;

    BattleState state;
    int currentAction;
    int currentSpell;

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




    private void Start() 
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
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction()
    {

        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Que souhaitez vous faire ?"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerSpell()
    {
        state = BattleState.PlayerSpell;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableSpellSelector(true);
    }

    IEnumerator PerformPlayerSpell()
    {
        state = BattleState.Busy;

        isPlayerTurn = true;
        var spell = player.EquippedWeapon.Spells[currentSpell];
        yield return dialogBox.TypeDialog($"{player.Pseudo} utilise {spell.Base.Name}.");

        yield return new WaitForSeconds(1f);

        var damageDetails = UseSpell(spell.Base);

        StartCoroutine(playerHud.UpdatePlayerMana(player));
        yield return enemyHud.UpdateEnemyHp(enemy);
        
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"Vous avez vaincu {enemy.name}.");
        }
        else
        {
            isPlayerTurn = false;
            yield return EnemySpell();
        }
    }

    public IEnumerator EnemySpell()
    {
        state = BattleState.EnemySpell;
        isPlayerTurn = false;

        var spell = enemy.GetRandomSpell();
        while (spell.ManaCost > enemy.Mana)
        {
            // Tant que le spell selectionné a un coup en mana trop elevé, on en choisis un autre
            spell = enemy.GetRandomSpell();
        }

        yield return dialogBox.TypeDialog($"{enemy.name} utilise {spell.Name}");
        yield return new WaitForSeconds(1f);

        var damageDetails = UseSpell(spell);

         // Update des barres HP et mana
        StartCoroutine(enemyHud.UpdateEnemyMana(enemy));
        yield return playerHud.UpdatePlayerHp(player);
        
        yield return ShowDamageDetails(damageDetails); // Les détails du combat (critique etc)

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"Vous avez ete vaincu par {enemy.name}.");
        }
        else 
        {
            PlayerAction();
        }
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

    private void Update() 
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerSpell)
        {
            HandleSpellSelection();
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
                PlayerSpell();
            }
        }
    }

    void HandleSpellSelection()
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

            StartCoroutine(PerformPlayerSpell());
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
        int r = Random.Range(0, 100);
        int criticalChance = player.Critique * 2;
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
        int r = Random.Range(0, 100);
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
        {
            return Tourbillon(isPlayerTurn, effectiveness);
        }
        else
        {
            return null;
        }
    }

    public DamageDetails Tourbillon(bool isPlayerTurn, float effectiveness)
    {
        var damageDetails = new DamageDetails();

        if (isPlayerTurn)
        {
            // Si c'est le joueur qui lance l'attaque

            float critical = PlayerCritical();
            float modifiers = effectiveness * critical;
            int damage = (int) (modifiers * player.Force * 2);
            int damageTaken = (int)(damage * (1f - (float)enemy.Defense/100));

            Debug.Log($"Force joueur : {player.Force}, critique joueur : {player.Critique}, enemy defense : {enemy.Defense}");
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
            int damageTaken = (int)(damage * (1f - (float)player.Defense/100));
            Debug.Log($"Force enemy : {enemy.Force}, defense player : {player.Defense}");
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
}


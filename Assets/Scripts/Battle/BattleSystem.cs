using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerSpell, EnemyMove, Busy}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    [SerializeField] BattleHUD playerHud;
    [SerializeField] BattleHUD enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    BattleState state;
    int currentAction;

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

    public IEnumerator SetupBattle()
    {
        // Met à jour l'ui des barres du combats
        playerHud.SetPlayerData(player);
        enemyHud.SetEnemyData(enemy);

        yield return dialogBox.TypeDialog(enemy.name + " vous attaque.");
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choisissez une action"));
        dialogBox.EnableActionSelector(true);
    }

    private void Update() 
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
    }

    void HandleActionSelection()
    {
        
    }

}


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Inventory }

public class GameController : MonoBehaviour
{
    [SerializeField] HeroCharacterScript playerController;
    [SerializeField] HeroCharacterCollisions playerCollisions;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] Canvas UICanvas;
    [SerializeField] Inventory inventory;
    private Enemy currentEnemy;

    GameState state;
    GameState prevState;

    private void Start() 
    {
        playerCollisions.OnBattleStart += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        inventory.OnOpenInventory += OpenInventory;
        inventory.OnCloseInventory += CloseInventory;

        DialogManager.Instance.OnShowDialog += () => // Lorsqu'un dialogue est actif
        {
            prevState = state;
            state = GameState.Dialog;
        };
        DialogManager.Instance.OnCloseDialog += () => // Lorsqu'un dialogue n'est plus actif
        {
            if(state == GameState.Dialog)
            {
                state = prevState;
            }
        };
    }

    void StartBattle()
    {
        state = GameState.Battle;

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        UICanvas.enabled = false;
        
        if (playerCollisions.otherObj != null)
        {
            currentEnemy = playerCollisions.otherObj.GetComponent<Enemy>();
            battleSystem.enemy = currentEnemy.enemyBase;
            playerController.animator.SetInteger("dir", 0);
        }

        battleSystem.StartBattle();

    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;

        if (won)
        {
            currentEnemy.gameObject.SetActive(false);
            battleSystem.gameObject.SetActive(false);
            worldCamera.gameObject.SetActive(true);
            UICanvas.enabled = true;
        }
        else
        {
            Debug.Log("Vous avez perdu la partie");
        }
    }

    void OpenInventory()
    {
        if (state == GameState.FreeRoam)
        {  
            state = GameState.Inventory;
        }
    }

    void CloseInventory()
    {
        if (state == GameState.Inventory)
        {
            state = GameState.FreeRoam;
        }
    }

    private void Update() 
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
            playerCollisions.HandleUpdate();
            inventory.HandleUpdateOpen();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if (state == GameState.Inventory)
        {
            inventory.HandleUpdate();
        }
    }
}

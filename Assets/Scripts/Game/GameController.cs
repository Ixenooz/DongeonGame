using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Inventory, Augments }

public class GameController : MonoBehaviour
{
    [SerializeField] HeroCharacterScript playerController;
    [SerializeField] HeroCharacterCollisions playerCollisions;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] Canvas UICanvas;
    [SerializeField] Inventory inventory;
    [SerializeField] AugmentManager augmentManager;
    private Enemy currentEnemy;

    GameState state;
    GameState prevState;

    private void Start() 
    {
        playerCollisions.OnBattleStart += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        inventory.OnOpenInventory += OpenInventory;
        inventory.OnCloseInventory += CloseInventory;

        augmentManager.OnCloseAugment += CloseAugment;

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
    void EndBattle(bool won, bool isBoss)
    {
        if (won)
        {
            currentEnemy.gameObject.SetActive(false);
            battleSystem.gameObject.SetActive(false);
            worldCamera.gameObject.SetActive(true);
            UICanvas.enabled = true;
            if (!isBoss)
            {
                augmentManager.isBossAugment = false;
                StartCoroutine(OpenStatAugment());
            }
            else
            {
                augmentManager.isBossAugment = true;
                StartCoroutine(OpenBossAugment());
            }
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
    IEnumerator OpenStatAugment()
    {
        if (state != GameState.Augments)
        {
            yield return new WaitForSeconds(2);

            augmentManager.AugmentsObj.SetActive(true);
            augmentManager.GetRandomStatAugments(3);
            state = GameState.Augments;
        }
    }
    IEnumerator OpenBossAugment()
    {
        if (state != GameState.Augments)
        {
            yield return new WaitForSeconds(2);

            augmentManager.AugmentsObj.SetActive(true);
            augmentManager.isBossAugment = true;
            augmentManager.GetRandomBossAugment(3);
            state = GameState.Augments;
        }
    }

    void CloseAugment()
    {
        if (state == GameState.Augments)
        {
            augmentManager.AugmentsObj.SetActive(false);
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
        else if (state == GameState.Augments)
        {
            augmentManager.HandleUpdate();
        }
    }
}

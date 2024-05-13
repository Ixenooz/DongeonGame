using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog }

public class GameController : MonoBehaviour
{
    [SerializeField] HeroCharacterScript playerController;
    [SerializeField] HeroCharacterCollisions playerCollisions;

    GameState state;
    GameState prevState;

    private void Start() 
    {
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


    private void Update() {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
            playerCollisions.HandleUpdate();

        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChestBehavior : MonoBehaviour, Interactable
{
    
    [SerializeField] public Dialog dialog;
    public int chestGold;
    public GameObject ui;
    public GameObject coin;
    public bool isLooted = false;
    public Animator chestAnimator;
    public Animator goldAnimator;


    // GameObject du joueur
    private GameObject player;

    private void Start() 
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Je cherche l'objet avec le tag Player
    }

    public void ResetChest()
    {
        isLooted = false;
    }

    public IEnumerator Interact()
    {
        Debug.Log("Interacting with chest");
        if(isLooted == false)
        {   // Si le coffre n'a pas été ouvert
            if (chestGold > 0)
            {
                Player player = this.player.gameObject.GetComponent<Player>();

                //Je joue l'animation du coffre qui s'ouvre et j'affiche le coin 1s après
                chestAnimator.SetBool("isOpened", true);

                yield return new WaitForSeconds(1);

                player.currentGold += chestGold; // J'ajoute à nos golds actuels les golds du coffre
                coin.SetActive(true);
                goldAnimator.SetBool("isOpened", true);
                yield return DialogManager.Instance.ShowDialog(dialog);

                // Je cache le coin lorsque le dialogue est terminé  
                coin.SetActive(false);       
            }
            isLooted = true;
        }
        else
        {
            dialog = new Dialog(new List<string>() { "Vous avez deja ouvert ce coffre."});
            yield return DialogManager.Instance.ShowDialog(dialog);
        }
        
    }
}

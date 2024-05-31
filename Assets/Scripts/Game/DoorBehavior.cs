using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehavior : MonoBehaviour, Interactable
{
    [SerializeField] public Dialog dialog;

    public int doorNumber;
    public int doorCost;
    public bool isOpen = false;
    GameObject player;
    
    private void Start() 
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Je cherche l'objet avec le tag Player 
    }
    private void Update() 
    {

    }
    public IEnumerator Interact()
    {
        int selectedChoice = 0;
        Player player = this.player.GetComponent<Player>();
        if (player.currentGold >= doorCost)
        {
            dialog = new Dialog(new List<string>() { "Il vous faut utiliser " + doorCost.ToString() + " d'or pour ouvrir cette porte.", "Souhaitez vous l'ouvrir ?"});
            yield return DialogManager.Instance.ShowDialog(dialog, new List<string>() { "Oui", "Non" }, 
            (choiceIndex) => selectedChoice = choiceIndex );

            if (selectedChoice == 0)
            {
                // Oui
                player.currentGold -= doorCost;
                gameObject.SetActive(false);
                
            }
            else if (selectedChoice == 1)
            {
                // Non
                Debug.Log("No");
            }
        }
        else 
        {
            dialog = new Dialog(new List<string>() { "Il vous faut " + doorCost.ToString() + " d'or pour ouvrir cette porte.", "Vous n'avez pas assez d'or."});
            yield return DialogManager.Instance.ShowDialog(dialog);
        }
    }
}

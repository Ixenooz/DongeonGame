using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroCharacterCollisions : MonoBehaviour
{
    
    public Collider2D otherObj;
    
    public event Action OnBattleStart;

    void OnTriggerEnter2D(Collider2D other) //Lorsque le joueur rentre en collision avec un collider "other" isTrigger
    {

        if(other.gameObject.tag == "chest") // Si l'objet a le tag "chest"
        {
            otherObj = other;
            ChestBehavior cb = other.gameObject.GetComponent<ChestBehavior>(); //Récupération du script ChestBehavior
            cb.ui.SetActive(true); // On affiche l'ui pour ouvrir le coffre
        }
        if(other.gameObject.tag == "door")
        {
            otherObj = other;
        }
        if (other.gameObject.tag == "enemy")
        {
            otherObj = other;
            OnBattleStart.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag == "chest") // Si l'objet a le tag "chest"
        {
            ChestBehavior cb = other.gameObject.GetComponent<ChestBehavior>(); //Récupération du script ChestBehavior
            cb.ui.SetActive(false); // On cache l'ui pour ouvrir le coffre si on sort de range
            cb.coin.gameObject.SetActive(false); // Je cache le coin
            otherObj = null;
        }
        if (other.gameObject.tag == "door")
        {
            otherObj = null;
        }
    }

    public void HandleUpdate() 
    {
        if(Input.GetKeyUp(KeyCode.E))
        {
            StartCoroutine(Interact());
        }
    }

    IEnumerator Interact()
    {
        if (otherObj != null)
        {
            yield return otherObj.GetComponent<Interactable>()?.Interact();
        }
    }
}

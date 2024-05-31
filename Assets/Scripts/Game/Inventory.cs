using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameController gameController;
    [SerializeField] Player player;
    [SerializeField] TMP_Text Constitution;
    [SerializeField] TMP_Text Energie;
    [SerializeField] TMP_Text Force;
    [SerializeField] TMP_Text Defense;
    [SerializeField] TMP_Text Critique;
    [SerializeField] TMP_Text Erudition;
    [SerializeField] Image Weapon;
    public event Action OnOpenInventory;
    public event Action OnCloseInventory;

    public void UpdateInventory()
    {
        Constitution.SetText(player.Constitution.ToString());
        Energie.SetText(player.Energie.ToString());
        Force.SetText(player.Force.ToString());
        Defense.SetText(player.Defense.ToString());
        Critique.SetText(player.Critique.ToString());
        Erudition.SetText(player.Erudition.ToString());
    }

    private void CloseInventory()
    {
        OnCloseInventory.Invoke();
        gameObject.SetActive(false);
    }

    private void OpenInventory()
    {
        OnOpenInventory.Invoke();
        UpdateInventory();
        gameObject.SetActive(true);
    }

    public void HandleUpdateOpen()
    {
        // Si j'appuie sur I et si l'inventaire n'est pas affiché
        if (Input.GetKeyUp(KeyCode.I) && gameObject.activeSelf == false)
        {
            OpenInventory();
        }
    }
    
    public void HandleUpdate()
    {
        // Si j'appuie sur I et si l'inventaire est affiché
        if (Input.GetKeyUp(KeyCode.I) && gameObject.activeSelf)
        {
            CloseInventory();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerBase playerBase;
    public Weapon EquippedWeapon; //Arme actuellement équipée
    
    // Pseudo du joueur
    public string Pseudo = "Nopseudo";

    public int currentGold;
    public TMP_Text goldUI;

    // Valeur qui changera pendant un combat
    public int Hp;
    public int Mana;
    
    // Calcul des statistiques du personnage (baseStat + weaponStat)
    public int Constitution {
        get {return EquippedWeapon.Base.Constitution + playerBase.baseConstitution;}
    }
    public int Energie {
        get {return EquippedWeapon.Base.Energie + playerBase.baseEnergie;}
    }
    public int Force {
        get {return EquippedWeapon.Base.Force + playerBase.baseForce;}
    }
    public int Defense {
        get {return EquippedWeapon.Base.Defense + playerBase.baseDefense;}
    }
    public int Critique {
        get {return EquippedWeapon.Base.Critique + playerBase.baseCritique;}
    }
    public int Erudition {
        get {return EquippedWeapon.Base.Erudition + playerBase.baseErudition;}
    }

    // Valeur qui évoluera dans la partie en fonction des stats
    public int MaxHp {
        get { return Constitution * 10; }
    }
    public int MaxMana {
        get { return Energie * 5; }
    }

    private void Awake() 
    {
        playerBase.baseConstitution = 5;
        playerBase.baseEnergie = 5;
        playerBase.baseForce = 5;
        playerBase.baseDefense = 5;
        playerBase.baseCritique = 5;
        playerBase.baseErudition = 5;
    }
    
    private void Update()
    {
        goldUI.SetText(currentGold.ToString());
    }
    
}

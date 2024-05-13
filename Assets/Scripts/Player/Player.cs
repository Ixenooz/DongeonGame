using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerBase playerBase;
    public Weapon EquippedWeapon; //Arme actuellement équipée
    
    // Pseudo du joueur
    public string Pseudo = "Nopseudo";

    // Valeur qui changera pendant un combat
    public int Hp {get; set; }
    public int Mana {get; set; }
    
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
        get { return playerBase.baseConstitution * 5; }
    }
    public int MaxMana {
        get { return playerBase.baseEnergie * 5; }
    }
    
}

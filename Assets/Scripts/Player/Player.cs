using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerBase playerBase;

    public WeaponBase EquippedWeapon; //Arme actuellement équipée
    // Pseudo du joueur
    public string Pseudo = "Nopseudo";

    // Valeur qui changera pendant un combat
    public int Hp {get; set; }
    public int Mana {get; set; }
    
    // Calcul des statistiques du personnage (baseStat + weaponStat)
    public int Constitution {
        get {return EquippedWeapon.Constitution + playerBase.baseConstitution;}
    }
    public int Energie {
        get {return EquippedWeapon.Energie + playerBase.baseEnergie;}
    }
    public int Force {
        get {return EquippedWeapon.Force + playerBase.baseForce;}
    }
    public int Defense {
        get {return EquippedWeapon.Defense + playerBase.baseDefense;}
    }
    public int Critique {
        get {return EquippedWeapon.Critique + playerBase.baseCritique;}
    }
    public int Erudition {
        get {return EquippedWeapon.Erudition + playerBase.baseErudition;}
    }

    // Valeur qui évoluera dans la partie en fonction des stats
    public int MaxHp {
        get { return playerBase.baseConstitution * 5; }
    }
    public int MaxMana {
        get { return playerBase.baseEnergie * 5; }
    }
}

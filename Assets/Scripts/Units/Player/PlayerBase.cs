using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Player", menuName = "Player/Create a new player")]

public class PlayerBase : ScriptableObject
{
    // Statistiques de base, que l'on augmente avec les points
    public int baseConstitution = 5;
    public int baseEnergie = 5;
    public int baseForce = 5;
    public int baseDefense = 5;
    public int baseCritique = 5;
    public int baseErudition = 5;

}

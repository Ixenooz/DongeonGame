using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rarity { None, Commun, Rare, Epique, Legendaire }
public enum AugmentType { Stat, Weapon, Special}

public class AugmentBase
{
    public string name;
    public string description;
    public Rarity rarity;
    public AugmentType type;
    public int statValue;

    public AugmentBase(string name, string description, Rarity rarity, int statValue)
    {
        this.name = name;
        this.description = description;
        this.rarity = rarity;
        type = AugmentType.Stat;
        this.statValue = statValue;
    }

    public AugmentBase(string name, string description, Rarity rarity, AugmentType type)
    {
        this.name = name;
        this.description = description;
        this.rarity = rarity;
        this.type = type;
    }
}

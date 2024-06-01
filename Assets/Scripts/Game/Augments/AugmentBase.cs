using System;
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
    public Stat stat;
    public Tuple<Stat, Stat> tupleStats;
    public int goldLoss;

    public AugmentBase(string name)
    {
        this.name = name;
    }
    public AugmentBase(string name, string description)
    {
        this.name = name;
        this.description = description;
    }
    public AugmentBase(string name, string description, Tuple<Stat, Stat> tupleStats)
    {
        this.name = name;
        this.description = description;
        this.tupleStats = tupleStats;
    }
    public AugmentBase(string name, string description, int goldLoss)
    {
        this.name = name;
        this.description = description;
        this.goldLoss = goldLoss;
    }

    public AugmentBase(string name, string description, Rarity rarity, int statValue, Stat stat)
    {
        this.name = name;
        this.description = description;
        this.rarity = rarity;
        type = AugmentType.Stat;
        this.statValue = statValue;
        this.stat = stat;
    }

    public AugmentBase(string name, string description, Rarity rarity, AugmentType type)
    {
        this.name = name;
        this.description = description;
        this.rarity = rarity;
        this.type = type;
    }
}

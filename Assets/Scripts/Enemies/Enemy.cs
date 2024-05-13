using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/Create a new enemy")]

public class Enemy : ScriptableObject
{
    public string name;
    public Sprite sprite;

    // Valeur qui changera
    public int Hp {get; set; }
    public int Mana {get; set; }
    

    // Statistique de chaque ennemie
    [SerializeField] int Constitution;
    [SerializeField] int Energie;
    [SerializeField] int Force;
    [SerializeField] int Defense;
    [SerializeField] int Critique;
    [SerializeField] int Erudition;
    [SerializeField] List<SpellBase> Spells;

    public int MaxHp {
        get { return Constitution * 5; }
    }
    public int MaxMana {
        get { return Energie * 5;}
    }
}

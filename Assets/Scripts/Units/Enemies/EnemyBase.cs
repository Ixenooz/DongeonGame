using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType{
    None,
    Squelette,
    Vampire,
}

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/Create a new enemy")]

public class EnemyBase : ScriptableObject
{
    public string name;
    public EnemyType type;
    public Sprite sprite;

    // Valeur qui changera
    public int Hp;
    public int Mana;
    

    // Statistique de chaque ennemie
    [SerializeField] public int Constitution;
    [SerializeField] public int Energie;
    [SerializeField] public int Force;
    [SerializeField] public int Defense;
    [SerializeField] public int Critique;
    [SerializeField] public int Erudition;
    [SerializeField] public List<SpellBase> Spells;

    public int MaxHp {
        get { return Constitution * 10; }
    }
    public int MaxMana {
        get { return Energie * 5;}
    }

    public SpellBase GetRandomSpell()
    {
        int r = Random.Range(0, Spells.Count);
        return Spells[r];
    }
}

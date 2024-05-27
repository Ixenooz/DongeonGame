using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spell", menuName = "Weapon/Create a new spell")]

public class SpellBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] WeaponType weaponType;
    [SerializeField] SpellType spellType;
    [SerializeField] Sprite spellSprite;
    [SerializeField] int manaCost;

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public WeaponType WeaponType {
        get { return weaponType; }
    }

    public SpellType SpellType{
        get { return spellType; }
    }
    public Sprite SpellSprite {
        get { return spellSprite; }
    }
    public int ManaCost {
        get { return manaCost; }
    }
}

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

    [SerializeField] bool scaleWithConstitution = false;
    [SerializeField] bool scaleWithEnergie = false;
    [SerializeField] bool scaleWithForce = false;
    [SerializeField] bool scaleWithDefense = false;
    [SerializeField] bool scaleWithCritique = false;
    [SerializeField] bool scaleWithErudition = false;

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

    public bool ScaleWithConstitution {
        get { return scaleWithConstitution; }
        set { scaleWithConstitution = value; }
    }

    public bool ScaleWithEnergie {
        get { return scaleWithEnergie; }
        set { scaleWithEnergie = value; }
    }

    public bool ScaleWithForce {
        get { return scaleWithForce; }
        set { scaleWithForce = value; }
    }

    public bool ScaleWithDefense {
        get { return scaleWithDefense; }
        set { scaleWithDefense = value; }
    }

    public bool ScaleWithCritique {
        get { return scaleWithCritique; }
        set { scaleWithCritique = value; }
    }

    public bool ScaleWithErudition {
        get { return scaleWithErudition; }
        set { scaleWithErudition = value; }
    }



}

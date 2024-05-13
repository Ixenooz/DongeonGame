using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType {
    None,
    Epee,
    Bouclier,
    Lance,
    Dague,
    Arc,
}

public class Weapon
{
    WeaponBase Base;
    int level;

    public List<Spell> Spells { get; set; }

    public Weapon(WeaponBase wBase, int wLevel)
    {
        Base = wBase;
        level = wLevel;

        // Génère les spell selon le niveau de l'arme
        Spells = new List<Spell>();
        foreach (var spell in Base.LearnableSpells)
        {
            if (spell.Level <= level)
            {
                Spells.Add(new Spell(spell.Base));
            }
        }
    }
}

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
    Sceptre
}

public class Weapon : MonoBehaviour
{
    public WeaponBase Base;
    private int level;
    private List<Spell> spells;

    public Weapon(WeaponBase wBase)
    {
        Base = wBase;
        level = wBase.Level;

        // Génère les spell selon le niveau de l'arme
        spells = new List<Spell>();
        foreach (var spell in Base.LearnableSpells)
        {
            if (spell.Level <= level)
            {
                spells.Add(new Spell(spell.Base));
            }
        }
    }

    public List<Spell> Spells
    {
        get
        {
            // Génère les spell selon le niveau de l'arme
            spells = new List<Spell>();
            spells.Clear();

            foreach (var spell in Base.LearnableSpells)
            {
                if (spell.Level <= Base.Level)
                {
                    spells.Add(new Spell(spell.Base));
                }
            }
            return spells;
        }
    }
}

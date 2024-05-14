using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellType
{
    None,
    Tranchant,
    Glacial,
    Empoisonne,
    Enflamme
}

public class Spell
{
    public SpellBase Base {get; set;}

    public Spell (SpellBase spellBase)
    {
        Base = spellBase;
    }
}


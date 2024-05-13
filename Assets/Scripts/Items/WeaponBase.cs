using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Create a new weapon")]

public class WeaponBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] int level;

    // Statistiques de l'arme
    [SerializeField] int constitution;
    [SerializeField] int energie;
    [SerializeField] int force;
    [SerializeField] int defense;
    [SerializeField] int critique;
    [SerializeField] int erudition;

    //Liste des compétences que peut avoir l'arme
    [SerializeField] List<LearnableSpell> learnableSpells;

    public string Name {
        get { return name; }
    }
    public int Level {
        get { return level; }
    }
    public int Constitution {
        get { return constitution; }
    }
    public int Energie {
        get { return energie; }
    }
    public int Force {
        get { return force; }
    }
    public int Defense {
        get { return defense; }
    }
    public int Critique {
        get { return critique; }
    }
    public int Erudition {
        get { return erudition; }
    }
    public List<LearnableSpell> LearnableSpells {
        get { return learnableSpells; }
    }

    [System.Serializable]
    public class LearnableSpell 
    {
        [SerializeField] SpellBase spellBase;
        [SerializeField] int level;
        
        public SpellBase Base {
            get { return spellBase; }
        }
        
        public int Level {
            get { return level; }
        }


    }
}


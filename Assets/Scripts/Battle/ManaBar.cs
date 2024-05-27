using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : MonoBehaviour
{
    [SerializeField] GameObject mana;

    // Mettre à jour la barre de vie
    public void SetMana(float manaNormalized)
    {
        mana.transform.localScale = new Vector3(manaNormalized, 1f);
    }

    public IEnumerator SetManaSmooth(float newMana)
    {
        float curMana = mana.transform.localScale.x;

        if (newMana < curMana)
        {
            float changeAmt = curMana - newMana;
            while (curMana - newMana > Mathf.Epsilon)
            {
                curMana -= changeAmt * Time.deltaTime;
                mana.transform.localScale = new Vector3(curMana, 1f);
                yield return null;
            }
        }
        else 
        {
            float changeAmt = newMana - curMana;
            while (newMana - curMana > Mathf.Epsilon)
            {
                curMana += changeAmt * Time.deltaTime;
                mana.transform.localScale = new Vector3(curMana, 1f);
                yield return null;
            }
        }

        
        mana.transform.localScale = new Vector3(newMana, 1f);
    }

}
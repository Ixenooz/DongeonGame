using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroCharacterGold : MonoBehaviour
{
    public int currentGold;
    public TMP_Text goldUI;
    private void Update()
    {
        goldUI.SetText(currentGold.ToString());
    }
}

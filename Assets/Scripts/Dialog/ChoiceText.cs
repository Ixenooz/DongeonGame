using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceText : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    private void Awake() {
        text = GetComponent<TMP_Text>();
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            text.color = Color.red;
        }
        else if (!selected)
        {
            text.color = Color.black;
        }
    }

    public TMP_Text TextField => text;
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor = Color.red;
    [SerializeField] TMP_Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject spellSelector;

    [SerializeField] List<TMP_Text> actionTexts;
    [SerializeField] List<SpellPrefab> spellPrefab;

    public void SetDialog(string dialog)
    {
        dialogText.SetText(dialog);
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.SetText("");
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    // Activer la dialogue dans le combat
    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    // Activer le choix d'action dans le combat
    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    //Activer le choix de spell dans le combat
    public void EnableSpellSelector(bool enabled)
    {
        spellSelector.SetActive(enabled);
    }

    // Pour choisir une action (pour le moment il n'y a que Attaquer)
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i=0; i<actionTexts.Count; ++i)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = highlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }
}

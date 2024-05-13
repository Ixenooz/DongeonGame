using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceBox : MonoBehaviour
{
    [SerializeField] ChoiceText choiceTextPrefab;
    List<ChoiceText> choiceTexts;
    int currentChoice;
    int temp = 1;

    bool choiceSelected = false;
    public IEnumerator ShowChoices(List<string> choices, Action<int> onChoiceSelected)
    {
        choiceSelected = false;
        currentChoice = 0;

        gameObject.SetActive(true);

        // Je supprime les choix déja existants
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
            temp = 1;
        }

        choiceTexts = new List<ChoiceText>();
        foreach (var choice in choices)
        {
            var choiceTextObj = Instantiate(choiceTextPrefab, transform);
            choiceTextPrefab = choiceTextObj;
            choiceTextObj.name = "choice " + temp.ToString();
            temp++;
            choiceTextObj.GetComponent<TMPro.TextMeshProUGUI>().enabled = true;
            choiceTextObj.TextField.SetText(choice);
            choiceTexts.Add(choiceTextObj);
        }

        yield return new WaitUntil(() => choiceSelected == true);

        onChoiceSelected?.Invoke(currentChoice);
        gameObject.SetActive(false);
    }

    private void Update() 
    {
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            currentChoice++;
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            currentChoice--;
        }
        currentChoice = Mathf.Clamp(currentChoice, 0, choiceTexts.Count - 1);

        for (int i = 0; i < choiceTexts.Count; i++)
        {
            choiceTexts[i].SetSelected(i == currentChoice);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            choiceSelected = true;
        }

    }
}

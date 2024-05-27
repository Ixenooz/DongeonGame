using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusSprite : MonoBehaviour
{
    [SerializeField] List<Sprite> ConsitutionSprite;
    [SerializeField] List<Sprite> EnergieSprite;
    [SerializeField] List<Sprite> ForceSprite;
    [SerializeField] List<Sprite> DefenseSprite;
    [SerializeField] List<Sprite> CritiqueSprite;
    [SerializeField] List<Sprite> EruditionSprite;

    public Image StatBoost;
    public Image Status;

    [SerializeField] Sprite Brulure;
    [SerializeField] Sprite Poison;

    public void SetBrulure()
    {
        Status.sprite = Brulure;
    }
    public void SetPoison()
    {
        Status.sprite = Poison;
    }
}

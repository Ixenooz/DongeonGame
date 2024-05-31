using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    [SerializeField] GameObject Augments;
    [SerializeField] AugmentHUD augment1;
    [SerializeField] AugmentHUD augment2;
    [SerializeField] AugmentHUD augment3;
    [SerializeField] List<Sprite> statSprites;

    private (Rarity, int) GetRarity()
    {
        float roll = Random.value;
        if (roll < 0.5f) // 50% chance for common
        {
            return (Rarity.Commun, 1);
        }
            else if (roll < 0.80f) // 30% chance for rare
        {
            return (Rarity.Rare, Random.Range(2, 3));
        }
        else if (roll < 0.95f) // 15% chance for epic
        {
            return (Rarity.Epique, Random.Range(3, 4));
        }
        else // 5% chance for legendary
        {
            return (Rarity.Legendaire, Random.Range(4, 7));
        }
    }

    private Stat GetStat()
    {
        int roll = Random.Range(0, 6);

        if (roll == 0)
            return Stat.Constitution;
        else if (roll == 1)
            return Stat.Energie;
        else if (roll == 2)
            return Stat.Force;
        else if (roll == 3)
            return Stat.Defense;
        else if (roll == 4)
            return Stat.Critique;
        else
            return Stat.Erudition;
    }
    public List<AugmentBase> GetRandomStatAugments(int count)
    {
        List<AugmentBase> Augments = new List<AugmentBase>();

        for (int i = 0; i < count; i++)
        {
            var stat = GetStat();
            var rarity = GetRarity().Item1;
            var rarityValue = GetRarity().Item2;

            Augments.Add(new AugmentBase($"Plus de {stat}", $" Augmente votre {stat} de {rarityValue}", rarity, rarityValue));
        }

        return Augments;
    }
    public void SetStatAugment(int count)
    {
        List<AugmentBase> Augments = GetRandomStatAugments(count);

        augment1.nameText.SetText(Augments[0].name);
        augment2.nameText.SetText(Augments[1].name);
        augment3.nameText.SetText(Augments[2].name);

        augment1.descriptionText.SetText(Augments[0].description);
        augment2.descriptionText.SetText(Augments[1].description);
        augment3.descriptionText.SetText(Augments[2].description);
    }

    private void Update() 
    {
        if (Input.GetKeyUp(KeyCode.B) && Augments.activeSelf == false)
        {
            Augments.SetActive(true);
            SetStatAugment(3);
        }
        else if (Input.GetKeyUp(KeyCode.B) && Augments.activeSelf == true)
        {
            SetStatAugment(3);
        }
        if (Input.GetKeyUp(KeyCode.A) && Augments.activeSelf == true)
        {
            Augments.SetActive(false);
        }
    }
}

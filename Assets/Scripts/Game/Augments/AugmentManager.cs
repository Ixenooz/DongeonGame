using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    [SerializeField] Player player;
    public GameObject AugmentsObj;
    [SerializeField] List<AugmentHUD> AugmentsHUD;
    [SerializeField] List<Sprite> statSprites;
    private List<AugmentBase> AugmentsStat;
    private List<AugmentBase> AugmentsBoss;
    private List<AugmentBase> AugmentsBossData;
    private List<int> availableRerolls = new List<int> { 50, 50, 50 };
    private int selectedAugment;
    public bool isBossAugment;
    public event Action OnCloseAugment;
    public static AugmentManager Instance { get; private set; }
    public Dictionary<string, bool> isActive; // Augments boss actives

    private void Awake()
    {
        Instance = this;
        AugmentsBossData = new List<AugmentBase>() { new("Contrat cupide"), new("Futur enrichissant"), new("Emprunt a risque"), new("Ceci est un test"), new("Ceci est un test2") };
    }

    private (Rarity, int) GetRarity()
    {
        float roll = UnityEngine.Random.value;
        if (roll < 0.5f) // 50% chance for common
        {
            return (Rarity.Commun, 1);
        }
        else if (roll < 0.80f) // 30% chance for rare
        {
            return (Rarity.Rare, UnityEngine.Random.Range(2, 3));
        }
        else if (roll < 0.95f) // 15% chance for epic
        {
            return (Rarity.Epique, UnityEngine.Random.Range(3, 4));
        }
        else // 5% chance for legendary
        {
            return (Rarity.Legendaire, UnityEngine.Random.Range(4, 7));
        }
    }

    private Stat GetStat()
    {
        int roll = UnityEngine.Random.Range(0, 6);
        return (Stat)roll;
    }

    private bool IsStatAlreadyProposed(Stat stat)
    {
        foreach (var augment in AugmentsStat)
        {
            if (augment.stat == stat)
            {
                return true;
            }
        }
        return false;
    }

    public void GetRandomStatAugments(int count)
    {
        AugmentsStat = new List<AugmentBase>();

        for (int i = 0; i < count; i++)
        {
            Stat stat = GetStat();
            // L'augment proposée ne doit pas être déjà proposée

            while (IsStatAlreadyProposed(stat))
            {
                stat = GetStat();
            }

            var rarity = GetRarity().Item1;
            var rarityValue = GetRarity().Item2;
            AugmentsStat.Add(new AugmentBase($"Plus de {stat}", $"Augmente votre {stat} de {rarityValue}", rarity, rarityValue, stat));
        }

        // Set up HUD
        for (int i = 0; i < count; i++)
        {
            AugmentsHUD[i].nameText.SetText(AugmentsStat[i].name);
            AugmentsHUD[i].descriptionText.SetText(AugmentsStat[i].description);
        }
    }

    private void RerollStatAugment(int selectedAugment)
    {
        if (availableRerolls[selectedAugment] > 0)
        {
            Stat stat = GetStat();

            // L'augment reroll ne doit pas être déjà proposée
            while (IsStatAlreadyProposed(stat))
            {
                stat = GetStat();
            }

            var rarity = GetRarity().Item1;
            var rarityValue = GetRarity().Item2;
            AugmentsStat[selectedAugment] = new AugmentBase($"Plus de {stat}", $"Augmente votre {stat} de {rarityValue}.", rarity, rarityValue, stat);

            // Update HUD
            AugmentsHUD[selectedAugment].nameText.SetText(AugmentsStat[selectedAugment].name);
            AugmentsHUD[selectedAugment].descriptionText.SetText(AugmentsStat[selectedAugment].description);

            availableRerolls[selectedAugment]--;
        }
    }

    public void GetRandomBossAugment(int count)
    {
        AugmentsBoss = new List<AugmentBase>();

        for (int i = 0; i < count; i++)
        {
            AugmentBase newAugment = null;
            while (newAugment == null || IsBossAugmentAlreadyProposed(newAugment))
            {
                var r = UnityEngine.Random.Range(0, AugmentsBossData.Count);

                if (AugmentsBossData[r].name == "Contrat cupide")
                {
                    var stats = GenerateContratCupide();
                    newAugment = new AugmentBase("Contrat cupide", $"Vous doublez votre {stats.Item1}, mais reduisez de moitie votre {stats.Item2}.", stats);
                }
                else if (AugmentsBossData[r].name == "Futur enrichissant")
                {
                    var goldLoss = UnityEngine.Random.Range(5, 16);
                    newAugment = new AugmentBase("Futur enrichissant", $"Vous doublez l'or que vous recevez dans les coffres, mais vous en perdez {goldLoss}", goldLoss);
                }
                else if (AugmentsBossData[r].name == "Emprunt a risque")
                {
                    newAugment = new AugmentBase("Emprunt a risque", "Votre solde d'or peut devenir negatif, mais s'il l'est à la fin d'un etage, il est double.");
                }
                else if (AugmentsBossData[r].name == "Ceci est un test")
                {
                    newAugment = new AugmentBase("Ceci est un test", "Ceci est un test pour verifier que tout fonctionne bien.");
                }
                else if (AugmentsBossData[r].name == "Ceci est un test2")
                {
                    newAugment = new AugmentBase("Ceci est un test2", "Ceci est un test2 pour verifier que tout fonctionne bien.");
                }
            }
            if (newAugment != null && !IsBossAugmentAlreadyProposed(newAugment))
            {
                AugmentsBoss.Add(newAugment);
            }
        }

        // Set up HUD
        for (int i = 0; i < count; i++)
        {
            AugmentsHUD[i].nameText.SetText(AugmentsBoss[i].name);
            AugmentsHUD[i].descriptionText.SetText(AugmentsBoss[i].description);
        }
    }
    private bool IsBossAugmentAlreadyProposed(AugmentBase augment)
    {
        foreach (var existingAugment in AugmentsBoss)
        {
            if (existingAugment.name == augment.name)
            {
                return true;
            }
        }
        return false;
    }
    private void RerollBossAugment(int selectedAugment)
    {
        if (availableRerolls[selectedAugment] > 0)
        {
            AugmentBase newAugment = null;
            int attempts = 0;
            while (newAugment == null || IsBossAugmentAlreadyProposed(newAugment))
            {
                var r = UnityEngine.Random.Range(0, AugmentsBossData.Count);
                Debug.Log($"r = {r}");

                if (AugmentsBossData[r].name == "Contrat cupide")
                {
                    var stats = GenerateContratCupide();
                    newAugment = new AugmentBase("Contrat cupide", $"Vous doublez votre {stats.Item1}, mais reduisez de moitie votre {stats.Item2}.", stats);
                }
                else if (AugmentsBossData[r].name == "Futur enrichissant")
                {
                    var goldLoss = UnityEngine.Random.Range(5, 16);
                    newAugment = new AugmentBase("Futur enrichissant", $"Vous doublez l'or que vous recevez dans les coffres, mais vous en perdez {goldLoss}", goldLoss);
                }
                else if (AugmentsBossData[r].name == "Emprunt a risque")
                {
                    newAugment = new AugmentBase("Emprunt a risque", "Votre solde d'or peut devenir negatif, mais s'il l'est à la fin d'un etage il est double.");
                }
                else if(AugmentsBossData[r].name == "Ceci est un test")
                {
                    newAugment = new AugmentBase("Ceci est un test", "Ceci est un test pour vérifier que tout fonctionne bien.");
                }
                else if (AugmentsBossData[r].name == "Ceci est un test2")
                {
                    newAugment = new AugmentBase("Ceci est un test2", "Ceci est un test2 pour verifier que tout fonctionne bien.");
                }
                Debug.Log(newAugment.name);
                attempts++;
                if (attempts > 10) break; // Avoid infinite loop

            }

            if (newAugment != null && !IsBossAugmentAlreadyProposed(newAugment))
            {
                AugmentsBoss[selectedAugment] = newAugment;

                // Update HUD
                AugmentsHUD[selectedAugment].nameText.SetText(AugmentsBoss[selectedAugment].name);
                AugmentsHUD[selectedAugment].descriptionText.SetText(AugmentsBoss[selectedAugment].description);
            }

            availableRerolls[selectedAugment]--;
        }
    }


    private void HandleNavigation()
    {
        // Naviguer
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            AugmentsHUD[selectedAugment].gameObject.GetComponent<Animator>().SetBool("selected", false);
            selectedAugment = math.clamp(selectedAugment + 1, 0, 2);
            AugmentsHUD[selectedAugment].gameObject.GetComponent<Animator>().SetBool("selected", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            AugmentsHUD[selectedAugment].gameObject.GetComponent<Animator>().SetBool("selected", false);
            selectedAugment = math.clamp(selectedAugment - 1, 0, 2);
            AugmentsHUD[selectedAugment].gameObject.GetComponent<Animator>().SetBool("selected", true);
        }
    }
    private void HandleStatSelection()
    {
        // Reroll
        if (Input.GetKeyUp(KeyCode.R))
        {
            RerollStatAugment(selectedAugment);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            Debug.Log($"Vous avez choisis l'augment : {AugmentsStat[selectedAugment].name}, vous avez gagné {AugmentsStat[selectedAugment].statValue} de {AugmentsStat[selectedAugment].stat} !");
            if (AugmentsStat[selectedAugment].stat == Stat.Constitution)
            {
                player.playerBase.baseConstitution += AugmentsStat[selectedAugment].statValue;
            }
            else if (AugmentsStat[selectedAugment].stat == Stat.Energie)
            {
                player.playerBase.baseEnergie += AugmentsStat[selectedAugment].statValue;
            }
            else if (AugmentsStat[selectedAugment].stat == Stat.Force)
            {
                player.playerBase.baseForce += AugmentsStat[selectedAugment].statValue;
            }
            else if (AugmentsStat[selectedAugment].stat == Stat.Defense)
            {
                player.playerBase.baseDefense += AugmentsStat[selectedAugment].statValue;
            }
            else if (AugmentsStat[selectedAugment].stat == Stat.Critique)
            {
                player.playerBase.baseCritique += AugmentsStat[selectedAugment].statValue;
            }
            else if (AugmentsStat[selectedAugment].stat == Stat.Erudition)
            {
                player.playerBase.baseErudition += AugmentsStat[selectedAugment].statValue;
            }        
            OnCloseAugment.Invoke();
        }
    }

    private void HandleBossSelection()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            RerollBossAugment(selectedAugment);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            Debug.Log($"Vous avez choisis l'augment : {AugmentsBoss[selectedAugment].name} !");
            if (AugmentsBoss[selectedAugment].name == "Contrat cupide")
            {
                ContratCupide(AugmentsBoss[selectedAugment].tupleStats.Item1, AugmentsBoss[selectedAugment].tupleStats.Item2);
                AugmentsBossData.RemoveAt(0);
                Debug.Log(AugmentsBossData);
                OnCloseAugment.Invoke();
            }
        }

    }

    public void HandleUpdate()
    {
        HandleNavigation();

        if (!isBossAugment)
        {
            HandleStatSelection();
        }
        else if (isBossAugment)
        {
            HandleBossSelection();
        }
    }

    private Tuple<Stat, Stat> GenerateContratCupide()
    {
        Stat doubleStat = GetStat();
        Stat halveStat = GetStat();

        if (doubleStat == halveStat)
        {
            while (halveStat == doubleStat)
            {
                halveStat = GetStat();
            }
        }
        return new Tuple<Stat, Stat>(doubleStat, halveStat);
    }

    private void ContratCupide(Stat doubleStat, Stat halveStat)
    {
        switch (doubleStat)
        {
            case Stat.Constitution:
                player.playerBase.baseConstitution *= 2;
                break;
            case Stat.Energie:
                player.playerBase.baseEnergie *= 2;
                break;
            case Stat.Force:
                player.playerBase.baseForce *= 2;
                break;
            case Stat.Defense:
                player.playerBase.baseDefense *= 2;
                break;
            case Stat.Critique:
                player.playerBase.baseCritique *= 2;
                break;
            case Stat.Erudition:
                player.playerBase.baseErudition *= 2;
                break;
        }
        switch (halveStat)
        {
            case Stat.Constitution:
                player.playerBase.baseConstitution = Mathf.Max(1, player.playerBase.baseConstitution / 2);
                break;
            case Stat.Energie:
                player.playerBase.baseEnergie = Mathf.Max(1, player.playerBase.baseEnergie / 2);
                break;
            case Stat.Force:
                player.playerBase.baseForce = Mathf.Max(1, player.playerBase.baseForce / 2);
                break;
            case Stat.Defense:
                player.playerBase.baseDefense = Mathf.Max(1, player.playerBase.baseDefense / 2);
                break;
            case Stat.Critique:
                player.playerBase.baseCritique = Mathf.Max(1, player.playerBase.baseCritique / 2);
                break;
            case Stat.Erudition:
                player.playerBase.baseErudition = Mathf.Max(1, player.playerBase.baseErudition / 2);
                break;
        }
    }
}

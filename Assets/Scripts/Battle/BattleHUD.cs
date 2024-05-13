using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] TMP_Text pseudoText;
    [SerializeField] HPBar hpBar;
    [SerializeField] ManaBar manaBar;

    public void SetPlayerData(Player player)
    {
        pseudoText.SetText(player.Pseudo);
        hpBar.SetHP((float) player.Hp / player.MaxHp);
        manaBar.SetMana((float) player.Mana / player.MaxMana);
    }
    public void SetEnemyData (Enemy enemy)
    {
        GetComponent<Image>().sprite = enemy.sprite;
        pseudoText.SetText(enemy.name);
        hpBar.SetHP((float)enemy.Hp / enemy.MaxHp);
        manaBar.SetMana(enemy.Mana / enemy.MaxMana); 
    }
}


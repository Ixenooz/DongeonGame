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
        hpBar.SetHP((float) enemy.Hp / enemy.MaxHp);
        manaBar.SetMana((float) enemy.Mana / enemy.MaxMana); 
    }
    public IEnumerator UpdatePlayerHp(Player player)
    {
        yield return hpBar.SetHPSmooth((float) player.Hp / player.MaxHp);
    }
    public IEnumerator UpdateEnemyHp(Enemy enemy)
    {
        yield return hpBar.SetHPSmooth((float) enemy.Hp / enemy.MaxHp);
    }

    
    public IEnumerator UpdatePlayerMana(Player player)
    {
         yield return manaBar.SetManaSmooth((float) player.Mana / player.MaxMana);
    }
    public IEnumerator UpdateEnemyMana(Enemy enemy)
    {
         yield return manaBar.SetManaSmooth((float) enemy.Mana / enemy.MaxMana);
    }
}


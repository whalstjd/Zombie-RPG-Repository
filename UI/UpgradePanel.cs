using DataInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject newObj;
    public Sprite purchasableSprite_Green;
    public Sprite purchasableSprite_Brown;
    public Sprite unpurchasableSprite;

    [Header("Player Upgrade")]
    public Text[] playerUpgrade_Lv_Text;
    public Text[] playerUpgrade_Value_Text;
    public Text[] playerUpgrade_Price_Text;
    public Image[] playerUpgrade_Buy_Btn_Img;

    [Header("Team Upgrade")]
    public Text[] teamUpgrade_Lv_Text;
    public Text[] teamUpgrade_Value_Text;
    public Text[] teamUpgrade_Price_Text;
    public Image[] teamUpgrade_Buy_Btn_Img;

    [Header("Weapon Upgrade")]
    public Text[] weaponUpgrade_Lv_Text;
    public Text[] weaponUpgrade_Value_Text;
    public Text[] weaponUpgrade_Next_value_Text;
    public Text[] weaponUpgrade_Rate_Text;
    public Text[] weaponUpgrade_Price_Text;
    public Image[] weaponUpgrade_Buy_Btn_Img;

    private GameData gameData;
    private GameConfig gameConfig;

    // 업그레이드 레벨 배열
    private int[] playerUpgradeLevels = new int[6];  // AttackDamage, AttackSpeed, MaxHP, Exp, Money, Item
    private int[] teamUpgradeLevels = new int[6];    // AttackDamage, AttackSpeed, MaxHP, Exp, Money, Item
    private int[] weaponUpgradeLevels = new int[2];  // AttackDamage, MaxHP

    private void OnEnable()
    {
        if (DataManager.Instance != null)
        {
            gameData = DataManager.Instance.gameData;
            gameConfig = DataManager.Instance.gameConfig;
            LoadUpgradeLevels();
            Upgrade_Panel_Update();
        }
    }

    private void Start()
    {
        if (DataManager.Instance != null)
        {
            gameData = DataManager.Instance.gameData;
            gameConfig = DataManager.Instance.gameConfig;
            LoadUpgradeLevels();
        }
    }

    private void LoadUpgradeLevels()
    {
        // 플레이어 업그레이드 레벨 로드
        for (int i = 0; i < playerUpgradeLevels.Length; i++)
        {
            playerUpgradeLevels[i] = gameData.playerUpgradeLv[i];
        }

        // 팀 업그레이드 레벨 로드
        for (int i = 0; i < teamUpgradeLevels.Length; i++)
        {
            teamUpgradeLevels[i] = gameData.teamUpgradeLv[i];
        }

        // 무기 업그레이드 레벨 로드
        for (int i = 0; i < weaponUpgradeLevels.Length; i++)
        {
            weaponUpgradeLevels[i] = gameData.weaponUpgradeLv[i];
        }
    }

    private void SaveUpgradeLevels()
    {
        // 플레이어 업그레이드 레벨 저장
        for (int i = 0; i < playerUpgradeLevels.Length; i++)
        {
            gameData.playerUpgradeLv[i] = playerUpgradeLevels[i];
        }

        // 팀 업그레이드 레벨 저장
        for (int i = 0; i < teamUpgradeLevels.Length; i++)
        {
            gameData.teamUpgradeLv[i] = teamUpgradeLevels[i];
        }

        // 무기 업그레이드 레벨 저장
        for (int i = 0; i < weaponUpgradeLevels.Length; i++)
        {
            gameData.weaponUpgradeLv[i] = weaponUpgradeLevels[i];
        }
    }

    public void Upgrade_Panel_Update()
    {
        UpdateAllUpgrades();
        PurchaseUpdate();
    }

    private void UpdateAllUpgrades()
    {
        UpdateUpgrades(playerUpgrade_Lv_Text, playerUpgrade_Value_Text, playerUpgrade_Price_Text, UpgradeTargetEnum.PLAYER);
        UpdateUpgrades(teamUpgrade_Lv_Text, teamUpgrade_Value_Text, teamUpgrade_Price_Text, UpgradeTargetEnum.TEAM);
        UpdateWeaponUpgrades();
    }

    private void UpdateUpgrades(Text[] lvTexts, Text[] valueTexts, Text[] priceTexts, UpgradeTargetEnum target)
    {
        for (int i = 0; i < lvTexts.Length; i++)
        {
            var upgrade = CreateUpgrade(target, i);
            UpdateUpgradeUI(upgrade, lvTexts[i], valueTexts[i], priceTexts[i]);
        }
    }

    private void UpdateWeaponUpgrades()
    {
        for (int i = 0; i < weaponUpgrade_Lv_Text.Length; i++)
        {
            var upgrade = CreateUpgrade(UpgradeTargetEnum.WEAPON, i);
            UpdateWeaponUpgradeUI(upgrade, i);
        }
    }

    private Upgrade CreateUpgrade(UpgradeTargetEnum target, int index)
    {
        return new Upgrade
        {
            targetEnum = target,
            typeEnum = (UpgradeTypeEnum)index,
            curLv = GetUpgradeLevel(target, index)
        };
    }

    private void UpdateUpgradeUI(Upgrade upgrade, Text lvText, Text valueText, Text priceText)
    {
        lvText.text = $"Lv. {upgrade.curLv}";
        valueText.text = upgrade.curLv < upgrade.maxLv
            ? $"{Mathf.Round((upgrade.value - 1) * 100)}%  ->  {Mathf.Round((upgrade.nextValue - 1) * 100)}%"
            : $"{Mathf.Round((upgrade.value - 1) * 100)}%";
        priceText.text = upgrade.curLv < upgrade.maxLv ? upgrade.price.ToString() : "MAX";
    }

    private void UpdateWeaponUpgradeUI(Upgrade upgrade, int index)
    {
        weaponUpgrade_Lv_Text[index].text = $"+{upgrade.curLv}";
        weaponUpgrade_Value_Text[index].text = $"{(index == 0 ? "데미지" : "체력")} : {upgrade.value}";

        if (upgrade.curLv < upgrade.maxLv)
        {
            weaponUpgrade_Next_value_Text[index].text = $"(다음 단계 : {upgrade.nextValue})";
            weaponUpgrade_Price_Text[index].text = $"가격 : {upgrade.price}";
        }
        else
        {
            weaponUpgrade_Next_value_Text[index].text = "";
            weaponUpgrade_Price_Text[index].text = "MAX";
        }

        weaponUpgrade_Rate_Text[index].text = $"{upgrade.upgrade_Rate}%";
    }

    public void PurchaseUpdate()
    {
        bool isNew = false;
        UpdatePurchaseButtons(playerUpgrade_Buy_Btn_Img, UpgradeTargetEnum.PLAYER, purchasableSprite_Green, ref isNew);
        UpdatePurchaseButtons(teamUpgrade_Buy_Btn_Img, UpgradeTargetEnum.TEAM, purchasableSprite_Brown, ref isNew);
        UpdatePurchaseButtons(weaponUpgrade_Buy_Btn_Img, UpgradeTargetEnum.WEAPON, purchasableSprite_Green, ref isNew);

        UpdateUIs();
        if (!newObj.activeSelf)
            newObj.SetActive(isNew);
    }

    private void UpdatePurchaseButtons(Image[] buttons, UpgradeTargetEnum target, Sprite purchasableSprite, ref bool isNew)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            var upgrade = CreateUpgrade(target, i);
            bool canUpgrade = upgrade.CanUpgrade(gameData.possession.GetPossessionCount(PossessionTypeEnum.Money)) && upgrade.curLv < upgrade.maxLv;

            if (!canUpgrade)
            {
                buttons[i].sprite = unpurchasableSprite;
            }
            else if (buttons[i].sprite != purchasableSprite)
            {
                buttons[i].sprite = purchasableSprite;
                isNew = true;
            }
        }
    }

    private void UpdateUIs()
    {
        UIManager.Instance.Player_UI_Update();
        UIManager.Instance.Monster_UI_Update();
        UIManager.Instance.Goods_UI_Update();
    }

    public void Player_Upgrade(int i) => TryUpgrade(UpgradeTargetEnum.PLAYER, i);
    public void Team_Upgrade(int i) => TryUpgrade(UpgradeTargetEnum.TEAM, i);
    public void Weapon_Upgrade(int i) => TryUpgrade(UpgradeTargetEnum.WEAPON, i);

    private void TryUpgrade(UpgradeTargetEnum target, int index)
    {
        var upgrade = CreateUpgrade(target, index);

        if (!upgrade.CanUpgrade(gameData.possession.GetPossessionCount(PossessionTypeEnum.Money)))
        {
            GameManager.Instance.PlayAudio(GameManager.Instance.unupgradable_Clip);
            return;
        }

        if (upgrade.TryUpgrade(gameData.possession))
        {
            SetUpgradeLevel(target, index, upgrade.curLv);
            GameManager.Instance.PlayAudio(GameManager.Instance.upgradable_Clip);
        }

        Upgrade_Panel_Update();
    }

    private int GetUpgradeLevel(UpgradeTargetEnum target, int index)
    {
        return target switch
        {
            UpgradeTargetEnum.PLAYER => playerUpgradeLevels[index],
            UpgradeTargetEnum.TEAM => teamUpgradeLevels[index],
            UpgradeTargetEnum.WEAPON => weaponUpgradeLevels[index],
            _ => 0
        };
    }

    private void SetUpgradeLevel(UpgradeTargetEnum target, int index, int level)
    {
        switch (target)
        {
            case UpgradeTargetEnum.PLAYER:
                playerUpgradeLevels[index] = level;
                break;
            case UpgradeTargetEnum.TEAM:
                teamUpgradeLevels[index] = level;
                break;
            case UpgradeTargetEnum.WEAPON:
                weaponUpgradeLevels[index] = level;
                break;
        }
        SaveUpgradeLevels();
    }
}

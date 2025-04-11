using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
// using GoogleMobileAds.Api;
using DataInfo;
// using UnityEngine.Purchasing;

public class StorePanel : MonoBehaviour
{
    public Sprite acquired_Spr;
    public Sprite unacquired_Spr;

    [Header("Free")]
    public Image[] daily_Acquired_Img;
    public Text[] daily_Acquired_Text;

    [Header("Ad")]
    public Image[] ad_Acquired_Img;
    public Text[] ad_Acquired_Text;
    public float[] remainTime;

    [Header("Start_Up")]
    public Image startUp_Acquired_Img;
    public Text startUp_Acquired_Text;
    public bool startUp_Acquired_Bool;

    [Header("Product ID")]
    public readonly string productId_StartUp_id = "startup";
    public readonly string productId_Package1_id = "package1";
    public readonly string productId_Package2_id = "package2";
    public readonly string productId_Package3_id = "package3";
    public readonly string productId_Package4_id = "package4";

    private void Start()
    {
        OpenPanel();
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        UpdateFreeRewards();
        UpdateAdRewards();
        UpdateStartUpPackage();
    }

    private void UpdateFreeRewards()
    {
        for (int i = 0; i < daily_Acquired_Img.Length; i++)
        {
            DateTime last_Daily_Date = new DateTime(
                DataManager.Instance.gameData.last_Daily_Year[i],
                DataManager.Instance.gameData.last_Daily_Month[i],
                DataManager.Instance.gameData.last_Daily_Day[i]);

            bool canClaim = last_Daily_Date < DateTime.Today;
            daily_Acquired_Img[i].sprite = canClaim ? unacquired_Spr : acquired_Spr;
            daily_Acquired_Img[i].GetComponent<Button>().enabled = canClaim;
            daily_Acquired_Text[i].text = canClaim ? "획득하기" : "획득 완료";
        }
    }

    private void UpdateAdRewards()
    {
        for (int i = 0; i < ad_Acquired_Img.Length; i++)
        {
            DateTime last_Daily_Date = new DateTime(
                DataManager.Instance.gameData.last_Daily_Year[i + 3],
                DataManager.Instance.gameData.last_Daily_Month[i + 3],
                DataManager.Instance.gameData.last_Daily_Day[i + 3]);

            bool canClaim = last_Daily_Date < DateTime.Today;
            ad_Acquired_Img[i].sprite = canClaim ? unacquired_Spr : acquired_Spr;
            ad_Acquired_Img[i].GetComponent<Button>().enabled = canClaim;
            ad_Acquired_Text[i].text = canClaim ? "광고보상\n획득하기" : "획득 완료";
        }
    }

    private void UpdateStartUpPackage()
    {
        bool isAcquired = DataManager.Instance.gameData.adSkip;
        startUp_Acquired_Img.sprite = isAcquired ? acquired_Spr : unacquired_Spr;
        startUp_Acquired_Img.GetComponent<Button>().enabled = !isAcquired;
        startUp_Acquired_Text.text = isAcquired ? "구매 완료" : "990원";
    }

    public void AttendanceReward(int num)
    {
        DateTime last_Daily_Date = new DateTime(
            DataManager.Instance.gameData.last_Daily_Year[num],
            DataManager.Instance.gameData.last_Daily_Month[num],
            DataManager.Instance.gameData.last_Daily_Day[num]);

        if (last_Daily_Date < DateTime.Today)
        {
            switch (num)
            {
                case 0:
                    DataManager.Instance.gameData.possession.AddPossession(PossessionTypeEnum.Potion, 5);
                    UIManager.Instance.Message("포션 5개 획득!");
                    break;
                case 1:
                    DataManager.Instance.gameData.possession.AddPossession(PossessionTypeEnum.SilverKey, 2);
                    UIManager.Instance.Message("은 열쇠 2개 획득!");
                    break;
                case 2:
                    DataManager.Instance.gameData.possession.AddPossession(PossessionTypeEnum.GoldKey, 1);
                    UIManager.Instance.Message("금 열쇠 1개 획득!");
                    break;
            }

            UpdateLastClaimDate(num);
            UpdateFreeRewards();
            UIManager.Instance.Goods_UI_Update();
        }
    }

    public void Ad_Reward(int num)
    {
        DateTime last_Daily_Date = new DateTime(
            DataManager.Instance.gameData.last_Daily_Year[num + 3],
            DataManager.Instance.gameData.last_Daily_Month[num + 3],
            DataManager.Instance.gameData.last_Daily_Day[num + 3]);

        if (last_Daily_Date < DateTime.Today)
        {
            if (DataManager.Instance.gameData.adSkip)
            {
                switch (num)
                {
                    case 0:
                        DataManager.Instance.gameData.possession.AddPossession(PossessionTypeEnum.Potion, 8);
                        UIManager.Instance.Message("포션 8개 획득!");
                        break;
                    case 1:
                        DataManager.Instance.gameData.possession.AddPossession(PossessionTypeEnum.SilverKey, 4);
                        UIManager.Instance.Message("은 열쇠 4개 획득!");
                        break;
                    case 2:
                        DataManager.Instance.gameData.possession.AddPossession(PossessionTypeEnum.GoldKey, 2);
                        UIManager.Instance.Message("금 열쇠 2개 획득!");
                        break;
                }

                UpdateLastClaimDate(num + 3);
                UpdateAdRewards();
                UIManager.Instance.Goods_UI_Update();
            }
            else
            {
                // TODO: 광고 시스템 구현
                // GameManager.Instance.ad.ShowRewardAd(GetPossessionType(num), GetRewardAmount(num), GetRewardPossessionType(num));
            }
        }
    }

    private void UpdateLastClaimDate(int index)
    {
        DataManager.Instance.gameData.last_Daily_Year[index] = DateTime.Today.Year;
        DataManager.Instance.gameData.last_Daily_Month[index] = DateTime.Today.Month;
        DataManager.Instance.gameData.last_Daily_Day[index] = DateTime.Today.Day;
        DataManager.Instance.SaveGameData();
    }

    private PossessionTypeEnum GetPossessionType(int num)
    {
        return num switch
        {
            0 => PossessionTypeEnum.Potion,
            1 => PossessionTypeEnum.SilverKey,
            2 => PossessionTypeEnum.GoldKey,
            _ => PossessionTypeEnum.Potion
        };
    }

    private int GetRewardAmount(int num)
    {
        return num switch
        {
            0 => 8,
            1 => 4,
            2 => 2,
            _ => 0
        };
    }

    private PossessionTypeEnum GetRewardPossessionType(int num)
    {
        return num switch
        {
            0 => PossessionTypeEnum.Potion,
            1 => PossessionTypeEnum.SilverKey,
            2 => PossessionTypeEnum.GoldKey,
            _ => PossessionTypeEnum.Potion
        };
    }

    // public void PurchasseComplete(UnityEngine.Purchasing.Product product)
    // {
    //     if (product.definition.id == productId_StartUp_id)
    //     {
    //         DataManager.Instance.gameData.potion += 50;
    //         DataManager.Instance.gameData.silverKey += 10;
    //         DataManager.Instance.gameData.goldKey += 5;
    //         DataManager.Instance.gameData.adSkip = true;

    //         DataManager.Instance.resetData.adSkip = true;
    //         DataManager.Instance.Json_Reset_Data_Save();
    //         startUp_Acquired_Img.GetComponent<Button>().enabled = false;
    //         startUp_Acquired_Img.sprite = unacquired_Spr;
    //     }
    //     else if (product.definition.id == productId_Package1_id)
    //     {
    //         DataManager.Instance.gameData.potion += 30;
    //         DataManager.Instance.gameData.silverKey += 7;
    //         DataManager.Instance.gameData.goldKey += 3;
    //     }
    //     else if (product.definition.id == productId_Package2_id)
    //     {
    //         DataManager.Instance.gameData.potion += 65;
    //         DataManager.Instance.gameData.silverKey += 15;
    //         DataManager.Instance.gameData.goldKey += 7;
    //     }
    //     else if (product.definition.id == productId_Package3_id)
    //     {
    //         DataManager.Instance.gameData.potion += 150;
    //         DataManager.Instance.gameData.silverKey += 40;
    //         DataManager.Instance.gameData.goldKey += 20;
    //     }
    //     UIManager.Instance.Goods_UI_Update();
    //     UIManager.Instance.Message("구매 완료");
    //     DataManager.Instance.Json_Game_Data_Save();
    //     Debug.Log("구매 완료");
    // }

    // /* 구매 실패 처리 함수 */
    // public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason reason)
    // {
    //     UIManager.Instance.Message("구매 실패.\n" + reason);
    //     Debug.Log("구매 실패.\n" + reason);
    // }

    //public void IAP(int num)
    //{
    //    switch (num)
    //    {
    //        case 0:
    //            GameManager.Instance.iapManager.Purchase(GameManager.Instance.iapManager.productId_StartUp_id);
    //            break;
    //        case 1:
    //            GameManager.Instance.iapManager.Purchase(GameManager.Instance.iapManager.productId_Package1_id);
    //            break;
    //        case 2:
    //            GameManager.Instance.iapManager.Purchase(GameManager.Instance.iapManager.productId_Package2_id);
    //            break;
    //        case 3:
    //            GameManager.Instance.iapManager.Purchase(GameManager.Instance.iapManager.productId_Package3_id);
    //            break;
    //    }
    //}
}

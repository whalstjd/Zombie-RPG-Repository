// using System.Collections;
// using System.Collections.Generic;
// using System;
// using UnityEngine;
// using GoogleMobileAds.Api;
// using GoogleMobileAds.Common;
// using DataInfo;
// using System.Runtime.InteropServices.WindowsRuntime;

// public enum RewardType { DAILY_POTION, DAILY_SILVER_KEY, DAILY_GOLD_KEY, RESURRECTION_AND_POTION };

// public class AdmobRewardAd : MonoBehaviour
// {
//     private RewardedAd daily_Potion_RewardedAd;
//     private RewardedAd daily_SilverKey_RewardedAd;
//     private RewardedAd daily_GoldKey_RewardedAd;
//     private RewardedAd daily_Resurrection_RewardedAd;
//     private InterstitialAd interstitialAd;

//     string adUnitId;

//     void Start()
//     {
//         MobileAds.Initialize((InitializationStatus initStatus) =>
//         {
//             //�ʱ�ȭ �Ϸ�
//         });
//         LoadRewardedAd(RewardType.DAILY_POTION, daily_Potion_RewardedAd);
//         LoadRewardedAd(RewardType.DAILY_SILVER_KEY, daily_SilverKey_RewardedAd);
//         LoadRewardedAd(RewardType.DAILY_GOLD_KEY, daily_GoldKey_RewardedAd);
//         LoadRewardedAd(RewardType.RESURRECTION_AND_POTION, daily_Resurrection_RewardedAd);
//         LoadInterstitialAd();
//     }

//     public void LoadRewardedAd(RewardType rewardType, RewardedAd rewardAd) //���󱤰� �ε� �ϱ�
//     {
//         switch (rewardType)
//         {
//             case RewardType.DAILY_POTION:
//                 adUnitId = "ca-app-pub-5598405241212680/5659852026"; //����
//                 break;
//             case RewardType.DAILY_SILVER_KEY:
//                 adUnitId = "ca-app-pub-5598405241212680/6953269314"; //������
//                 break;
//             case RewardType.DAILY_GOLD_KEY:
//                 adUnitId = "ca-app-pub-5598405241212680/2702516196"; //Ȳ�ݿ���
//                 break;
//             case RewardType.RESURRECTION_AND_POTION:
//                 adUnitId = "ca-app-pub-5598405241212680/8653363798"; //��Ȱ+����
//                 break;
//         }
//         //adUnitId = "ca-app-pub-3940256099942544/5224354917"; //�׽�Ʈ���̵�

//         // Clean up the old ad before loading a new one.
//         if (rewardAd != null)
//         {
//             rewardAd.Destroy();
//             rewardAd = null;
//         }

//         // create our request used to load the ad.
//         var adRequest = new AdRequest.Builder().Build();

//         // send the request to load the ad.
//         RewardedAd.Load(adUnitId, adRequest,
//             (RewardedAd ad, LoadAdError error) =>
//             {
//                 // if error is not null, the load request failed.
//                 if (error != null || ad == null)
//                 {
//                     Debug.LogError("Rewarded ad failed to load an ad " +
//                                    "with error : " + error);
//                     return;
//                 }

//                 //Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
//                 rewardAd = ad;

//                 switch (rewardType)
//                 {
//                     case RewardType.DAILY_POTION:
//                         daily_Potion_RewardedAd= rewardAd; //����
//                         break;
//                     case RewardType.DAILY_SILVER_KEY:
//                          daily_SilverKey_RewardedAd = rewardAd; //������
//                         break;
//                     case RewardType.DAILY_GOLD_KEY:
//                          daily_GoldKey_RewardedAd = rewardAd; //Ȳ�ݿ���
//                         break;
//                     case RewardType.RESURRECTION_AND_POTION:
//                          daily_Resurrection_RewardedAd = rewardAd; //��Ȱ
//                         break;
//                     default:
//                         daily_Potion_RewardedAd = rewardAd; //����
//                         break;
//                 }
//             });
//     }
//     public void ShowRewardAd(ItemEnum itemEnum, int count, RewardType rewardType) //���󱤰� ����
//     {
//         RewardedAd rewardAd;
//         switch (rewardType)
//         {
//             case RewardType.DAILY_POTION:
//                 rewardAd = daily_Potion_RewardedAd; //����
//                 break;
//             case RewardType.DAILY_SILVER_KEY:
//                 rewardAd = daily_SilverKey_RewardedAd; //������
//                 break;
//             case RewardType.DAILY_GOLD_KEY:
//                 rewardAd = daily_GoldKey_RewardedAd; //Ȳ�ݿ���
//                 break;
//             case RewardType.RESURRECTION_AND_POTION:
//                 rewardAd = daily_Resurrection_RewardedAd; //��Ȱ
//                 break;
//             default:
//                 rewardAd = daily_Potion_RewardedAd; //����
//                 break;
//         }
//         const string rewardMsg =
//             "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

//         if (rewardAd != null && rewardAd.CanShowAd())
//         {
//             rewardAd.Show((Reward reward) =>
//             {
//                 switch (itemEnum)
//                 {
//                     case ItemEnum.Potion:
//                         DataManager.Instance.gameData.potion += count;
//                         UIManager.Instance.Message("���� " + count + "�� ȹ��!");
//                         break;
//                     case ItemEnum.SilverKey:
//                         DataManager.Instance.gameData.silverKey += count;
//                         UIManager.Instance.Message("������ " + count + "�� ȹ��!");
//                         break;
//                     case ItemEnum.GoldKey:
//                         DataManager.Instance.gameData.goldKey += count;
//                         UIManager.Instance.Message("Ȳ�ݿ��� " + count + "�� ȹ��!");
//                         break;
//                 }
//                 if (rewardType == RewardType.RESURRECTION_AND_POTION)
//                 {
//                     DataManager.Instance.gameData.playerStat.hp = DataManager.Instance.gameData.playerStat.Maxhp;
//                     GameManager.Instance.player.isDie = false;
//                     GameManager.Instance.player.animator.SetInteger("State", 0);

//                     UIManager.Instance.restartPanel.gameObject.SetActive(false);
//                 }
//                 else
//                 {
//                     DataManager.Instance.gameData.last_Daily_Year[(int)rewardType + 3] = DateTime.Today.Year;
//                     DataManager.Instance.gameData.last_Daily_Month[(int)rewardType + 3] = DateTime.Today.Month;
//                     DataManager.Instance.gameData.last_Daily_Day[(int)rewardType + 3] = DateTime.Today.Day;
//                     UIManager.Instance.storePanel.ad_Acquired_Img[(int)rewardType].GetComponent<UnityEngine.UI.Button>().enabled = false;
//                     UIManager.Instance.storePanel.ad_Acquired_Img[(int)rewardType].sprite = UIManager.Instance.storePanel.acquired_Spr;
//                     UIManager.Instance.storePanel.ad_Acquired_Text[(int)rewardType].text = "ȹ�� �Ϸ�";
//                 }
//                 UIManager.Instance.Goods_UI_Update();
//                 UIManager.Instance.Player_UI_Update();
//                 Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));

//                 RegisterReloadHandler(rewardType, rewardAd);
//             });
//         }
//     }

//     public void LoadInterstitialAd() //���鱤�� �ε� �ϱ�
//     {
//         adUnitId = "ca-app-pub-5598405241212680/5468280335"; //��Ȱ
//         //adUnitId = "ca-app-pub-3940256099942544/1033173712"; //�׽�Ʈ

//         // Clean up the old ad before loading a new one.
//         if (interstitialAd != null)
//         {
//             interstitialAd.Destroy();
//             interstitialAd = null;
//         }

//         //Debug.Log("Loading the rewarded ad.");

//         // create our request used to load the ad.
//         var adRequest = new AdRequest.Builder().Build();
//         InterstitialAd.Load(adUnitId, adRequest,
//             (InterstitialAd ad, LoadAdError error) =>
//             {
//                 // if error is not null, the load request failed.
//                 if (error != null || ad == null)
//                 {
//                     Debug.LogError("Rewarded ad failed to load an ad " +
//                                    "with error : " + error);
//                     return;
//                 }

//                 //Debug.Log("Rewarded ad loaded with response : "  + ad.GetResponseInfo());

//                 interstitialAd = ad;
//             });
//     }
//     public void ShowAd() //���鱤�� ����
//     {
//         if (interstitialAd != null && interstitialAd.CanShowAd())
//         {
//             interstitialAd.Show();
//         }
//     }

//     private void RegisterReloadHandler(RewardType rewardType, RewardedAd ad) //���� ��ε�
//     {
//         // Raised when the ad closed full screen content.
//         ad.OnAdFullScreenContentClosed += (null);
//         {
//             Debug.Log("Rewarded Ad full screen content closed.");

//             // Reload the ad so that we can show another as soon as possible.
//             LoadRewardedAd(rewardType, ad);
//         };
//         // Raised when the ad failed to open full screen content.
//         ad.OnAdFullScreenContentFailed += (AdError error) =>
//         {
//             Debug.LogError("Rewarded ad failed to open full screen content " +
//                            "with error : " + error);

//             // Reload the ad so that we can show another as soon as possible.
//             LoadRewardedAd(rewardType, ad);
//         };
//     }
// }
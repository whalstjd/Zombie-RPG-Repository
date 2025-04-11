using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataInfo;
/// <summary>
/// 게임 설정을 관리하는 패널 클래스
/// </summary>
public class SettingPanel : MonoBehaviour
{
    public Slider bgm_Slider;            // BGM 볼륨 슬라이더
    public Slider effect_Slider;         // 효과음 볼륨 슬라이더
    public Toggle tipToggle;             // 팁 표시 토글

    public InputField couponField;       // 쿠폰 입력 필드
    public string[] moneyCoupons;        // 돈 쿠폰 배열
    public string[] potionCoupons;       // 포션 쿠폰 배열

    /// <summary>
    /// 설정 패널 데이터 로드
    /// </summary>
    public void Get_SettingPanel_Data()
    {
        bgm_Slider.value = DataManager.Instance.gameData.bgmSound;
        effect_Slider.value = DataManager.Instance.gameData.effectSound;
        tipToggle.isOn = DataManager.Instance.gameData.toolTip;
        UIManager.Instance.tipObj.SetActive(DataManager.Instance.gameData.toolTip);
    }

    /// <summary>
    /// BGM 볼륨 설정
    /// </summary>
    public void Set_BGM(float _value)
    {
        DataManager.Instance.gameData.bgmSound += _value;
        if (DataManager.Instance.gameData.bgmSound < 0) DataManager.Instance.gameData.bgmSound = 0;
        else if (DataManager.Instance.gameData.bgmSound > 1) DataManager.Instance.gameData.bgmSound = 1;

        bgm_Slider.value = DataManager.Instance.gameData.bgmSound;
        GameManager.Instance.BGM_AudioSource.volume = DataManager.Instance.gameData.bgmSound;
        DataManager.Instance.SaveGameData();
    }

    /// <summary>
    /// 효과음 볼륨 설정
    /// </summary>
    public void Set_Effect(float _value)
    {
        DataManager.Instance.gameData.effectSound += _value;
        if (DataManager.Instance.gameData.effectSound < 0) DataManager.Instance.gameData.effectSound = 0;
        else if (DataManager.Instance.gameData.effectSound > 1) DataManager.Instance.gameData.effectSound = 1;

        effect_Slider.value = DataManager.Instance.gameData.effectSound;
        DataManager.Instance.SaveGameData();
    }

    /// <summary>
    /// 팁 표시 설정
    /// </summary>
    public void SetToolTip()
    {
        DataManager.Instance.gameData.toolTip = tipToggle.isOn;
        UIManager.Instance.tipObj.SetActive(DataManager.Instance.gameData.toolTip);
        DataManager.Instance.SaveGameData();
    }

    /// <summary>
    /// 쿠폰 사용 처리
    /// </summary>
    public void Coupon()
    {
        bool used = DataManager.Instance.gameData.usedCoupons.Contains(couponField.text.ToLower());
        if (used)
        {
            UIManager.Instance.Message("이미 사용한 쿠폰입니다.");
            return;
        }

        int moneyIdx = System.Array.IndexOf(moneyCoupons, couponField.text.ToLower());
        int potionIdx = System.Array.IndexOf(potionCoupons, couponField.text.ToLower());
        if (moneyIdx != -1)
        {
            DataManager.Instance.gameData.usedCoupons.Add(couponField.text.ToLower());
            DataManager.Instance.gameData.possession.AddPossession(PossessionTypeEnum.Money, 500);
            UIManager.Instance.Message("500원 획득!");
        }
        else if (potionIdx != -1)
        {
            DataManager.Instance.gameData.usedCoupons.Add(couponField.text.ToLower());
            DataManager.Instance.gameData.possession.AddPossession(PossessionTypeEnum.Potion, 10);
            UIManager.Instance.Message("포션 10개 획득!");
        }
        else
        {
            UIManager.Instance.Message("유효하지 않은 쿠폰입니다.");
        }
        UIManager.Instance.Goods_UI_Update();
        DataManager.Instance.SaveGameData();
    }

    /// <summary>
    /// 게임 초기화
    /// </summary>
    public void InitGame()
    {
        DataManager.Instance.DeleteGameDataFile();
        GameManager.Instance.Start();
    }
}
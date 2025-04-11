using DataInfo;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 게임의 UI를 관리하는 싱글톤 클래스
/// 플레이어, 팀, 적, 보스 등의 UI 요소를 관리합니다.
/// </summary>
public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static UIManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    [Header("Player Info")]
    public Text player_Lv_Text;          // 플레이어 레벨 텍스트
    public Slider player_Exp_Slider;     // 플레이어 경험치 슬라이더
    public Slider player_HP_Slider;      // 플레이어 체력 슬라이더
    public GameObject player_Level_Up_Obj; // 플레이어 레벨업 표시 오브젝트

    [Header("Stage")]
    public Text num_Stage_Text;          // 스테이지 번호 텍스트
    public Text name_Stage_Text;         // 스테이지 이름 텍스트

    [Header("Team Info")]
    public Text[] team_Lv_Text;          // 팀원 레벨 텍스트 배열
    public Slider[] team_HP_Sliders;     // 팀원 체력 슬라이더 배열
    public GameObject[] team_Portrait_Obj; // 팀원 초상화 오브젝트 배열
    public Image[] team_Portrait_Img;     // 팀원 초상화 이미지 배열
    public GameObject[] team_Level_Up_Obj; // 팀원 레벨업 표시 오브젝트 배열

    [Header("Enemy Info")]
    public Text[] enemy_Lv_Text;         // 적 레벨 텍스트 배열
    public Slider[] enemy_HP_Sliders;    // 적 체력 슬라이더 배열
    public GameObject[] enemy_Portrait_Obj; // 적 초상화 오브젝트 배열
    public Image[] enemy_Portrait_Img;    // 적 초상화 이미지 배열

    [Header("Panels")]
    public StatPanel statPanel;          // 스탯 패널
    public UpgradePanel upgradePanel;    // 업그레이드 패널
    public StorePanel storePanel;        // 상점 패널
    public SettingPanel settingPanel;    // 설정 패널
    public RestartPanel restartPanel;    // 재시작 패널

    [Header("Goods")]
    public Text money_Text;              // 돈 텍스트
    public Text potion_Text;             // 포션 텍스트
    public Text goldKey_Text;            // 골드 키 텍스트
    public Text silverKey_Text;          // 실버 키 텍스트

    [Header("Boss")]
    public Slider boss_HP_Slider;        // 보스 체력 슬라이더
    public Text boss_HP_Text;            // 보스 체력 텍스트

    [Header("Message")]
    public Image msgImg;                 // 메시지 배경 이미지
    public Text msgText;                 // 메시지 텍스트

    [Header("Tips")]
    public GameObject tipObj;            // 팁 오브젝트
    public Text tipText;                 // 팁 텍스트

    const string URL = "https://docs.google.com/spreadsheets/d/13knJXOsLj21ol2tIXEgwoYUo1SzDd6TzG9NOaLS03Ww/export?format=tsv&range=A2:B4";
    Coroutine msgCor;                    // 메시지 코루틴 참조

    // 게임 팁 텍스트 배열
    public string[] tips = {
        "좀비를 처치하면 랜덤한 위치에 새로운 좀비가 생성됩니다.",
        "플레이어는 좀비를 처치할 때마다 경험치를 얻습니다.",
        "레벨업을 하면 스탯이 자동으로 상승합니다.",
        "좀비를 처치하면 다시 생성될 때까지 시간이 걸립니다.",
        "자신보다 강한 좀비는 처치할 때 경험치를 더 많이 얻습니다.",
        "좀비가 죽은 위치에 플레이어가 위치하면 해당 위치에 새로운 좀비가 생성됩니다.",
        "레벨업 시에 스탯을 선택하여 상승시킬 수 있습니다."
    };

    /// <summary>
    /// UI 매니저 초기화
    /// </summary>
    public void Initialize()
    {
        Player_UI_Update();
        Goods_UI_Update();
        Stage_UI_Update();
        StartCoroutine(TipCor());
        StartCoroutine(GetCoupons());
        tipObj.SetActive(DataManager.Instance.gameData.toolTip);
    }

    /// <summary>
    /// 스테이지 UI 업데이트
    /// </summary>
    public void Stage_UI_Update()
    {
        name_Stage_Text.text = DataManager.Instance.gameConfig.map.floors[DataManager.Instance.gameData.curFloor_Num].stages[DataManager.Instance.gameData.curStage_Num].name;
        num_Stage_Text.text = (DataManager.Instance.gameData.curFloor_Num+1) + " - " + (DataManager.Instance.gameData.curStage_Num+1);
    }

    /// <summary>
    /// 플레이어 UI 업데이트
    /// </summary>
    public void Player_UI_Update()
    {
        player_Lv_Text.text = "Lv. " + DataManager.Instance.gameData.playerStat.lv;
        player_Exp_Slider.value = DataManager.Instance.gameData.playerStat.cur_Exp / DataManager.Instance.gameData.playerStat.full_Exp;
        player_HP_Slider.value = DataManager.Instance.gameData.playerStat.hp / DataManager.Instance.gameData.playerStat.Maxhp;
        if (statPanel.gameObject.activeSelf)
            statPanel.StatUpdate();
    }

    /// <summary>
    /// 레벨업 표시 오브젝트 활성화/비활성화
    /// </summary>
    public void LvUp_newObj(int teamNum, bool isOn)
    {
        if (teamNum == -1)
            player_Level_Up_Obj.SetActive(isOn);
        else
            team_Level_Up_Obj[teamNum].SetActive(isOn);
    }

    /// <summary>
    /// 몬스터 UI 업데이트
    /// </summary>
    public void Monster_UI_Update()
    {
        // 팀 몬스터 UI 업데이트
        for (int i = 0; i < DataManager.Instance.teamMonsters.Length; i++)
        {
            if (DataManager.Instance.gameData.teamStats[i].monsterType == MonsterTypeEnum.NONE)
                break;
            team_Lv_Text[i].text = DataManager.Instance.teamMonsters[i].monsterStat.lv.ToString();
            team_HP_Sliders[i].value = DataManager.Instance.teamMonsters[i].monsterStat.hp / DataManager.Instance.teamMonsters[i].monsterStat.Maxhp;
        }

        // 적 몬스터 UI 업데이트
        for (int i = 0; i < DataManager.Instance.enemyMonsters.Length; i++)
        {
            if (DataManager.Instance.enemyMonsters[i] == null)
                break;
            enemy_Lv_Text[i].text = DataManager.Instance.enemyMonsters[i].monsterStat.lv.ToString();
            enemy_HP_Sliders[i].value = DataManager.Instance.enemyMonsters[i].monsterStat.hp / DataManager.Instance.enemyMonsters[i].monsterStat.Maxhp;
        }

        Boss_UI_Update();
        if (statPanel.gameObject.activeSelf)
            statPanel.StatUpdate();
    }

    /// <summary>
    /// 아이템 UI 업데이트
    /// </summary>
    public void Goods_UI_Update()
    {
        money_Text.text = ((int)DataManager.Instance.gameData.possession.GetPossessionCount(PossessionTypeEnum.Money)).ToString("###,###,##0");
        potion_Text.text = DataManager.Instance.gameData.possession.GetPossessionCount(PossessionTypeEnum.Potion).ToString();
        goldKey_Text.text = DataManager.Instance.gameData.possession.GetPossessionCount(PossessionTypeEnum.GoldKey).ToString();
        silverKey_Text.text = DataManager.Instance.gameData.possession.GetPossessionCount(PossessionTypeEnum.SilverKey).ToString();
    }

    /// <summary>
    /// 보스 UI 업데이트
    /// </summary>
    public void Boss_UI_Update()
    {
        boss_HP_Slider.gameObject.SetActive(DataManager.Instance.gameConfig.map.floors[DataManager.Instance.gameData.curFloor_Num].stages[DataManager.Instance.gameData.curStage_Num].isBossStage && DataManager.Instance.gameData.isBossClear[DataManager.Instance.gameData.curFloor_Num] == false);
        if (DataManager.Instance.gameConfig.map.floors[DataManager.Instance.gameData.curFloor_Num].stages[DataManager.Instance.gameData.curStage_Num].isBossStage == false || DataManager.Instance.gameData.isBossClear[DataManager.Instance.gameData.curFloor_Num]) return;
        boss_HP_Slider.value = DataManager.Instance.bossMonster.monsterStat.hp / DataManager.Instance.bossMonster.monsterStat.Maxhp;
        boss_HP_Text.text = (int)DataManager.Instance.bossMonster.monsterStat.hp + " / " + (int)DataManager.Instance.bossMonster.monsterStat.Maxhp;
    }

    /// <summary>
    /// 새로운 몬스터 UI 생성
    /// </summary>
    public void NewMonsterUI(int num, bool isTeam, Monster mon)
    {
        if(isTeam)
        {
            team_Portrait_Img[num].gameObject.SetActive(true);
            team_HP_Sliders[num].gameObject.SetActive(true);
            team_Lv_Text[num].gameObject.SetActive(true);
            team_Portrait_Img[num].sprite = mon.portraitSprite;
            team_HP_Sliders[num].value = mon.monsterStat.hp / mon.monsterStat.Maxhp;
            team_Lv_Text[num].text = mon.monsterStat.lv.ToString();
        }
        else
        {
            enemy_Portrait_Obj[num].SetActive(true);
            enemy_Portrait_Img[num].sprite = mon.portraitSprite;
            enemy_HP_Sliders[num].value = mon.monsterStat.hp / mon.monsterStat.Maxhp;
            enemy_Lv_Text[num].text = mon.monsterStat.lv.ToString();
        }
    }

    /// <summary>
    /// 몬스터 UI 제거
    /// </summary>
    public void DestroyMonsterUI(int num, bool _isTeam)
    {
        if (_isTeam)
        {
            team_Portrait_Img[num].gameObject.SetActive(false);
            team_HP_Sliders[num].gameObject.SetActive(false);
            team_Lv_Text[num].gameObject.SetActive(false);
        }
        else
        {
            enemy_Portrait_Obj[num].SetActive(false);
        }
    }

    /// <summary>
    /// 메시지 표시
    /// </summary>
    public void Message(string msg)
    {
        if(msgCor == null)
            msgCor = StartCoroutine(MessageCor(msg));
        else
        {
            StopCoroutine(msgCor);
            msgCor = StartCoroutine(MessageCor(msg));
        }
        GameManager.Instance.PlayAudio(GameManager.Instance.message_Clip);
    }

    /// <summary>
    /// 메시지 표시 코루틴
    /// </summary>
    IEnumerator MessageCor(string msg)
    {
        Debug.Log(msg);
        msgImg.gameObject.SetActive(true);
        msgImg.color = new Color(msgImg.color.r, msgImg.color.g, msgImg.color.b, 175f / 255f);
        msgText.color = new Color(msgText.color.r, msgText.color.g, msgText.color.b, 175f / 255f);

        msgText.text = msg;
        yield return new WaitForSeconds(1f);
        for (int i = 175; i > 0; i -= 5)
        {
            msgImg.color = new Color(msgImg.color.r, msgImg.color.g, msgImg.color.b, ((float)i) / 255f);
            msgText.color = new Color(msgImg.color.r, msgImg.color.g, msgImg.color.b, ((float)i) / 255f);
            yield return new WaitForSeconds(0.05f);
        }
        msgImg.gameObject.SetActive(false);
        msgCor = null;
    }

    /// <summary>
    /// 쿠폰 데이터 가져오기
    /// </summary>
    IEnumerator GetCoupons()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;

        string[] money_potion = data.Split("\n");
        string[] moneyCou = new string[money_potion.Length];
        string[] potionCou = new string[money_potion.Length];

        for (int i = 0; i < money_potion.Length; i++)
        {
            moneyCou[i] = money_potion[i].Split("\t")[0];
            potionCou[i] = money_potion[i].Split("\t")[1];
        }
        settingPanel.moneyCoupons = moneyCou;
        settingPanel.potionCoupons = potionCou;
    }

    /// <summary>
    /// 팁 표시 코루틴
    /// </summary>
    IEnumerator TipCor()
    {
        while (true)
        {
            tipText.text = "* " + tips[Random.Range(0, tips.Length)];
            yield return new WaitForSeconds(10f);
        }
    }
}

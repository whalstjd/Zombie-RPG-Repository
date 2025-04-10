using DataInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatPanel : MonoBehaviour
{
    public Stat SelectedStat;
    public int selected_Team_num = 0;

    //name
    [Header("Name")]
    public InputField nameInputField;

    //lv, exp
    [Header("Lv_HP")]
    public Text lv_Text;
    public Slider exp_Slider;
    public Slider hp_Slider;

    //stat
    [Header("Stat")]
    public Text[] stat_Cur_Point_Texts;
    public Text[] stat_Max_Point_Texts;
    public Image[] stat_Upgrade_Img;

    public Text stat_Remain_Point_Text;
    public RectTransform stat_ShowOther;
    public GameObject stat_Window_1;
    public GameObject stat_Window_2;

    public Text[] stat_Values_Texts;

    public Sprite stat_Upgradeable_Sprite;
    public Sprite stat_Upgradeunable_Sprite;

    [Header("AutoPotion")]
    public Image autoPotion_Img;
    public Sprite autoPotion_Off_Sprite;
    public Sprite autoPotion_On_Sprite;
    public Text autoPotioin_Text;

    //Other
    [Header("Exile")]
    public GameObject exileWindowObj;


    private void Start()
    {
        SelectedStat = DataManager.Instance.gameData.playerStat;
    }
    public void ShowOtherStats()
    {
        stat_ShowOther.localScale *= -1;
        if (stat_Window_1.activeSelf)
        {
            stat_Window_1.SetActive(false);
            stat_Window_2.SetActive(true);
        }
        else
        {
            stat_Window_1.SetActive(true);
            stat_Window_2.SetActive(false);
        }
    }

    public void SelectTeam(int _num)
    {
        //������ ���� ������ '�÷��̾ �ƴϰ�' '����Ÿ���� none(������)'�̸� ����
        if (_num != -1 && DataManager.Instance.gameData.teamMonsterType[_num] == 0) return;

        gameObject.SetActive(true);

        selected_Team_num = _num;
        if (_num == -1)
        {
            SelectedStat = DataManager.Instance.gameData.playerStat;
            UIManager.Instance.LvUp_newObj(-1, false);
        }
        else
        {
            SelectedStat = DataManager.Instance.gameData.teamStats[_num];
            UIManager.Instance.LvUp_newObj(_num, false);
        }
        StatUpdate();
    }


    public void UpgradeStat(int i)
    {
        TrainingTypeEnum statType = (TrainingTypeEnum)i;
        //스탯 포인트 부족
        if (SelectedStat.trainingRemainPoint <= 0)
            return;
        //스탯 포인트 최대
        if (SelectedStat.trainingCurPoint[i] >= SelectedStat.trainingMaxPoint[i])
            return;
        if (stat_Upgrade_Img[i].sprite == stat_Upgradeunable_Sprite)
            return;

        if (selected_Team_num == -1)
        {
            DataManager.Instance.gameData.playerStat.SetPoint(statType, 1);
        }
        else
        {
            DataManager.Instance.gameData.teamStats[selected_Team_num].SetPoint(statType, 1);
        }
        GameManager.Instance.PlayAudio(GameManager.Instance.stat_Up_Clip);
        UIManager.Instance.Player_UI_Update();
        StatUpdate();
    }
    public void NameChange()
    {
        SelectedStat._name = nameInputField.text;
    }

    public void StatUpdate()
    {
        //name lv hp
        nameInputField.text = SelectedStat._name;
        lv_Text.text = "Lv." + SelectedStat.lv;
        exp_Slider.value = SelectedStat.cur_Exp / SelectedStat.full_Exp;
        hp_Slider.value = SelectedStat.hp / SelectedStat.Maxhp;

        //�ڵ����� Ȱ��ȭ����
        autoPotion_Img.sprite = SelectedStat.autoPotion ? autoPotion_On_Sprite : autoPotion_Off_Sprite;
        autoPotioin_Text.color = SelectedStat.autoPotion ? new Color(1, 157f / 255f, 107f / 255f) : new Color(200f / 255f, 200f / 255f, 200f / 255f);

        //stat
        stat_Remain_Point_Text.text = SelectedStat.trainingRemainPoint.ToString();
        for (int i = 0; i < stat_Cur_Point_Texts.Length; i++)
        {
            try
            {
                stat_Cur_Point_Texts[i].text = SelectedStat.trainingCurPoint[i].ToString();
                stat_Max_Point_Texts[i].text = "Max " + SelectedStat.trainingMaxPoint[i].ToString();
                if (SelectedStat.trainingRemainPoint > 0 && SelectedStat.trainingCurPoint[i] < SelectedStat.trainingMaxPoint[i])
                {
                    stat_Upgrade_Img[i].sprite = stat_Upgradeable_Sprite;
                }
                else
                {
                    stat_Upgrade_Img[i].sprite = stat_Upgradeunable_Sprite;
                }
            }
            catch { Debug.Log("Stat Update Error"); }
        }

        stat_Values_Texts[0].text = SelectedStat.Damage.ToString();
        stat_Values_Texts[1].text = SelectedStat.AttackSpeed + "초";
        stat_Values_Texts[2].text = Mathf.Round(SelectedStat.hp * 10) * 0.1f + "/" + SelectedStat.Maxhp;
        stat_Values_Texts[3].text = Mathf.Round((SelectedStat.Exp_Plus - 1) * 1000) * 0.1f + "%";
        stat_Values_Texts[4].text = Mathf.Round((SelectedStat.Money_Plus - 1) * 1000) * 0.1f + "%";
        stat_Values_Texts[5].text = Mathf.Round(SelectedStat.Item_Plus * 10) * 0.1f + "%";
    }


    public void AutoPotionChange()
    {
        SelectedStat.autoPotion = !SelectedStat.autoPotion;
        autoPotion_Img.sprite = SelectedStat.autoPotion ? autoPotion_On_Sprite : autoPotion_Off_Sprite;
        autoPotioin_Text.color = SelectedStat.autoPotion ? new Color(1, 157f / 255f, 107f / 255f) : new Color(200f / 255f, 200f / 255f, 200f / 255f);

        if (SelectedStat.autoPotion && SelectedStat.hp * 2 < SelectedStat.Maxhp)
            UsePotion();
    }

    public void UsePotion()
    {
        SelectedStat.UsePotion();
        UIManager.Instance.Player_UI_Update();
        UIManager.Instance.Monster_UI_Update();
        UIManager.Instance.Goods_UI_Update();
        StatUpdate();
    }

    public void ExileTeam()
    {
        if (SelectedStat.entityType == EntityType.PLAYER)
        {
            UIManager.Instance.Message("플레이어를 추방할 수 없습니다!");
            gameObject.SetActive(false);
            return;
        }

        if (SelectedStat.entityType != EntityType.TEAM)
        {
            UIManager.Instance.Message("해당 몬스터는 이미 적입니다!");
            gameObject.SetActive(false);
            return;
        }
        GameManager.Instance.TeamChange(DataManager.Instance.teamMonsters[selected_Team_num], false);
        gameObject.SetActive(false);
    }
}

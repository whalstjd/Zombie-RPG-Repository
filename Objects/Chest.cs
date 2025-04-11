using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataInfo;

public class Chest : MonoBehaviour
{
    public int decoCount;
    public enum Chest_Type { CHEST_0, CHEST_1, CHEST_2 }
    public Chest_Type chest_Type = 0;

    public Vector2Int randPotion;
    public Vector2 randExp;
    public Vector2Int randSilver;
    public Vector2Int randGold;

    public Sprite brokenChest;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (chest_Type)
            {
                case Chest_Type.CHEST_0:
                    OpenChest();
                    break;
                case Chest_Type.CHEST_1:
                    if(DataManager.Instance.gameData.possession.GetPossessionCount(PossessionTypeEnum.SilverKey) > 0)
                    {
                        DataManager.Instance.gameData.possession.RemovePossession(PossessionTypeEnum.SilverKey, 1);
                        OpenChest();
                    }
                    else
                    {
                        UIManager.Instance.Message("실버 키가 부족합니다.");
                    }
                    break;
                case Chest_Type.CHEST_2:
                    if (DataManager.Instance.gameData.possession.GetPossessionCount(PossessionTypeEnum.GoldKey) > 0)
                    {
                        DataManager.Instance.gameData.possession.RemovePossession(PossessionTypeEnum.GoldKey, 1);
                        OpenChest();
                    }
                    else
                    {
                        UIManager.Instance.Message("골드 키가 부족합니다.");
                    }
                    break;
            }
        }
    }

    void OpenChest()
    {
        GameManager.Instance.PlayAudio(GameManager.Instance.chest_Unlock_Clip);

        int randInt = Random.Range(0, 4);
        int value = 0;
        switch (randInt)
        {
            case 0:
                value = Random.Range(randPotion.x, randPotion.y-1);
                UIManager.Instance.Message("포션 " + value + "개 획득!");
                DataManager.Instance.gameData.possession.AddPossession(PossessionTypeEnum.Potion, value);
                break;
            case 1:
                value = (int)(DataManager.Instance.gameData.playerStat.full_Exp * Random.Range(randExp.x, randExp.y));
                UIManager.Instance.Message("경험치 " + System.Math.Round(value / DataManager.Instance.gameData.playerStat.full_Exp, 1)*100 + "% 획득!");
                DataManager.Instance.gameData.playerStat.SetExp(value);
                break;
            case 2:
                value = Random.Range(randSilver.x, randSilver.y - 1);
                UIManager.Instance.Message("실버 키 " + value + "개 획득!");
                DataManager.Instance.gameData.possession.AddPossession(PossessionTypeEnum.SilverKey, value);
                break;
            case 3:
                value = Random.Range(randGold.x, randGold.y - 1);
                UIManager.Instance.Message("골드 키 " + value + "개 획득!");
                DataManager.Instance.gameData.possession.AddPossession(PossessionTypeEnum.GoldKey, value);
                break;
        }
        Debug.Log(value);
        GameManager.Instance.cur_Stage.decorations.RemoveAt(decoCount);
        GameManager.Instance.cur_Stage.decoration_pos.RemoveAt(decoCount);
        gameObject.GetComponent<SpriteRenderer>().sprite = brokenChest;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        UIManager.Instance.Goods_UI_Update();
        UIManager.Instance.Player_UI_Update();
    }
}

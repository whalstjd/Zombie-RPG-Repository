using DataInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartPanel : MonoBehaviour
{
    public void Ad_Restart()
    {
        // GameManager.Instance.ad.ShowRewardAd(ItemEnum.Potion, 5, RewardType.RESURRECTION_AND_POTION);
    }

    public void Default_Restart()
    {
        // GameManager.Instance.ad.ShowAd();

        DataManager.Instance.gameData.playerStat.hp = DataManager.Instance.gameData.playerStat.Maxhp;
        GameManager.Instance.player.isDie = false;
        GameManager.Instance.player.animator.SetInteger("State", 0);

        UIManager.Instance.restartPanel.gameObject.SetActive(false);

        DataManager.Instance.gameData.curFloor_Num = 0;
        DataManager.Instance.gameData.curStage_Num = 0;
        GameManager.Instance.LoadField(0, 0);
        UIManager.Instance.Goods_UI_Update();
        UIManager.Instance.Player_UI_Update();
        UIManager.Instance.Boss_UI_Update();
        UIManager.Instance.Monster_UI_Update();
    }
}

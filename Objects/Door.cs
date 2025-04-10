using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    public Transform sliderPos;
    public Slider doorSlider;

    public Vector2Int dir;
    public float time = 0;
    public float enterTime = 1;

    private void Start()
    {
        doorSlider.transform.position = sliderPos.position;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            time += Time.deltaTime;
            doorSlider.value = time / enterTime;
        }
        if (time > enterTime)
        {
            DataManager.Instance.gameData.curStage_Num += dir.x;

            //레벨 제한
            if (DataManager.Instance.gameData.curStage_Num > DataManager.Instance.gameConfig.map.floors[DataManager.Instance.gameData.curFloor_Num].stages.Length - 1)
            {
                if (DataManager.Instance.gameData.playerStat.lv < DataManager.Instance.gameConfig.map.floors[DataManager.Instance.gameData.curFloor_Num + 1].lvLimit)
                {
                    DataManager.Instance.gameData.curStage_Num -= dir.x;
                    GameManager.Instance.PlayAudio(GameManager.Instance.error_Clip);
                    UIManager.Instance.Message("레벨이 부족합니다.\n필요 레벨 : Lv." + DataManager.Instance.gameConfig.map.floors[DataManager.Instance.gameData.curFloor_Num + 1].lvLimit);
                    time = -0.5f;
                    return;
                }
            }

            //층 이동 확인
            if (DataManager.Instance.gameData.curStage_Num < 0)
            {
                DataManager.Instance.gameData.curFloor_Num--;
                DataManager.Instance.gameData.curStage_Num = DataManager.Instance.gameConfig.map.floors[DataManager.Instance.gameData.curFloor_Num].stages.Length - 1;
            }
            else if (DataManager.Instance.gameData.curStage_Num > DataManager.Instance.gameConfig.map.floors[DataManager.Instance.gameData.curFloor_Num].stages.Length - 1)
            {
                DataManager.Instance.gameData.curFloor_Num++;
                DataManager.Instance.gameData.curStage_Num = 0;
            }
            collision.transform.position = new Vector2(dir.x * -9, dir.y * -4f);
            GameManager.Instance.PlayAudio(GameManager.Instance.door_Clip);
            GameManager.Instance.LoadField(DataManager.Instance.gameData.curFloor_Num, DataManager.Instance.gameData.curStage_Num);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        time = 0;
        doorSlider.value = 0;
    }
}
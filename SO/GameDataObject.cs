using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataInfo;
using System;
using UnityEditor;

[CreateAssetMenu(fileName = "GameDataSO", menuName = "Create GameData", order = 1)]
public class GameDataObject : ScriptableObject
{
    public GameData gameData;

    private void OnEnable()
    {
        if (gameData == null)
        {
            RestoreDefaultValues();
        }
    }

    public void RestoreDefaultValues()
    {
        gameData = new GameData();
        
        // 플레이어 스탯 기본값 설정
        gameData.playerStat = new Stat
        {
            _name = "Player",
            entityType = EntityType.PLAYER,
            autoPotion = false,
            lv = 1,
            cur_Exp = 0,
            exp_Difficulty = 1,
            trainingCurPoint = new int[6] { 0, 0, 0, 0, 0, 0 },
            trainingMaxPoint = new int[6] { 30, 30, 30, 30, 30, 30 },
        };
        gameData.playerStat.Initialize();

        // 팀 몬스터 타입 기본값 설정
        gameData.teamMonsterType = new MonsterTypeEnum[6];
        for (int i = 0; i < gameData.teamMonsterType.Length; i++)
        {
            gameData.teamMonsterType[i] = MonsterTypeEnum.NONE;
        }

        // 팀 몬스터 스탯 기본값 설정
        gameData.teamStats = new Stat[6];
        for (int i = 0; i < gameData.teamStats.Length; i++)
        {
            gameData.teamStats[i] = new Stat
            {
                _name = "",
                entityType = EntityType.TEAM,
                autoPotion = false,
                lv = 1,
                cur_Exp = 0,
                exp_Difficulty = 0,
                trainingCurPoint = new int[6] { 0, 0, 0, 0, 0, 0 },
                trainingMaxPoint = new int[6] { 30, 30, 30, 30, 30, 30 },
            };
            gameData.teamStats[i].Initialize();
        }

        // 아이템 기본값 설정
        gameData.items = new Item();
        gameData.items.AddItem(ItemTypeEnum.Money, 300);
        gameData.items.AddItem(ItemTypeEnum.Potion, 10);
        gameData.items.AddItem(ItemTypeEnum.GoldKey, 1);
        gameData.items.AddItem(ItemTypeEnum.SilverKey, 2);

        // 업그레이드 레벨 기본값 설정
        gameData.playerUpgradeLv = new int[6] { 0, 0, 0, 0, 0, 0 };
        gameData.teamUpgradeLv = new int[6] { 0, 0, 0, 0, 0, 0 };
        gameData.weaponUpgradeLv = new int[2] { 0, 0 };

        // 맵 데이터 기본값 설정
        gameData.curFloor_Num = 0;
        gameData.curStage_Num = 0;
        gameData.isBossClear = new bool[13]; // 13개의 스테이지에 대한 보스 클리어 상태
        for (int i = 0; i < 13; i++)
        {
            gameData.isBossClear[i] = false;
        }

        // 시스템 상태 기본값 설정
        gameData.isBeginner = true;
        gameData.adSkip = false;
        gameData.effectSound = 0.5f;
        gameData.bgmSound = 0.5f;
        gameData.toolTip = true;
        gameData.usedCoupons = new List<string>();
        
        // 일일 보상 날짜 초기화
        DateTime today = DateTime.Today;
        gameData.last_Daily_Year = new int[6];
        gameData.last_Daily_Month = new int[6];
        gameData.last_Daily_Day = new int[6];
        for (int i = 0; i < 6; i++)
        {
            gameData.last_Daily_Year[i] = 2025;
            gameData.last_Daily_Month[i] = 4;
            gameData.last_Daily_Day[i] = 10;
        }

        // 변경사항 저장
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameDataObject))]
public class GameDataObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameDataObject gameDataObject = (GameDataObject)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("기본값 복구"))
        {
            if (EditorUtility.DisplayDialog("기본값 복구",
                "게임 데이터를 기본값으로 복구합니다.\n현재 진행 상황이 모두 기본값으로 변경됩니다.\n계속하시겠습니까?",
                "복구", "취소"))
            {
                gameDataObject.RestoreDefaultValues();
                EditorUtility.DisplayDialog("복구 완료", "게임 데이터가 기본값으로 복구되었습니다.", "확인");
            }
        }
    }
}
#endif
using UnityEngine;
using DataInfo;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "GameConfig", menuName = "Create GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    [Header("업그레이드 설정")]
    public List<Upgrade> upgrades = new List<Upgrade>();

    [Header("트레이닝 설정")]
    public List<Training> trainings = new List<Training>();
    

    [Header("맵 설정")]
    [Tooltip("맵 데이터")]
    public Map map = new Map();

    public void InitializeTrainings()
    {
        trainings = new List<Training>
        {
            new Training 
            { 
                typeEnum = TrainingTypeEnum.AttackDamage,
                baseValue = 1f,
                increaseRate = 0.2f,
                pointIncreaseRate = 1.2f
            },
            new Training 
            { 
                typeEnum = TrainingTypeEnum.AttackSpeed,
                baseValue = 1f,
                increaseRate = 0.2f,
                pointIncreaseRate = 1.2f
            },
            new Training 
            { 
                typeEnum = TrainingTypeEnum.MaxHP,
                baseValue = 1f,
                increaseRate = 0.2f,
                pointIncreaseRate = 1.2f
            },
            new Training 
            { 
                typeEnum = TrainingTypeEnum.Exp,
                baseValue = 1f,
                increaseRate = 0.2f,
                pointIncreaseRate = 1.2f
            },
            new Training 
            { 
                typeEnum = TrainingTypeEnum.Money,
                baseValue = 1f,
                increaseRate = 0.2f,
                pointIncreaseRate = 1.2f
            },
            new Training 
            { 
                typeEnum = TrainingTypeEnum.Item,
                baseValue = 1f,
                increaseRate = 0.2f,
                pointIncreaseRate = 1.2f
            }
        };
    }

    public void InitializeUpgrades()
    {
        upgrades = new List<Upgrade>
        {
            // 플레이어 업그레이드
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.PLAYER,
                typeEnum = UpgradeTypeEnum.AttackDamage,
                baseValue = 1f,
                increaseRate = 0.01f,
                basePrice = 5f,
                priceIncreaseRate = 2f,
                maxLv = 100
            },
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.PLAYER,
                typeEnum = UpgradeTypeEnum.AttackSpeed,
                baseValue = 1f,
                increaseRate = 0.025f,
                basePrice = 5f,
                priceIncreaseRate = 2f,
                maxLv = 100
            },
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.PLAYER,
                typeEnum = UpgradeTypeEnum.MaxHP,
                baseValue = 1f,
                increaseRate = 0.075f,
                basePrice = 5f,
                priceIncreaseRate = 2f,
                maxLv = 100
            },
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.PLAYER,
                typeEnum = UpgradeTypeEnum.Exp,
                baseValue = 1f,
                increaseRate = 0.025f,
                basePrice = 5f,
                priceIncreaseRate = 2f,
                maxLv = 100
            },
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.PLAYER,
                typeEnum = UpgradeTypeEnum.Money,
                baseValue = 1f,
                increaseRate = 0.025f,
                basePrice = 5f,
                priceIncreaseRate = 2f,
                maxLv = 100
            },
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.PLAYER,
                typeEnum = UpgradeTypeEnum.Item,
                baseValue = 1f,
                increaseRate = 0.025f,
                basePrice = 5f,
                priceIncreaseRate = 2f,
                maxLv = 100
            },

            // 팀 업그레이드
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.TEAM,
                typeEnum = UpgradeTypeEnum.AttackDamage,
                baseValue = 1f,
                increaseRate = 0.01f,
                basePrice = 5f,
                priceIncreaseRate = 2f,
                maxLv = 100
            },
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.TEAM,
                typeEnum = UpgradeTypeEnum.AttackSpeed,
                baseValue = 1f,
                increaseRate = 0.025f,
                basePrice = 5f,
                priceIncreaseRate = 2f,
                maxLv = 100
            },
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.TEAM,
                typeEnum = UpgradeTypeEnum.MaxHP,
                baseValue = 1f,
                increaseRate = 0.075f,
                basePrice = 5f,
                priceIncreaseRate = 2f,
                maxLv = 100
            },
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.TEAM,
                typeEnum = UpgradeTypeEnum.Exp,
                baseValue = 1f,
                increaseRate = 0.025f,
                basePrice = 5f,
                priceIncreaseRate = 2f,
                maxLv = 100
            },
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.TEAM,
                typeEnum = UpgradeTypeEnum.Money,
                baseValue = 1f,
                increaseRate = 0.025f,
                basePrice = 5f,
                priceIncreaseRate = 2f,
                maxLv = 100
            },
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.TEAM,
                typeEnum = UpgradeTypeEnum.Item,
                baseValue = 1f,
                increaseRate = 0.025f,
                basePrice = 5f,
                priceIncreaseRate = 2f,
                maxLv = 100
            },

            // 무기 업그레이드
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.WEAPON,
                typeEnum = UpgradeTypeEnum.AttackDamage,
                baseValue = 5f,
                increaseRate = 2f,
                basePrice = 5f,
                priceIncreaseRate = 3f,
                maxLv = 100,
                successRate = 1f
            },
            new Upgrade
            {
                targetEnum = UpgradeTargetEnum.WEAPON,
                typeEnum = UpgradeTypeEnum.MaxHP,
                baseValue = 30f,
                increaseRate = 8f,
                basePrice = 5f,
                priceIncreaseRate = 3f,
                maxLv = 100,
                successRate = 1f
            }
        };
    }

    public void InitializeMap()
    {
        map = new Map
        {
            floors = new Floor[13]
            {
                new Floor { name = "좀비 스테이지", lvLimit = 1, stages = new Stage[4] 
                {
                    new Stage { name = "시작의 문", isBossStage = false, spawnMonsterType = MonsterTypeEnum.NONE, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "부서진 거리", isBossStage = false, spawnMonsterType = MonsterTypeEnum.ZOMBIE, spawnCount = 3, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "좀비의 소굴", isBossStage = false, spawnMonsterType = MonsterTypeEnum.ZOMBIE, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "좀비 군단장", isBossStage = true, spawnMonsterType = MonsterTypeEnum.ZOMBIE, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }},
                new Floor { name = "슬라임 스테이지", lvLimit = 5, stages = new Stage[4]
                {
                    new Stage { name = "습지의 시작", isBossStage = false, spawnMonsterType = MonsterTypeEnum.SLIME, spawnCount = 2, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "점액의 늪", isBossStage = false, spawnMonsterType = MonsterTypeEnum.SLIME, spawnCount = 4, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "슬라임 군단", isBossStage = false, spawnMonsterType = MonsterTypeEnum.SLIME, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "슬라임 왕", isBossStage = true, spawnMonsterType = MonsterTypeEnum.SLIME, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }},
                new Floor { name = "박쥐 스테이지", lvLimit = 10, stages = new Stage[4]
                {
                    new Stage { name = "어두운 동굴", isBossStage = false, spawnMonsterType = MonsterTypeEnum.BAT, spawnCount = 2, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "박쥐의 소굴", isBossStage = false, spawnMonsterType = MonsterTypeEnum.BAT, spawnCount = 4, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "박쥐 군단", isBossStage = false, spawnMonsterType = MonsterTypeEnum.BAT, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "박쥐 여왕", isBossStage = true, spawnMonsterType = MonsterTypeEnum.BAT, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }},
                new Floor { name = "쥐 스테이지", lvLimit = 15, stages = new Stage[4]
                {
                    new Stage { name = "하수구 입구", isBossStage = false, spawnMonsterType = MonsterTypeEnum.RAT, spawnCount = 2, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "쥐의 터널", isBossStage = false, spawnMonsterType = MonsterTypeEnum.RAT, spawnCount = 4, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "쥐의 군단", isBossStage = false, spawnMonsterType = MonsterTypeEnum.RAT, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "쥐의 왕", isBossStage = true, spawnMonsterType = MonsterTypeEnum.RAT, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }},
                new Floor { name = "거미 스테이지", lvLimit = 20, stages = new Stage[4]
                {
                    new Stage { name = "거미의 둥지", isBossStage = false, spawnMonsterType = MonsterTypeEnum.SPIDER, spawnCount = 2, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "거미줄의 미로", isBossStage = false, spawnMonsterType = MonsterTypeEnum.SPIDER, spawnCount = 4, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "거미 군단", isBossStage = false, spawnMonsterType = MonsterTypeEnum.SPIDER, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "거미 여왕", isBossStage = true, spawnMonsterType = MonsterTypeEnum.SPIDER, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }},
                new Floor { name = "까마귀 스테이지", lvLimit = 25, stages = new Stage[4]
                {
                    new Stage { name = "까마귀의 둥지", isBossStage = false, spawnMonsterType = MonsterTypeEnum.CROW, spawnCount = 2, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "까마귀의 소굴", isBossStage = false, spawnMonsterType = MonsterTypeEnum.CROW, spawnCount = 4, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "까마귀 군단", isBossStage = false, spawnMonsterType = MonsterTypeEnum.CROW, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "까마귀 군주", isBossStage = true, spawnMonsterType = MonsterTypeEnum.CROW, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }},
                new Floor { name = "고블린 스테이지", lvLimit = 30, stages = new Stage[4]
                {
                    new Stage { name = "고블린 마을", isBossStage = false, spawnMonsterType = MonsterTypeEnum.GOBLIN, spawnCount = 2, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "고블린 요새", isBossStage = false, spawnMonsterType = MonsterTypeEnum.GOBLIN, spawnCount = 4, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "고블린 군단", isBossStage = false, spawnMonsterType = MonsterTypeEnum.GOBLIN, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "고블린 왕", isBossStage = true, spawnMonsterType = MonsterTypeEnum.GOBLIN, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }},
                new Floor { name = "스켈레톤 스테이지", lvLimit = 35, stages = new Stage[4]
                {
                    new Stage { name = "망자의 무덤", isBossStage = false, spawnMonsterType = MonsterTypeEnum.SKELETON, spawnCount = 2, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "해골의 전당", isBossStage = false, spawnMonsterType = MonsterTypeEnum.SKELETON, spawnCount = 4, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "스켈레톤 군단", isBossStage = false, spawnMonsterType = MonsterTypeEnum.SKELETON, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "스켈레톤 군주", isBossStage = true, spawnMonsterType = MonsterTypeEnum.SKELETON, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }},
                new Floor { name = "유령 스테이지", lvLimit = 40, stages = new Stage[4]
                {
                    new Stage { name = "유령의 집", isBossStage = false, spawnMonsterType = MonsterTypeEnum.GHOST, spawnCount = 2, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "망령의 방", isBossStage = false, spawnMonsterType = MonsterTypeEnum.GHOST, spawnCount = 4, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "유령 군단", isBossStage = false, spawnMonsterType = MonsterTypeEnum.GHOST, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "유령 군주", isBossStage = true, spawnMonsterType = MonsterTypeEnum.GHOST, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }},
                new Floor { name = "오크 스테이지", lvLimit = 45, stages = new Stage[4]
                {
                    new Stage { name = "오크의 영토", isBossStage = false, spawnMonsterType = MonsterTypeEnum.ORC, spawnCount = 2, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "오크의 요새", isBossStage = false, spawnMonsterType = MonsterTypeEnum.ORC, spawnCount = 4, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "오크 군단", isBossStage = false, spawnMonsterType = MonsterTypeEnum.ORC, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "오크 대장", isBossStage = true, spawnMonsterType = MonsterTypeEnum.ORC, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }},
                new Floor { name = "벌레 스테이지", lvLimit = 50, stages = new Stage[4]
                {
                    new Stage { name = "벌레의 굴", isBossStage = false, spawnMonsterType = MonsterTypeEnum.WORM, spawnCount = 2, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "벌레의 소굴", isBossStage = false, spawnMonsterType = MonsterTypeEnum.WORM, spawnCount = 4, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "벌레 군단", isBossStage = false, spawnMonsterType = MonsterTypeEnum.WORM, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "벌레 여왕", isBossStage = true, spawnMonsterType = MonsterTypeEnum.WORM, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }},
                new Floor { name = "베홀더 스테이지", lvLimit = 55, stages = new Stage[4]
                {
                    new Stage { name = "베홀더의 방", isBossStage = false, spawnMonsterType = MonsterTypeEnum.BEHOLDER, spawnCount = 2, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "베홀더의 전당", isBossStage = false, spawnMonsterType = MonsterTypeEnum.BEHOLDER, spawnCount = 4, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "베홀더 군단", isBossStage = false, spawnMonsterType = MonsterTypeEnum.BEHOLDER, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "베홀더 군주", isBossStage = true, spawnMonsterType = MonsterTypeEnum.BEHOLDER, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }},
                new Floor { name = "사이클롭스 스테이지", lvLimit = 60, stages = new Stage[4]
                {
                    new Stage { name = "거인의 영토", isBossStage = false, spawnMonsterType = MonsterTypeEnum.CYCLOPE, spawnCount = 2, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "거인의 요새", isBossStage = false, spawnMonsterType = MonsterTypeEnum.CYCLOPE, spawnCount = 4, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "사이클롭스 군단", isBossStage = false, spawnMonsterType = MonsterTypeEnum.CYCLOPE, spawnCount = 6, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() },
                    new Stage { name = "사이클롭스 왕", isBossStage = true, spawnMonsterType = MonsterTypeEnum.CYCLOPE, spawnCount = 0, decorations = new List<DecorationEnum>(), decoration_pos = new List<Vector2>() }
                }}
            }
        };
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameConfig))]
public class GameConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameConfig gameConfig = (GameConfig)target;


        EditorGUILayout.Space();
        if (GUILayout.Button("업그레이드 설정 초기화"))
        {
            if (EditorUtility.DisplayDialog("업그레이드 설정 초기화",
                "업그레이드 설정을 기본값으로 초기화합니다.\n계속하시겠습니까?",
                "초기화", "취소"))
            {
                gameConfig.InitializeUpgrades();
                EditorUtility.SetDirty(gameConfig);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("초기화 완료", "업그레이드 설정이 초기화되었습니다.", "확인");
            }
        }


        EditorGUILayout.Space();
        if (GUILayout.Button("트레이닝 설정 초기화"))
        {
            if (EditorUtility.DisplayDialog("트레이닝 설정 초기화",
                "트레이닝 설정을 기본값으로 초기화합니다.\n계속하시겠습니까?",
                "초기화", "취소"))
            {
                gameConfig.InitializeTrainings();
                EditorUtility.SetDirty(gameConfig);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("초기화 완료", "트레이닝 설정이 초기화되었습니다.", "확인");
            }
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("맵 설정 초기화"))
        {
            if (EditorUtility.DisplayDialog("맵 설정 초기화",
                "맵 설정을 기본값으로 초기화합니다.\n계속하시겠습니까?",
                "초기화", "취소"))
            {
                gameConfig.InitializeMap();
                EditorUtility.SetDirty(gameConfig);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("초기화 완료", "맵 설정이 초기화되었습니다.", "확인");
            }
        }
    }
}
#endif
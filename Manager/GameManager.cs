using DataInfo;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    //맵 매니저, 오디오 매니저
    private static GameManager instance;

    public Player player;
    public Transform entityParent;

    [Header("current Data")]
    public Floor cur_Floor;
    public Stage cur_Stage;

    [Header("Floor And Door")]
    public GameObject stage_Floor_1, stage_Floor_2;

    public GameObject left_Close_Door_1, left_Open_Door_1;
    public GameObject left_Close_Door_2, left_Open_Door_2;
    public GameObject right_Close_Door_1, right_Open_Door_1;
    public GameObject right_Close_Door_2, right_Open_Door_2;

    [Header("Movement Limit")]
    public Vector2 moveLimit_Min;
    public Vector2 moveLimit_Max;

    [Header("Cut_Scene")]
    public GameObject cutScene;

    [Header("Pay")]
    public GameObject payPanel;
    public GameObject payPanel_2;

    #region Audio
    [Header("Clips")]
    public AudioSource BGM_AudioSource;
    public AudioSource gm_AudioSource;
    public AudioClip bgm_Clip;
    public AudioClip player_Skill_Swap_Clip;
    public AudioClip player_Die_Clip;
    public AudioClip monster_Die_Clip;
    public AudioClip boss_Die_Clip;
    public AudioClip damaged_Clip;
    public AudioClip heal_Clip;
    public AudioClip chest_Unlock_Clip;
    public AudioClip stat_Up_Clip;
    public AudioClip upgradable_Clip;
    public AudioClip unupgradable_Clip;
    public AudioClip message_Clip;
    public AudioClip door_Clip;
    public AudioClip slash_Clip;
    public AudioClip net_Clip;
    public AudioClip cutScene_Clip;
    public AudioClip error_Clip;
    #endregion

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

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    public void Start()
    {
        DataManager.Instance.Initialize();
        LoadField(DataManager.Instance.gameData.curFloor_Num, DataManager.Instance.gameData.curStage_Num);
        UIManager.Instance.Initialize();
    }

    public void LoadField(int floorNum, int stageNum)
    {
        if (DataManager.Instance.gameConfig == null ||
            DataManager.Instance.gameConfig.map == null || 
            DataManager.Instance.gameConfig.map.floors == null || 
            floorNum >= DataManager.Instance.gameConfig.map.floors.Length)
        {
            Debug.LogError("맵 데이터가 초기화되지 않았거나 잘못된 층 번호입니다.");
            return;
        }

        cur_Floor = DataManager.Instance.gameConfig.map.floors[floorNum];
        
        if (cur_Floor == null ||
            cur_Floor.stages == null || 
            stageNum >= cur_Floor.stages.Length)
        {
            Debug.LogError("스테이지 데이터가 초기화되지 않았거나 잘못된 스테이지 번호입니다.");
            return;
        }

        cur_Stage = cur_Floor.stages[stageNum];
        UIManager.Instance.Stage_UI_Update();
        UIManager.Instance.Goods_UI_Update();
        Destroy_Data_and_Entities();
        Decorations();
        DoorActive();
        TeamSpawn();
        BossSpawn();
        EnemySpawn();
    }

    void Destroy_Data_and_Entities()
    {
        int enemyCount = DataManager.Instance.enemyMonsters.Length;
        int entityCount = entityParent.childCount;
        //적 몬스터 제거
        for (int i = 0; i < enemyCount; i++)
        {
            UIManager.Instance.DestroyMonsterUI(i, false);
            DataManager.Instance.enemyMonsters[i] = null;
        }
        //플레이어를 제외한 모든 엔티티 제거
        for (int i = 0; i < entityCount; i++)
        {
            if (entityParent.GetChild(i).CompareTag("Player")) continue;
            Destroy(entityParent.GetChild(i).gameObject);
        }
    }

    void Decorations()
    {
        if (cur_Stage.decorations.Count <= 0) return;

        for (int i = 0; i < cur_Stage.decorations.Count; i++)
        {
            int decoInteger = (int)cur_Stage.decorations[i];
            GameObject obj = Instantiate(DataManager.Instance.decorationsPrefab[decoInteger]
                                        , cur_Stage.decoration_pos[i]
                                        , Quaternion.identity);
            obj.transform.SetParent(entityParent);

            if(cur_Stage.decorations[i] == DecorationEnum.chest_0_closed
                || cur_Stage.decorations[i] == DecorationEnum.chest_1_closed
                || cur_Stage.decorations[i] == DecorationEnum.chest_2_closed)
            {
                obj.GetComponent<Chest>().decoCount = i;
            }
        }
    }
    
    public void DoorActive()
    {
        bool is_1_Chapter = (DataManager.Instance.gameData.curFloor_Num < 7) ? true : false;
        bool isOpen = (cur_Stage.isBossStage == false || DataManager.Instance.gameData.isBossClear[DataManager.Instance.gameData.curFloor_Num]) ? true : false;

        stage_Floor_1.SetActive(is_1_Chapter);
        stage_Floor_2.SetActive(is_1_Chapter == false);
        if (DataManager.Instance.gameData.curFloor_Num == 0 && DataManager.Instance.gameData.curStage_Num == 0)
            DoorCtrl(is_1_Chapter, true, false); //왼쪽 문, 닫힘
        else
            DoorCtrl(is_1_Chapter, true, true); //왼쪽 문, 열림

        DoorCtrl(is_1_Chapter, false, isOpen); //오른쪽 문, 열림
    }
    void DoorCtrl(bool is_1_Chapter, bool isLeft, bool isOpen)
    {
        if (is_1_Chapter)
        {
            if (isLeft)
            {
                if (isOpen)
                {
                    left_Open_Door_1.SetActive(true);
                    left_Close_Door_1.SetActive(false);
                }
                else
                {
                    left_Open_Door_1.SetActive(false);
                    left_Close_Door_1.SetActive(true);
                }
            }
            else
            {
                if (isOpen)
                {
                    right_Open_Door_1.SetActive(true);
                    right_Close_Door_1.SetActive(false);
                }
                else
                {
                    right_Open_Door_1.SetActive(false);
                    right_Close_Door_1.SetActive(true);
                }
            }
        }
        else
        {
            if (isLeft)
            {
                if (isOpen)
                {
                    left_Open_Door_2.SetActive(true);
                    left_Close_Door_2.SetActive(false);
                }
                else
                { 
                    left_Open_Door_2.SetActive(false);
                    left_Close_Door_2.SetActive(true);
                }
            }
            else
            {
                if (isOpen)
                {
                    right_Open_Door_2.SetActive(true);
                    right_Close_Door_2.SetActive(false);
                }
                else
                {
                    right_Open_Door_2.SetActive(false);
                    right_Close_Door_2.SetActive(true);
                }
            }
        }
    }

    void TeamSpawn()
    {
        //gameData에 저장된 팀 몬스터 데이터를 불러와서 스폰
        for (int i = 0; i < DataManager.Instance.gameData.teamStats.Length; i++)
        {
            if (DataManager.Instance.gameData.teamStats[i] == null) continue;
            if(DataManager.Instance.gameData.teamStats[i].monsterType == MonsterTypeEnum.NONE) continue;

            Vector2 pos;
            //위치 설정
            {
                float pos_x, pos_y;
                pos_x = Random.Range(player.transform.position.x - 3, player.transform.position.x + 3);
                pos_y = Random.Range(player.transform.position.y - 2, player.transform.position.y + 2);
                pos = new Vector2(pos_x, pos_y);
            }
            GameObject teamObj = Instantiate(DataManager.Instance.monstersPrefab[(int)DataManager.Instance.gameData.teamStats[i].monsterType], pos, Quaternion.identity);

            //위치가 맵 밖으로 나가면 안되므로 제한
            {
                if (teamObj.transform.position.x < moveLimit_Min.x)
                    teamObj.transform.position = new Vector2(moveLimit_Min.x, teamObj.transform.position.y);
                else if (teamObj.transform.position.x > moveLimit_Max.x)
                    teamObj.transform.position = new Vector2(moveLimit_Max.x, teamObj.transform.position.y);

                if (teamObj.transform.position.y < moveLimit_Min.y)
                    teamObj.transform.position = new Vector2(moveLimit_Min.x, teamObj.transform.position.y);
                else if (teamObj.transform.position.y > moveLimit_Max.y)
                    teamObj.transform.position = new Vector2(moveLimit_Max.x, teamObj.transform.position.y);
            }

            //Other Settings
            {
                Monster teamMon = DataManager.Instance.teamMonsters[i] = teamObj.GetComponent<Monster>();
                teamMon.tag = "Team";
                teamObj.layer = LayerMask.NameToLayer("Team");
                teamMon.name = "Team" + i;
                teamMon.teamNum = i;
                teamMon.belt.color = new Color(0, 1, 0, 200f / 255f);
                teamMon.monsterStat = DataManager.Instance.gameData.teamStats[i];
                teamMon.monsterStat.Initialize(teamObj);
                teamMon.transform.SetParent(entityParent);
                teamMon.curState = MonsterState.MOVE;
                UIManager.Instance.NewMonsterUI(i, true, teamMon);
            }
        }
    }

    void BossSpawn()
    {
        if (cur_Stage.isBossStage == false || DataManager.Instance.gameData.isBossClear[DataManager.Instance.gameData.curFloor_Num])
            return;
        //위치 설정
        Vector2 pos = new Vector2(4, 0);
        GameObject bossObj = Instantiate(DataManager.Instance.bossMonstersPrefab[(int)cur_Stage.spawnMonsterType], pos, Quaternion.identity);

        //Other Settings
        {
            Monster bossMon = DataManager.Instance.bossMonster = bossObj.GetComponent<Monster>();
            bossMon.tag = "Boss";
            bossObj.layer = LayerMask.NameToLayer("Enemy");
            bossMon.name = "Boss";
            bossMon.teamNum = 0;
            bossMon.belt.color = new Color(1, 0, 0, 200f / 255f);
            bossMon.monsterStat.Initialize(bossObj);
            bossMon.transform.SetParent(entityParent);
        }
    }

    public void EnemySpawn()
    {
        if (cur_Stage.spawnMonsterType == MonsterTypeEnum.NONE || cur_Stage.spawnCount == 0) return;

        if (Get_Entity_Count(false) <= 0)
        {
            float pos_x, pos_y;
            for (int i = 0; i < cur_Stage.spawnCount; i++)
            {
                //위치 설정
                pos_x = Random.Range(moveLimit_Min.x + 0.5f, moveLimit_Max.x - 0.5f);
                pos_y = Random.Range(moveLimit_Min.y + 0.5f, moveLimit_Max.y - 0.5f);
                Vector2 pos = new Vector2(pos_x, pos_y);
                GameObject enemyObj = Instantiate(DataManager.Instance.monstersPrefab[(int)cur_Stage.spawnMonsterType], pos, Quaternion.identity);

                //Other Settings
                {
                    Monster enemyMon = DataManager.Instance.enemyMonsters[i] = enemyObj.GetComponent<Monster>();
                    enemyObj.tag = "Enemy";
                    enemyObj.layer = LayerMask.NameToLayer("Enemy");
                    enemyMon.name = "Enemy" + i;
                    enemyMon.teamNum = i;
                    enemyMon.belt.color = new Color(1, 0, 0, 200f / 255f);
                    enemyMon.monsterStat.monsterType = cur_Stage.spawnMonsterType;
                    enemyMon.monsterStat.Initialize(enemyObj);
                    enemyMon.transform.SetParent(entityParent);
                }
            }
        }
    }

    /// <summary>
    /// _isTeam의 상태를 확인합니다.
    /// </summary>
    public int Get_Entity_Count(bool isTeam)
    {
        if (isTeam)
        {
            // 팀의 수를 세는 로직
            int teamCount = 0;
            for (int i = 0; i < DataManager.Instance.gameData.teamStats.Length; i++)
            {
                if (DataManager.Instance.gameData.teamStats[i] != null && 
                    DataManager.Instance.gameData.teamStats[i].monsterType != MonsterTypeEnum.NONE)
                {
                    teamCount++;
                }
            }
            return teamCount;
        }
        else
        {
            // 적의 수를 세는 로직
            int enemyCount = 0;
            foreach (var entity in DataManager.Instance.enemyMonsters)
            {
                if (entity != null && entity.gameObject.CompareTag("Enemy"))
                {
                    enemyCount++;
                }
            }
            return enemyCount;
        }
    }

    /// <summary>
    /// 팀의 첫 번째 빈 슬롯을 찾습니다.
    /// </summary>
    public int Get_Team_First_None(bool _isTeam)
    {
        for (int i = 0; i < DataManager.Instance.gameData.teamStats.Length; i++)
        {
            if (DataManager.Instance.gameData.teamStats[i] == null) continue;
            if (DataManager.Instance.teamMonsters[i] != null && DataManager.Instance.teamMonsters[i].gameObject.CompareTag("Team"))
                return i;
        }
        return -1;
    }

    public void PlayAudio(AudioClip clip)
    {
        gm_AudioSource.volume = DataManager.Instance.gameData.effectSound;
        gm_AudioSource.PlayOneShot(clip);
    }

    private void OnApplicationQuit()
    {
        DataManager.Instance.SaveGameData();
    }
    //private void OnApplicationPause()
    //{
    //    Debug.Log("Pause");
    //    DataManager.Instance.Initalize();
    //}

    /// <summary>
    /// _isTeam의 상태를 변경합니다.
    /// </summary>
    public void TeamChange(Monster mon, bool _isTeam)
    {
        if (mon.gameObject.CompareTag("Team"))
        {
            mon.tag = _isTeam ? "Team" : "Enemy";
            mon.gameObject.layer = LayerMask.NameToLayer(_isTeam ? "Team" : "Enemy");
            mon.belt.color = _isTeam ? new Color(0, 1, 0, 200f / 255f) : new Color(1, 0, 0, 200f / 255f);
            mon.monsterStat.hp = mon.monsterStat.Maxhp;

            //Enemy to Team
            if (_isTeam == true)
            {
                // 기존 데이터 제거
                DataManager.Instance.enemyMonsters[mon.teamNum] = null;
                UIManager.Instance.DestroyMonsterUI(mon.teamNum, !_isTeam);

                //팀 번호 찾기
                int newTeamNum = -1;
                for (int i = 0; i < DataManager.Instance.gameData.teamStats.Length; i++)
                {
                    if (DataManager.Instance.teamMonsters[i] == null || !DataManager.Instance.teamMonsters[i].gameObject.CompareTag("Team"))
                    {
                        newTeamNum = i;
                        break;
                    }
                }
                if (newTeamNum == -1) Debug.LogError("Team Num is Error. View GameManager Script.");

                //팀 데이터 넣기
                mon.teamNum = newTeamNum;
                mon.name = "Team" + mon.teamNum;
                DataManager.Instance.teamMonsters[mon.teamNum] = mon;
                DataManager.Instance.gameData.teamStats[mon.teamNum] = mon.monsterStat;
                DataManager.Instance.gameData.teamStats[mon.teamNum].monsterType = mon.monsterStat.monsterType;
                mon.monsterStat.hp = mon.monsterStat.Maxhp;
                UIManager.Instance.NewMonsterUI(mon.teamNum, _isTeam, mon);
                if (Get_Entity_Count(false) == 0) Invoke("EnemySpawn", 1f);
            }
            //Team to Enemy
            else
            {
                // 기존 데이터 제거
                DataManager.Instance.teamMonsters[mon.teamNum] = null;
                DataManager.Instance.gameData.teamStats[mon.teamNum] = null;
                UIManager.Instance.DestroyMonsterUI(mon.teamNum, !_isTeam);

                //팀 번호 찾기
                int newEnemyNum = -1;
                for (int i = 0; i < DataManager.Instance.enemyMonsters.Length; i++)
                {
                    if (DataManager.Instance.enemyMonsters[i] == null)
                    {
                        newEnemyNum = i;
                        break;
                    }
                }
                if (newEnemyNum == -1) Debug.LogError("Enemy Num is Error. View GameManager Script.");

                //팀 데이터 넣기
                mon.teamNum = newEnemyNum;
                mon.name = "Enemy" + mon.teamNum;
                DataManager.Instance.enemyMonsters[mon.teamNum] = mon;
                UIManager.Instance.NewMonsterUI(mon.teamNum, _isTeam, mon);
            }
        }
    }
}

using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DataInfo;

/// <summary>
/// 게임의 데이터를 관리하는 싱글톤 클래스
/// 게임 설정, 저장 데이터, 프리팹 등을 관리합니다.
/// </summary>
public class DataManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    private static DataManager instance;
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
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataManager>();
            }
            return instance;
        }
    }

    [Header("Game Data")]
    public GameDataObject gameDataSO;      // 현재 게임 데이터
    public GameConfig gameConfigSO;        // 게임 설정 데이터
    public GameObject cutScene;          // 컷신 오브젝트

    public GameData gameData => gameDataSO.gameData;
    public GameConfig gameConfig => gameConfigSO;

    [Header("Prefabs")]
    public GameObject[] monstersPrefab;      // 일반 몬스터 프리팹 배열
    public GameObject[] bossMonstersPrefab;  // 보스 몬스터 프리팹 배열
    public GameObject[] decorationsPrefab;   // 장식물 프리팹 배열

    [Header("Monsters")]
    public Monster[] teamMonsters = new Monster[6];    // 팀 몬스터 배열
    public Monster[] enemyMonsters = new Monster[10];  // 적 몬스터 배열
    public Monster bossMonster;                        // 보스 몬스터

    // 저장 파일 경로
    private string dataPath;    // 게임 데이터 저장 경로

    /// <summary>
    /// 데이터 매니저 초기화
    /// 저장 경로 설정 및 데이터 로드
    /// </summary>
    public void Initialize()
    {
        dataPath = Application.persistentDataPath + "/gameData.json";
        LoadGameData();
        StartCoroutine(AutoSave());

        if (gameData.isBeginner)
        {
            gameData.isBeginner = false;
            gameData.playerStat.Initialize(); // Stat 초기화
            SaveGameData();
            GameManager.Instance.cutScene.SetActive(true);
            GameManager.Instance.cutScene.GetComponent<CutScene>().StartCoroutine("CutCor");
        }
        else if (gameData.playerStat.hp <= 0)
        {
            UIManager.Instance.restartPanel.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 게임 데이터 로드
    /// </summary>
    public void LoadGameData()
    {
        try
        {
            if (File.Exists(dataPath))
            {
                string encodedData = File.ReadAllText(dataPath);
                byte[] bytes = System.Convert.FromBase64String(encodedData);
                string jsonData = System.Text.Encoding.UTF8.GetString(bytes);
                JsonUtility.FromJsonOverwrite(jsonData, gameDataSO);
            }
            else
            {
                gameDataSO.RestoreDefaultValues();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"게임 데이터 로드 중 오류 발생: {e.Message}");
            gameDataSO.RestoreDefaultValues();
        }
    }

    /// <summary>
    /// 현재 게임 데이터를 JSON 파일로 저장
    /// </summary>
    public void SaveGameData()
    {
        string json = JsonUtility.ToJson(gameDataSO, true);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        string code = System.Convert.ToBase64String(bytes);
        File.WriteAllText(dataPath, code);
    }

    /// <summary>
    /// 자동 저장 코루틴
    /// 10초마다 게임 데이터 저장
    /// </summary>
    private IEnumerator AutoSave()
    {
        while (true)
        {
            SaveGameData();
            yield return new WaitForSeconds(10f);
        }
    }

    /// <summary>
    /// 게임 데이터 파일 삭제
    /// </summary>
    public void DeleteGameDataFile()
    {
        File.Delete(dataPath);
    }
}

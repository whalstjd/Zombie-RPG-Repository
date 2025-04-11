using UnityEngine;
using System.Collections.Generic;

namespace DataInfo
{
    [System.Serializable]
    public class GameData
    {
        // 플레이어 스탯
        public Stat playerStat;
        // 팀 몬스터 스탯
        public Stat[] teamStats;
        // 아이템 목록
        public Possession possession;
        // 플레이어 업그레이드 레벨
        public int[] playerUpgradeLv;
        public int[] teamUpgradeLv;
        public int[] weaponUpgradeLv;
        // 맵 데이터
        public int curFloor_Num;
        public int curStage_Num;
        public bool[] isBossClear;
        // 시스템 상태
        public bool isBeginner;
        public bool adSkip;
        public float effectSound;
        public float bgmSound;
        public bool toolTip = true;
        public List<string> usedCoupons = new List<string>();
        public int[] last_Daily_Year = new int[6];
        public int[] last_Daily_Month = new int[6];
        public int[] last_Daily_Day = new int[6];
    }
}
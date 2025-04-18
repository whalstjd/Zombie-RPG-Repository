using UnityEngine;

namespace DataInfo
{
    [System.Serializable]
    public class Stat
    {
        public string _name = "";
        public bool autoPotion = false;
        public int lv = 1;
        public float cur_Exp = 0;
        public float exp_Difficulty = 1;
        public float full_Exp { get { return 5 + (lv * lv * exp_Difficulty * 4); } set { } }
        public int trainingRemainPoint {
            get
            {
                int usedPoint = 0;
                for (int i = 0; i < trainingCurPoint.Length; i++)
                {
                    usedPoint += trainingCurPoint[i];
                }
                return lv - usedPoint;
            }
        }

        [Tooltip("현재 포인트: damage, attackSpeed, maxHP, exp, money, item")]
        public int[] trainingCurPoint = new int[6];
        [Tooltip("최대 포인트: damage, attackSpeed, maxHP, exp, money, item")]
        public int[] trainingMaxPoint = new int[6];

        public float hp;
        public GameObject gameObject;
        public MonsterTypeEnum monsterType;
        
        private float damage;
        private float attackSpeed;
        private float maxHP;
        private float exp_Value;
        private float money_Value;
        private float item_Value;

        public Stat()
        {
            _name = "";
            autoPotion = false;
            lv = 1;
            cur_Exp = 0;
            exp_Difficulty = 1;
            trainingCurPoint = new int[6];
            trainingMaxPoint = new int[6];
            hp = 30; // 기본 HP 값으로 설정
            monsterType = MonsterTypeEnum.NONE; // 기본값 설정
        }

        public void Initialize(GameObject obj = null)
        {
            gameObject = obj;
            hp = Maxhp; // MaxHP와 동일하게 설정
        }

        public float GetUpgradeValue(UpgradeTargetEnum target, UpgradeTypeEnum type)
        {
            try
            {
                // 초기화 시점에는 기본값 반환
                if (DataManager.Instance == null || DataManager.Instance.gameConfig == null)
                {
                    return 0f;
                }

                var upgrade = DataManager.Instance.gameConfig.upgrades.Find(x => x.targetEnum == target && x.typeEnum == type);
                if (upgrade == null) return 0f;

                int level = 0;
                switch (target)
                {
                    case UpgradeTargetEnum.PLAYER:
                        if (DataManager.Instance.gameData == null || DataManager.Instance.gameData.playerUpgradeLv == null) return 1f;
                        switch (type)
                        {
                            case UpgradeTypeEnum.AttackDamage: level = DataManager.Instance.gameData.playerUpgradeLv[0]; break;
                            case UpgradeTypeEnum.AttackSpeed: level = DataManager.Instance.gameData.playerUpgradeLv[1]; break;
                            case UpgradeTypeEnum.MaxHP: level = DataManager.Instance.gameData.playerUpgradeLv[2]; break;
                            case UpgradeTypeEnum.Exp: level = DataManager.Instance.gameData.playerUpgradeLv[3]; break;
                            case UpgradeTypeEnum.Money: level = DataManager.Instance.gameData.playerUpgradeLv[4]; break;
                            case UpgradeTypeEnum.Item: level = DataManager.Instance.gameData.playerUpgradeLv[5]; break;
                        }
                        break;
                    case UpgradeTargetEnum.TEAM:
                        if (DataManager.Instance.gameData == null || DataManager.Instance.gameData.teamUpgradeLv == null) return 1f;
                        switch (type)
                        {
                            case UpgradeTypeEnum.AttackDamage: level = DataManager.Instance.gameData.teamUpgradeLv[0]; break;
                            case UpgradeTypeEnum.AttackSpeed: level = DataManager.Instance.gameData.teamUpgradeLv[1]; break;
                            case UpgradeTypeEnum.MaxHP: level = DataManager.Instance.gameData.teamUpgradeLv[2]; break;
                            case UpgradeTypeEnum.Exp: level = DataManager.Instance.gameData.teamUpgradeLv[3]; break;
                            case UpgradeTypeEnum.Money: level = DataManager.Instance.gameData.teamUpgradeLv[4]; break;
                            case UpgradeTypeEnum.Item: level = DataManager.Instance.gameData.teamUpgradeLv[5]; break;
                        }
                        break;
                    case UpgradeTargetEnum.WEAPON:
                        if (DataManager.Instance.gameData == null || DataManager.Instance.gameData.weaponUpgradeLv == null) return 1f;
                        switch (type)
                        {
                            case UpgradeTypeEnum.AttackDamage: level = DataManager.Instance.gameData.weaponUpgradeLv[0]; break;
                            case UpgradeTypeEnum.MaxHP: level = DataManager.Instance.gameData.weaponUpgradeLv[1]; break;
                        }
                        break;
                }

                return upgrade.baseValue * Mathf.Pow(1 + upgrade.increaseRate, level);
            }
            catch (System.Exception)
            {
                return 1f;
            }
        }

        private float GetTrainingValue(TrainingTypeEnum type)
        {
            var training = DataManager.Instance.gameConfig.trainings.Find(x => x.typeEnum == type);
            if (training == null) return 0;

            return training.value;
        }

        public float Damage
        {
            get
            {
                float playerBonus = 0;
                float teamBonus = 0;
                float trainingBonus = GetTrainingValue(TrainingTypeEnum.AttackDamage);

                if (gameObject.CompareTag("Player"))
                {
                    playerBonus = GetUpgradeValue(UpgradeTargetEnum.PLAYER, UpgradeTypeEnum.AttackDamage);
                }
                else if (gameObject.CompareTag("Team"))
                {
                    teamBonus = GetUpgradeValue(UpgradeTargetEnum.TEAM, UpgradeTypeEnum.AttackDamage);
                }

                return damage = 5 + (playerBonus + teamBonus + trainingBonus) * (trainingCurPoint[(int)TrainingTypeEnum.AttackDamage] * 0.5f);
            }
            private set { }
        }

        public float AttackSpeed
        {
            get
            {
                float playerBonus = 0;
                float teamBonus = 0;
                float trainingBonus = GetTrainingValue(TrainingTypeEnum.AttackSpeed);

                if (gameObject.CompareTag("Player"))
                {
                    playerBonus = GetUpgradeValue(UpgradeTargetEnum.PLAYER, UpgradeTypeEnum.AttackSpeed);
                }
                else if (gameObject.CompareTag("Team"))
                {
                    teamBonus = GetUpgradeValue(UpgradeTargetEnum.TEAM, UpgradeTypeEnum.AttackSpeed);
                }

                return attackSpeed = 1f + ((35f - trainingCurPoint[(int)TrainingTypeEnum.AttackSpeed] - (playerBonus + teamBonus + trainingBonus)) / 10f);
            }
            private set { }
        }

        public float Maxhp
        {
            get
            {
                float playerBonus = 0;
                float teamBonus = 0;
                float weaponBonus = 0;
                float trainingBonus = GetTrainingValue(TrainingTypeEnum.MaxHP);

                if (gameObject != null)
                {
                    if (gameObject.CompareTag("Player"))
                    {
                        playerBonus = GetUpgradeValue(UpgradeTargetEnum.PLAYER, UpgradeTypeEnum.MaxHP);
                        weaponBonus = GetUpgradeValue(UpgradeTargetEnum.WEAPON, UpgradeTypeEnum.MaxHP);
                    }
                    else if (gameObject.CompareTag("Team"))
                    {
                        teamBonus = GetUpgradeValue(UpgradeTargetEnum.TEAM, UpgradeTypeEnum.MaxHP);
                    }
                }

                return maxHP = 30 + (lv * 3) + weaponBonus + (playerBonus + teamBonus + trainingBonus) * trainingCurPoint[(int)TrainingTypeEnum.MaxHP] * 16.5f;
            }
            private set { }
        }

        public float Exp_Plus
        {
            get
            {
                float playerBonus = 0;
                float teamBonus = 0;
                float trainingBonus = GetTrainingValue(TrainingTypeEnum.Exp);

                if (gameObject.CompareTag("Player"))
                {
                    playerBonus = GetUpgradeValue(UpgradeTargetEnum.PLAYER, UpgradeTypeEnum.Exp);
                }
                else if (gameObject.CompareTag("Team"))
                {
                    teamBonus = GetUpgradeValue(UpgradeTargetEnum.TEAM, UpgradeTypeEnum.Exp);
                }

                return exp_Value = 1 + (playerBonus + teamBonus + trainingBonus) * trainingCurPoint[(int)TrainingTypeEnum.Exp] * 0.033f;
            }
            private set { }
        }

        public float Money_Plus
        {
            get
            {
                float playerBonus = 0;
                float teamBonus = 0;
                float trainingBonus = GetTrainingValue(TrainingTypeEnum.Money);

                if (gameObject.CompareTag("Player"))
                {
                    playerBonus = GetUpgradeValue(UpgradeTargetEnum.PLAYER, UpgradeTypeEnum.Money);
                }
                else if (gameObject.CompareTag("Team"))
                {
                    teamBonus = GetUpgradeValue(UpgradeTargetEnum.TEAM, UpgradeTypeEnum.Money);
                }

                return money_Value = 1 + (playerBonus + teamBonus + trainingBonus) * trainingCurPoint[(int)TrainingTypeEnum.Money];
            }
            private set { }
        }

        public float Item_Plus
        {
            get
            {
                float playerBonus = 0;
                float teamBonus = 0;
                float trainingBonus = GetTrainingValue(TrainingTypeEnum.Item);

                if (gameObject.CompareTag("Player"))
                {
                    playerBonus = GetUpgradeValue(UpgradeTargetEnum.PLAYER, UpgradeTypeEnum.Item);
                }
                else if (gameObject.CompareTag("Team"))
                {
                    teamBonus = GetUpgradeValue(UpgradeTargetEnum.TEAM, UpgradeTypeEnum.Item);
                }

                return item_Value = 5 + (playerBonus + teamBonus + trainingBonus) * (trainingCurPoint[(int)TrainingTypeEnum.Item] * 3f);
            }
            private set { }
        }

        public void UsePotion()
        {
            if (hp < Maxhp)
            {
                hp = Mathf.Min(hp + 30, Maxhp);
                DataManager.Instance.gameData.possession.RemovePossession(PossessionTypeEnum.Potion, 1);
                UIManager.Instance.Goods_UI_Update();
            }
        }

        public bool SetExp(float value)
        {
            cur_Exp += value;
            if (cur_Exp >= full_Exp)
            {
                cur_Exp -= full_Exp;
                lv++;
                return true;
            }
            return false;
        }

        public void SetPoint(TrainingTypeEnum type, int value)
        {
            trainingCurPoint[(int)type] += value;
        }
    }
} 
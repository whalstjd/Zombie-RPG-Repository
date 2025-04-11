using UnityEngine;
using System.Collections.Generic;

namespace DataInfo
{
    public enum UpgradeTargetEnum 
    { 
        PLAYER, 
        TEAM, 
        WEAPON 
    }
    
    public enum UpgradeTypeEnum
    {
        AttackDamage,
        AttackSpeed,
        MaxHP,
        Exp,
        Money,
        Item,
    }

    [System.Serializable]
    public class Upgrade
    {
        public UpgradeTargetEnum targetEnum = UpgradeTargetEnum.PLAYER;
        public UpgradeTypeEnum typeEnum = UpgradeTypeEnum.AttackDamage;
        public int curLv = 0;
        public int maxLv = 100;

        public float baseValue = 1f;
        public float increaseRate = 0.1f;
        public float basePrice = 100f;
        public float priceIncreaseRate = 1.5f;
        public float successRate = 1f;

        public float value
        {
            get
            {
                return baseValue * Mathf.Pow(1 + increaseRate, curLv);
            }
        }

        public float nextValue
        {
            get
            {
                if (curLv + 1 > maxLv) return -1;

                return baseValue * Mathf.Pow(1 + increaseRate, curLv + 1);
            }
        }

        public float price
        {
            get
            {
                return basePrice * Mathf.Pow(priceIncreaseRate, curLv);
            }
        }

        public float upgrade_Rate
        {
            get
            {
                if (targetEnum != UpgradeTargetEnum.WEAPON) return 100f;
                
                return successRate * 100;
            }
        }

        public bool CanUpgrade(float availableMoney)
        {
            return curLv < maxLv && availableMoney >= price;
        }

        public bool TryUpgrade(Possession possession)
        {
            float availableMoney = possession.GetPossessionCount(PossessionTypeEnum.Money);
            if (!CanUpgrade(availableMoney)) return false;
            
            if (targetEnum == UpgradeTargetEnum.WEAPON)
            {
                float successRate = upgrade_Rate / 100f;
                if (Random.value > successRate) return false;
            }
            
            possession.RemovePossession(PossessionTypeEnum.Money, (int)price);
            curLv++;
            return true;
        }
    }
} 
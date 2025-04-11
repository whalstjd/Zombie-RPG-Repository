using UnityEngine;
using System.Collections.Generic;

namespace DataInfo
{
    public enum PossessionTypeEnum
    {
        None,
        Money,
        Potion,
        GoldKey,
        SilverKey
    }
    
    [System.Serializable]
    public class Possession
    {
        [Header("아이템 수량")]
        [SerializeField] private int money;
        [SerializeField] private int potion;
        [SerializeField] private int goldKey;
        [SerializeField] private int silverKey;

        public Possession()
        {
            money = 0;
            potion = 0;
            goldKey = 0;
            silverKey = 0;
        }

        public void AddPossession(PossessionTypeEnum possessionType, int count)
        {
            switch (possessionType)
            {
                case PossessionTypeEnum.Money:
                    money += count;
                    break;
                case PossessionTypeEnum.Potion:
                    potion += count;
                    break;
                case PossessionTypeEnum.GoldKey:
                    goldKey += count;
                    break;
                case PossessionTypeEnum.SilverKey:
                    silverKey += count;
                    break;
            }
        }

        public void RemovePossession(PossessionTypeEnum possessionType, int count)
        {
            switch (possessionType)
            {
                case PossessionTypeEnum.Money:
                    money = Mathf.Max(0, money - count);
                    break;
                case PossessionTypeEnum.Potion:
                    potion = Mathf.Max(0, potion - count);
                    break;
                case PossessionTypeEnum.GoldKey:
                    goldKey = Mathf.Max(0, goldKey - count);
                    break;
                case PossessionTypeEnum.SilverKey:
                    silverKey = Mathf.Max(0, silverKey - count);
                    break;
            }
        }

        public int GetPossessionCount(PossessionTypeEnum possessionType)
        {
            switch (possessionType)
            {
                case PossessionTypeEnum.Money:
                    return money;
                case PossessionTypeEnum.Potion:
                    return potion;
                case PossessionTypeEnum.GoldKey:
                    return goldKey;
                case PossessionTypeEnum.SilverKey:
                    return silverKey;
                default:
                    return 0;
            }
        }
    }
} 
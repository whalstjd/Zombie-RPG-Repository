using UnityEngine;
using System.Collections.Generic;

namespace DataInfo
{
    public enum ItemTypeEnum
    {
        None,
        Money,
        Potion,
        GoldKey,
        SilverKey
    }
    
    [System.Serializable]
    public class Item
    {
        [Header("아이템 수량")]
        [SerializeField] private int money;
        [SerializeField] private int potion;
        [SerializeField] private int goldKey;
        [SerializeField] private int silverKey;

        public Item()
        {
            money = 0;
            potion = 0;
            goldKey = 0;
            silverKey = 0;
        }

        public void AddItem(ItemTypeEnum itemType, int count)
        {
            switch (itemType)
            {
                case ItemTypeEnum.Money:
                    money += count;
                    break;
                case ItemTypeEnum.Potion:
                    potion += count;
                    break;
                case ItemTypeEnum.GoldKey:
                    goldKey += count;
                    break;
                case ItemTypeEnum.SilverKey:
                    silverKey += count;
                    break;
            }
        }

        public void RemoveItem(ItemTypeEnum itemType, int count)
        {
            switch (itemType)
            {
                case ItemTypeEnum.Money:
                    money = Mathf.Max(0, money - count);
                    break;
                case ItemTypeEnum.Potion:
                    potion = Mathf.Max(0, potion - count);
                    break;
                case ItemTypeEnum.GoldKey:
                    goldKey = Mathf.Max(0, goldKey - count);
                    break;
                case ItemTypeEnum.SilverKey:
                    silverKey = Mathf.Max(0, silverKey - count);
                    break;
            }
        }

        public int GetItemCount(ItemTypeEnum itemType)
        {
            switch (itemType)
            {
                case ItemTypeEnum.Money:
                    return money;
                case ItemTypeEnum.Potion:
                    return potion;
                case ItemTypeEnum.GoldKey:
                    return goldKey;
                case ItemTypeEnum.SilverKey:
                    return silverKey;
                default:
                    return 0;
            }
        }
    }
}

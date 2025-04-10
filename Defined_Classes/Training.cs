using UnityEngine;

namespace DataInfo
{    
    public enum TrainingTypeEnum
    {
        AttackDamage,
        AttackSpeed,
        MaxHP,
        Exp,
        Money,
        Item,
    }

    [System.Serializable]
    public class Training
    {
        public TrainingTypeEnum typeEnum = TrainingTypeEnum.AttackDamage;
        public int curPoint = 0;
        public int maxPoint = 100;
        public float baseValue = 1f;
        public float increaseRate = 0.2f;
        public float pointIncreaseRate = 1.2f;

        public float value
        {
            get
            {
                return baseValue * Mathf.Pow(1 + increaseRate, curPoint);
            }
        }

        public float nextValue
        {
            get
            {
                if (curPoint + 1 > maxPoint) return -1;
                return baseValue * Mathf.Pow(1 + increaseRate, curPoint + 1);
            }
        }

        public bool CanPointUp(int availableStatPoints)
        {
            return curPoint < maxPoint && availableStatPoints >= 1;
        }

        public bool PointUp(ref int availableStatPoints)
        {
            if (!CanPointUp(availableStatPoints)) return false;
            
            availableStatPoints -= 1;
            curPoint++;
            return true;
        }
    }
} 
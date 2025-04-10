using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Instantiate : MonoBehaviour
{
    public Player pm;

    public void Call_Skill()
    {
        GameObject obj = pm.isSword ? pm.swordObj : pm.netObj;
        GameObject a = Instantiate(obj, pm.transform.position + Vector3.up + new Vector3(pm.skill_x, pm.skill_y, 0).normalized, Quaternion.identity);
        float angle = Mathf.Atan2(pm.skill_y, pm.skill_x) * Mathf.Rad2Deg;
        a.transform.SetParent(GameManager.Instance.entityParent);
        a.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        pm.isUsingSkill = false;
    }
}

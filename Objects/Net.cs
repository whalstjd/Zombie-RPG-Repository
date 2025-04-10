using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataInfo;

public class Net : MonoBehaviour
{
    float speed;
    private void Start()
    {
        GameManager.Instance.PlayAudio(GameManager.Instance.net_Clip);
        speed = 5f + ((5 - DataManager.Instance.gameData.playerStat.AttackSpeed) * 1);
        Destroy(gameObject, 4f);
    }
    private void Update()
    {
        transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Monster collMonster = collision.GetComponent<Monster>();
            if (collMonster.monsterStat.entityType == EntityType.BOSS) return;
            collMonster.StartCoroutine(collMonster.CaughtNet(Random.Range(1.5f, 3.5f)));
            Destroy(gameObject);
        }
    }
}

using DataInfo;
using UnityEngine;

public class Slash : MonoBehaviour
{
    float speed;
    private void Start()
    {
        GameManager.Instance.PlayAudio(GameManager.Instance.slash_Clip);
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
            Monster targetMon = collision.GetComponent<Monster>();
            if (targetMon != null)
            {
                targetMon.Damaged(DataManager.Instance.gameData.playerStat.Damage);
            }
        }
    }
}

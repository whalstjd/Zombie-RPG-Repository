using DataInfo;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AdaptivePerformance.VisualScripting;

public enum MonsterState { IDLE, MOVE, ATTACK, DAMAGED, CAUGHT_NET, DIE }

public class Monster : MonoBehaviour
{
    public MonsterTypeEnum monsterType = MonsterTypeEnum.BAT;
    public MonsterState curState = MonsterState.IDLE;
    public Animator anim;

    //객체 초기 설정
    public bool yet_firstSetting = true;
    public bool completed_firstSetting = false;
    [Header("Stat")]
    [ShowIf(ShowIfAttribute.ActionOnConditionFail.DontDraw, ShowIfAttribute.ConditionOperator.And, nameof(yet_firstSetting))]
    public Rand rands;
    [ShowIf(ShowIfAttribute.ActionOnConditionFail.DontDraw, ShowIfAttribute.ConditionOperator.And, nameof(completed_firstSetting))]
    public Stat monsterStat;
    [ShowIf(ShowIfAttribute.ActionOnConditionFail.DontDraw, ShowIfAttribute.ConditionOperator.And, nameof(completed_firstSetting))]
    public Drop drops;

    [Header("Team")]
    public int teamNum = 0;

    [Header("Other")]
    public Sprite portraitSprite;
    public SpriteRenderer belt;
    public GameObject netObj;
    Vector3 destinationPos;
    public float net_Avoid = 0; //포획 시 회피율

    [Space]
    public GameObject target = null;
    public float curAttackDelay = 1f;

    private void Start()
    {
        if (monsterStat.entityType == EntityType.ENEMY)
        {
            monsterStat._name = System.Enum.GetName(typeof(MonsterTypeEnum), monsterType);
            //레벨
            monsterStat.lv = Random.Range(rands.lv_Rand.x, rands.lv_Rand.y);
            monsterStat.cur_Exp = 0;
            monsterStat.exp_Difficulty = Random.Range(0.8f, 1.2f);

            //스탯 포인트 초기화
            InitializeTrainingPoints();
            
            //팀
            monsterStat.entityType = EntityType.ENEMY;
            belt.color = monsterStat.entityType == EntityType.TEAM ? new Color(0, 1, 0, 200f / 255f) : new Color(1, 0, 0, 200f / 255f);

            //보상
            InitializeDrops();

            //UI생성
            UIManager.Instance.NewMonsterUI(teamNum, false, this);

            StartCoroutine(StayInPlace());
            monsterStat.hp = monsterStat.Maxhp;
        }

        if (monsterStat.entityType == EntityType.BOSS)
        {
            monsterStat.hp = monsterStat.Maxhp;
            UIManager.Instance.Boss_UI_Update();
        }
        UIManager.Instance.Monster_UI_Update();
        yet_firstSetting = false;
        completed_firstSetting = true;

        // 행동 결정 코루틴 시작
        StartCoroutine(DecisionCoroutine());
    }

    private IEnumerator DecisionCoroutine()
    {
        while (true)
        {
            if (curState == MonsterState.DIE)
            {
                yield break;
            }

            if (monsterStat.hp > 0)
            {
                if (GameManager.Instance.player.isDie)
                {
                    StateChange(MonsterState.IDLE);
                    yield return new WaitForSeconds(1f);
                    continue;
                }

                Vector3 trPivotPos = GetComponent<Pivot>().tr.position;

                //적일때
                if (monsterStat.entityType == EntityType.ENEMY)
                {
                    float range = monsterStat.entityType == EntityType.BOSS ? 9f : 4.5f;
                    Collider2D[] sight = Physics2D.OverlapCircleAll(trPivotPos, range, LayerMask.GetMask("Team"));
                    
                    if (sight.Length > 0)
                    {
                        target = sight[0].gameObject;
                        Collider2D[] attackRange = Physics2D.OverlapCircleAll(trPivotPos, 1.5f, LayerMask.GetMask("Team"));
                        
                        if (attackRange.Length <= 0)
                        {
                            StateChange(MonsterState.MOVE);
                        }
                        else if (curAttackDelay <= 0)
                        {
                            StateChange(MonsterState.ATTACK);
                        }
                        else
                        {
                            StateChange(MonsterState.IDLE);
                        }
                    }
                    else if (Vector3.Distance(destinationPos, trPivotPos) > 0.5f)
                    {
                        StateChange(MonsterState.MOVE);
                    }
                    else
                    {
                        StartCoroutine(StayInPlace());
                    }
                }
                //아군일때
                else if (monsterStat.entityType == EntityType.TEAM)
                {
                    Vector3 playerPos = GameManager.Instance.player.GetComponent<Pivot>().tr.position;
                    Collider2D[] sight = Physics2D.OverlapCircleAll(trPivotPos, 5f, LayerMask.GetMask("Enemy"));
                    
                    if (sight.Length > 0)
                    {
                        target = sight[0].gameObject;
                        Collider2D[] attackRange = Physics2D.OverlapCircleAll(trPivotPos, 1f, LayerMask.GetMask("Enemy"));
                        
                        if (attackRange.Length <= 0)
                        {
                            StateChange(MonsterState.MOVE);
                        }
                        else if (curAttackDelay <= 0)
                        {
                            StateChange(MonsterState.ATTACK);
                        }
                        else
                        {
                            StateChange(MonsterState.IDLE);
                        }
                    }
                    else if (Vector3.Distance(playerPos, trPivotPos) > 2f)
                    {
                        StateChange(MonsterState.MOVE);
                    }
                    else
                    {
                        StateChange(MonsterState.IDLE);
                    }
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void Update()
    {
        if (curState == MonsterState.DIE)
            return;

        curAttackDelay -= Time.deltaTime;

        // 이동 처리
        if (curState == MonsterState.MOVE)
        {
            Vector3 trPivotPos = GetComponent<Pivot>().tr.position;
            Vector3 dir;

            if (target != null)
            {
                dir = (target.GetComponent<Pivot>().tr.position - trPivotPos).normalized;
            }
            else if (monsterStat.entityType == EntityType.TEAM)
            {
                dir = (GameManager.Instance.player.GetComponent<Pivot>().tr.position - trPivotPos).normalized;
            }
            else
            {
                dir = (destinationPos - trPivotPos).normalized;
            }

            MovementScale(dir);
            transform.Translate(dir * Time.deltaTime);
        }
    }

    private void InitializeTrainingPoints()
    {
        // 최대 포인트 설정
        for (int i = 0; i < rands.trainingMaxPointRand.Length; i++)
        {
            monsterStat.trainingMaxPoint[i] = Random.Range(
                rands.trainingMaxPointRand[i].x, 
                rands.trainingMaxPointRand[i].y + 1
            );
        }

        // 현재 포인트 설정
        for (int i = 0; i < rands.trainingCurPointRand.Length; i++)
        {
            if (monsterStat.trainingRemainPoint <= 0) break;
            
            int maxPoint = Mathf.Min(
                rands.trainingCurPointRand[i].y + 1,
                monsterStat.trainingRemainPoint + 1
            );
            
            monsterStat.SetPoint(
                (TrainingTypeEnum)i,
                Random.Range(rands.trainingCurPointRand[i].x, maxPoint)
            );
        }
    }

    private void InitializeDrops()
    {
        drops.dropMoney = Random.Range(monsterStat.lv * 10, monsterStat.lv * 50);
        drops.dropExp = Random.Range(monsterStat.lv * 10, monsterStat.lv * 25);
    }

    void MovementScale(Vector2 dir)
    {
        if (monsterStat.entityType == EntityType.BOSS)
        {
            if (dir.x < 0) transform.localScale = new Vector3(2, 2, 1);
            else if (dir.x > 0) transform.localScale = new Vector3(-2, 2, 1);
        }
        else
        {
            if (dir.x < 0) transform.localScale = new Vector3(1, 1, 1);
            else if (dir.x > 0) transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    IEnumerator StayInPlace()
    {
        StateChange(MonsterState.IDLE);
        float pos_x = Random.Range(GameManager.Instance.moveLimit_Min.x + 0.5f, GameManager.Instance.moveLimit_Max.x - 0.5f);
        float pos_y = Random.Range(GameManager.Instance.moveLimit_Min.y + 0.5f, GameManager.Instance.moveLimit_Max.y - 0.5f);
        destinationPos = new Vector3(pos_x, pos_y, 0);
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        if (curState != MonsterState.DIE && curState != MonsterState.CAUGHT_NET)
            StateChange(MonsterState.MOVE);
    }

    public void Attack()
    {
        if (target.TryGetComponent(out Monster targetMon))
        {
            targetMon.Damaged(monsterStat.Damage);

            //적이 죽었을때
            if (targetMon.curState == MonsterState.DIE)
            {
                //적이 아군을 죽였을때
                if (monsterStat.entityType == EntityType.ENEMY)
                {
                    DataManager.Instance.gameData.teamMonsterType[targetMon.teamNum] = MonsterTypeEnum.NONE;
                }
                //아군이 적을 죽였을때
                else if (monsterStat.entityType == EntityType.TEAM)
                {
                    //exp
                    bool teamLvUp = monsterStat.SetExp(targetMon.drops.dropExp * monsterStat.Exp_Plus);
                    if (teamLvUp) UIManager.Instance.LvUp_newObj(teamNum, true);

                    //money
                    DataManager.Instance.gameData.items.AddItem(ItemTypeEnum.Money, (int)(targetMon.drops.dropMoney * monsterStat.Money_Plus));

                    //item
                    if (targetMon.monsterStat.entityType != EntityType.BOSS)
                    {
                        if (monsterStat.Item_Plus + Random.Range(0, 100) >= 100)
                        {
                            int rand = Random.Range(0, 6);
                            if (rand <= 2)
                            {
                                targetMon.drops.dropItem = targetMon.rands.dropItems[0];
                                targetMon.drops.itemDeco = DecorationEnum.chest_0_closed;
                            }
                            else if (rand <= 4)
                            {
                                targetMon.drops.dropItem = targetMon.rands.dropItems[1];
                                targetMon.drops.itemDeco = DecorationEnum.chest_1_closed;
                            }
                            else
                            {
                                targetMon.drops.dropItem = targetMon.rands.dropItems[2];
                                targetMon.drops.itemDeco = DecorationEnum.chest_2_closed;
                            }
                        }
                        else
                            targetMon.drops.dropItem = null;
                    }

                    //보스 처리
                    if (monsterStat.entityType == EntityType.BOSS)
                    {
                        GameManager.Instance.PlayAudio(GameManager.Instance.boss_Die_Clip);
                        GameManager.Instance.cur_Floor.isBossClear = true;
                        UIManager.Instance.Boss_UI_Update();
                        GameManager.Instance.Invoke("DoorActive", 3f);
                    }
                    else
                    {
                        GameManager.Instance.PlayAudio(GameManager.Instance.monster_Die_Clip);
                    }
                    UIManager.Instance.Goods_UI_Update();
                    GameManager.Instance.Invoke("EnemySpawn", 4f);
                    targetMon.Invoke("DropItem", 3f);
                }
                UIManager.Instance.DestroyMonsterUI(targetMon.teamNum, targetMon.monsterStat.entityType == EntityType.TEAM);
                Destroy(targetMon.gameObject, 3f);
                targetMon = null;
            }
            //살아있을때
            else
            {
                GameManager.Instance.PlayAudio(GameManager.Instance.damaged_Clip);

                if (targetMon.monsterStat.entityType == EntityType.TEAM && targetMon.monsterStat.autoPotion && targetMon.monsterStat.hp * 2 < targetMon.monsterStat.Maxhp && DataManager.Instance.gameData.items.GetItemCount(ItemTypeEnum.Potion) > 0)
                    targetMon.monsterStat.UsePotion();
                if (targetMon.curState != MonsterState.ATTACK)
                    targetMon.StateChange(MonsterState.DAMAGED);
            }
            UIManager.Instance.Monster_UI_Update();
            UIManager.Instance.Player_UI_Update();
        }
        else
        {
            GameManager.Instance.player.Damaged(monsterStat.Damage, teamNum);
        }
    }

    public void Damaged(float value)
    {
        if (curState == MonsterState.DIE) return;

        monsterStat.hp -= value;
        if (monsterStat.hp <= 0)
        {
            monsterStat.hp = 0;
            StateChange(MonsterState.DIE);
            // 충돌 무시 설정
            GetComponent<Collider2D>().enabled = false;
            
            // 아군이 아닐 때만 플레이어가 경험치를 얻음
            if (monsterStat.entityType != EntityType.TEAM)
            {
                bool playerLvUp = DataManager.Instance.gameData.playerStat.SetExp(drops.dropExp * DataManager.Instance.gameData.playerStat.Exp_Plus);
                if (playerLvUp) UIManager.Instance.LvUp_newObj(-1, true);
            }

            //돈 획득
            DataManager.Instance.gameData.items.AddItem(ItemTypeEnum.Money, (int)(drops.dropMoney * DataManager.Instance.gameData.playerStat.Money_Plus));

            //아이템 드롭
            if (monsterStat.entityType != EntityType.BOSS)
            {
                for (int i = rands.dropItems.Length - 1; i >= 0; i--)
                {
                    if (DataManager.Instance.gameData.playerStat.Item_Plus * rands.itemsRate[i] + Random.Range(0, 100) >= 100)
                    {
                        drops.dropItem = rands.dropItems[i];
                        drops.itemDeco = (DecorationEnum)i;
                        break;
                    }
                    else
                        drops.dropItem = null;
                }
            }
            Invoke("DropItem", 3f);

            //보스 처리
            if (monsterStat.entityType == EntityType.BOSS)
            {
                GameManager.Instance.PlayAudio(GameManager.Instance.boss_Die_Clip);
                GameManager.Instance.cur_Floor.isBossClear = true;
                UIManager.Instance.Boss_UI_Update();
                GameManager.Instance.Invoke("DoorActive", 3f);
            }
            else
            {
                GameManager.Instance.PlayAudio(GameManager.Instance.monster_Die_Clip);
            }

            UIManager.Instance.Goods_UI_Update();
            GameManager.Instance.Invoke("EnemySpawn", 4f);
            UIManager.Instance.DestroyMonsterUI(teamNum, monsterStat.entityType == EntityType.TEAM);
            Destroy(gameObject, 3f);
        }
        else
        {
            GameManager.Instance.PlayAudio(GameManager.Instance.damaged_Clip);
            if (monsterStat.entityType == EntityType.TEAM && monsterStat.autoPotion && monsterStat.hp * 2 < monsterStat.Maxhp && DataManager.Instance.gameData.items.GetItemCount(ItemTypeEnum.Potion) > 0)
                monsterStat.UsePotion();
            if (curState != MonsterState.ATTACK)
                StateChange(MonsterState.DAMAGED);
        }
        UIManager.Instance.Monster_UI_Update();
        UIManager.Instance.Player_UI_Update();
    }

    /// <summary>
    /// 포획 처리
    /// </summary>
    /// <param name="sec">포획 시간</param>
    public IEnumerator CaughtNet(float sec)
    {
        if (curState == MonsterState.CAUGHT_NET) yield break;

        StateChange(MonsterState.CAUGHT_NET);
        netObj.SetActive(true);
        yield return new WaitForSeconds(sec);

        if (curState == MonsterState.DIE)
        {
            netObj.SetActive(false);
            yield break;
        }

        // 팀으로 전환하기 전에 한 번 더 체크
        int currentTeamCount = GameManager.Instance.Get_Entity_Count(true);
        if (currentTeamCount >= 6)
        {
            UIManager.Instance.Message("동시에 데리고 갈 수 있는 몬스터는 최대 6마리입니다.");
            netObj.SetActive(false);
            curState = MonsterState.MOVE;
            anim.SetInteger("State", (int)MonsterState.MOVE);
            yield break;
        }

        if (DataManager.Instance.gameData.playerStat.lv < monsterStat.lv)
        {
            UIManager.Instance.Message("이 몬스터가 플레이어보다 레벨이 높습니다.");
            netObj.SetActive(false);
            curState = MonsterState.MOVE;
            anim.SetInteger("State", (int)MonsterState.MOVE);
            yield break;
        }

        float randValue = Random.Range(0, 100);
        float catchPercent = (DataManager.Instance.gameData.playerStat.lv - monsterStat.lv) * (monsterStat.Maxhp / monsterStat.hp * 3f) - net_Avoid;
        if (catchPercent + randValue >= 100)
        {
            // 팀으로 전환하기 전에 한 번 더 체크
            if (GameManager.Instance.Get_Entity_Count(true) < 6)
            {
                UIManager.Instance.Message("몬스터를 포획했습니다!");
                monsterStat.entityType = EntityType.TEAM;  // 팀으로 전환
                GameManager.Instance.TeamChange(this, true);
            }
            else
            {
                UIManager.Instance.Message("팀이 가득 찼습니다.");
            }
        }
        else
        {
            UIManager.Instance.Message("몬스터를 포획하지 못했습니다.\n포획 확률: " + Mathf.Round(catchPercent * 10) * 0.1f + "%");
        }

        netObj.SetActive(false);
        if (curState == MonsterState.DIE) yield break;
        curState = MonsterState.MOVE;
        anim.SetInteger("State", (int)MonsterState.MOVE);
    }

    public void DropItem()
    {
        if (drops.dropItem == null) return;
        GameObject a = Instantiate(drops.dropItem, transform.position, Quaternion.identity);
        GameManager.Instance.cur_Stage.decorations.Add(drops.itemDeco);
        GameManager.Instance.cur_Stage.decoration_pos.Add(transform.position);
        a.transform.SetParent(GameManager.Instance.entityParent);
    }

    /// <summary>
    /// 상태 변경
    /// </summary>
    /// <param name="nextState">monsterState Enum으로 변경할 값</param>
    public void StateChange(MonsterState nextState)
    {
        if (curState == MonsterState.DIE) return;

        //이전 상태가 포획 상태면 변경 불가
        if (curState == MonsterState.CAUGHT_NET && nextState != MonsterState.DIE) return;
        curState = nextState;
        anim.SetInteger("State", (int)nextState);
    }

    [System.Serializable]
    public class Rand
    {
        public Vector2Int lv_Rand;
        [Tooltip("훈련 최대 포인트 범위 [AttackDamage, AttackSpeed, MaxHP, Exp, Money, Item]")]
        public Vector2Int[] trainingMaxPointRand = new Vector2Int[6];
        [Tooltip("훈련 현재 포인트 범위 [AttackDamage, AttackSpeed, MaxHP, Exp, Money, Item]")]
        public Vector2Int[] trainingCurPointRand = new Vector2Int[6];
        public GameObject[] dropItems;
        public float[] itemsRate;
    }
    [System.Serializable]
    public class Drop
    {
        public float dropMoney;
        public float dropExp;
        public DecorationEnum itemDeco;
        public GameObject dropItem;
    }
    //public void Die()
    //{
    //    anim.SetTrigger("isDie");

    //}
}

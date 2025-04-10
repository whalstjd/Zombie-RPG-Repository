using DataInfo;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

/// <summary>
/// 플레이어 캐릭터를 제어하는 클래스
/// </summary>
public class Player : MonoBehaviour {

    /// <summary>
    /// 조이스틱 참조
    /// </summary>
    [SerializeField]private bl_Joystick Joystick;

    public Animator animator;

    //Skill
    public Image skill_CoolDown_Img;
    public Text skill_CoolDownText;
    bool isSkillCool = false; //공격 쿨타임이 돌아가는 중인지
    //skill+
    public bool isUsingSkill = false; //공격중인지
    public Image skill_Portrait_Img;
    public float skill_x, skill_y;

    //Swap
    public Image swap_CoolDownImg;
    public Text swap_CoolDownText;
    bool isSwapCool = false; //공격 <-> 잡기 변경 쿨타임
    //swap+
    public bool isSword = true;
    public GameObject swordObj;
    public Sprite sword_Sprite;
    public GameObject netObj;
    public Sprite net_Sprite;
    public SpriteResolver backhand_Weapon_SR;

    //Other
    public bool isDie = false; //죽었는지

    /// <summary>
    /// 스킬 사용 시 실행되는 함수
    /// </summary>
    public void UseSkill()
    {
        if (isUsingSkill || isSkillCool || isDie) return;

        isUsingSkill = true;
        isSkillCool = true;

        animator.SetInteger("State", 2);
        StartCoroutine(Skill_CoolDownCor());
    }

    IEnumerator Skill_CoolDownCor()
    {
        float remainCoolDown = DataManager.Instance.gameData.playerStat.AttackSpeed;
        skill_CoolDown_Img.gameObject.SetActive(true);
        skill_CoolDownText.gameObject.SetActive(true);
        while (remainCoolDown > 0)
        {
            yield return null;
            remainCoolDown -= Time.deltaTime;
            skill_CoolDown_Img.fillAmount = remainCoolDown / DataManager.Instance.gameData.playerStat.AttackSpeed;

            skill_CoolDownText.text = (Mathf.Ceil(remainCoolDown * 10) / 10).ToString();
        }
        skill_CoolDown_Img.gameObject.SetActive(false);
        skill_CoolDownText.gameObject.SetActive(false);
        isSkillCool = false;
    }

    /// <summary>
    /// 무기 변환 시 실행되는 함수
    /// </summary>
    public void UseSwap()
    {
        if (isDie || isSwapCool) return;
        isSwapCool = true;
        Weapon_Setting(!isSword);
        GameManager.Instance.PlayAudio(GameManager.Instance.player_Skill_Swap_Clip);

        StartCoroutine(Swap_CoolDownCor());
    }

    IEnumerator Swap_CoolDownCor()
    {
        float coolDownRemain = 1;
        swap_CoolDownText.gameObject.SetActive(true);
        while (coolDownRemain > 0)
        {
            yield return null;
            coolDownRemain -= Time.deltaTime;
            swap_CoolDownImg.fillAmount = coolDownRemain / 1;
            swap_CoolDownText.text = (Mathf.Ceil(coolDownRemain * 10) / 10).ToString();
        }
        swap_CoolDownText.gameObject.SetActive(false);
        isSwapCool = false;
    }

    public void Weapon_Setting(bool _isSword)
    {
        isSword = _isSword;
        string labelname = isSword ? "Dagger" : "Net";
        backhand_Weapon_SR.SetCategoryAndLabel("Item", labelname);
        skill_Portrait_Img.sprite = isSword ? sword_Sprite : net_Sprite;
        skill_CoolDown_Img.sprite = isSword ? sword_Sprite : net_Sprite;
    }

    public void Damaged(float value, int _teamNum)
    {
        DataManager.Instance.gameData.playerStat.hp -= value;
        Monster mon = DataManager.Instance.enemyMonsters[_teamNum];

        //사망처리
        if (DataManager.Instance.gameData.playerStat.hp <= 0)
        {
            animator.SetInteger("State", 5);
            isDie = true;
            DataManager.Instance.gameData.playerStat.cur_Exp = 0;
            GameManager.Instance.PlayAudio(GameManager.Instance.player_Die_Clip);
            UIManager.Instance.restartPanel.gameObject.SetActive(true);
        }
        else
        {
            GameManager.Instance.PlayAudio(GameManager.Instance.damaged_Clip);
            Debug.Log("Still Live");
            if (DataManager.Instance.gameData.playerStat.autoPotion && DataManager.Instance.gameData.playerStat.hp * 2 < DataManager.Instance.gameData.playerStat.Maxhp && DataManager.Instance.gameData.items.GetItemCount(ItemTypeEnum.Potion) > 0)
            {
                DataManager.Instance.gameData.playerStat.UsePotion();
            }
            if (isUsingSkill == false)
                animator.SetInteger("State", 3);
        }
        UIManager.Instance.Player_UI_Update();
        DataManager.Instance.SaveGameData();
    }

    public void PickUp_Item(ItemTypeEnum _itemTypeEnum)
    {
        if (isDie) return;

        DataManager.Instance.gameData.items.AddItem(_itemTypeEnum, 1);
        UIManager.Instance.Goods_UI_Update();
        if (DataManager.Instance.gameData.playerStat.autoPotion && DataManager.Instance.gameData.playerStat.hp * 2 < DataManager.Instance.gameData.playerStat.Maxhp && DataManager.Instance.gameData.items.GetItemCount(ItemTypeEnum.Potion) > 0)
        {
            DataManager.Instance.gameData.playerStat.UsePotion();
        }
    }

    void Update()
    {
        if (isDie) return;


        //Movement
        if(isUsingSkill == false && isDie == false)
        {
            float h = Joystick.Horizontal;
            float v = Joystick.Vertical;

            if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
            {
                //좌우 반전
                transform.localScale = new Vector2(h > 0 ? -1 : 1, 1);

                //h,v의 마지막 위치를 저장하고 스킬쓸 때 사용
                skill_x = h;
                skill_y = v;

                animator.SetInteger("State", 1);
                Vector2 translate = (new Vector2(h, v) * Time.deltaTime) * 2f;
                transform.Translate(translate);

                //플레이어 위치 강제 조정
                if (transform.position.x < GameManager.Instance.moveLimit_Min.x)
                    transform.position = new Vector2(GameManager.Instance.moveLimit_Min.x, transform.position.y);
                else if (transform.position.x > GameManager.Instance.moveLimit_Max.x)
                    transform.position = new Vector2(GameManager.Instance.moveLimit_Max.x, transform.position.y);

                if (transform.position.y < GameManager.Instance.moveLimit_Min.y)
                    transform.position = new Vector2(transform.position.x, GameManager.Instance.moveLimit_Min.y);
                else if (transform.position.y > GameManager.Instance.moveLimit_Max.y)
                    transform.position = new Vector2(transform.position.x, GameManager.Instance.moveLimit_Max.y);
            }
            else
                animator.SetInteger("State", 0);
        }
    }
}
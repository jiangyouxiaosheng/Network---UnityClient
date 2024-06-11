using UnityEngine;
public class BaseHuman : MonoBehaviour
{
    //是否正在移动
    protected bool isMoving = false;
    //移动目标点
    private Vector3 targetPosition;
    //移动速度
    public float speed = 10f;
    //动画组件
   // private Animator animator;
    //描述
    public string desc = "";

    //是否正在攻击
    internal bool isAttacking = false;
    internal float attackTime = float.MinValue;

    // Use this for initialization
    protected void Start()
    {
      //  animator = GetComponent<Animator>();//获得动画组件
    }

    // Update is called once per frame
    internal void Update()
    {
        MoveUpdate();
        AttackUpdate();
    }
    //移动到某处
    public void MoveTo(Vector3 pos)
    {
        targetPosition = pos;
        isMoving = true;
      //  animator.SetBool("isMoving", true);
    }

    //移动Update
    public void MoveUpdate()
    {
        if (isMoving == false)
        {
            return;
        }

        Vector3 pos = transform.position;
        transform.position = Vector3.MoveTowards(pos, targetPosition, speed * Time.deltaTime);
        transform.LookAt(targetPosition);//改变朝向
        if (Vector3.Distance(pos, targetPosition) < 0.05f)
        {
            isMoving = false;
            //animator.SetBool("isMoving", false);
        }
    }

    //攻击动作
    public void Attack()
    {
        isAttacking = true;
        attackTime = Time.time;//记录当前攻击的时间
      // animator.SetBool("isAttacking", true);
    }

    //攻击Update
    public void AttackUpdate()
    {
        if (!isAttacking) return;
        if (Time.time - attackTime < 1.2f) return;//冷却时间，1.2s内只能攻击一次
        isAttacking = false;
       // animator.SetBool("isAttacking", false);
    }
}
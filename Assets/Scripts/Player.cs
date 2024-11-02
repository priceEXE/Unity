using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public float health;//生命值

    [SerializeField] private float speed;//移动速度

    [SerializeField] private float jumpforce;//跳跃力度

    private bool isfacingright = true;

    private float xInput;

    private bool isonground;//是否在地面

    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;//地面检测距离
    [SerializeField] private LayerMask whatIsGround;//检测图层
    
    [Header("Dash info")]
    [SerializeField] private float dashDuration;//冲刺的持续时间

    [SerializeField] private float dashTime;//冲刺时间计数器
    [SerializeField] private float dashSpeed;//冲刺速度

    [SerializeField] private float dashColdTime;//冲刺冷却时间
    private bool isDashing = false;//是否在冲刺

    //private bool couldDashing = true;//是否接受冲刺键输入

    [SerializeField] private int stateCodeOfDash = 0;//冲刺状态机
    
    //private int facingdirection = 1;
    private Rigidbody2D rb;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CollisonCkeck();
        CheckInput();
        Walk();
        DashCheck();
        FlipController();
        AnimatorController();
    }

    private void DashCheck()//冲刺功能
    {
        if(stateCodeOfDash == 0)//初始状态
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                isDashing = true;
                dashTime = 0;
                stateCodeOfDash = 1;
            }
            return;
        }
        else if(stateCodeOfDash == 1)//运行状态
        {
            dashTime += Time.deltaTime;
            rb.velocity = new Vector2( (isfacingright ? 1 : -1) * dashSpeed,0);
            if (dashTime >= dashDuration) 
            {
                isDashing = false;
                dashTime = 0;
                stateCodeOfDash = 2;
            }
            return;
        }
        else if(stateCodeOfDash == 2)
        {
            dashTime += Time.deltaTime;
            if(dashTime >= dashColdTime)
            {
                isDashing = false;
                dashTime = 0;
                stateCodeOfDash = 0;
            }
            return;
        }
    }

    private void CollisonCkeck()//地面碰撞检测，根据检测线在地面图层检测地面
    {
        isonground = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    private void CheckInput()//输入检测器
    {
        xInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Walk()//移动动作
    {
        if(!isDashing)
        {
            rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
        }
    }

    private void Jump()//跳跃方法
    {
        if(isonground)
            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
    }

    private void AnimatorController()//动画指令器
    {
        bool isMoving = rb.velocity.x != 0;
        animator.SetBool("isMoving",isMoving);
        animator.SetBool("isOnGround",isonground);
        animator.SetFloat("yVelocity",rb.velocity.y);
        animator.SetBool("isDashing",isDashing);

    }

    private void Flip()//翻转方法
    {
        //facingdirection = facingdirection * -1;
        isfacingright = !isfacingright;
        transform.Rotate(0,180,0);
    }

    private void FlipController()//翻转指令器
    {
        if(rb.velocity.x > 0 && !isfacingright)
        {
            Flip();
        }
        else if(rb.velocity.x < 0 && isfacingright)
        {
            Flip();
        }
    }
    private void OnDrawGizmos() //绘制检测线工具，在编辑器页面确定具体值
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundCheckDistance));
    }
}

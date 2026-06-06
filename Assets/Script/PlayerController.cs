using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // 组件引用
    private Rigidbody rb;
    private Animator playerAnimator;
    public GameObject playerCamera;

    // 输入变量
    private float horizontalInput;
    private float verticalInput;

    // 用来平滑输入的新变量
    private float smoothX;
    private float smoothZ;
    private float currentVelocityX;
    private float currentVelocityZ;
    public float smoothTime = 0.15f;


    // 控制变量
    private enum PlayerMovingState { Walking, Running, Courching}
    private PlayerMovingState nowState;
    private float moveSpeed = 2f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        nowState = PlayerMovingState.Walking;
    }

    void Update()
    {
        Debug.Log(nowState);
        //设置输入延迟，帮助混合树动画过渡
        smoothX = Mathf.SmoothDamp(smoothX, horizontalInput, ref currentVelocityX, smoothTime);
        smoothZ = Mathf.SmoothDamp(smoothZ, verticalInput, ref currentVelocityZ, smoothTime);
        playerAnimator.SetFloat("VelocityX", smoothX);
        playerAnimator.SetFloat("VelocityZ", smoothZ);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(smoothX * moveSpeed, rb.linearVelocity.y, smoothZ * moveSpeed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //设置不同移动状态下的速度
        Vector2 input = context.ReadValue<Vector2>();
        if (nowState == PlayerMovingState.Running) 
        {
            horizontalInput = input.x * 1.5f;
            verticalInput = input.y * 1.5f;
        }
        else if(nowState == PlayerMovingState.Walking)
        {
            horizontalInput = input.x;
            verticalInput = input.y;    
        }
        else if (nowState == PlayerMovingState.Courching)
        {
            horizontalInput = input.x * 0.7f;
            verticalInput = input.y * 0.7f;
        }
    }
    public void OnRun(InputAction.CallbackContext context) 
    {
        if (context.performed) 
        {
            if (nowState == PlayerMovingState.Running) 
            {
                nowState = PlayerMovingState.Walking;
            }
            else
            {
                nowState = PlayerMovingState.Running;
            }
        }
    }
    public void OnCourch(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            // 如果已经处于下蹲动画
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Crouch"))
            {
                nowState = PlayerMovingState.Walking;
                playerAnimator.SetBool("IsCourching", false);
                
            }
            //  不处于下蹲动画
            else 
            {
                nowState = PlayerMovingState.Courching;
                playerAnimator.SetBool("IsCourching", true);
            }
        }
    }
}

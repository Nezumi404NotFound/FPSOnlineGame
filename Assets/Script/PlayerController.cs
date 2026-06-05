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
    private Vector3 currentVelocity;
    public float smoothTime = 0.15f;


    // 控制变量
    private float moveSpeed = 2f;
    private bool isRunning = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
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
        Vector2 input = context.ReadValue<Vector2>();
        if (isRunning) 
        {
            horizontalInput = input.x * 1.5f;
            verticalInput = input.y * 1.5f;
        }
        else if(!isRunning)
        {
            horizontalInput = input.x;
            verticalInput = input.y;    
        }
    }
    public void OnRun(InputAction.CallbackContext context) 
    {
        if (context.performed) 
        {
            if (isRunning == true) 
            {
                isRunning = false;
            }
            else
            {
                isRunning = true;
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
                playerAnimator.SetBool("IsCourching", false);
            }
            //  不处于下蹲动画
            else 
            {
                playerAnimator.SetBool("IsCourching", true);
            }
        }
    }
    public void OnSlash(InputAction.CallbackContext context) 
    {
        if (context.performed) 
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Slash"))
            {
                return;
            }
            else
            {
                playerAnimator.SetTrigger("Slash_trigger");
            }
        }
    }
}

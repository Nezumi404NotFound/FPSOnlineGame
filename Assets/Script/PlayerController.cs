using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    // 组件引用
    private Rigidbody rb;
    private Animator playerAnimator;
    public GameObject playerCamera;

    // 输入变量
    private Vector2 rawInput;
    private float horizontalSpeed;
    private float verticalSpeed;

    // 用来平滑输入的新变量
    private float smoothX;
    private float smoothZ;
    private float currentVelocityX;
    private float currentVelocityZ;
    public float smoothTime = 0.15f;


   // 玩家移动状态
    public enum PlayerMovingState {Walking, Running, Courching}
    private PlayerMovingState nowState;

    // 控制变量
    private float moveSpeed = 2f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        nowState = PlayerMovingState.Walking;
    }

    void Update()
    {
        CalculateMovementSpeed();
        Debug.Log(horizontalSpeed + ", " + verticalSpeed);
        // 设置输入延迟，帮助混合树动画过渡
        smoothX = Mathf.SmoothDamp(smoothX, horizontalSpeed, ref currentVelocityX, smoothTime);
        smoothZ = Mathf.SmoothDamp(smoothZ, verticalSpeed, ref currentVelocityZ, smoothTime);
        playerAnimator.SetFloat("VelocityX", smoothX);
        playerAnimator.SetFloat("VelocityZ", smoothZ);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(smoothX * moveSpeed, rb.linearVelocity.y, smoothZ * moveSpeed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        rawInput = context.ReadValue<Vector2>();
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
            if (nowState == PlayerMovingState.Courching)
            {
                nowState = PlayerMovingState.Walking;
                playerAnimator.SetBool("IsCourching", false);
            }
            else
            {
                nowState = PlayerMovingState.Courching;
                playerAnimator.SetBool("IsCourching", true);
            }
        }
    }
    private void CalculateMovementSpeed() 
    {
        if (nowState == PlayerMovingState.Running)
        {
            horizontalSpeed = rawInput.x * 1.5f;
            verticalSpeed = rawInput.y * 1.5f;
        }
        else if (nowState == PlayerMovingState.Walking)
        {
            horizontalSpeed = rawInput.x;
            verticalSpeed = rawInput.y;
        }
        else if (nowState == PlayerMovingState.Courching)
        {
            horizontalSpeed = rawInput.x * 0.7f;
            verticalSpeed = rawInput.y * 0.7f;
        }
    }
}

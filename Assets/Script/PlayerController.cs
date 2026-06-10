using Cinemachine;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : NetworkBehaviour
{
    // 组件引用
    private Rigidbody rb;
    private Animator playerAnimator;



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
    public bool isAiming = false;
    

    // 地面检测变量
    private bool isGrounded;
    public GameObject groundCheckPoint;
    private float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    //控制射击变量
    private bool isShootRaycast;
    private float fireRate = 10.0f;
    private float nextFireTime = 0f;

    //枪口焰粒子特效变量
    public ParticleSystem gunFirePrefab;

    //相机控制变量
    private Camera mainCamera;
    public GameObject cameraFollowPoint;
    public CinemachineVirtualCamera virtualCamera;
    private Vector3 cameraOriginPosition;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        mainCamera = Camera.main;
        nowState = PlayerMovingState.Walking;
        gunFirePrefab.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        cameraOriginPosition = cameraFollowPoint.transform.localPosition;
    }

    void Update()
    {
        //设置不同状态速度
        CalculateMovementSpeed();

        //判断是否着地
        isGrounded = Physics.CheckSphere(groundCheckPoint.transform.position, groundCheckDistance, groundLayer);

        //开火射线检测
        if (isShootRaycast && Time.time >= nextFireTime) 
        {
            ShootRaycast();
        }

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
            else if (nowState == PlayerMovingState.Walking)
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
    public void OnJump(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            if(isGrounded)
            {
                playerAnimator.SetTrigger("Jump_trigger");
                rb.AddForce(Vector3.up * 4f, ForceMode.Impulse);
            }
        }
    }
    public void OnFire(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            playerAnimator.SetBool("IsFiring", true);
            isShootRaycast = true;
        }
        else if(context.canceled)
        {
            playerAnimator.SetBool("IsFiring", false);
            isShootRaycast = false;
        }
    }
    public void OnAim(InputAction.CallbackContext context) 
    {
        //瞄准时相机向前偏移
        if (context.performed) 
        {
            StartCoroutine(MovingCameraInAiming());
        }
        //取消瞄准时相机回到原位
        else if (context.canceled)
        {
            StopCoroutine(MovingCameraInAiming());
            cameraFollowPoint.transform.localPosition = cameraOriginPosition;
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
        //射击时减少移速
        if (playerAnimator.GetBool("IsFiring"))
        {
            horizontalSpeed *= 0.8f;
            verticalSpeed *= 0.8f;
        }
    }
    public void ShootRaycast() 
    {
        //计算下一次射击检测时间
        nextFireTime = Time.time + (1f / fireRate);
        //刷新枪口焰粒子特效
        if (gunFirePrefab != null) 
        {
            gunFirePrefab.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            gunFirePrefab.Play(true);
        }
        Vector3 rayOrigin = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        Vector3 rayDirection = mainCamera.transform.forward;
        RaycastHit hit;
        if(Physics.Raycast(rayOrigin, rayDirection, out hit))
        {
            // 处理射击命中逻辑
        }
    }
    IEnumerator MovingCameraInAiming() 
    {
        Vector3 targetPosition = virtualCamera.transform.position + Vector3.forward * 1f;
        while (Vector3.Distance(cameraFollowPoint.transform.localPosition, targetPosition) > 0.001f) 
        {
            cameraFollowPoint.transform.localPosition = Vector3.Lerp(cameraFollowPoint.transform.localPosition, targetPosition, Time.deltaTime * 5f);
            yield return null;
        }
        cameraFollowPoint.transform.localPosition = targetPosition;
    }
}

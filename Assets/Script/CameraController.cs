using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject cameraFollowPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void LateUpdate()
    {
        //相机跟随玩家水平高度
        playerCamera.transform.position = new Vector3(playerCamera.transform.position.x, cameraFollowPosition.transform.position.y, playerCamera.transform.position.z);
    }
}

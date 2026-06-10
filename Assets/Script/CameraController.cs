using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
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
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening) { return; }
        if (virtualCamera == null) { return; }
        NetworkObject localPlayerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        if (localPlayerObject != null)
        {
            var playerController = localPlayerObject.GetComponent<PlayerController>();
            if (playerController.virtualCamera == null)
            {
                playerController.virtualCamera = this.virtualCamera;
            }
            if (playerController != null) 
            {
                virtualCamera.Follow = playerController.cameraFollowPoint.transform;
                virtualCamera.LookAt = playerController.cameraFollowPoint.transform;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("카메라 추적 세팅")] 
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float followSpeed;
    [SerializeField] private float distanceTargetCamera = 4f;

    [Header("카메라 회전 세팅")] 
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float maxUpPitch = 70f;
    [SerializeField] private float minDownPitch = -30f;

    private float yaw; //y축 회전값(좌 우)
    private float pitch; //x축 회전값(위 아래)
    

    private void LateUpdate()
    {
        //마우스 입력
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minDownPitch, maxUpPitch);
        
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 rotatedOffset = rotation * new Vector3(0, 0, -distanceTargetCamera);
        transform.position = target.position + rotatedOffset;
        
        transform.LookAt(target.position + cameraOffset);
    }

    public void ChangeDistanceCamera(float rate)
    {
        distanceTargetCamera *= rate;
    }
}

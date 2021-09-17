using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAiming : MonoBehaviour
{
    [SerializeField] public float turnSpeed = 15;
    Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
    }
    private void FixedUpdate()
    {
        float yCamera = mainCamera.transform.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yCamera, 0), turnSpeed*Time.fixedDeltaTime);
    }
}

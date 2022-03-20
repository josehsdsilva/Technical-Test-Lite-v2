using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target = null;

    float smoothSpeed = 0.05f;
    public Vector3 offset;

    public void Start()
    {
        target = FindObjectOfType<PlayerController>().transform;
    }

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + target.forward * -8 + offset;
        transform.position = desiredPosition;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GetTargetPos() - transform.position), Time.deltaTime * 3);
    }

    Vector3 GetTargetPos()
    {
        return new Vector3(target.position.x, 3f, target.position.z);
    }
}

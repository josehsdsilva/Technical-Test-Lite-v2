using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public void UpdateY(float yDifference)
    {
        DebugLog($"UpdateY: { yDifference }");
        transform.Translate(new Vector3(0, yDifference, 0));
    }

    void DebugLog(string log)
    {
        Debug.Log(log);
    }

    void Update() 
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        movement.Normalize();
        transform.Translate(movement * 6.0f * Time.deltaTime);
    }
}

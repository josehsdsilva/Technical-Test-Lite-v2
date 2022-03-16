using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed = 3;
    private string floorTag = "Floor";

    public void UpdateY(float yDifference)
    {
        transform.Translate(new Vector3(0, yDifference, 0));
    }

    void FixedUpdate() 
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(horizontalInput, 0, 1);
        movement.Normalize();
        transform.Translate(movement * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(floorTag))
        {
            Destroy(this.gameObject);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed = 20;
    private string floorTag = "Floor";

    private MapSpawner mapSpawner;
    public Vector3[] path;
    private int pathIndex = 0;
    private Vector3 target;
    private bool levelFinished = false;

    private void Start()
    {
        mapSpawner = FindObjectOfType<MapSpawner>();
        path = mapSpawner.Positions;
        UpdateTarget();
    }

    public void UpdateY(float yDifference)
    {
        transform.Translate(new Vector3(0, yDifference, 0));
    }

    void FixedUpdate() 
    {
        if(levelFinished) return;
        // float horizontalInput = Input.GetAxis("Horizontal");

        // Vector3 movement = new Vector3(horizontalInput, 0, 1);
        // movement.Normalize();
        Vector3 movement = GetTargetPos() - transform.position;
        transform.Translate(movement.normalized * speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, GetTargetPos()) <= 0.2f)
        {
            UpdateTarget();
        }
    }

    private void UpdateTarget()
    {
        if(pathIndex >= path.Length)
        {
            levelFinished = true;
            return;
        }
        target = path[pathIndex];
        pathIndex++;
    }

    Vector3 GetTargetPos()
    {
        return new Vector3(target.x, transform.position.y, target.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(floorTag))
        {
            Destroy(this.gameObject);
        }
    }

}

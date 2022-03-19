using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MapSpawner mapSpawner;

    private float speed = 6;
    private string floorTag = "Floor";

    public List<Vector3> path;
    private int[] rotations;
    private int pathIndex = 0;
    private Vector3 target;
    private bool levelFinished = false;

    private void Start()
    {
        path = mapSpawner.Level.positions.ToList();
        rotations = mapSpawner.Level.rotations;
        SmoothPath();
        UpdateTarget();
    }

    private void SmoothPath()
    {
        for (int i = 1; i < rotations.Length; i++)
        {
            if(rotations[i] != rotations[i-1])
            {
                path[i-1] = path[i-1] + ((path[i]-path[i-1]) / 2);
            }
        }
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
        if(pathIndex >= path.Count)
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

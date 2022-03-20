using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MapSpawner mapSpawner;

    private float rotationSpeed = 4;
    private float speed = 6;
    private string floorTag = "Floor";

    public List<Vector3> path;
    private int[] rotations;
    private int pathIndex = 0;
    private Vector3 target;
    private bool levelFinished = false;

    private void Start()
    {
        path = mapSpawner.Level.path.ToList();
        UpdateTarget();
    }

    public void UpdateY(float yDifference)
    {
        transform.Translate(new Vector3(0, yDifference, 0));
    }

    void FixedUpdate() 
    {
        if(levelFinished) return;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(GetTargetPos() - transform.position), Time.deltaTime * rotationSpeed);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Serializable Field

    public MapSpawner mapSpawner;
    [SerializeField]
    UnityEvent cubeConnected;
    [SerializeField]
    UnityEvent cubeDisconnected;

    #endregion

    #region Fields

    private List<Vector3> path;
    private int[] rotations;
    private Vector3 target;
    private Vector3 sideOffset;
    private float rotationSpeed = 5;
    private float speed = 6;
    private float input;
    private string floorTag = "Floor";

    private int pathIndex = 0;

    #endregion

    private void Start()
    {
        path = mapSpawner.Level.path.ToList();
    }

    public void CubeConnected(float yDifference)
    {
        transform.Translate(new Vector3(0, yDifference, 0));
        cubeConnected.Invoke();
    }

    public void CubeDisconnected()
    {
        cubeDisconnected.Invoke();
    }

    public void SetInput(float _input)
    {
        if(_input == 0)
        {
            input = 0;
            Debug.Log(input);
            return;
        }
        input = _input > 0 ? 0.1f : -0.1f;
        Debug.Log(input);
    }

    void FixedUpdate() 
    {

        if(input != 0 && Vector3.Distance(transform.position, GetTargetPos()) >= 1.5)
        {
            transform.Translate(Vector3.right * input);
            sideOffset += Vector3.right * input;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((GetTargetPos() - transform.position).normalized), Time.deltaTime * rotationSpeed);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, GetTargetPos()) <= 0.3f)
        {
            pathIndex++;
            if(pathIndex >= path.Count)
            {
                PlayerPrefs.SetInt("level" + (mapSpawner.currentLevel), 1);
                SceneManager.LoadScene("LevelSelection");
                return;
            }
        }
        target = path[pathIndex] + sideOffset;

        if(transform.position.y < 0)
        {
            SceneManager.LoadScene("LevelSelection");
            return;
        }
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

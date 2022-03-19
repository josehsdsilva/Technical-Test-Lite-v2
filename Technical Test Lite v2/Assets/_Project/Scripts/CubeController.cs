using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    #region Serializable Fields


    #endregion

    #region Fields

    private PlayerController playerController;
    private float cubeHeight = 0.55f;
    private string cubeTag = "Cube";
    private string wallTag = "Wall";
    private Transform parentTransform;
    [SerializeField]
    private bool connected = false;

    #endregion

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(cubeTag))
        {
            // Collided with cube
            if(connected && !other.GetComponent<CubeController>().connected)
            {
                // Can connect
                playerController.UpdateY(cubeHeight);
                other.GetComponent<CubeController>().Connect(transform);
            }
        }
        else if(other.CompareTag(wallTag))
        {
            // Collided with wall
            Disconnect();
        }
    }

    void Connect(Transform parent)
    {
        connected = true;
        transform.parent = parent;
        transform.position = new Vector3(parent.position.x, transform.position.y, parent.position.z);
    }
    
    void Disconnect()
    {
        if(transform.parent == null) return;

        CubeController parenteCubeController = transform.parent.GetComponent<CubeController>();
        transform.parent = playerController.mapSpawner.transform;
    }
    
}

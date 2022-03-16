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
    private bool connected;

    #endregion

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        // if(transform.parent != null) connected = true;
    }

    void Connect(Transform parent)
    {
        if(transform.parent != null) return;

        transform.parent = parent;
        transform.position = new Vector3(parent.position.x, transform.position.y, parent.position.z);
        
    }
    
    async Task Disconnect()
    {
        if(transform.parent == null) return;

        CubeController parenteCubeController = transform.parent.GetComponent<CubeController>();
        if(parenteCubeController != null) parenteCubeController.connected = false;
        transform.parent.DetachChildren();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(cubeTag))
        {
            // Collided with cube
            if(!connected && transform.parent != null && other.transform.parent == null )
            {
                // Can connect
                connected = true;
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

}

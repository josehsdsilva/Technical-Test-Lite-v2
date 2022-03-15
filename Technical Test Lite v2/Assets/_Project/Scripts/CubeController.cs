using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    #region Serializable Fields

    [SerializeField]
    private PlayerController playerController;

    #endregion

    #region Fields

    private float cubeHeight = 0.55f;
    private string cubeTag = "GoodCube";
    private Transform parentTransform;

    #endregion

    void Connect(Transform parent)
    {
        if(transform.parent != null) return;

        transform.parent = parent;
        transform.position = new Vector3(parent.position.x, transform.position.y, parent.position.z);
    }
    
    void Disconnect()
    {
        transform.parent.DetachChildren();

        playerController.UpdateY(-cubeHeight);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(cubeTag) && transform.parent != null)
        {
            playerController.UpdateY(cubeHeight);
            other.GetComponent<CubeController>().Connect(transform);
            SetColliderEnable(false);
        }
    }

    void SetColliderEnable(bool enabled)
    {
        GetComponent<BoxCollider>().enabled = enabled;
    }
}

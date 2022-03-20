using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    private TouchControls touchControls;
    private Coroutine coroutine;

    Vector2 lastPos, currenPos;

    private void Awake()
    {
        touchControls = new TouchControls();
    }

    private void OnEnable()
    {
        touchControls.Enable();
    }

    private void OnDisable()
    {
        touchControls.Disable();
    }

    private void Start()
    {
        touchControls.Touch.TouchPress.started += _ => StartTouch();
        touchControls.Touch.TouchPress.canceled += _ => EndTouch();
    }

    private void StartTouch()
    {
        coroutine = StartCoroutine(Delta());
    }
    private void EndTouch()
    {
        StopCoroutine(coroutine);
        playerController.SetInput(0);
    }

    IEnumerator Delta()
    {
        while(true)
        {
            currenPos = touchControls.Touch.TouchPosition.ReadValue<Vector2>();
            playerController.SetInput(currenPos.x - lastPos.x);
            lastPos = currenPos;
            yield return null;
        }
    }

}

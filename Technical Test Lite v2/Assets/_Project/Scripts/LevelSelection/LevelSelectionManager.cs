﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    [SerializeField] GameObject button;
    private void Start()
    {
        Map[] levels = Resources.LoadAll<Map>("Maps");
        int levelCount = levels.Length;

        GameObject go;
        for (int i = 0; i < levelCount; i++)
        {
            go = Instantiate(button, this.transform);
            go.GetComponent<ButtonController>().level = i + 1;
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}


﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private float startTime;
    [SerializeField]
    private float duration;
    //public Material water2plastic;
    public List<Renderer> renderers;

    // Start is called before the first frame update
    void Awake()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Scene1":

                break;
            case "Scene2":
                float completionPercentage = Time.time / (startTime + duration);
                foreach (Renderer rend in renderers)
                {
                    rend.material.SetFloat("_Blend", Mathf.Min(1, completionPercentage));
                }
                break;
            default:
                break;
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                PrintDataAndLoadScene();
            }
        }

    }

    private void FixedUpdate()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Scene1":

                break;
            case "Scene2":
                print("logging position");
                DataManager.AddPosition(new Vector2(MeasureDepth.Instance.CenterOfMass.x, MeasureDepth.Instance.CenterOfMass.y));

                break;
            default:
                break;
        }
    }

    private void PrintDataAndLoadScene()
    {
        string nextScene = "";
        switch (SceneManager.GetActiveScene().name)
        {
            case "Scene1":
                DataManager.PrintTriggerData();
                nextScene = "Scene2";
                break;
            case "Scene2":
                DataManager.PrintPositionData();
                nextScene = "Scene1";
                break;
            default:
                break;
        }
        SceneManager.LoadScene(nextScene);
        //fade to black and load scene async via corountine

    }

    private void FadeToBlack()
    {
            throw new NotImplementedException();
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warper : MonoBehaviour {

    public WarpSystem ws;
    public float velocity;   
    public float rotationVelocity;
    public GameMenu gameMenu;

    public GameObject Warpel;
    public GameObject WarpTrail;
    public GameObject BoomBloom;

    private Warp currentSection;
    private float spaceRotation;
    private float sparkRotation;
    private Transform space;
    private Transform rotator;

    private float deltaToRotation;
    private float systemRotation;
    private float distanceTraveled;

    public bool faded = false;
    private int _gameMode = 0;

    private void Awake()
    {
        space = ws.transform.parent;
        rotator = transform.GetChild(0);       
        gameObject.SetActive(false);
    }

    public void StartGame(int gameMode)
    {
        _gameMode = gameMode;
        sparkRotation = 0f;
        spaceRotation = 0f;
        systemRotation = 0f;
        distanceTraveled = 0f;
        ws.GameMode = gameMode;
        currentSection = ws.SetupFirstWarp();
        SetupCurrentWarp();
        faded = false;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        float delta = velocity * Time.deltaTime;
        distanceTraveled += delta;
        systemRotation += delta * deltaToRotation;

        if(systemRotation >= currentSection.CurveAngle)
        {
            delta = (systemRotation - currentSection.CurveAngle) / deltaToRotation;
            currentSection = ws.SetupNextWarp();
            SetupCurrentWarp();
            systemRotation = delta * deltaToRotation;
        }

        ws.transform.localRotation = Quaternion.Euler(0f, 0f, systemRotation);
        UpdateSparkRotation();
    }

    private void SetupCurrentWarp()
    {
        deltaToRotation = 360f / (2f * Mathf.PI * currentSection.CurveRadius);
        spaceRotation += currentSection.RelativeRotation;
        if(spaceRotation < 0f)
        {
            spaceRotation += 360f;
        } 
        else if(spaceRotation >= 360f)
        {
            spaceRotation -= 360f;
        }

        space.localRotation = Quaternion.Euler(spaceRotation, 0f, 0f);
    }

    private void UpdateSparkRotation()
    {
        sparkRotation += rotationVelocity * Time.deltaTime * GetDirection();
        if(sparkRotation < 0f)
        {
            sparkRotation += 360f;
        }
        else if(sparkRotation >= 360f)
        {
            sparkRotation -= 360f;
        }

        rotator.localRotation = Quaternion.Euler(sparkRotation, 0f, 0f);
    }

    public void Fade()
    {
        if(!faded)
        {
            faded = true;
            _gameMode = 0;
            Warpel.SetActive(false);
            WarpTrail.SetActive(false);
            gameMenu.GameOver();
        }
    }

    private float GetDirection()
    {
        var LeftRight = 0;
        var screenWidth = Screen.width / 2;
#if UNITY_EDITOR
        if (_gameMode > 0)
        {
            if (Input.GetMouseButton(1))
            {
                return 1;
            }
            else if (Input.GetMouseButton(0))
            {
                return -1;
            }
        }

        return Input.GetAxis("Horizontal");
#endif

#if UNITY_ANDROID
            if (Input.touchCount > 0)
        {
            // touch x position is bigger than half of the screen, moving right
            if (Input.GetTouch(0).position.x > screenWidth)
                LeftRight = 1;
            // touch x position is smaller than half of the screen, moving left
            else if (Input.GetTouch(0).position.x < screenWidth)
                LeftRight = -1;
        }
#endif

        return LeftRight;
    }
}

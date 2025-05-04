using System.Collections.Generic;
using UnityEngine;
using System;

public class BackgroundsManager : MonoBehaviour
{
    public static BackgroundsManager instance;

    public GameObject level1Background;
    public GameObject level2Background;
    public GameObject level3Background;

    //Stage 1
    public GameObject greenMountains;
    public GameObject bottomSnowMountains;
    public GameObject topSnowMountains;
    public GameObject sky;
    public GameObject bottomWhiteClouds;
    public GameObject topWhiteClouds;
    public GameObject night;
    public GameObject bottomGreyClouds;
    public GameObject topGreyClouds;
    public GameObject space1;
    public GameObject space2;
    public GameObject infinateSpace1;
    public GameObject infinateSpace2;

    //Stage 2
    public GameObject sandHills;
    public GameObject sandMountains;
    public GameObject Yellowsky;
    public GameObject bottomClouds;
    public GameObject MiddleClouds;
    public GameObject topClouds;
    public GameObject BottomNightClouds;
    public GameObject TopNightClouds;
    public GameObject night2;
    public GameObject space1_2;
    public GameObject space2_2;
    public GameObject infinateSpace1_2;
    public GameObject infinateSpace2_2;

    private Dictionary<int, Action> scoreEvents = new Dictionary<int, Action>();
    void Start()
    {
        instance = this;
    }

    public void SwitchBackground()
    {
        if (GameManager.levelNumber == 3)
        {
            level3Background.SetActive(true);
            level1Background.SetActive(false);
            level2Background.SetActive(false);
        }
        else if (GameManager.levelNumber == 2)
        {
            level1Background.SetActive(false);
            level2Background.SetActive(true);
            level3Background.SetActive(false);
        }
        else
        {
            level1Background.SetActive(true);
            level2Background.SetActive(false);
            level3Background.SetActive(false);
        }
    }
    

    public void AddBackGrounds(int score)
    {

        if (GameManager.levelNumber == 1)
        {
            switch (score)
            {
                case 1:
                    sky.SetActive(true);
                    break;
                case 3:
                    bottomWhiteClouds.SetActive(true);
                    break;
                case 6:
                    topWhiteClouds.SetActive(true);
                    break;
                case 9:
                    night.SetActive(true);
                    break;
                case 16:
                    space1.SetActive(true);
                    break;
                case 25:
                    space2.SetActive(true);
                    break;
                case 36:
                    infinateSpace1.SetActive(true);
                    break;
                case 47:
                    infinateSpace2.SetActive(true);
                    break;
            }
        }
        else if (GameManager.levelNumber == 2)
        {
            switch (score)
            {

                case 3:
                    MiddleClouds.SetActive(true);
                    break;
                case 8:
                    night2.SetActive(true);
                    break;
                case 16:
                    space1_2.SetActive(true);
                    break;
                case 25:
                    space2_2.SetActive(true);
                    break;
                case 36:
                    infinateSpace1_2.SetActive(true);
                    break;
                case 47:
                    infinateSpace2_2.SetActive(true);
                    break;
            }
        }
    }

    public void DestroyBackGrounds(int score)
    {
        if (GameManager.levelNumber == 1)
        {
            switch (score)
            {
                case 4:
                    Destroy(greenMountains);
                    break;
                case 5:
                    Destroy(bottomSnowMountains);
                    break;
                case 7:
                    Destroy(topSnowMountains);
                    break;
                case 9:
                    Destroy(bottomWhiteClouds);
                    break;
                case 10:
                    Destroy(sky);
                    break;
                case 12:
                    Destroy(topWhiteClouds);
                    break;
                case 15:
                    Destroy(bottomGreyClouds);
                    break;
                case 17:
                    Destroy(topGreyClouds);
                    break;
                case 18:
                    Destroy(night);
                    break;
                case 28:
                    Destroy(space1);
                    break;
                case 39:
                    Destroy(space2);
                    break;
            }
          
        }
        else
        {
            switch (score)
            {
                case 3:
                    Destroy(sandHills);
                    break;
                case 4:
                    Destroy(sandMountains);
                    break;
                case 6:
                    Destroy(bottomClouds);
                    break;
                case 9:
                    Destroy(MiddleClouds);
                    break;
                case 12:
                    Destroy(Yellowsky);
                    break;
                case 18:
                    Destroy(night2);
                    break;
                case 28:
                    Destroy(space1_2);
                    break;
                case 39:
                    Destroy(space2_2);
                    break;
            }
        }

        // Update infinite space for scores >= 50
        if (score >= 50)
        {
            UpdateInfiniteSpace(score);
        }
    }

    void UpdateInfiniteSpace(int score)
    {
        float offset = 122f; // The vertical movement offset.

        //score % 40 calculates the remainder when the score is divided by 40.
        //This allows the logic to repeat every 40 points (50, 60, 70, 80 → 90, 100, 110, 120, etc.).

        if (score % 40 == 10 || score % 40 == 30) // For scores like 50, 70, 90, etc.
        {
            if (GameManager.levelNumber == 1)
            {
                infinateSpace1.transform.localPosition = new Vector2(0, infinateSpace1.transform.localPosition.y + offset);
            }
            else
            {
                infinateSpace1_2.transform.localPosition = new Vector2(0, infinateSpace1.transform.localPosition.y + offset);
            }

            
        }
        else if (score % 40 == 20 || score % 40 == 0) // For scores like 60, 80, 100, etc.
        {
            if (GameManager.levelNumber == 1)
            {
                infinateSpace2.transform.localPosition = new Vector2(0, infinateSpace2.transform.localPosition.y + offset);
            }
            else
            {
                infinateSpace2_2.transform.localPosition = new Vector2(0, infinateSpace2.transform.localPosition.y + offset);
            }
                
        }
    }
}


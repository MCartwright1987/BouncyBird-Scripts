using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using MaskTransitions;

public class WorldSelectMenu : MonoBehaviour
{
    public static WorldSelectMenu instance;

    public RuntimeAnimatorController[] animatorControllers;

    [SerializeField] Sprite[] worldImage;

    [SerializeField] GameObject[] toUnlockTxt;
    [SerializeField] Button startButton;

    [SerializeField] Image worldImageOnDisplay;
    [SerializeField] TextMeshProUGUI stageNumber;


    public int playerSelectIndex = 0;

    void Start()
    {
        instance = this;

        if (GameManager.levelNumber == 2)
        {
            worldImageOnDisplay.sprite = worldImage[1];
            stageNumber.text = "2";

        }
        else if (GameManager.levelNumber == 3)
        {
            worldImageOnDisplay.sprite = worldImage[2];
            stageNumber.text = "3";
        }
    }

    public void SwitchLevel(bool left)
    {
        int currentLevel = GameManager.levelNumber;

        if (left) currentLevel--; 
        else currentLevel++ ;

        if (currentLevel < 1) currentLevel = 3;
        if (currentLevel > 3) currentLevel = 1;

        if (currentLevel == 2)
        {
            worldImageOnDisplay.sprite = worldImage[1];
            stageNumber.text = "2";
        }
        else if (currentLevel == 3)
        {
            worldImageOnDisplay.sprite = worldImage[2];
            stageNumber.text = "3";
        }
        else if (currentLevel == 1)
        {
            worldImageOnDisplay.sprite = worldImage[0];
            stageNumber.text = "1";
        }

        GameManager.levelNumber = currentLevel;


        if (CanvasManager.instance.leaderBoardCanvas.activeSelf == false)
        {
            GameManager.instance.ToggleDisableBirdAndLevelSelect();
            TransitionManager.Instance.PlayTransition(0.6f);
            GameManager.instance.UpdateLevelDelayed();
        }
        else GameManager.instance.UpdateLevel();

    }
}

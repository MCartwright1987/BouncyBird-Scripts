using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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


    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        if (GameManager.levelNumber == 2)
        {
            worldImageOnDisplay.sprite = worldImage[1];
            stageNumber.text = "2";

        }
    }

    public void ResetUnlocks()
    {

    }

    public void UnlocksAllUnlocks()
    {

    }

    public void SwitchLevel(bool left)
    {
        int currentLevel = GameManager.levelNumber;

        if (left) currentLevel--; 
        else currentLevel++ ;

        if (currentLevel < 0) currentLevel = 3;
        if (currentLevel > 3) currentLevel = 0;



        if (currentLevel == 2)
        {
            worldImageOnDisplay.sprite = worldImage[1];
            stageNumber.text = "2";
        }
        else if (currentLevel == 3)
        {
            worldImageOnDisplay.sprite = worldImage[0];
            stageNumber.text = "3";
        }
        else if (currentLevel == 1)
        {
            worldImageOnDisplay.sprite = worldImage[0];
            stageNumber.text = "1";
        }

        GameManager.instance.SwitchLevel();
    }
}

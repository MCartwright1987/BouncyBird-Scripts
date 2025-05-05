using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;    

    [SerializeField] GameObject button1;
    [SerializeField] GameObject button2;
    [SerializeField] GameObject button3;
    [SerializeField] GameObject button4;

    private void Start()
    {
        Instance = this;

        button1.SetActive(true);
    }

    public void LockEverything()
    {
        SaveSystem.SetBool("player1Unlocked", false);
        SaveSystem.SetBool("player2Unlocked", false);
        SaveSystem.SetBool("player3Unlocked", false);
        SaveSystem.SetBool("player4Unlocked", false);
        SaveSystem.SetBool("player5Unlocked", false);
        SaveSystem.SetBool("player6Unlocked", false);
        SaveSystem.SetBool("player7Unlocked", false);
        SaveSystem.SetBool("player8Unlocked", false);
        SaveSystem.SetBool("player9Unlocked", false);
        SaveSystem.SetBool("player10Unlocked", false);

        SaveSystem.SetInt("AdsRemoved", 0);

        AdsInitializer.instance.InitializeAds();

        AdsInitializer.instance.gameObject.SetActive(true);
        GameManager.instance.skipAdsTokensTopOfScreen.SetActive(true);
        GameManager.instance.StartScreenPlayAdBtn.SetActive(true);

        SaveSystem.SetInt("BestCombinedTimeAndScore", 0);
        SaveSystem.SetInt("BestCombinedTimeAndScore2", 0);
        SaveSystem.SetInt("BestCombinedTimeAndScore3", 0);
    }

    public void UnlockEverything()
    {
        SaveSystem.SetBool("player1Unlocked", true);
        SaveSystem.SetBool("player2Unlocked", true);
        SaveSystem.SetBool("player3Unlocked", true);
        SaveSystem.SetBool("player4Unlocked", true);
        SaveSystem.SetBool("player5Unlocked", true);
        SaveSystem.SetBool("player6Unlocked", true);
        SaveSystem.SetBool("player7Unlocked", true);
        SaveSystem.SetBool("player8Unlocked", true);
        SaveSystem.SetBool("player9Unlocked", true);
        SaveSystem.SetBool("player10Unlocked", true);

        SaveSystem.SetInt("AdsRemoved", 1);

        AdsInitializer.instance.gameObject.SetActive(false);
        GameManager.instance.skipAdsTokensTopOfScreen.SetActive(false);
        GameManager.instance.StartScreenPlayAdBtn.SetActive(false);
    }

    public void SetAskForReviewToZero()
    {
        PlayerPrefs.SetInt("ReviewAsked", 0);
    }

    public void ReduceScoreToOne()
    {
        int combinedScore = GameManager.instance.CombineScoreAndTime(1, 61, 0);

        SaveSystem.SetInt("BestCombinedTimeAndScore", combinedScore);
        SaveSystem.SetInt("BestCombinedTimeAndScore2", combinedScore);
        SaveSystem.SetInt("BestCombinedTimeAndScore3", combinedScore);

        LeaderboardManager.Instance.SubmitScoreLeaderBoard1(combinedScore);
        LeaderboardManager.Instance.SubmitScoreLeaderBoard2(combinedScore);
        LeaderboardManager.Instance.SubmitScoreLeaderBoard3(combinedScore);
    }

    // these are the hidden buttons on the main screen that have to be 
    //pressed in order to access the debug menu used for testing
    public void Button1()
    {
        button1.SetActive(false);
        button2.SetActive(true);

        StartCoroutine(SetButtonsOneActive());
    }

    public void Button2()
    {
        button2.SetActive(false);
        button3.SetActive(true);
    }

    public void Button3()
    {
        button3.SetActive(false);
        button4.SetActive(true);
    }

    public void Button4()
    {
        if (!button1.activeSelf)
        {
            CanvasManager.instance.ToggleDebugCanvas(true);
        }       
    }

    IEnumerator SetButtonsOneActive()
    {
        yield return new WaitForSeconds(2);

        button1.SetActive(true);
        button2.SetActive(false);
        button3.SetActive(false);
        button4 .SetActive(false);
    }


}

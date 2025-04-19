using Samples.Purchasing.Core.BuyingConsumables;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using System.Threading.Tasks;
using Unity.VisualScripting;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;

    [SerializeField] GameObject debugCanvas;
    [SerializeField] GameObject leaderBoardCanvas;
    [SerializeField] GameObject selectLanguageCanvas;
    public GameObject settingsCanvas;
    [SerializeField] GameObject shopCanvas;
    public GameObject inputNameCanvas;
    [SerializeField] GameObject howToPlayCanvas;
    [SerializeField] GameObject selectFlagCanvas;
    public GameObject continueCanvas;
    [SerializeField] GameObject areYouSureCanvas;
    public GameObject internetUnreachableCanvas;
    public GameObject birdSelectCanvas;
    public GameObject worldSelectCanvas;
    public GameObject trailSelectCanvas;
    public GameObject startScreenCanvas;
    public GameObject levelNumbersCanvas;
    public GameObject inGameCanvas;
    public GameObject ConsentCanvas;

    [SerializeField] GameObject blockLeaderBoardButtonImage;
   

    [SerializeField] Button changeUsernameBtn;
    [SerializeField] GameObject restorePurchasesBtn;

    [SerializeField] GameObject[] settingsLanguageTextObjects;
    [SerializeField] Image flagImage;
    public Sprite[] flags;

    [SerializeField] RectTransform continueStreakFill;

    [SerializeField] int continueStreakIncrement;
    [SerializeField] GameObject BlockContinueCanvasButtons;
    public float tempContinueStreakValue = 0;
    private int tempContinueTallyValue = 0;

    [SerializeField] TextMeshProUGUI continueTally;

    public GameObject[] continueStreakPrize;

    [SerializeField] Animator[] startScreenAnimators;

    public RectTransform[] StartScreenCanvases;

    bool howToPlayOpenedFromSettingsMenu = false;

    private void Awake()
    {
        instance = this;     
    }

    IEnumerator ActivateBlockButtonsImageBriefly()
    {
        blockLeaderBoardButtonImage.SetActive(true);
        yield return new WaitForSeconds(1.1f);
        blockLeaderBoardButtonImage.SetActive(false);
    }

    public void ActivateStartScreenAnimations()
    {
        foreach (var animation in startScreenAnimators)
        {
            animation.enabled = true;
        }
    }

    public void ShowAd() => RewardedAdsButton.instance.ShowAd();

    public void AddToContinueStreakAndCloseContinueCanvas() => StartCoroutine(ProgressCanvasStreak());

    public void ResetContinueStreak()=> SaveSystem.SetFloat("ContinueStreak", 0);

    IEnumerator ProgressCanvasStreak()
    {
        float targetAmount = tempContinueStreakValue + (170 / continueStreakIncrement);

        float duration = 1f; // Adjust duration for smoothness
        float elapsed = 0f;

        Audio.instance.ProgressBar.Play();

        BlockContinueCanvasButtons.SetActive(true);

        tempContinueTallyValue++;
        continueTally.text = tempContinueTallyValue.ToString();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newWidth = Mathf.Lerp(tempContinueStreakValue, targetAmount, elapsed / duration);
            continueStreakFill.sizeDelta = new Vector2(newWidth, continueStreakFill.sizeDelta.y);
            yield return null;
        }

        // Ensure it reaches the exact target value at the end
        continueStreakFill.sizeDelta = new Vector2(targetAmount, continueStreakFill.sizeDelta.y);

        if (targetAmount >= 166) // should be 170 but it rounds the float down so ends up 168
        {
            if (continueStreakPrize[0].activeSelf == true) GameManager.instance.AddSkipAdTokens(1);
            else if (continueStreakPrize[1].activeSelf == true) GameManager.instance.AddSkipAdTokens(2);
            else if (continueStreakPrize[2].activeSelf == true)
            {
                GameManager.instance.UnlockPlayer("player10Unlocked", 10, false);
            }
        }
        else SaveSystem.SetFloat("ContinueStreak", targetAmount);


        SaveSystem.SetInt("ContinueTally", tempContinueTallyValue);

        yield return new WaitForSeconds(0.4f);
        BlockContinueCanvasButtons.SetActive(false);
        //if (changeBarColor) ChangeFillBarColor(tempContinueTallyValue);
        ToggleContinueCanvas();
    }

    public void ChangeFillBarColor(int tally)
    {
        Image fillBar = continueStreakFill.GetComponent<Image>();

        // Get the tally within a 0-14 range by using modulo
        int cycleTally = tally % 15;

        if (cycleTally < 3)
            fillBar.color = new Color(0.3568628f, 0.8313726f, 0.345098f); // Green
        else if (cycleTally >= 3 && cycleTally < 6)
            fillBar.color = new Color(0.9921569f, 1, 0.4117647f); // Yellow
        else if (cycleTally >= 6 && cycleTally < 9)
            fillBar.color = new Color(0.2901961f, 0.8196079f, 1); // Blue
        else if (cycleTally >= 9 && cycleTally < 12)
            fillBar.color = new Color(0.7823129f, 0.4999999f, 1); // Purple
        else if (cycleTally >= 12 && cycleTally < 15)
            fillBar.color = new Color(0.372549f, 0.3411765f, 0.3098039f); // Grey
    }


    public void ChangeContinueStreakPrize(int tally)
    {
        SetContinueStreakPrize(SaveSystem.GetInt("ContinueStreakPrize"));

        if (tally >= 12 && tally <=14 && SaveSystem.GetBool("player10Unlocked") == false)
        {
            SetContinueStreakPrize(2);
            return;
        }

        if (tally == 0 || tally % 6 == 0)
        {
            SetContinueStreakPrize(0);
        }
        // If the tally is a multiple of 3 but not 6, give 1 token
        else if (tally % 3 == 0)
        {
            SetContinueStreakPrize(1);
        }
    }

    public void SetContinueStreakPrize(int prizeNumber)
    {
        for (int i = 0; i < continueStreakPrize.Length; i++)
        {
            if (i != prizeNumber) continueStreakPrize[i].SetActive(false);
            else continueStreakPrize[i].SetActive(true);
        }

        SaveSystem.SetInt("ContinueStreakPrize", prizeNumber);
    }

    public void ToggleContinueCanvas()
    {
        if (continueCanvas.activeSelf)
        {
            continueCanvas.SetActive(false);
        }
        else
        {
            ChangeFillBarColor(SaveSystem.GetInt("ContinueTally"));
            ChangeContinueStreakPrize(SaveSystem.GetInt("ContinueTally"));

            continueStreakFill.sizeDelta = new Vector2(SaveSystem.GetFloat("ContinueStreak"), 18);
            continueTally.text = SaveSystem.GetInt("ContinueTally").ToString();


            //to prevent players from closing game down to save continue streak
            tempContinueStreakValue = SaveSystem.GetFloat("ContinueStreak");
            SaveSystem.SetFloat("ContinueStreak", 0);

            tempContinueTallyValue = SaveSystem.GetInt("ContinueTally");
            SaveSystem.SetInt("ContinueTally", 0);


            continueCanvas.SetActive(true);
            continueCanvas.transform.GetChild(0).transform.GetChild(2).GetComponent<Button>().interactable = SaveSystem.GetInt("SkipAdTokens") > 0;
        }
    }

    public void AcceptLeaderBoardUpload()
    {
        PlayerPrefs.SetInt("AcceptLeaderBoardUpload", 1);
        ToggleConsentCanvas(false);
        TogglePickUsernameCanvas();
    }

    public void UploadToLeaderBoard()
    {
        if (SaveSystem.HasKey("BestCombinedTimeAndScore"))
        {
            LeaderboardManager.Instance.SubmitScoreLeaderBoard1(SaveSystem.GetInt("BestCombinedTimeAndScore"));
        }

        if (SaveSystem.HasKey("BestCombinedTimeAndScore2"))
        {
            LeaderboardManager.Instance.SubmitScoreLeaderBoard2(SaveSystem.GetInt("BestCombinedTimeAndScore2"));
        }
    }


    public void SelectFlag(int flagNumber)
    {
        PlayerPrefs.SetInt("FlagNumber", flagNumber);
        flagImage.sprite = flags[flagNumber];

        if (SaveSystem.HasKey("BestCombinedTimeAndScore"))
        {
            int newCombinedScore = AddFlagToCombinedScore(SaveSystem.GetInt("BestCombinedTimeAndScore"), flagNumber);
            SaveSystem.SetInt("BestCombinedTimeAndScore", newCombinedScore);

            int bestScore = (GameManager.instance.GetScoreFromCombinedValue(SaveSystem.GetInt("BestCombinedTimeAndScore")));

            if (bestScore > LeaderboardManager.LastScoreLeaderBoard1)
            {
                if (PlayerPrefs.GetInt("AcceptLeaderBoardUpload") == 1)
                {
                    LeaderboardManager.Instance.SubmitScoreLeaderBoard1(newCombinedScore);
                }
            }         
        }

        if (SaveSystem.HasKey("BestCombinedTimeAndScore2"))
        {
            int newCombinedScore = AddFlagToCombinedScore(SaveSystem.GetInt("BestCombinedTimeAndScore2"), flagNumber);
            SaveSystem.SetInt("BestCombinedTimeAndScore2", newCombinedScore);

            int bestScore = (GameManager.instance.GetScoreFromCombinedValue(SaveSystem.GetInt("BestCombinedTimeAndScore2")));

            if (bestScore > LeaderboardManager.LastScoreLeaderBoard2)
            {
                if (PlayerPrefs.GetInt("AcceptLeaderBoardUpload") == 1)
                {
                    LeaderboardManager.Instance.SubmitScoreLeaderBoard2(newCombinedScore);
                }
            }
        }
    }

    public int AddFlagToCombinedScore(int combinedScore, int flag)
    {
        flag = Mathf.Clamp(flag, 0, 29);

        // Clear the last 5 bits (flag) using bitwise AND with a mask
        int clearedCombinedScore = combinedScore & ~0x1F; // 0x1F (0001 1111) clears last 5 bits

        // Add the new flag in the last 5 bits
        return clearedCombinedScore | flag;
    }
    public void ToggleConsentCanvas(bool active)
    {
        ConsentCanvas.SetActive(active);
        birdSelectCanvas.SetActive(!active);
    }

    public void ToggleSelectFlagCanvas(bool active)
    {
        selectFlagCanvas.SetActive(active);
    }

    public void ToggleDebugCanvas(bool active)
    {
        debugCanvas.SetActive(active);
    }

    public void ToggleLevelNumbersCanvas(bool active)
    {
        levelNumbersCanvas.SetActive(active);
    }

    public void ToggleBirdSelectCanvas(bool active)
    {
        birdSelectCanvas.SetActive(active);
    }

    public void ToggleWorldSelectCanvas(bool active)
    {
        worldSelectCanvas.SetActive(active);
    }

    public void ToggleTrailSelectCanvas(bool active)
    {
        trailSelectCanvas.SetActive(active);
    }

    public void ToggleSettingsMenu()
    {

        howToPlayOpenedFromSettingsMenu = true;

        bool isActive = settingsCanvas.activeSelf;

        if (isActive) CycleThroughLanguageButtonText();

        settingsCanvas.SetActive(!isActive);
        birdSelectCanvas.SetActive(isActive);

        
        if (!isActive && PlayerPrefs.GetInt("AcceptLeaderBoardUpload") == 0)
        {
            changeUsernameBtn.interactable = false;
        }
        else
        {
            changeUsernameBtn.interactable = true;
        }

    }

    public void ToggleLanguageSelectCanvas()
    {
        selectLanguageCanvas.SetActive(!selectLanguageCanvas.activeSelf);
    }

    public void ToggleShopMenu()
    {
        if (shopCanvas.activeSelf)
        {
            int playerIndex = PlayerSelectMenu.instance.playerSelectIndex;
            PlayerSelectMenu.instance.SwitchPlayerImage(playerIndex);

            shopCanvas.SetActive(false);
            birdSelectCanvas.SetActive(true);
        }
        else
        {
            birdSelectCanvas.SetActive(false);

            if (Application.internetReachability == NetworkReachability.NotReachable ||
                UnityServicesInitializer.instance.userAuthenticated == false)
            {
                internetUnreachableCanvas.SetActive(true);
            }
            else
            {
                shopCanvas.SetActive(true);
                UpdateShopUi();
            }
        }
    }

    public void ToggleHowToPlayMenu()
    {
        if (howToPlayCanvas.activeSelf)
        {
            howToPlayCanvas.SetActive(false);
            if (howToPlayOpenedFromSettingsMenu)
            {
                settingsCanvas.SetActive(true);
            }
            else birdSelectCanvas.SetActive(true);
        }
        else
        {
            howToPlayCanvas.SetActive(true);
            birdSelectCanvas.SetActive(false);
            settingsCanvas.SetActive(false);          
        }

     
    }

  //public void RemovePickNameButtonsFromHowToPlayMenu()
  //{
  //    Transform howToPlayChildren = howToPlayCanvas.transform.GetChild(0).transform.GetChild(6);
  //    howToPlayChildren.gameObject.SetActive(false);
  //    howToPlayCanvas.transform.GetChild(0).transform.GetChild(5).gameObject.SetActive(true);
  //}

    public void ToggleAreYouSureCanvas()
    {
        areYouSureCanvas.SetActive(!areYouSureCanvas.activeSelf);
    }

    public void ToggleInternetUnreachable()
    {
        if (internetUnreachableCanvas.activeSelf)
        {
            internetUnreachableCanvas.SetActive(false);

            internetUnreachableCanvas.transform.GetChild(0).transform.GetChild(4).gameObject.SetActive(false);
            internetUnreachableCanvas.transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(false);
          
            birdSelectCanvas.SetActive(true);
                    
        }
        else
        {
            internetUnreachableCanvas.SetActive(true);
            birdSelectCanvas.SetActive(false);
        }
    }

    public void ToggleLeaderBoard()
    {
        if (LeaderboardManager.Instance == null) return;

        if (leaderBoardCanvas.activeSelf)
        {
            leaderBoardCanvas.SetActive(false);
            LeaderboardManager.Instance.yourScoreButton.interactable = false;
            StartCoroutine(ActivateBlockButtonsImageBriefly());
        }
        else if (Application.internetReachability == NetworkReachability.NotReachable ||
            UnityServicesInitializer.instance.userAuthenticated == false)
        {
            ToggleInternetUnreachable();
        }
        else
        {
            leaderBoardCanvas.SetActive(true);
            LeaderboardManager.Instance.ShowLeaderboard();           
        }
    }

    public void TogglePickUsernameCanvas()
    {
        if (inputNameCanvas.activeSelf == false)
        {
            flagImage.sprite = flags[PlayerPrefs.GetInt("FlagNumber")];
        }

        inputNameCanvas.SetActive(!inputNameCanvas.activeSelf);
        birdSelectCanvas.SetActive(!inputNameCanvas.activeSelf);
    }

    public void UpdateShopUi()
    {
        //remove ios restore purchases button. android automatically restores them
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            restorePurchasesBtn.SetActive(false);
        }

        InAppPurchasing.instance.UpdateStorePrices();
        Transform shopItems = shopCanvas.transform.GetChild(0).transform.GetChild(1);

        for (int i = 3; i <= 5; i++)
        {
            if (SaveSystem.GetBool($"player{i + 2}Unlocked"))
            {
                shopItems.GetChild(i).GetChild(3).gameObject.SetActive(true);
                shopItems.GetChild(i).GetComponent<Button>().enabled = false;
            }
        }

        if (SaveSystem.GetInt("AdsRemoved") == 1)
        {
            for (int i = 0; i <= 2; i++)
            {
                shopItems.GetChild(i).GetChild(3).gameObject.SetActive(true);
                shopItems.GetChild(i).GetComponent<Button>().enabled = false;
            }
        }
    }

    public void StartCycleThroughLanguageButtonText()
    {
        foreach (GameObject languageTextObject in settingsLanguageTextObjects)
        {
            languageTextObject.SetActive(false);
        }

        settingsLanguageTextObjects[1].SetActive(true);

        StartCoroutine("CycleThroughLanguageButtonText");
    }

    IEnumerator CycleThroughLanguageButtonText()
    {
        int count = 2;

        while (true)
        {
            yield return new WaitForSeconds(1);

            //set language active
            settingsLanguageTextObjects[count].SetActive(true);

            //set the the last language inactive
            if (count != 0)
            {
                settingsLanguageTextObjects[count - 1].SetActive(false);
            }
            else settingsLanguageTextObjects[settingsLanguageTextObjects.Length - 1].SetActive(false);

            //reset the counter if on the last language
            if (count == settingsLanguageTextObjects.Length - 1)
            {

                count = 0;
            }
            else count++;
        }
    }
}

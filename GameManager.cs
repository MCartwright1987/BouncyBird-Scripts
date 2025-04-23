using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using MaskTransitions;
using Unity.Services.Authentication;
using Samples.Purchasing.Core.BuyingConsumables;
using Unity.Services.Core;
//using UnityEngine.UIElements;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool doNotUploadScore;

    public Inputs inputsScript;

    public GameObject redButtons;
    public GameObject startBtn;
    [SerializeField] GameObject pauseBtn;
    [SerializeField] GameObject resumeBtn;
    [SerializeField] GameObject deleteHighScoreBtn;
    [SerializeField] GameObject releaseBirdButton;
    [SerializeField] Button skipAdBtn;
    public GameObject StartScreenPlayAdBtn;

    public int score;
    float sessionTime;
    bool timerActive = false;

    public TextMeshProUGUI skipAdsText;
    public TextMeshProUGUI sessionTimeText;
    [SerializeField] TextMeshProUGUI scoreTxt;
    [SerializeField] TextMeshProUGUI bestScoreTxt;

    [SerializeField] RectTransform canvasRectTransform;

    [SerializeField] Transform mainCamTransform;
    public Transform spawningObjectsAnchor;

    public int screenTransitionsCounter;

    [SerializeField] Transform moveablePanel;
    [SerializeField] Transform TowerNumbersParent;

    [SerializeField] GameObject advanceLevelTrigger;
    [SerializeField] Transform mountainsSectionOfBackGround;
    [SerializeField] Transform mountainsSectionOfBackGround2;
    [SerializeField] Transform WorldSelectCanvas;
    public GameObject crateExplosionParticleSystem;

    [SerializeField] float inGameElapsedTime = 0f;

    public static bool gameStarted;

    [SerializeField] GameObject addLivesAnimation;

    [SerializeField] GameObject emptyLifeImage;

    public GameObject blockBttonsImage; // RED BUTTONS
    [SerializeField] GameObject blockBirdAndLevelSelectImage;

    public float timeTakenToReachLastLevel;

    private Color numberColor = new Color(0, 0.5437484f, 1, 0.34f);

    [SerializeField] GameObject debugPad;

    public GameObject skipAdsTokensTopOfScreen;

    [SerializeField] GameObject newHighScoreText;

    public static int levelNumber = 1;
    private int numberOfLevels = 2;

    public float canvasMultiple;

    [SerializeField] RectTransform birdSelectTransform;
    [SerializeField] RectTransform stageSelectTransform;

    void Start()
    {
        instance = this;    
        
        // first launch set when userName set.
        if (SaveSystem.GetInt("FirstLaunch") != 1)
        {
            SaveSystem.SetInt("SkipAdTokens", 3);
            CanvasManager.instance.ToggleHowToPlayMenu();
            CanvasManager.instance.birdSelectCanvas.SetActive(false);
            SaveSystem.SetInt("FirstLaunch", 1);
        }
        //else CanvasManager.instance.RemovePickNameButtonsFromHowToPlayMenu();
        
        if (SaveSystem.GetInt("AdsRemoved") == 1)
        {
            StartScreenPlayAdBtn.SetActive(false);
            skipAdsTokensTopOfScreen.SetActive(false);
        }
        else //ads not removed
        {
            skipAdsText.text = SaveSystem.GetInt("SkipAdTokens").ToString();
        }
        
        UpdateTimer();
        sessionTime = 0;
        
        Invoke("DeActivateBlockButtonsImage", 0.6f);
        
        if (PlayerPrefs.GetInt("newHighScore") == 1)
        {
            Invoke("PlayHighScoreAnimation", 0.75f);
            PlayerPrefs.SetInt("newHighScore", 0);
        }
        else if (GetScoreFromCombinedValue(SaveSystem.GetInt("BestCombinedTimeAndScore")) >= 3)
        {
            //for players whos score is already above 3 before the review request update.
            ReviewManager.Instance.RequestReview();
        }

        SetScreenSize();

        UpdateLevel();
    }

    public void SwitchLevel()
    {
        if (levelNumber < numberOfLevels) levelNumber++;
        else levelNumber = 1;

        UpdateLevel();
    }

    public void UpdateLevel()
    {
        if (levelNumber == 2)
        {
            moveablePanel.GetChild(2).GetChild(1).GetComponent<YellowPads>().ToggleActivatePads(true);
            moveablePanel.GetChild(2).GetChild(0).GetComponent<YellowPads>().ToggleActivatePads(false);

            BackgroundsManager.instance.SwitchBackground();

            Audio.instance.GetComponent<AudioSource>().clip = Audio.instance.Music2;

            int bestScore = GetScoreFromCombinedValue(SaveSystem.GetInt("BestCombinedTimeAndScore2"));
            bestScoreTxt.text = bestScore.ToString();

            scoreTxt.text = PlayerPrefs.GetInt("LastScore2").ToString();
        }
        else
        {
            moveablePanel.GetChild(2).GetChild(1).GetComponent<YellowPads>().ToggleActivatePads(false);
            moveablePanel.GetChild(2).GetChild(0).GetComponent<YellowPads>().ToggleActivatePads(true);

            BackgroundsManager.instance.SwitchBackground();

            Audio.instance.GetComponent<AudioSource>().clip = Audio.instance.Music1;

            int bestScore = GetScoreFromCombinedValue(SaveSystem.GetInt("BestCombinedTimeAndScore"));
            bestScoreTxt.text = bestScore.ToString();

            scoreTxt.text = PlayerPrefs.GetInt("LastScore").ToString();
        }

        Audio.instance.GetComponent<AudioSource>().Play();

        HotAirBalloon.Instance.SwitchSprites();

        Player.Instance.SwitchMoonGlowColor();
    }

    public void SetNewHighestScoreTrue() => PlayerPrefs.SetInt("newHighScore", 1);

    public void DeActivateBlockButtonsImage()=> blockBttonsImage.SetActive(false);

    private void Update()
    {
        if (timerActive)
        {
            sessionTime += Time.deltaTime;
    
            UpdateTimer();
        }
    }

    public void SetScreenSize() // from 20x9 and wider
    {
        RectTransform titleSpace = CanvasManager.instance.startScreenCanvas.transform.GetChild(0).GetComponent<RectTransform>();

        RectTransform thumbSpace = CanvasManager.instance.inGameCanvas.transform.GetChild(0).GetComponent<RectTransform>();
        RectTransform thumbArrowLeft = thumbSpace.GetChild(2).GetComponent<RectTransform>();
        RectTransform thumbArrowRight = thumbSpace.GetChild(1).GetComponent<RectTransform>();
        RectTransform pauseButton = pauseBtn.GetComponent<RectTransform>();
        RectTransform bestScore = bestScoreTxt.transform.parent.GetComponent<RectTransform>();
        RectTransform score = scoreTxt.transform.parent.GetComponent<RectTransform>();

        Transform UnlockBirdAnimationAnchor = mainCamTransform.GetChild(0).transform;
        Transform unlockBirdText = UnlockBirdAnimation.instance.UnlockBirdText.transform;

        // if screen width larger than default
        if (canvasRectTransform.rect.width > 276) // canvasRectTransform.rect.width < 377)// 340 was max
        {
            float screenWidthDifferenceFromDefault = Mathf.Min(canvasRectTransform.rect.width - 278, 99);

            float newCameraSizeReduction = screenWidthDifferenceFromDefault * 0.035f; // 0.039

            //zoom and move thecamera
            Camera.main.orthographicSize = 12 - newCameraSizeReduction;
            
            //move camera position to suit the ui
            float newCameraYpositionAdition = screenWidthDifferenceFromDefault * 0.019f;
            mainCamTransform.position = new Vector3(0, newCameraYpositionAdition, -10);

            // move the pause button
            pauseButton.anchoredPosition = new Vector2(pauseButton.anchoredPosition.x + (screenWidthDifferenceFromDefault/2), pauseButton.anchoredPosition.y);
            score.localPosition = new Vector2(107.5f + (screenWidthDifferenceFromDefault / 2f), score.localPosition.y);
            bestScore.localPosition = new Vector2(-107.5f - (screenWidthDifferenceFromDefault / 2f), bestScore.localPosition.y);
            //scale and move the thumbspace
            thumbSpace.localPosition = new Vector2(0, screenWidthDifferenceFromDefault * -1);
            thumbArrowLeft.localPosition = new Vector2(-84.5f - screenWidthDifferenceFromDefault, -252.5f + screenWidthDifferenceFromDefault * 0.5f);
            thumbArrowRight.localPosition = new Vector2(84.5f + screenWidthDifferenceFromDefault, -252.5f + screenWidthDifferenceFromDefault * 0.5f);

            if (canvasRectTransform.rect.width > 342)
            {
                thumbArrowLeft.gameObject.SetActive(false);
                thumbArrowRight.gameObject.SetActive(false);
            }
            else
            {
                thumbArrowLeft.gameObject.SetActive(true);
                thumbArrowRight.gameObject.SetActive(true);
            }

            float newArrowScale = 0.5f - (screenWidthDifferenceFromDefault * 0.003f);

            thumbArrowRight.localScale = new Vector2(newArrowScale, newArrowScale);
            thumbArrowLeft.localScale = new Vector2(newArrowScale, newArrowScale);

            UnlockBirdAnimationAnchor.localPosition = new Vector2(0, screenWidthDifferenceFromDefault * -0.022f);
            unlockBirdText.localPosition = new Vector2(0, unlockBirdText.localPosition.y + (screenWidthDifferenceFromDefault * 0.2f));

            newHighScoreText.transform.localPosition = new Vector2(0, newHighScoreText.transform.localPosition.y + (screenWidthDifferenceFromDefault * 0.2f));
            //move mountains background
            mountainsSectionOfBackGround.localPosition = new Vector2(0, 2.8f - (screenWidthDifferenceFromDefault * 0.011f));
            mountainsSectionOfBackGround2.localPosition = new Vector2(3.13f, -6.27f - (screenWidthDifferenceFromDefault * 0.011f));

            WorldSelectCanvas.localPosition = new Vector2(0, -106.7f - (screenWidthDifferenceFromDefault * 0.25f));
            //move floor object
            PickUpsManager.instance.floorObject.transform.localPosition = new Vector3(0, -7.95f - (screenWidthDifferenceFromDefault * 0.005f), 11);

            titleSpace.transform.localPosition = new Vector3(0, 0 - (screenWidthDifferenceFromDefault * 0.5f), 0);
            titleSpace.transform.GetChild(1).transform.localPosition = new Vector3(0, -252.5f - (screenWidthDifferenceFromDefault * -0.25f), 0);

            redButtons.transform.localPosition = new Vector3(0, 0 - (screenWidthDifferenceFromDefault * 0.2f), 0);

            redButtons.transform.localScale = new Vector2(1 + (screenWidthDifferenceFromDefault * 0.0016f), 1 + (screenWidthDifferenceFromDefault * 0.0016f));
            releaseBirdButton.transform.localScale = new Vector2(1 + (screenWidthDifferenceFromDefault * 0.0016f), 1 + (screenWidthDifferenceFromDefault * 0.0016f));

            birdSelectTransform.anchoredPosition = new Vector2(0, 0 - (screenWidthDifferenceFromDefault * 0.2f));
            stageSelectTransform.anchoredPosition = new Vector2(0, -106.7f - (screenWidthDifferenceFromDefault * 0.5f));

            birdSelectTransform.localScale = new Vector2(1 + (screenWidthDifferenceFromDefault * 0.001f), 1 + (screenWidthDifferenceFromDefault * 0.001f));
            stageSelectTransform.localScale = new Vector2(1 + (screenWidthDifferenceFromDefault * 0.001f), 1 + (screenWidthDifferenceFromDefault * 0.001f));

            foreach (var rectTransform in CanvasManager.instance.StartScreenCanvases)
            {
                rectTransform.localScale = new Vector2(2.57f + (screenWidthDifferenceFromDefault * canvasMultiple), 2.57f + (screenWidthDifferenceFromDefault * canvasMultiple));
            }

        }
    }

    public void PlayHighScoreAnimation()
    {
        StartCoroutine(HighScoreTextAnimation());

        int bestScore = SaveSystem.GetInt("BestCombinedTimeAndScore");
        if (levelNumber == 2) bestScore = SaveSystem.GetInt("BestCombinedTimeAndScore2");

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            bool consentFormActivated = false;

            if ((levelNumber == 1 && bestScore > LeaderboardManager.LastScoreLeaderBoard1) || (levelNumber == 2 && bestScore > LeaderboardManager.LastScoreLeaderBoard2))
            {
                if (PlayerPrefs.GetInt("AcceptLeaderBoardUpload") == 1)
                {
                    if (levelNumber == 1) LeaderboardManager.Instance.SubmitScoreLeaderBoard1(SaveSystem.GetInt("BestCombinedTimeAndScore"));
                    else LeaderboardManager.Instance.SubmitScoreLeaderBoard2(SaveSystem.GetInt("BestCombinedTimeAndScore2"));
                }
                else
                {
                    CanvasManager.instance.ToggleConsentCanvas(true);
                    consentFormActivated = true;
                }
            }

            else if (bestScore >= 3 && consentFormActivated == false)
            {
                ReviewManager.Instance.RequestReview();
            }
        }     
    }

    public IEnumerator HighScoreTextAnimation()
    {
        newHighScoreText.SetActive(true);

        Audio.instance.newHighScore.Play();

        yield return new WaitForSeconds(1.75f);

        newHighScoreText.SetActive(false);
    }

    private void UpdateTimer()
    {
        float minutes = Mathf.FloorToInt(sessionTime / 60); // Convert seconds to minutes
        float seconds = Mathf.FloorToInt(sessionTime % 60); // Get the remaining seconds (% divides it and returns the remainder)
        sessionTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void MoveCameraSmoothly()=> StartCoroutine(MoveCameraSmoothlyCoroutine());

    IEnumerator MoveCameraSmoothlyCoroutine()
    {
        score++;

        Time.timeScale = 0.1f;

        StartCoroutine(AdvanceLevelAnimation());

        MoveAndUpdatePads();

        MoveAndUpdateLevelNumbers();

        CheckChallenges();

        Vector3 initialMovablePanelPositionion = mainCamTransform.position;
        Vector3 targetPosition = new Vector3(0, mainCamTransform.position.y + 5.8f, -10);

        float elapsedTime = 0.0f;

       while (elapsedTime < 0.15f) // was 0.125
       {
           elapsedTime += Time.deltaTime;
       
           float t = Mathf.Clamp01(elapsedTime / 0.15f);
       
           mainCamTransform.position = Vector3.Lerp(initialMovablePanelPositionion, targetPosition, t);
       
           yield return null;
       }

        spawningObjectsAnchor.position = new Vector2(0, spawningObjectsAnchor.position.y + 5.8f);

        mainCamTransform.position = targetPosition;

        if (PickUpsManager.instance.TimeSlowed) Time.timeScale = 0.7f;
        else Time.timeScale = 1;

        PickUpsManager.instance.SpawnPickUp();

        timeTakenToReachLastLevel = sessionTime;
    }

    void MoveAndUpdatePads()
    {
        //Toggle the new level pads Active/usable
        moveablePanel.GetChild(1).GetChild(levelNumber-1).GetComponent<YellowPads>().ToggleActivatePads(true);

        // activate pads above the new level (transparent)
        moveablePanel.GetChild(0).gameObject.SetActive(true);

        //Move the bottom pad above the top pads and set inactive
        Transform lastChild = moveablePanel.GetChild(moveablePanel.childCount - 1);                 
        lastChild.transform.localPosition = new Vector2(0, lastChild.transform.position.y + 17.4f);
        lastChild.GetChild(levelNumber - 1).GetComponent<YellowPads>().ToggleActivatePads(false);
        lastChild.gameObject.SetActive(false);
        
        //Reset the order in the higherarchy                                
        lastChild.SetAsFirstSibling();
    }

    void MoveAndUpdateLevelNumbers()
    {
        // use these 2 lines to just show the just number that your on instead of 2
        TowerNumbersParent.GetChild(1).gameObject.SetActive(false);
        TowerNumbersParent.GetChild(0).gameObject.SetActive(true);

        //Move the LevelNumber above the top Level Number
        Transform lastChild = TowerNumbersParent.GetChild(TowerNumbersParent.childCount - 1);
        lastChild.transform.localPosition = new Vector3(0, lastChild.transform.position.y + 4.2f, 1); // -1.5 for 1 space

        //Reset the order in the higherarchy  
        lastChild.SetAsFirstSibling(); //REMOVE THIS SO JUST ONE NUMBER MOVES UP

        //Update Level Numbers
        lastChild.GetComponent<TMP_Text>().text = (score + 1).ToString();

        StartCoroutine(FlashLevelNumber());
    }

    IEnumerator FlashLevelNumber()
    {
        Transform number = TowerNumbersParent.GetChild(1);
        numberColor.a = 1;
        number.GetComponent<TMP_Text>().color = numberColor;

        yield return new WaitForSeconds(0.8f);

        numberColor.a = 0.34f;
        number.GetComponent<TMP_Text>().color = numberColor;
    }

    IEnumerator AdvanceLevelAnimation()
    {
        BackgroundsManager.instance.AddBackGrounds(score);

        var particleSystem = advanceLevelTrigger.transform.GetChild(1).GetComponent<ParticleSystem>();
        var emission = particleSystem.emission; // Get the EmissionModule
        var main = particleSystem.main;

        //state to default
        emission.rateOverTime = 19f;
        advanceLevelTrigger.transform.GetChild(1).gameObject.SetActive(false);

        //move the level trigger and activate disperse particle system
        advanceLevelTrigger.transform.position = new Vector3(0, advanceLevelTrigger.transform.position.y + 5.8f, -1);
        advanceLevelTrigger.transform.GetChild(1).gameObject.SetActive(true);

        advanceLevelTrigger.transform.GetChild(0).gameObject.SetActive(true);

        //makes only 1 burst of particles
        emission.rateOverTime = 0f; // Set the rateOverTime to 0

        main.simulationSpeed = 6.6f;

        while (Time.timeScale < 0.6f)
        {
            yield return new WaitForEndOfFrame();
        }

        main.simulationSpeed = 1;

        BackgroundsManager.instance.DestroyBackGrounds(score);
    }

    void CheckChallenges()
    {
        if (score == 1) StartCoroutine(ActivateContinue());

        //unlock New birds
        else if (score == 3) UnlockPlayer("player1Unlocked", 1, false);
        else if (score == 5 && Player.Instance.powerUpsHit < 1) UnlockPlayer("player2Unlocked", 2, false);
        else if (score == 7 && HotAirBalloon.Instance.levelReached < 7) UnlockPlayer("player3Unlocked", 3, false);
        else if (score == 12) UnlockPlayer("player4Unlocked", 4, false);

        // activate moonglow to see bird better on space backgroud
        else if (score == 15) Player.Instance.moonglow.gameObject.SetActive(true);

        if (levelNumber == 2)
        {
            if (score == 8 && HotAirBalloon.Instance.levelReached < 8) UnlockPlayer("player8Unlocked", 8, true);
            if (score == 13) UnlockPlayer("player9Unlocked", 9 ,false);
        }
    }

    public void UnlockPlayer(string saveKey, int animationIndex ,bool fliped)
    {
        if (SaveSystem.GetBool(saveKey) == false)
        {
            SaveSystem.SetBool(saveKey, true);
            UnlockBirdAnimation.instance.PlayAnimation(animationIndex, fliped);
        }
    }

    public void StartGame()
    {
        ToggleDisableBirdAndLevelSelect();

        Player.Instance.rigidBody.simulated = true; //release th bird

        PinksManager.instance.SpawnPink();
        PickUpsManager.instance.SpawnPickUp();
        HotAirBalloon.Instance.speed = 0.5f;        
        timerActive = true;
        CanvasManager.instance.startScreenCanvas.SetActive(false);
        CanvasManager.instance.ActivateStartScreenAnimations();

        StartCoroutine(LerpNumberTransparency());

        CanvasManager.instance.inGameCanvas.SetActive(true);
        PlayerSelectMenu.instance.UpdateSpritesAndCollidersOnPlay();
        GetComponent<PlayerInput>().enabled = true;

        TowerNumbersParent.gameObject.SetActive(true);
        advanceLevelTrigger.SetActive(true);

        Invoke("DeactivateBirdAndLevelSelectCanvases", 0.5f);
        Invoke("ToggleDisableBirdAndLevelSelect", 0.5f);
    }

    public void ToggleDisableBirdAndLevelSelect()
    {
        blockBirdAndLevelSelectImage.SetActive(!blockBirdAndLevelSelectImage.activeSelf);
    }


    public void DeactivateBirdAndLevelSelectCanvases()
    {
        CanvasManager.instance.birdSelectCanvas.SetActive(false);
        CanvasManager.instance.worldSelectCanvas.SetActive(false);
    }

    IEnumerator LerpNumberTransparency()
    {
        TextMeshProUGUI number0 = CanvasManager.instance.levelNumbersCanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        
        float TransparencyValue = 0;
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            TransparencyValue = Mathf.Lerp(0, 0.4196078f, elapsed / 0.5f);
            number0.color = new Color(0, 0.5437484f, 1, TransparencyValue);
            yield return null;
        }
        number0.color = new Color(0, 0.5437484f, 1, 0.4196078f);
    }

    public void TogglePauseMovingObjects()
    {
        TogglePauseHotAirBalloon();
        PickUpsManager.instance.TogglePausePickUps();
        PinksManager.instance.TogglePausePinks();
    }

    void TogglePauseHotAirBalloon()
    {
        if (HotAirBalloon.Instance.speed == 0.5f) HotAirBalloon.Instance.speed = 0;
        else HotAirBalloon.Instance.speed = 0.5f;
    }

    public void ReleaseBirdFromContinue()
    {
        Player.Instance.rigidBody.simulated = true; //release th bird
        releaseBirdButton.gameObject.SetActive(false);
        if (PickUpsManager.instance.TimeSlowed) Time.timeScale = 0.7f;
        GetComponent<PlayerInput>().enabled = true;
        TogglePauseMovingObjects();
        timerActive = true;
        pauseBtn.SetActive(true);
    }
    public void DeactivateReleaseBirdFromContinueButton()
    {
        releaseBirdButton.gameObject.SetActive(false);
    }

    public void QuitGame() => Application.Quit();

    public void PauseAndResume()
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
            pauseBtn.SetActive(false);
            resumeBtn.SetActive(true);

            Audio.instance.GetComponent<AudioSource>().Pause();
            GetComponent<PlayerInput>().enabled = false;
        }
        else
        {
            if (PickUpsManager.instance.TimeSlowed == false) Time.timeScale = 1;
            else Time.timeScale = 0.7f;

            pauseBtn.SetActive(true);
            resumeBtn.SetActive(false);

            Audio.instance.GetComponent<AudioSource>().Play();
            GetComponent<PlayerInput>().enabled = true;
        }
    }

    public void ScorePlus1()
    {
        score++;
        scoreTxt.text = score.ToString();
    }

    public void Death()
    {
        Audio.instance.fail.Play();
        Player.Instance.GetComponent<CircleCollider2D>().enabled = false;

        TransitionManager.Instance.PlayTransition(2f);
        Time.timeScale = 1;

        if (addLivesAnimation.activeSelf == true)
        {
            Invoke("SetTheSceneForContinue", 0.5f);
            
            return;
        }
                    
        Audio.instance.GetComponent<AudioSource>().Pause();

        Invoke("LoadScene", 0.55f);

        // combine the time and score for unity leaderBoard
        //maximum score = 9999 and max time = 72.8 hours
        int combinedScore = CombineScoreAndTime(score, (int)timeTakenToReachLastLevel, PlayerPrefs.GetInt("FlagNumber"));

        if (score >= 1)
        {
            if (levelNumber == 1)
            {
                if (combinedScore > SaveSystem.GetInt("BestCombinedTimeAndScore") || !SaveSystem.HasKey("BestCombinedTimeAndScore"))
                {
                    PlayerPrefs.SetInt("newHighScore", 1);

                    // Update best score
                    SaveSystem.SetInt("BestCombinedTimeAndScore", combinedScore);
                }

                PlayerPrefs.SetFloat("lastSessionTime", sessionTime);
                PlayerPrefs.SetInt("LastScore", score);
            }
            else if (levelNumber == 2)
            {
                if (combinedScore > SaveSystem.GetInt("BestCombinedTimeAndScore2") || !SaveSystem.HasKey("BestCombinedTimeAndScore2"))
                {
                    PlayerPrefs.SetInt("newHighScore", 1);

                    // Update best score
                    SaveSystem.SetInt("BestCombinedTimeAndScore2", combinedScore);
                }


                PlayerPrefs.SetFloat("lastSessionTime2", sessionTime);
                PlayerPrefs.SetInt("LastScore2", score);
            }
        } 
    }

    public int CombineScoreAndTime(int score, float timeInSeconds, int flag)
    {
        //This is to combine all the data i need for the leaderboard into one int

        // Scale time to an integer
        int scaledTime = Mathf.RoundToInt(timeInSeconds);

        int maxScore = 255;  // 8 bits (0-255)
        score = Mathf.Clamp(score, 0, maxScore);

        int maxTime = 4095;  // 12 bits (0-4095, ~1 hour)
        scaledTime = Mathf.Clamp(scaledTime, 0, maxTime);

        int maxFlag = 31;  // 5 bits (0-31)
        flag = Mathf.Clamp(flag, 0, maxFlag);

        // Invert time so that smaller times result in higher values
        int invertedTime = maxTime - scaledTime;

        // Combine into a 32-bit integer: [8-bit score | 12-bit inverted time | 5-bit flag]
        int combinedValue = (score << 17) | (invertedTime << 5) | flag;

        return combinedValue;
    }


    public int GetScoreFromCombinedValue(int combinedValue)
    {
        return (combinedValue >> 17) & 0xFF; // Extracts the top 8 bits
    }

    public void SetTheSceneForContinue()
    {
        Player.Instance.GetComponent<CircleCollider2D>().enabled = true;
        if (SaveSystem.GetInt("AdsRemoved") != 1)
        {
            CanvasManager.instance.ToggleContinueCanvas();
        }           
        addLivesAnimation.SetActive(false);
        emptyLifeImage.SetActive(true);
        Player.Instance.Respawn();
        releaseBirdButton.SetActive(true);
        TogglePauseMovingObjects();
        GetComponent<PlayerInput>().enabled = false;
        Inputs.instance.screenGrabed = false;
        moveablePanel.transform.position = Vector2.zero;
        timerActive = false;
        pauseBtn.SetActive(false);
    }

    public void UseSkipAdToken()
    {
        SaveSystem.SetInt("SkipAdTokens", SaveSystem.GetInt("SkipAdTokens") - 1);
        skipAdsText.text = SaveSystem.GetInt("SkipAdTokens").ToString();
        CanvasManager.instance.AddToContinueStreakAndCloseContinueCanvas();
    }
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator ActivateContinue()
    {
        Audio.instance.addLives.Play();

        addLivesAnimation.SetActive(true);

        yield return new WaitForSeconds(1f);
        emptyLifeImage.SetActive(false);

        yield return new WaitForSeconds(2f);
        addLivesAnimation.GetComponent<Animator>().enabled = false;
    }

    public async void TryConnectInternet()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                UnityServicesInitializer.instance.userAuthenticated = true;
            }

            catch (System.Exception e)
            {
                Debug.LogWarning("Authentication failed: " + e.Message);
            }
        }
        else
        {
            Debug.Log("User already authenticated!");
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            CanvasManager.instance.internetUnreachableCanvas.transform.GetChild(0).transform.GetChild(4).gameObject.SetActive(false);
            CanvasManager.instance.internetUnreachableCanvas.transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(true);
        }
        else //connected
        {      
            CanvasManager.instance.internetUnreachableCanvas.transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(false);
            CanvasManager.instance.internetUnreachableCanvas.transform.GetChild(0).transform.GetChild(4).gameObject.SetActive(true);

            AdsInitializer.instance.InitializeAds();
            InAppPurchasing.instance.InitializePurchasing();

            DisplayNameManager.Instance.SetUnityIdIfconnectionLostWhenSavingIt();

            RewardedAdsButton.instance.LoadAd();

            await LeaderboardManager.Instance.SaveLastScoreInLeaderBoard("HighScore");
            await LeaderboardManager.Instance.SaveLastScoreInLeaderBoard("High_Score_Stage_2");
        }
    }

    public void AddSkipAdTokens(int number)
    {
        SaveSystem.SetInt("SkipAdTokens", SaveSystem.GetInt("SkipAdTokens") + number);
        skipAdsText.text = SaveSystem.GetInt("SkipAdTokens").ToString();
        Audio.instance.addSkipAdTokens.Play();
    }
    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://www.blogger.com/blog/post/edit/862660359855154186/8565106823714256058");
    }
}

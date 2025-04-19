using Samples.Purchasing.Core.BuyingConsumables;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
//using UnityEngine.Localization.Settings;
using System.Threading.Tasks;

public class UnityServicesInitializer : MonoBehaviour
{
    public static UnityServicesInitializer instance;
    public bool userAuthenticated = false;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Show black screen to block player input while initializing
        transform.GetChild(0).gameObject.SetActive(true);
        Audio.instance.GetComponent<AudioSource>().Stop();

        SetLanguageIfChanged();
        PreWarmAnimations();

        Invoke("TurnBlackScreenOffAndStartGame", 1.75f);
        InitializeUnityOnlineServices();
    }

    public async void InitializeUnityOnlineServices()
    {
        await UnityServices.InitializeAsync();

        // Authenticate user anonymously if not already signed in
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                userAuthenticated = true;
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

        if (userAuthenticated)
        {
            await SaveOfflineLeaderboardScores();
            HandleLeaderboardUploadConsent();
        }

        DisplayNameManager.Instance.SetUnityIdIfconnectionLostWhenSavingIt();
        HandleOffensiveUsernameCheck();
        ResetLegacyHighScoresIfNeeded();
    }

    private async Task SaveOfflineLeaderboardScores()
    {
        await LeaderboardManager.Instance.SaveLastScoreInLeaderBoard("HighScore");
        await LeaderboardManager.Instance.SaveLastScoreInLeaderBoard("High_Score_Stage_2");
    }

    private void HandleLeaderboardUploadConsent()
    {
        bool hasConsent = PlayerPrefs.GetInt("AcceptLeaderBoardUpload") == 1;

        if (SaveSystem.HasKey("BestCombinedTimeAndScore"))
        {
            int bestScore = SaveSystem.GetInt("BestCombinedTimeAndScore");
            if (bestScore > LeaderboardManager.LastScoreLeaderBoard1)
            {
                if (hasConsent)
                    LeaderboardManager.Instance.SubmitScoreLeaderBoard1(bestScore);
                else
                    CanvasManager.instance.ToggleConsentCanvas(true);
            }
        }

        if (SaveSystem.HasKey("BestCombinedTimeAndScore2"))
        {
            int bestScore = SaveSystem.GetInt("BestCombinedTimeAndScore2");
            if (bestScore > LeaderboardManager.LastScoreLeaderBoard2)
            {
                if (hasConsent)
                    LeaderboardManager.Instance.SubmitScoreLeaderBoard2(bestScore);
                else
                    CanvasManager.instance.ToggleConsentCanvas(true);
            }
        }
    }

    private void HandleOffensiveUsernameCheck()
    {
        string playerName = PlayerPrefs.GetString("playerName");

        if (DisplayNameManager.Instance.IsUsernameOffensive(playerName) &&
            !CanvasManager.instance.inputNameCanvas.activeSelf)
        {
            CanvasManager.instance.TogglePickUsernameCanvas();
        }
    }

    private void ResetLegacyHighScoresIfNeeded()
    {
        string playerName = PlayerPrefs.GetString("playerName");

        if (PlayerPrefs.GetInt("ResetNonUpdatedScores") != 1)
        {
            if (playerName.StartsWith("Joker9018") || playerName.StartsWith("Dragonracer333"))
            {
                GameManager.instance.ReduceScoreToOne();
            }
        }

        if (PlayerPrefs.GetInt("ResetNonUpdatedScores2") != 1)
        {
            if (playerName.StartsWith("Dragonracer333"))
            {
                GameManager.instance.ReduceScoreToOne();
            }
        }
    }

    public void SetContinueStreakPrize()
    {
        CanvasManager.instance.continueStreakPrize[SaveSystem.GetInt("ContinueStreakPrize")].SetActive(true);
    }

    public void SetLanguageIfChanged()
    {
        if (PlayerPrefs.HasKey("Language") && LanguageManager.instance != null)
        {
            LanguageManager.instance.SetLanguage(PlayerPrefs.GetString("Language"));
        }
    }

    public void PreWarmAnimations()
    {
        Player.Instance.mallet.SetActive(true);
        Player.Instance.mallet.SetActive(false);

        Player.Instance.jetPack.SetActive(true);
        Player.Instance.jetPack.SetActive(false);

        GameManager.instance.crateExplosionParticleSystem.GetComponent<ParticleSystem>().Play();
    }

    public void TurnBlackScreenOffAndStartGame()
    {
        if (!Audio.instance.GetComponent<AudioSource>().isPlaying)
        {
            Audio.instance.GetComponent<AudioSource>().Play();
        }

        SetContinueStreakPrize();

        if (userAuthenticated)
        {
            AdsInitializer.instance.InitializeAds();
            InAppPurchasing.instance.InitializePurchasing();

            PlayerPrefs.SetInt("ResetNonUpdatedScores", 1);
            PlayerPrefs.SetInt("ResetNonUpdatedScores2", 1);
        }

        // Hide black screen
        transform.GetChild(0).gameObject.SetActive(false);
    }
}

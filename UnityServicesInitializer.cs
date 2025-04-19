using Samples.Purchasing.Core.BuyingConsumables;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;

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
    void Start()
    {
        //turn black screen on to stop player pressing buttons while online services initialize
        transform.GetChild(0).gameObject.SetActive(true);
        Audio.instance.GetComponent<AudioSource>().Stop();
        SetLangugeIfChanged();
        PreWarmAnimations();

        Invoke("TurnBlackScreenOffAndStartGame", 1.75f);
        InitializeUnityOnlineServices();    
    }

    public async void InitializeUnityOnlineServices()
    {
        await UnityServices.InitializeAsync();

        // Check if the player is already signed in
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
        

        // Online Only Functions
        if (userAuthenticated)
        {
            // get randomly assigned unity name or created name for sign in option
            DisplayNameManager.Instance.GetPlayerName();
         
            //submit top score to leaderBoard incase player was offline when last trying to submit it

            await LeaderboardManager.Instance.Save100thScoreInLeaderBoard("HighScore");
            await LeaderboardManager.Instance.Save100thScoreInLeaderBoard("High_Score_Stage_2");

            //shorthand way of setting a bool
            bool hasAcceptedLeaderboardUpload = PlayerPrefs.GetInt("AcceptLeaderBoardUpload") == 1;

            if (SaveSystem.HasKey("BestCombinedTimeAndScore"))
            {
                int bestScore = (SaveSystem.GetInt("BestCombinedTimeAndScore"));

                if (bestScore > LeaderboardManager.LastScoreLeaderBoard1)
                {
                    if (hasAcceptedLeaderboardUpload)
                    {
                        LeaderboardManager.Instance.SubmitScoreLeaderBoard1(SaveSystem.GetInt("BestCombinedTimeAndScore"));
                    }
                    else
                    {
                        CanvasManager.instance.ToggleConsentCanvas(true);
                    }
                }
            }

            if (SaveSystem.HasKey("BestCombinedTimeAndScore2"))
            {
                int bestScore = (SaveSystem.GetInt("BestCombinedTimeAndScore2"));

                if (bestScore > LeaderboardManager.LastScoreLeaderBoard2)
                {
                    if (hasAcceptedLeaderboardUpload)
                    {
                        LeaderboardManager.Instance.SubmitScoreLeaderBoard2(SaveSystem.GetInt("BestCombinedTimeAndScore2"));
                    }
                    else
                    {
                        CanvasManager.instance.ToggleConsentCanvas(true);
                    }
                }
            }
        }

        DisplayNameManager.Instance.SetUnityIdIfconnectionLostWhenSavingIt();

        string playerName = PlayerPrefs.GetString("playerName");

        if (DisplayNameManager.Instance.IsUsernameOffensive(playerName) == true)
        {
            if (CanvasManager.instance.inputNameCanvas.activeSelf == false) CanvasManager.instance.TogglePickUsernameCanvas();
        }

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
             
    public void SetLangugeIfChanged()
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
        if (Audio.instance.GetComponent<AudioSource>().isPlaying == false)
        {
            Audio.instance.GetComponent<AudioSource>().Play();
        }

        SetContinueStreakPrize();

        if (userAuthenticated)
        {
            AdsInitializer.instance.InitializeAds();

            //ensures in app purchased birds animate after game startsec
            InAppPurchasings.instance.InitializePurchasing();

            PlayerPrefs.SetInt("ResetNonUpdatedScores", 1);
            PlayerPrefs.SetInt("ResetNonUpdatedScores2", 1);
        }

        // turn black screen off
        transform.GetChild(0).gameObject.SetActive(false);    
    }
}


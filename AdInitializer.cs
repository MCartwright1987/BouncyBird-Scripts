using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Advertisements;


using static AdsInitializer;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    public static AdsInitializer instance;
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;

    public bool adsInitialized = false;

    void Awake()
    {
       if (instance != null && instance != this)
       {
           Destroy(this.gameObject);
           return;
       }
              
        instance = this;
        DontDestroyOnLoad(this.gameObject); // Makes the AdsManager persistent
    }

    public void InitializeAds()
    {
#if UNITY_IOS
            _gameId = _iOSGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    private void SetNonPersonalizedAds()
    {
        if (Advertisement.isInitialized)
        {
            MetaData privacyMetaData = new MetaData("gdpr");
            privacyMetaData.Set("consent", "false"); // Disable personalized ads
            Advertisement.SetMetaData(privacyMetaData);
            //Debug.Log("Non-personalized ads enabled.");
        }
    }

    public void OnInitializationComplete()
    {
       //Debug.Log("Unity Ads initialization complete.");
        GetComponent<RewardedAdsButton>().LoadAd();
        adsInitialized = true;
        SetNonPersonalizedAds(); //ios makes a fuss otherwise.
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}

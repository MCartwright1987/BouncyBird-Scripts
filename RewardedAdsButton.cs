using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using System.Collections;

public class RewardedAdsButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static RewardedAdsButton instance;

    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms

    private bool adLoaded = false;

    void Awake()
    {
        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif
    }

    void Start() 
    {
        instance = this;

        //if (adLoaded) _showAdButton.interactable = true;

    }

    // Call this public method when you want to get an ad ready to show.
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        //Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        //Debug.Log("Ad Loaded: " + adUnitId);

        adLoaded = true;

        if (adUnitId.Equals(_adUnitId))
        {
            // Enable the button for users to click:
            //GameManager.instance.StartScreenPlayAdBtn.GetComponent<Button>().interactable = true;
        }
    }

    // Implement a method to execute when the user clicks the button:
    public void ShowAd()
    {
        adLoaded = false;

        if (Application.internetReachability == NetworkReachability.NotReachable ||
            UnityServicesInitializer.instance.userAuthenticated == false || 
            AdsInitializer.instance.adsInitialized == false)
        {
            CanvasManager.instance.ToggleInternetUnreachable();
        }
        else
        {
            Advertisement.Show(_adUnitId, this);
            //_showAdButtonStart.interactable = false;          
        }       
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            //Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            if (CanvasManager.instance.continueCanvas.activeSelf)
            {
                CanvasManager.instance.AddToContinueStreakAndCloseContinueCanvas();
            }
            else if (CanvasManager.instance.settingsCanvas.activeSelf)
            {
                CanvasManager.instance.settingsCanvas.SetActive(false);
                CanvasManager.instance.TogglePickUsernameCanvas();
            }
            else
            {
                GameManager.instance.AddSkipAdTokens(1);
            }
        }
        LoadAd();
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            StartCoroutine(RetryLoadAd(adUnitId));
        }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            StartCoroutine(RetryLoadAd(adUnitId));
        }        
    }

    private IEnumerator RetryLoadAd(string adUnitId)
    {
        yield return new WaitForSeconds(5); // Wait for 5 seconds before retrying
        if (adLoaded == false) Advertisement.Load(adUnitId, this);

        Debug.Log("Retry load ad");
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

}

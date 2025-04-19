using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//using Google.Play.Review;


#if UNITY_ANDROID
//using Google.Play.Review;
#endif

#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class ReviewManager : MonoBehaviour
{
    public static ReviewManager Instance { get; private set; }
    //private static PlayReviewInfo _playReviewInfo;
#if UNITY_ANDROID
    //private Google.Play.Review.ReviewManager _googlePlayReviewManager;
#endif
    private void Awake()
    {
        // Ensure there's only one instance of ReviewManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this; // Set the singleton instance
            DontDestroyOnLoad(gameObject); // Optionally persist across scenes
        }
    }

    void Start()
    {
#if UNITY_ANDROID
        //_googlePlayReviewManager = new Google.Play.Review.ReviewManager();
#endif
    }

    // Call this method when you want to prompt the user for a review
    public void RequestReview()
    {
        // Check if the player has already been asked for a review (using PlayerPrefs to remember)
        if (PlayerPrefs.GetInt("ReviewAsked") == 1)
        {
            //Debug.Log("Player has already been asked for a review. Skipping.");
            return; // Skip the review request if already asked
        }

#if UNITY_ANDROID
        // Android: Start the review request flow
        //StartCoroutine(RequestFlowAndroidCoroutine());
#elif UNITY_IOS
        // iOS: Show in-app review dialog
        RequestFlowiOS();
#endif
    }

#if UNITY_ANDROID
   //private IEnumerator RequestFlowAndroidCoroutine()
   //{
   //    Debug.Log("Initializing in-app review request flow for Android...");
   //
   //    // Request the review flow info
   //    var requestFlowOperation = _googlePlayReviewManager.RequestReviewFlow();
   //    yield return requestFlowOperation;
   //
   //    if (requestFlowOperation.Error != ReviewErrorCode.NoError)
   //    {
   //        Debug.LogError("Failed to request review flow: " + requestFlowOperation.Error.ToString());
   //        yield break;
   //    }
   //
   //    _playReviewInfo = requestFlowOperation.GetResult();
   //    Debug.Log("Received review info. Launching review flow for Android...");
   //
   //    // Launch the review flow
   //    var launchFlowOperation = _googlePlayReviewManager.LaunchReviewFlow(_playReviewInfo);
   //    yield return launchFlowOperation;
   //
   //    if (launchFlowOperation.Error != ReviewErrorCode.NoError)
   //    {
   //        Debug.LogError("Failed to launch review flow: " + launchFlowOperation.Error.ToString());
   //        yield break;
   //    }
   //
   //    // After showing the review, mark the player as having been asked
   //    PlayerPrefs.SetInt("ReviewAsked", 1);
   //    PlayerPrefs.Save(); // Save the PlayerPrefs to disk
   //
   //    Debug.Log("Review flow launched successfully for Android!");
   //}
#endif

#if UNITY_IOS
    private void RequestFlowiOS()
    {
        Debug.Log("Requesting in-app review for iOS...");

        // Call the iOS-specific API to request an in-app review
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // iOS doesn't have an explicit request method; it uses SKStoreReviewController
            if (Device.RequestStoreReview())
            {
                Debug.Log("Review request initiated successfully on iOS!");

                // After showing the review, mark the player as having been asked
                PlayerPrefs.SetInt("ReviewAsked", 1);
                PlayerPrefs.Save(); // Save the PlayerPrefs to disk
            }
            else
            {
                Debug.LogWarning("Failed to request review on iOS.");
            }
        }


    }
#endif
}

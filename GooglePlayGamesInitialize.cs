//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting.FullSerializer;
//using UnityEngine;
//
//public class GooglePlayGamesInitialize : MonoBehaviour
//{
//    public static GooglePlayGamesInitialize _instance;
//
//
//    void Awake()
//    {
//        if (_instance != null && _instance != this)
//        {
//            Destroy(this.gameObject);
//            return;
//        }
//
//        _instance = this;
//        DontDestroyOnLoad(this.gameObject);
//    }
//
//    void Start()
//    {
//        PlayGamesPlatform.DebugLogEnabled = true;
//
//        try
//        {
//            PlayGamesPlatform.Activate();
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError("Error activating Play Games Platform: " + ex.Message);
//        }
//
//        SignIn();
//    }
//
//    public void SignIn()
//    {
//        //PlayGamesPlatform.Instance.ManuallyAuthenticate((status) =>
//        PlayGamesPlatform.Instance.Authenticate((status) =>
//        {
//            switch (status)
//            {
//                case SignInStatus.Success:
//                    Debug.Log("Signed in successfully.");
//                    break;
//                case SignInStatus.Canceled:
//                    Debug.LogWarning("Sign in was canceled.");
//                    break;
//                case SignInStatus.InternalError:
//                    Debug.LogError("An internal error occurred during sign-in.");
//                    break;
//                default:
//                    Debug.LogError($"Failed to sign in. Error message: {status}");
//                    break;
//            }
//        });
//    }
//
//    public void SignInManually()
//    {
//        PlayGamesPlatform.Instance.ManuallyAuthenticate((status) =>       
//        {
//            switch (status)
//            {
//                case SignInStatus.Success:
//                    Debug.Log("Manually Signed in successfully.");
//                    break;
//                case SignInStatus.Canceled:
//                    Debug.LogWarning("Manually Sign in was canceled.");
//                    break;
//                case SignInStatus.InternalError:
//                    Debug.LogError("An internal error occurred during Manually sign-in.");
//                    break;
//                default:
//                    Debug.LogError($"Failed to Manually sign in. Error message: {status}");
//                    break;
//            }
//        });
//    }
//
//}

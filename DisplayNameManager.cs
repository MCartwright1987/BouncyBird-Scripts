using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.CloudSave;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;
using System;
using static System.Net.Mime.MediaTypeNames;

public class DisplayNameManager : MonoBehaviour
{
    public static DisplayNameManager Instance;

    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button saveNameButton;

    private List<string> bannedWords = new List<string>();
    [SerializeField] List<string> existingUsernames = new List<string>();
    [SerializeField] GameObject warnOffensiveUsernameText;
    [SerializeField] GameObject warnUsernameAlreadyUsedText;
    [SerializeField] GameObject warnUsernameLessThan10LongText;

    [SerializeField] GameObject RandomUserNameTextObject;
    [SerializeField] TextMeshProUGUI userNameBtnText;

    private void Start()
    {
        Instance = this;

        LoadBannedWords();
        //saveNameButton.onClick.AddListener(SaveDisplayName);

    }

    //public async void GetPlayerName()
    //{
    //    Debug.Log("test");
    //    try
    //    {
    //        // Await the call to GetPlayerNameAsync to get the player's name
    //        string playerName = await AuthenticationService.Instance.GetPlayerNameAsync(true);
    //        PlayerPrefs.SetString("playerName", playerName);
    //
    //        //LoadUsernamesFromLeaderboard();
    //
    //        if (playerName.Length < 15) //5 added on with hash and number by unity
    //        {
    //            //CanvasManager.instance.TogglePickUsernameCanvas();
    //        }
    //        else
    //        {
    //            LoadUsernamesFromLeaderboard();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError("Error retrieving player name: " + ex.Message);
    //        //RandomUserNameTextObject.SetActive(true);
    //    }
    //}

    public async void LoadUsernamesFromAllLeaderboards()
    {
        try
        {
            var leaderboardIds = new[] { "HighScore", "High_Score_Stage_2", "High_Score_Stage_3" }; // Add your leaderboard IDs here
            var options = new GetScoresOptions
            {
                Limit = 250
            };

            existingUsernames.Clear(); // Clear before starting

            foreach (var leaderboardId in leaderboardIds)
            {
                var leaderboardEntries = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options);

                foreach (var entry in leaderboardEntries.Results)
                {
                    var trimmedName = entry.PlayerName.Substring(0, Mathf.Min(entry.PlayerName.Length, 14));
                    if (!existingUsernames.Contains(trimmedName))
                    {
                        existingUsernames.Add(trimmedName);
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load usernames from leaderboards: " + e.Message);
        }
    }

    public void SetUnityIdIfconnectionLostWhenSavingIt()
    {
        //if the name was input but couldn't upload then it will have saved to player prefs
        //this tries to upload the saved ID, Deletes the save and will only save again if it fails.
        //Debug.Log("setIDCAlled");

        if (PlayerPrefs.HasKey("UnityID"))
        {
            Debug.Log(PlayerPrefs.GetString("UnityID"));
            SaveDisplayName(PlayerPrefs.GetString("UnityID"));
            PlayerPrefs.DeleteKey("UnityID");
        }
    }

    public void SaveUnityIdAsInputName()
    {
        SaveDisplayName(nameInputField.text.Trim());
        PlayerPrefs.SetString("playerName", nameInputField.text.Trim());
    }

    public async void SaveDisplayName(string displayName)
    {
        PlayerPrefs.SetString("playerName", displayName);

        if (string.IsNullOrEmpty(displayName))
        {
            Debug.LogWarning("Display name cannot be empty.");
            return;
        }

        // First, save it to Unity Authentication
        if (AuthenticationService.Instance.IsSignedIn)
        {
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(displayName);
                Debug.Log("Unity ID display name updated: " + displayName);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to update Unity ID display name: " + e.Message);

                PlayerPrefs.SetString("UnityID", displayName);
                return;
            }
        }
        else
        {
            Debug.LogWarning("Player is not signed in. Unable to update Unity ID display name.");
            PlayerPrefs.SetString("UnityID", displayName);
            return;
        }

        
    }

    public void CheckIfUserNameIsOffensive(string displayName)
    {
        //dont allow # in name
        nameInputField.text = nameInputField.text.Replace("#", "");

        if (IsUsernameOffensive(displayName))
        {
            warnOffensiveUsernameText.SetActive(true);
            saveNameButton.interactable = false;
            warnUsernameLessThan10LongText.SetActive(false);
            return;
        }
        else
        {
            warnOffensiveUsernameText?.SetActive(false);
            if (displayName.Length > 0) saveNameButton.interactable = true;
        }

        //check display name is atleast 10 characters
        if (displayName.Length < 3)
        {
            saveNameButton.interactable = false;

            if (warnOffensiveUsernameText.activeSelf == false &&
                warnUsernameAlreadyUsedText.activeSelf == false)
            {
                warnUsernameLessThan10LongText.SetActive(true);
            }
        }
        else warnUsernameLessThan10LongText.SetActive(false);        
    }

    public void CheckIfUserNameIsTaken(string displayName)
    {
        if (IsUsernameTaken(displayName))
        {
            warnUsernameAlreadyUsedText.SetActive(true);
            warnUsernameLessThan10LongText.SetActive(false);
            saveNameButton.interactable = false;
            return;
        }
        else
        {
            warnUsernameAlreadyUsedText?.SetActive(false);
            if (displayName.Length > 2 && warnOffensiveUsernameText.activeInHierarchy == false) saveNameButton.interactable = true;
        }       
    }

    private void LoadBannedWords()
    {
        // Load your text file from Resources
        TextAsset badWordsFile = Resources.Load<TextAsset>("full-list-of-bad-words_text-file_2022_05_05");
        if (badWordsFile != null)
        {
            string[] lines = badWordsFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                // Trim spaces and check for empty after trimming
                string trimmedWord = line.Trim();
                if (!string.IsNullOrEmpty(trimmedWord) && !trimmedWord.Contains(" "))
                {
                    bannedWords.Add(trimmedWord);
                }
            }
        }
        else
        {
            Debug.LogError("Bad words file not found in Resources.");
        }
    }

    public bool IsUsernameOffensive(string username)
    {
        foreach (string bannedWord in bannedWords)
        {
            if (username.ToLower().Contains(bannedWord.ToLower()))
            {
                return true; // Offensive username detected

            }
        }
        return false; // No offensive content found
    }

    public bool IsUsernameTaken(string username)
    {
        string inputBase = username.Split('#')[0].ToLower();

        foreach (string taken in existingUsernames)
        {
            string takenBase = taken.Split('#')[0].ToLower();

            if (inputBase == takenBase)
            {
                return true;
            }
        }

        return false;
    }
}

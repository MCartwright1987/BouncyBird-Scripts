using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using TMPro; // Use TextMesh Pro for text components 
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using HeathenEngineering;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    public Canvas leaderBoardCanvas;

    public string leaderboardId = "HighScore";

    public GameObject scoreEntryPrefab; // Assign a prefab that represents a score entry
    public GameObject nameEntryPrefab;
    public GameObject rankEntryPrefab; // New prefab for ranks
    public GameObject timeEntryPrefab;
    public GameObject flagEntryPrefab;

    public Transform scoresContainer; // Assign the parent transform for score entries
    public Transform namesContainer;
    public Transform ranksContainer; // New container for ranks
    public Transform timesContainer;
    public Transform flagsContainer;

    public RectTransform VerticalGroupsContainer;

    [SerializeField] ScrollRect scrollRect;

    private int scoresPerPage = 20;
    private int currentOffset = 0;
    private List<LeaderboardEntry> allEntries = new List<LeaderboardEntry>();
    private bool isLoadingMore = false;
    public static int LastScoreLeaderBoard1 = 0;
    public static int LastScoreLeaderBoard2 = 0;
    public static int LastScoreLeaderBoard3 = 0;

    public int thisPlayerRank;

    public Button yourScoreButton;
    [SerializeField] Button minusLevelBtn;
    [SerializeField] Button plusLevelBtn;
    [SerializeField] Button TopScoreBtn;

    public float temp;

    public bool canceledLoadLeaderBoard = false;

    // Unity Servies Authenticated by Ads Script
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }

    public void SetCancelLoadLeaderboard()=> canceledLoadLeaderBoard = true;
    public void SetButtonsCooldown()
    {
        minusLevelBtn.interactable = false;
        plusLevelBtn.interactable = false;
        yourScoreButton.interactable = false;
        TopScoreBtn.interactable = false;

        //Invoke("EnableButtons", 0.5f);
    }

    public void EnableButtons()
    {
        minusLevelBtn.interactable = true;
        plusLevelBtn.interactable = true;
        yourScoreButton.interactable = true;
        TopScoreBtn.interactable = true;
    }

    public async void SubmitScoreLeaderBoard1(long score)
    {
        leaderboardId = "HighScore";


        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
            //Debug.Log("Score submitted1: " + score);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to submit score: " + e.Message);
        }
    }

    public async void SubmitScoreLeaderBoard2(long score)
    {
        leaderboardId = "High_Score_Stage_2";

        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
            //Debug.Log("Score submitted2: " + score);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to submit score: " + e.Message);
        }
    }

    public async void SubmitScoreLeaderBoard3(long score)
    {
        leaderboardId = "High_Score_Stage_3";

        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to submit score: " + e.Message);
        }
    }

    public async Task SaveLastScoreInLeaderBoard(string leaderBoardId)
    {
        leaderboardId = leaderBoardId;

        try
        {
            // Get at least 100 scores from the leaderboard
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, new GetScoresOptions { Limit = 200 });

            if (scoresResponse.Results.Count < 200)
            {
                if (scoresResponse.Results.Count > 0)
                {
                    var lastScore = scoresResponse.Results[scoresResponse.Results.Count - 1].Score;
                    if (leaderBoardId == "HighScore") LastScoreLeaderBoard1 = (int)lastScore;
                    else if (leaderBoardId == "High_Score_Stage_2") LastScoreLeaderBoard2 = (int)lastScore;
                    else if (leaderBoardId == "High_Score_Stage_3") LastScoreLeaderBoard3 = (int)lastScore;
                }
            }
            else
            {
                var hundredthScore = scoresResponse.Results[199].Score;

                if (leaderBoardId == "HighScore") LastScoreLeaderBoard1 = (int)hundredthScore;
                else if (leaderBoardId == "High_Score_Stage_2") LastScoreLeaderBoard2 = (int)hundredthScore;
                else if (leaderBoardId == "High_Score_Stage_3") LastScoreLeaderBoard3 = (int)hundredthScore;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to check leaderboard: " + e.Message);
        }
    }

    public async Task<LeaderboardScoresPage> GetLeaderboardScores(int offset)
    {
        if (GameManager.levelNumber == 3) leaderboardId = "High_Score_Stage_3";
        else if (GameManager.levelNumber == 2) leaderboardId = "High_Score_Stage_2";
        else leaderboardId = "HighScore";

        try
        {
            var options = new GetScoresOptions { Limit = scoresPerPage, Offset = offset };
            var scoresPage = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options);
            return scoresPage;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to get scores: " + e.Message);
            return null;
        }
    }

    public void ClearLeaderBoard()
    {
        StopCoroutine(ScrollToRankCoroutine(0));
        // Clear previous entries
        foreach (Transform child in scoresContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in namesContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in ranksContainer) // Clear previous ranks
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in timesContainer) // Clear previous ranks
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in flagsContainer) // Clear previous ranks
        {
            Destroy(child.gameObject);
        }
    }

    public async void ShowLeaderboard()
    {
        //bool touchedCreen = false;
        canceledLoadLeaderBoard = false;

        ClearLeaderBoard();
        VerticalGroupsContainer.position = new Vector2(VerticalGroupsContainer.position.x, 0);

        string username = AuthenticationService.Instance.PlayerName;
        currentOffset = 0;
        allEntries.Clear();

        // Load the first batch of scores
        var firstBatch = await GetLeaderboardScores(currentOffset);
        if (firstBatch != null && firstBatch.Results.Count > 0)
        {
            allEntries.AddRange(firstBatch.Results);
            PopulateLeaderboard(firstBatch.Results, username);
            currentOffset += scoresPerPage;

            // Continue loading more scores if needed
            while (!canceledLoadLeaderBoard && allEntries.Count < 200)  // Adjust condition as necessary
            {
                //if (Input.GetMouseButton(0) || Input.touchCount > 0) touchedCreen = true;

                var nextBatch = await GetLeaderboardScores(currentOffset);
                if (nextBatch != null && nextBatch.Results.Count > 0)
                {
                    allEntries.AddRange(nextBatch.Results);
                    PopulateLeaderboard(nextBatch.Results, username);
                    currentOffset += scoresPerPage;
                }
                else
                {
                    break;  // No more scores to load, exit the loop
                }
            }

            

            // Now that all entries are loaded, find the player's rank
            thisPlayerRank = allEntries.FindIndex(entry => entry.PlayerName == username) + 1;

            //yourScoreButton.interactable = true;

            if (canceledLoadLeaderBoard) canceledLoadLeaderBoard = false;
            else EnableButtons();

        }
    }

    public void GoToPlayersScore()
    {
        if (thisPlayerRank != 0 && thisPlayerRank <= 200)
        {
            StartCoroutine(ScrollToRankCoroutine(thisPlayerRank));
        } 
    }

    public void GoToTopScore()
    {
        StartCoroutine(ScrollToRankCoroutine(4));
    }

    private void PopulateLeaderboard(List<LeaderboardEntry> entries, string username)
    {
        foreach (var entry in entries)
        {
            GameObject scoreEntry = Instantiate(scoreEntryPrefab, scoresContainer);
            var textComponentScore = scoreEntry.GetComponent<TextMeshProUGUI>();

            GameObject nameEntry = Instantiate(nameEntryPrefab, namesContainer);
            var textComponentName = nameEntry.GetComponent<TextMeshProUGUI>();

            GameObject rankEntry = Instantiate(rankEntryPrefab, ranksContainer);
            var textComponentRank = rankEntry.GetComponent<TextMeshProUGUI>();

            GameObject timeEntry = Instantiate(timeEntryPrefab, timesContainer);
            var textComponentTime = timeEntry.GetComponent<TextMeshProUGUI>();

            GameObject flagEntry = Instantiate(flagEntryPrefab, flagsContainer);
            var imageComponentflag = flagEntry.GetComponent<Image>();

            if (textComponentScore != null && textComponentName != null && textComponentRank != null)
            {
                bool isPlayer = entry.PlayerName == username;
                textComponentName.color = isPlayer ? Color.yellow : Color.white;
                textComponentScore.color = isPlayer ? Color.yellow : Color.white;
                textComponentRank.color = isPlayer ? Color.yellow : Color.white;
                textComponentTime.color = isPlayer ? Color.yellow : Color.white;

                textComponentScore.text = $"{GetScoreFromCombinedValue((int)entry.Score)}";

                int flagNumber = GetFlagFromCombinedValue((int)entry.Score);
                imageComponentflag.sprite = CanvasManager.instance.flags[Mathf.Clamp(flagNumber, 0, CanvasManager.instance.flags.Length - 1)];

                float time = GetTimeFromCombinedValue((int)entry.Score);
                float minutes = Mathf.FloorToInt(time / 60);
                float seconds = Mathf.FloorToInt(time % 60);
                textComponentTime.text = string.Format("{0:00}:{1:00}", minutes, seconds);

                textComponentName.text = RemoveSuffix(entry.PlayerName.Length > 14 ? entry.PlayerName.Substring(0, 14) : entry.PlayerName);
                textComponentRank.text = (allEntries.IndexOf(entry) + 1).ToString();
            }
        }

        //AdjustVerticalGroupsContainerHeight();
    }

    public int GetScoreFromCombinedValue(int combinedValue)
    {
        // Extracts the top 8 bits (score)
        return (combinedValue >> 17) & 0xFF;
    }

    public int GetTimeFromCombinedValue(int combinedValue)
    {
        // Extract the 12-bit inverted time
        int invertedTime = (combinedValue >> 5) & 0xFFF; // 0xFFF = 12-bit mask (4095)

        int maxTime = 4095; // Same max value used in CombineScoreAndTime

        // Reverse the inversion to get the original time
        return maxTime - invertedTime;
    }

    public int GetFlagFromCombinedValue(int combinedValue)
    {
        //you can have flags numbered from 0 to 31

        // Extract the 5-bit flag (last 5 bits)
        int flag = combinedValue & 0x1F; // 0x1F = 5-bit mask (31)

        // Ensure the extracted flag is within 0-29 range
        return Mathf.Clamp(flag, 0, 29);
    }


    string RemoveSuffix(string name)
    {
        // Find the index of '#'
        int hashIndex = name.IndexOf('#');

        // If '#' exists, remove everything from it onward
        if (hashIndex != -1)
        {
            name = name.Substring(0, hashIndex);
        }

        return name;
    }

    private IEnumerator ScrollToRankCoroutine(int rank)
    {
        // Wait for the layout to update to ensure sizes and positions are correct
        yield return new WaitForEndOfFrame();
    
    
        // Get the target entry's position
        RectTransform targetEntry = (RectTransform)namesContainer.GetChild(rank);

        float targetY = 0;

       //Debug.Log("targetEntry.Position " + targetEntry.position.y);
       //Debug.Log("VerticalGroupsContainer.localPosition " + VerticalGroupsContainer.localPosition.y);

        targetY = targetEntry.localPosition.y * -1 - 56; //was 48 for 3 above


        // Smoothly move the container to the target position
        float duration = 0.8f;//rank * 0.01f; //WAS 0.075 Adjust for desired scroll speed
        float elapsedTime = 0;
    
        Vector2 initialPosition = VerticalGroupsContainer.localPosition;
    
        yield return new WaitForSeconds(0.75f);
    
        while (elapsedTime < duration)
        {
            //if (Input.GetMouseButton(0) || Input.touchCount > 0) break;
    
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            VerticalGroupsContainer.localPosition = new Vector2(
                VerticalGroupsContainer.localPosition.x,
                Mathf.Lerp(initialPosition.y, targetY, t)
            );
    
            yield return null; // Wait for the next frame
        }
    }

    public void DisableCanvas() => leaderBoardCanvas.enabled = false;

    public void AdjustVerticalGroupsContainerHeight()
    {
        if (namesContainer.childCount > 10)
        {
            VerticalGroupsContainer.sizeDelta = new Vector2(100, (namesContainer.childCount * 10.01f) + 1.25f);
        }    
    }
}

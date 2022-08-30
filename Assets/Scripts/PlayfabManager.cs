using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System.Linq;

public class PlayfabManager : SingletonComponent<PlayfabManager>
{
    [SerializeField] TMP_InputField username;
    [SerializeField] TMP_InputField wallet;

    private GUI _uiManager;
    private List<PlayerLeaderboardEntry> _leaderboard;

    private void Awake()
    {
        _uiManager = GUI.Instance;
    }

    public void OnLoginButtonPressed()
    {
        Debug.Log("Logging in...");

        var walletText = wallet.text;
        var nametText = username.text;

        if (string.IsNullOrEmpty(walletText) || string.IsNullOrEmpty(nametText))
            LoginWarning();
        else
            Login(wallet.text + username.text);
    }

    void LoginWarning()
    {
        Debug.LogError("Login error");
        GUI.Instance.ShowWarning();
    }

    void UpdateNameWarning()
    {
        Debug.LogError("Update name error");
        GUI.Instance.ShowWarning(); 
    }

    void Login(string userId)
    {
        var loginRequest = new LoginWithCustomIDRequest()
        {            
            CustomId = userId,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        }; 

        PlayFabClientAPI.LoginWithCustomID(loginRequest, OnLoginSuccess, OnError);
    }

    void UpdateName(string username)
    {
        var setNameRequest = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(setNameRequest, OnDisplayNameUpdate, OnUpdateNameError);
    }

    void OnUpdateNameError(PlayFabError error)
    {
        Debug.Log("Update Name Error");
        Debug.Log(error.GenerateErrorReport());
        UpdateNameWarning();
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Username set to: " + result.DisplayName);
        
        _uiManager.HideWarning();
        _uiManager.OnLogginButtonPressed();
        _uiManager.ToggleLoadingMessage();

        SendLeaderboard(_uiManager.GetScore);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Successfull login");
        UpdateName(username.text);
    }

    void OnLoginError(PlayFabError error)
    {
        Debug.Log("Login Error");
        Debug.Log(error.GenerateErrorReport());
        LoginWarning();
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error");
        Debug.Log(error.GenerateErrorReport());
    }

    public void SendLeaderboard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "GameScore",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfull leaderboard update");
        Invoke(nameof(LoadLeaderboard), 1f);
    }
 
    public void LoadLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "GameScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
    }

    void OnLeaderboardGet(GetLeaderboardResult result)
    {
        _leaderboard = new List<PlayerLeaderboardEntry>();

        foreach (var item in result.Leaderboard)
        {
            Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue + " " + item.DisplayName);
            _leaderboard.Add(item);
        }

        _uiManager.ToggleLoadingMessage();
        _uiManager.UpdateLeaderBoard();  
    }

    public List<PlayerLeaderboardEntry> GetLeaderboard() => _leaderboard;
}

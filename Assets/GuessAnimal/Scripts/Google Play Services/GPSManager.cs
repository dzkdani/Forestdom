using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GPSManager : MonoBehaviour
{
    private static GPSManager _instance;
    public static GPSManager instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public static PlayGamesPlatform playGamesPlatform;

    private void Start()
    {
        GPSLogin();
    }

    void GPSLogin()
    {
        if (playGamesPlatform == null)
        {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;

            playGamesPlatform = PlayGamesPlatform.Activate();
        }

        Social.Active.localUser.Authenticate(success =>
        {
            if (success)
            {
                Debug.Log("Login Successfull!");
                UpdateAchievement(0, 100f);
            }
            else
            {
                Debug.Log("Login Failed!");
            }
        });
    }

    #region Achievements
    public void GPSAchievement()
    {
        if (playGamesPlatform == null)
        {
            GPSLogin();
        }
        else
        {
            Social.ShowAchievementsUI();
        }
    }

    public void UpdateAchievement(int id, float progress)
    {
        switch (id)
        {
            case '0': Social.ReportProgress(GPGSIds.achievement_thank_your_for_playing_the_game, progress, 
                (bool success) => { Debug.Log("First Achievement"); });
                break;
            case '1': Social.ReportProgress(GPGSIds.achievement_read_100_animals_name, progress, 
                (bool success) => { Debug.Log("100 Animal name reached"); });
                break;
            case '2': Social.ReportProgress(GPGSIds.achievement_hear_100_animals_sound, progress,
                (bool success) => { Debug.Log("100 Animal sound reached"); });
                break;
            case '3': Social.ReportProgress(GPGSIds.achievement_first_1000_score, progress,
                (bool success) => { Debug.Log("1000 Score reached"); });
                break;
            case '4':
                break;
            default:
                break;
        }
    }
    #endregion

    #region Leaderboard
    public void GPSLeaderboard()
    {
        if (playGamesPlatform == null)
        {
            GPSLogin();
        }
        else
        {
            Social.ShowLeaderboardUI();
        }
    }

    public void UpdateLeaderboard(int score)
    {
        if (PlayerPrefs.GetInt("Highscore") < score)
        {
            PlayerPrefs.SetInt("Highscore", score);
            Social.ReportScore(PlayerPrefs.GetInt("Highscore"), GPGSIds.leaderboard_highscore,
                (bool success) =>
                {
                    Debug.Log("Highscore updated");
                });
        }
    }
    #endregion 
}

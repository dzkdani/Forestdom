using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public enum GameMode
{
    Audio,
    Image
}

public enum GameState
{
    Paused,
    Playing
}

public enum Timer
{
    On,
    Off
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance { get { return _instance; } }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }

    [Header("Game Status")]
    public GameState state;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] int currentScore;
    [SerializeField] TextMeshProUGUI livePointText;
    [SerializeField] int maxLivePoint = 4;
    [SerializeField] int livePointLeft;
    [SerializeField] int correctAnswer = 0;
    public int quizCount;

    #region Initialize
    private void Start()
    {
        state = GameState.Playing;
        quizCount = 0;
        correctAnswer = 0;
        livePointLeft = maxLivePoint;
        livePointText.text = livePointLeft.ToString();
        currentScore = 0;
        scoreText.text = currentScore.ToString();

        InitGame();
    }

    [Header("Quiz Objects")]
    public TextMeshProUGUI questionText;
    [Header("Sound Mode Components")]
    public GameObject soundMode;
    public AudioSource questionSound;
    public Button[] soundOptions = new Button[4];
    [Header("Normal Mode Components")]
    public GameObject nameMode;
    public Image questionImg;
    public GameObject nameModeAudioBtn;
    public Button[] nameOptions = new Button[4];

    void InitGame()
    { 
        LoadQuestion();
        LoadOption();
        QuizMode();
    }

    #region Question
    Questions currentQuestion;
    Questions lastQuestion;
    void LoadQuestion()
    {
        currentQuestion = QuizManager.instance.GetQuestion();

        if (lastQuestion != null && lastQuestion == currentQuestion)
        {
            do
            {
                currentQuestion = QuizManager.instance.GetQuestion();
            } while (lastQuestion == currentQuestion);
        }
    }
    #endregion

    #region Option
    [SerializeField] Questions[] loadedOptions = new Questions[4];
    void LoadOption()
    {
        //wrong opts
        for (int i = 0; i < loadedOptions.Length; i++)
        {
            loadedOptions[i] = QuizManager.instance.GetOptions();
        }

        //correct opts
        int correctIndex = Random.RandomRange(0, 4);
        loadedOptions[correctIndex] = currentQuestion;
    }
    #endregion

    void QuizMode()
    {
        switch (GameModeManager.instance.ActiveGameMode())
        {
            case GameMode.Audio:
                questionText.text = "What animal sound is this?";
                nameMode.SetActive(false);
                soundMode.SetActive(true);

                if (currentQuestion.haveSound && currentQuestion.animalSound != null)
                {
                    questionSound.clip = currentQuestion.animalSound; 
                }
                else
                {
                    Debug.Log("Animal Sound not found!");
                    questionSound.clip = null;
                }

                for (int i = 0; i < soundOptions.Length; i++)
                {
                    soundOptions[i].GetComponent<OptionButton>().SetOptionBtn(loadedOptions[i].animalId, loadedOptions[i].animalSprite,
                        loadedOptions[i].animalName, loadedOptions[i].haveSound);
                    soundOptions[i].GetComponent<Image>().sprite = loadedOptions[i].animalSprite;
                    soundOptions[i].GetComponent<Image>().SetNativeSize();
                    soundOptions[i].GetComponent<Transform>().localScale = new Vector3(0.7f, 0.7f, 0);
                }

                lastQuestion = currentQuestion;
                break;
            case GameMode.Image:
                questionText.text = "What animal is this?";
                soundMode.SetActive(false);
                nameMode.SetActive(true);
                
                questionImg.sprite = currentQuestion.animalSprite;
                questionImg.SetNativeSize();
                if (currentQuestion.haveSound && currentQuestion.animalSound != null)
                {
                    nameModeAudioBtn.SetActive(true);
                    questionSound.clip = currentQuestion.animalSound;
                }
                else
                {
                    nameModeAudioBtn.SetActive(false);
                    questionSound.clip = null;
                }
                
                for (int i = 0; i < nameOptions.Length; i++)
                {
                    nameOptions[i].GetComponent<OptionButton>().SetOptionBtn(loadedOptions[i].animalId, loadedOptions[i].animalSprite,
                        loadedOptions[i].animalName, loadedOptions[i].haveSound);
                    nameOptions[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = loadedOptions[i].animalName;
                }

                lastQuestion = currentQuestion;
                break;
        }
    }
    #endregion

    #region Check Answer & Re-Initialize
    public void CheckAnswer(int selectedId)
    {
        if (QuizManager.instance.CheckAnswer(selectedId))
        {
            CorrectAnswer();
        }
        else
        {
            WrongAnswer();
        }
    }

    void CorrectAnswer()
    {
        StopCoroutine("watiafteranswering");
        //correct audio [1]
        AudioManager.instance.PlaySFX(1);

        addScore();
        TimerManager.instance.BonusTime();

        StartCoroutine("waitafteranswering");
    }
    
    void addScore()
    {
        currentScore += 10;
        correctAnswer += 1;
        scoreText.text = currentScore.ToString();
    }

    void WrongAnswer()
    {
        StopCoroutine("watiafteranswering");
        //wrong audio [2]
        AudioManager.instance.PlaySFX(2);

        minusLive();
        TimerManager.instance.ResetBonusTime();

        StartCoroutine("waitafteranswering");
    }

    IEnumerator waitafteranswering()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        ReInitGame();
    }

    void minusLive()
    {
        livePointLeft -= 1;
        livePointText.text = livePointLeft.ToString();
    }

    void ReInitGame()
    {
        if (livePointLeft != 0 && TimerManager.instance.isTimeLeft())
        {
            quizCount += 1;
            InitGame();
        }
        else
        {
            GameEnded();
        }
    }
    #endregion

    #region End Game & Paused State
    [Header("Game End Components")]
    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] TextMeshProUGUI rewardedDetails;

    public void GameEnded()
    {
        state = GameState.Paused;
        AudioManager.instance.TogglePauseBGM();

        UpdateAchievements();
        RewardedAvailability();

        finalScoreText.text = currentScore.ToString();
        if (resultPanel.activeSelf)
        {
            return;
        }
        else
        {
            resultPanel.SetActive(true);
            resultPanel.transform.DOScale(1, 0.2f).From(0).SetEase(Ease.Linear);
            AudioManager.instance.PlaySFX(3);
        }

        GPSManager.instance.UpdateLeaderboard(currentScore);
    }

    public void TogglePauseGame()
    {
        if (state == GameState.Playing)
        {
            state = GameState.Paused;
        }
        else
        {
            state = GameState.Playing;
            TimerManager.instance.StartTimer();
        }
    }
    #endregion

    void UpdateAchievements()
    {
        switch (GameModeManager.instance.ActiveGameMode())
        {
            case GameMode.Audio:
                GPSManager.instance.UpdateAchievement(2, correctAnswer);
                break;
            case GameMode.Image:
                GPSManager.instance.UpdateAchievement(1, correctAnswer);
                break;
            default: 
                break;
        }

        if (currentScore >= 1000)
        {
            GPSManager.instance.UpdateAchievement(3, 100);
        }
        else; //add more here
    }

    #region Rewarded
    void RewardedAvailability()
    {
        //if player can still watch ad
        if (RewardedAdManager.instance.isRewardedAvailable())
        {
            rewardedDetails.gameObject.SetActive(true);
            AdManager.instance.RequestRewarded();
            if (livePointLeft == 0)
            {
                rewardedDetails.text = "*get extra 2+ lives";
            }
            else
            {
                rewardedDetails.text = "*get extra 30s time";
            }
        }
        else
        {
            rewardedDetails.transform.parent.GetComponent<Button>().interactable = false;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                rewardedDetails.text = "No Internet";
            }
            else
            {
                rewardedDetails.text = "*ad limit reached \ncheck again tomorrow";
            }
        }
    }

    public void CallRewarded()
    {
        AdManager.instance.ShowRewarded();
    }

    public void GetReward()
    {
        resultPanel.SetActive(false);
        if (TimerManager.instance.isTimeLeft() == false)
        {
            TimerManager.instance.ExtraTime();
            ReInitGame();
        }
        else
        {
            livePointLeft = 2;
            ReInitGame();
        }
        RewardedAdManager.instance.RewardRecieved();
    }

    #endregion
}

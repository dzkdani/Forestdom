using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TimerManager : MonoBehaviour
{
    private static TimerManager _instance;
    public static TimerManager instance { get { return _instance; } }
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
    }

    [Header("Timer Components")]
    [SerializeField] double totalTime;
    [SerializeField] double soundTimer;
    [SerializeField] double nameTimer;
    TextMeshProUGUI timerText;
    bool isPaused;

    private void Start()
    {
        timerText = gameObject.GetComponent<TextMeshProUGUI>();
        timerText.text = "";
        isPaused = false;

        StartTimer();
    }

    IEnumerator timer()
    {
        while (GameManager.instance.state == GameState.Playing)
        {
            if (totalTime >= 0)
            {
                //timer progression, make it faster
                TimerProgression();

                totalTime -= Time.deltaTime;
                int seconds = (int)totalTime % 60;
                int minutes = (int)totalTime / 60;
                if (seconds >= 10)
                {
                    timerText.text = "0" + minutes.ToString() + ":" + seconds.ToString();
                }
                else
                {
                    timerText.text = "0" + minutes.ToString() + ":0" + seconds.ToString();
                }

                //shake if time is less than 3
                if (totalTime <= 10)
                {
                    AudioManager.instance.PlaySFX(4);
                    gameObject.transform.DOShakeRotation(0.01f, 5, 2, 90, false);
                }
            }
            else
            {
                TimesUp();
            }
            yield return null;
        }
    }

    public bool isTimeLeft()
    {
        return totalTime >= 0;
    }

    public void StartTimer()
    {
        if (GameModeManager.instance.ActiveGameMode() == GameMode.Image)
        {
            totalTime = nameTimer;
        }
        else if (GameModeManager.instance.ActiveGameMode() == GameMode.Audio)
        {
            totalTime = soundTimer;
        }

        StartCoroutine("timer");
    }

    [Tooltip("multiplier for fasten timer progression, 0.2 is best")]
    [SerializeField] float timerMultiplier;
    void TimerProgression()
    {
        if (GameManager.instance.quizCount % 10 == 0) //check number question player answer for trigger progression
        {
            totalTime -= Time.deltaTime * timerMultiplier;
        }
    }

    int bonusTimeCount = 0;
    public void BonusTime()
    {
        if (bonusTimeCount == 3)
        {
            StopCoroutine("BonusTimeIndicator");
            totalTime += 5;
            bonusTimeCount = 0;
            AudioManager.instance.PlaySFX(5);
            StartCoroutine("BonusTimeIndicator");
        }
        else
        {
            bonusTimeCount += 1;
        }
    }

    IEnumerator BonusTimeIndicator()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);

    }

    public void ResetBonusTime()
    {
        bonusTimeCount = 0;
    }

    void TimesUp()
    {
        StopAllCoroutines();
        GameManager.instance.GameEnded();
    }

    public void ExtraTime()
    {
        totalTime += 31;
        StartTimer();
        AudioManager.instance.PlaySFX(5);
    }
}

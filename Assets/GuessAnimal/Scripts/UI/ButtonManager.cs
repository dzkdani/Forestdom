using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonManager : MonoBehaviour
{
    private static ButtonManager _instance;
    public static ButtonManager instance { get { return _instance; } }
    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    [Header("Tween Properties")]
    [SerializeField] Ease UIEase;
    [SerializeField] float buttonDuration;
    [SerializeField] float panelDuration;

    public void ButtonPressedTween(GameObject pressedButton, string buttonFunc, GameObject buttonPanel)
    {
        currentPressedBtnFunc = buttonFunc;
        currentPressedBtn = pressedButton.GetComponent<Button>();
        currentToggledPanel = buttonPanel;
        currentPressedBtn.interactable = false;


        Sequence btnPress = DOTween.Sequence()
            .Append(pressedButton.transform.DOScale(1.2f, buttonDuration).From(1).SetEase(UIEase))
            .Append(pressedButton.transform.DOScale(1, buttonDuration).SetEase(UIEase))
            .OnComplete(ButtonFunction);
    }

    [Tooltip("Current Pressed Button")]
    [SerializeField] Button currentPressedBtn;
    [Tooltip("Current Pressed Button Function")]
    [SerializeField] string currentPressedBtnFunc;
    void ButtonFunction()
    {
        switch (currentPressedBtnFunc)
        {
            case "Scene":
                TransitionManager.instance.SceneTransition(restart: false);
                break;
            case "Restart":
                TransitionManager.instance.SceneTransition(restart: true);
                break;
            case "Panel":
                TweenPanel();
                break;
            case "GPS":
                CallGPSFunc();
                break;
            case "Volume":
                //should add volume slider? or just mute all audio?
                break;
            case "Exit":
                TransitionManager.instance.QuitGame();
                break;
        }
        currentPressedBtn.interactable = true;
        currentPressedBtn = null;
        currentPressedBtnFunc = null;
    }
    
    void CallGPSFunc()
    {
        switch (currentPressedBtn.name)
        {
            case "Leaderboard":
                GPSManager.instance.GPSLeaderboard();
                break;
            case "Achievement":
                GPSManager.instance.GPSAchievement();
                break;
            default:
                Debug.Log("GPS Button is unknown");
                break;
        }
    }

    #region Panel
    [Tooltip("Current Toggled Panel in UI")]
    [SerializeField] GameObject currentToggledPanel;

    void PanelInTween(GameObject panel)
    {
        currentToggledPanel = panel;
        panel.transform.DOScale(1, panelDuration).From(0).SetEase(UIEase);
    }

    void PanelOutTween(GameObject panel)
    {
        currentToggledPanel = panel;
        panel.transform.DOScale(0, panelDuration).From(1).SetEase(UIEase).OnComplete(SetActivePanel);
    }
    void TweenPanel()
    {
        if (currentToggledPanel.gameObject.activeSelf == false)
        {
            currentToggledPanel.SetActive(true);
            PanelInTween(currentToggledPanel);
        }
        else
        {
            PanelOutTween(currentToggledPanel);
        }
    }
    void SetActivePanel()
    {
        currentToggledPanel.SetActive(false);
        currentToggledPanel = null;
    }
    #endregion
}

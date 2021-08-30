using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TransitionManager : MonoBehaviour
{
    private static TransitionManager _instance;
    public static TransitionManager instance { get { return _instance; } }
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

    [SerializeField] Ease transitionEase;
    [SerializeField] Image transitionPanel;
    [SerializeField] float transitionDuration;
    bool isQuit = false;
    bool isRestart = false;

    private void Start()
    {
        FadeIn(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        SceneManager.sceneLoaded += FadeIn;
    }

    public void SceneTransition(bool restart)
    {
        isRestart = restart;
        FadeOut();
    }


    void FadeIn(Scene scene, LoadSceneMode loadSceneMode)
    {
        transitionPanel.DOFade(0, transitionDuration).From(1).SetEase(transitionEase);
        transitionPanel.raycastTarget = false;
    }

    void FadeOut()
    {
        transitionPanel.raycastTarget = true;
        transitionPanel.DOFade(1, transitionDuration).From(0).SetEase(transitionEase).OnComplete(ChangeScene);
    }

    void ChangeScene()
    {
        transitionPanel.raycastTarget = true;

        if (isQuit == false)
        {
            if (isRestart == false)
            {
                if (SceneManager.GetActiveScene().buildIndex != 1)
                {
                    //load interstitial ad first here
                    AdManager.instance.RequestInterstitial();
                    SceneManager.LoadScene(1);
                }
                else
                {
                    PlayInterstitialAd(); //enable before build
                    //SceneManager.LoadScene(0); //in unity editor enable this
                }
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {
            Application.Quit();
        }
    }

    void PlayInterstitialAd()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }
        else
        {
            AdManager.instance.ShowInterstitial();
        }
    }

    public void QuitGame()
    {
        isQuit = true;
        FadeOut();
    }
}

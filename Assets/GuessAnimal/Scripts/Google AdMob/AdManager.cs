using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    private static AdManager _instance;
    public static AdManager instance { get { return _instance; } }
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

    [Header("AdMob App ID")]
    [SerializeField] string appID;

    [Header("AdMob Unit ID")]
    [SerializeField] string bannerID;
    [SerializeField] string rewardedID;
    [SerializeField] string interstitialID;

    BannerView bannerView;
    RewardBasedVideoAd rewardBasedVideo;
    InterstitialAd interstitial;

    private void Start()
    {
        MobileAds.Initialize(appId: appID);
        
        //init req
        RequestBanner();
        ShowBanner();
        RequestInterstitial();
        RequestRewarded();
    }

    #region Banner
    public void RequestBanner()
    {
        this.bannerView = new BannerView(bannerID, AdSize.SmartBanner, AdPosition.Bottom);

        // Called when an ad request has successfully loaded.
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;
        // Called when an ad is clicked.
        this.bannerView.OnAdOpening += this.HandleOnAdOpened;
        // Called when the user returned from the app after an ad click.
        this.bannerView.OnAdClosed += this.HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.bannerView.OnAdLeavingApplication += this.HandleOnAdLeavingApplication;
    }

    public void ShowBanner()
    {
        AdRequest request = new AdRequest.Builder().Build();

        this.bannerView.LoadAd(request);
    }

    public void DestroyBanner()
    {
        this.bannerView.Destroy();
    }
    #endregion

    #region Interstitial
    public void RequestInterstitial()
    {
        this.interstitial = new InterstitialAd(interstitialID);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        AdRequest request = new AdRequest.Builder().Build();
        this.interstitial.LoadAd(request);
    }

    public void ShowInterstitial()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
        else
        {
            Debug.Log("interstitial failed to load");
        }
    }

    public void DestroyInterstitial()
    {
        this.interstitial.Destroy();
    }
    #endregion

    #region RewardedVideo
    public void RequestRewarded()
    {
        rewardBasedVideo = RewardBasedVideoAd.Instance;

        // Called when an ad request has successfully loaded.
        this.rewardBasedVideo.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardBasedVideo.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardBasedVideo.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show. 
        //this.rewardBasedVideo.OnAdFailedToShow += HandleRewardedAdFailedToShow; ((dunno why this is not work))
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardBasedVideo.OnAdRewarded += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardBasedVideo.OnAdClosed += HandleRewardedAdClosed;

        AdRequest request = new AdRequest.Builder().Build();
        this.rewardBasedVideo.LoadAd(request, rewardedID);
    }

    public void ShowRewarded()
    {
        if (this.rewardBasedVideo.IsLoaded())
        {
            this.rewardBasedVideo.Show();
        }
        else
        {
            Debug.Log("rewarded failed to load");
        }
    }

    public void DestroyRewarded()
    {

    }
    #endregion

    #region Events and Delegates
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        Debug.Log(sender + " ad loaded");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log(sender + " ad not loaded error : " + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        Debug.Log(sender + " HandleAdOpened event received");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        Debug.Log(sender + " HandleAdClosed event received");

        //on intertitial ad finish watching
        SceneManager.LoadScene(0);
        DestroyInterstitial();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        Debug.Log(sender + " HandleAdLeavingApplication event received");
    }

    #region Rewarded Events & Delegates
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("Rewarded Loaded");
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log(
            "Rewarded FailedToLoad with message: "
                             + args.Message);
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        Debug.Log("Rewarded Opening");
    }

    //still cant subscribe this event
    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
       Debug.Log(
            "HandleRewardedAdFailedToShow event received with message: "
                             + args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
       Debug.Log("Rewarded Closed");
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        Debug.Log(
            "Rewarded Earned Reward of : "
                        + amount.ToString() + " " + type);

        GameManager.instance.GetReward();
    }
    #endregion

    #endregion
}

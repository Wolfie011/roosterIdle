using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    public static AdsInitializer current;

    [SerializeField] string _androidGameId = "5481756";
    [SerializeField] string _iOSGameId = "5481757";
    [SerializeField] bool _testMode = true;
    private string _gameId;

    [SerializeField] public List<RewardedAdsButton> _rewardedAdsButtons;
    [SerializeField] private DailyRewardsManager _rewardedAdsDailyRewards;
    [SerializeField] private LuckyWheelManager _rewardedAdsLuckyWheel;

     public PopUpADs popUpAD;

    void Awake()
    {
        current = this;
        InitializeAds();
    }

    public void InitializeAds()
    {
#if UNITY_IOS
            _gameId = _iOSGameId;
#elif UNITY_ANDROID
        _gameId = _androidGameId;
#elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
#endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }


    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        foreach(var button in _rewardedAdsButtons)
        {
            button.LoadAd();
        }
        _rewardedAdsDailyRewards.LoadAd();
        _rewardedAdsLuckyWheel.LoadAd();
    }
    public void InitializePopUp(PopUpADs prefab)
    {
        popUpAD = prefab;
        popUpAD.confirm.LoadAd();
    }
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}
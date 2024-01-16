using EasyUI.PickerWheelUI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class LuckyWheelManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms

    public static LuckyWheelManager current;

    #region UI
    public TextMeshProUGUI Name;

    [SerializeField] private Button uiSpinButton;
    [SerializeField] private TextMeshProUGUI uiSpinButtonText;

    [SerializeField] private Button uiSpinButtonPremium;
    [SerializeField] private TextMeshProUGUI uiSpinButtonTextPremium;

    [SerializeField] private PickerWheel pickerWheel;
    [SerializeField] private PickerWheel pickerWheelPremium;

    public bool countdown = false;
    private Timer timer;
    public TextMeshProUGUI timeLeftText;
    public GameObject block;
    public GameObject option;
    #endregion

    [ReadOnly] public LuckyData dataLCK = new LuckyData();

    private void Awake()
    {
        current = this;

        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

        // Disable the button until the ad is ready to show:
        _showAdButton.interactable = false;
    }
    public void InitializeOnLoad(LuckyData luckyData, bool isExpired)
    {
        dataLCK = luckyData;
        double secondsL = luckyData.secondsLeft - (DateTime.Now - luckyData.pauzeTime).TotalSeconds;
        //Debug.Log(secondsL);
        if (isExpired)
        {
            block.SetActive(false);
        }
        else
        {
            block.SetActive(true);
            timer = gameObject.AddComponent<Timer>();
            timer.Initialize("SpinTimer", DateTime.Now, TimeSpan.FromSeconds(secondsL));
            timer.TimerFinishedEvent.AddListener(delegate
            {
                uiSpinButton.interactable = true;
                uiSpinButtonPremium.interactable = true;

                BaseInit();

                block.SetActive(false);

                Destroy(timer);
            });
            timer.StartTimer();
            countdown = true;
            FixedUpdate();
        }
    }
    public void TimerInitialize()
    {
        TimeSpan h24 = new(0, 0, 55);
        timer = gameObject.AddComponent<Timer>();
        timer.Initialize("SpinTimer", DateTime.Now, h24);
        timer.TimerFinishedEvent.AddListener(delegate
        {
            uiSpinButton.interactable = true;
            uiSpinButtonPremium.interactable = true;

            BaseInit();

            block.SetActive(false);

            Destroy(timer);
        });
        timer.StartTimer();
        countdown = true;
        FixedUpdate();
    }
    private void FixedUpdate()
    {
        if (countdown)
        {
            timeLeftText.text = timer.DisplayTime();
        }
    }
    public static string GenerateUUID()
    {
        Guid guid = Guid.NewGuid();
        string uuid = guid.ToString();
        uuid = uuid.Replace("-", "").ToLower();
        return uuid;
    }

    private void Start()
    {
        uiSpinButton.onClick.AddListener(() =>
        {
            uiSpinButton.interactable = false;
            uiSpinButtonText.text = "Spinning";

            pickerWheel.OnSpinEnd(wheelPiece =>
            {
                switch (wheelPiece.rewardTypes)
                {
                    case RewardTypes.Coin:
                        GameManager.current.GetCoins(wheelPiece.Amount);
                        break;
                    case RewardTypes.Gem:
                        GameManager.current.GetCrystals(wheelPiece.Amount);
                        break;
                    case RewardTypes.GoldChest:
                        StorageManager.current.chests.Add(GenerateUUID(), StorageManager.current.ChestList[0]);
                        break;
                    case RewardTypes.PlatinumChest:
                        StorageManager.current.chests.Add(GenerateUUID(), StorageManager.current.ChestList[1]);
                        break;
                }

                uiSpinButton.interactable = false;
                uiSpinButtonText.text = "Spin";

                option.SetActive(true);
            });

            pickerWheel.Spin();

        });
        uiSpinButtonPremium.onClick.AddListener(() =>
        {
            uiSpinButtonPremium.interactable = false;
            uiSpinButtonTextPremium.text = "Spinning";

            pickerWheelPremium.OnSpinEnd(wheelPiece =>
            {
                switch (wheelPiece.rewardTypes)
                {
                    case RewardTypes.Coin:
                        GameManager.current.GetCoins(wheelPiece.Amount);
                        break;
                    case RewardTypes.Gem:
                        GameManager.current.GetCrystals(wheelPiece.Amount);
                        break;
                    case RewardTypes.GoldChest:
                        StorageManager.current.chests.Add(GenerateUUID(), StorageManager.current.ChestList[0]);
                        break;
                    case RewardTypes.PlatinumChest:
                        StorageManager.current.chests.Add(GenerateUUID(), StorageManager.current.ChestList[1]);
                        break;
                }
                uiSpinButtonPremium.interactable = false;
                uiSpinButtonTextPremium.text = "Spin";


                block.SetActive(true);
                TimerInitialize();

            });

            pickerWheelPremium.Spin();

        });

    }
    private void PremiumInit()
    {
        pickerWheel.gameObject.SetActive(false);
        pickerWheelPremium.gameObject.SetActive(true);
        Name.text = "Premium Wheele";
    }
    private void BaseInit()
    {
        pickerWheel.gameObject.SetActive(true);
        pickerWheelPremium.gameObject.SetActive(false);
        Name.text = "Lucky Wheele";
    }
    public void confirmAfter()
    {
        PremiumInit();
        option.SetActive(false);
    }

    public void nah()
    {
        TimerInitialize();
        BaseInit();
        option.SetActive(false);
        block.SetActive(true);
    }
    private void OnApplicationQuit()
    {
        if (!gameObject.GetComponent<Timer>())
        {
            dataLCK.secondsLeft = 0;
            dataLCK.startTime = new DateTime(2000, 10, 10, 10, 20, 30);
            dataLCK.finishTime = new DateTime(2000, 10, 10, 10, 20, 30);
        }
        else
        {
            dataLCK.startTime = gameObject.GetComponent<Timer>().startTime;
            dataLCK.secondsLeft = gameObject.GetComponent<Timer>().secondsLeft;
            dataLCK.finishTime = gameObject.GetComponent<Timer>().finishTime;
        }
        dataLCK.pauzeTime = DateTime.Now;
        GameManager.current.saveData.AddData(dataLCK); 
    }

    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
            // Configure the button to call the ShowAd() method when clicked:
            _showAdButton.onClick.AddListener(ShowAd);
            // Enable the button for users to click:
            _showAdButton.interactable = true;
        }
    }

    // Implement a method to execute when the user clicks the button:
    public void ShowAd()
    {
        // Disable the button:
        _showAdButton.interactable = false;
        // Then show the ad:
        Advertisement.Show(_adUnitId, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            confirmAfter();
            //Load another ad:
            Advertisement.Load(adUnitId, this);
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }

    void OnDestroy()
    {
        // Clean up the button listeners:
        _showAdButton.onClick.RemoveAllListeners();
    }
}

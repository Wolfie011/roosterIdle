using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using UnityEngine.Advertisements;

public class DailyRewardsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static DailyRewardsManager current;

    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms

    #region UI
    public Sprite active;
    public Sprite inactive;

    public Sprite highback;
    public Sprite highblock;

    public GameObject rewardWindow;
    public GameObject rewardPrefab;
    public Transform OneToSix;
    public Transform Seven;

    public int Strick;
    public List<Rewards> orderRewards;

    private Timer timer;
    public bool countdown = false;
    public TextMeshProUGUI timeLeftText;
    #endregion

    [ReadOnly] public DailyRewardsData dataDRD = new DailyRewardsData();
    private void Awake()
    {
        current = this;
        Load();

        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

        // Disable the button until the ad is ready to show:
        _showAdButton.interactable = false;
    }

    public void InitializeOnLoad(DailyRewardsData dailyRewardsData, bool isExpired)
    {
        dataDRD = dailyRewardsData;
        Strick = dailyRewardsData.strick;
        double secondsL = dailyRewardsData.secondsLeft - (DateTime.Now - dailyRewardsData.pauzeTime).TotalSeconds;

        if (isExpired)
        {
            if (Strick == 0)
            {
                unBlockFirst();
            }
            else
            {
                if (Strick == 6)
                {
                    Strick = 5;
                    unBlock();
                    Check();
                    Strick = 6;
                    unBlock();
                }
                else
                {
                    unBlock();
                    CheckPrev();
                }
            }
        }
        else
        {
            if (dailyRewardsData.secondsLeft == 0)
            {
                if (Strick == 0)
                {
                    unBlockFirst();
                }
                else
                {
                    if (Strick == 6)
                    {
                        Strick = 5;
                        unBlock();
                        Check();
                        Strick = 6;
                        unBlock();
                    }
                    else
                    {
                        unBlock();
                        CheckPrev();
                    }
                }
            }
            else
            {
                if (Strick == 6)
                {
                    Strick = 5;
                    unBlock();
                    Check();

                    Strick = 6;

                    timer = gameObject.AddComponent<Timer>();
                    timer.Initialize("DailyRewardsTimer", DateTime.Now, TimeSpan.FromSeconds(secondsL));
                    timer.TimerFinishedEvent.AddListener(delegate
                    {
                        unBlock();
                        countdown = false;
                        Destroy(timer);
                    });
                    timer.StartTimer();
                    countdown = true;
                    FixedUpdate();
                }
                else
                {
                    unBlockPrev();
                    CheckPrev();

                    timer = gameObject.AddComponent<Timer>();
                    timer.Initialize("DailyRewardsTimer", DateTime.Now, TimeSpan.FromSeconds(secondsL));
                    timer.TimerFinishedEvent.AddListener(delegate
                    {
                        unBlock();
                        countdown = false;
                        Destroy(timer);
                    });
                    timer.StartTimer();
                    countdown = true;
                    FixedUpdate();
                }
            }
        }
    }
    public void TimerInitialize()
    {
        _showAdButton.interactable = false;
        TimeSpan h24 = new(0, 0, 55);
        timer = gameObject.AddComponent<Timer>();
        timer.Initialize("DailyRewardsTimer", DateTime.Now, h24);
        timer.TimerFinishedEvent.AddListener(delegate {
            _showAdButton.interactable = true;
            unBlock();
            countdown = false;
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
        if (!countdown)
        {
            timeLeftText.text = "Collect";
        }
    }
    private void Check()
    {
        if (Strick < 6)
        {
            for (int i = 0; i <= Strick; i++)
            {
                OneToSix.GetChild(i).Find("mark").gameObject.SetActive(true);
            }
        }
        else
        {
            Seven.GetChild(0).Find("mark").gameObject.SetActive(true);
        }
    }
    private void CheckFirst()
    {
        OneToSix.GetChild(0).Find("mark").gameObject.SetActive(true);
    }
    private void CheckPrev()
    {
        if (Strick <= 5)
        {
            for (int i = 0; i < Strick; i++)
            {
                OneToSix.GetChild(i).Find("mark").gameObject.SetActive(true);
            }
        }
        else
        {
            Seven.GetChild(0).Find("mark").gameObject.SetActive(true);
        }
    }
    private void unBlock()
    {
        if (Strick < 6)
        {
            for (int i = 0; i <= Strick; i++)
            {
                OneToSix.GetChild(i).Find("block").gameObject.SetActive(false);
            }
        }
        else
        {
            Seven.GetChild(0).Find("block").gameObject.SetActive(false);
        }
    }
    private void unBlockPrev()
    {
        if (Strick <= 5)
        {
            for (int i = 0; i < Strick; i++)
            {
                OneToSix.GetChild(i).Find("block").gameObject.SetActive(false);
            }
        }
        else
        {
            Seven.GetChild(0).Find("block").gameObject.SetActive(false);
        }
    }
    private void unBlockFirst()
    {
        OneToSix.GetChild(0).Find("block").gameObject.SetActive(false);
    }
    private void Load()
    {
        foreach (Rewards reward in orderRewards)
        {
            if (reward.Day < 7)
            {
                // 1 - 6
                GameObject rewardHolder = Instantiate(rewardPrefab, OneToSix);
                rewardHolder.transform.Find("icon").GetComponent<Image>().sprite = reward.Icon;
                rewardHolder.transform.Find("day").GetComponent<TextMeshProUGUI>().text = $"Day {reward.Day}";
                rewardHolder.transform.Find("amount").GetComponent<TextMeshProUGUI>().text = $"{reward.amount}";
            }
            else
            {
                // 7 7 7
                GameObject rewardHolder = Instantiate(rewardPrefab, Seven);
                rewardHolder.GetComponent<Image>().sprite = highback;
                rewardHolder.transform.Find("block").GetComponent<Image>().sprite = highblock;
                rewardHolder.transform.Find("icon").GetComponent<Image>().sprite = reward.Icon;
                rewardHolder.transform.Find("day").GetComponent<TextMeshProUGUI>().text = $"Day {reward.Day}";
                rewardHolder.transform.Find("amount").GetComponent<TextMeshProUGUI>().text = $"{reward.amount}";
            }
        }
    }
    private void Clear()
    {
        int childCount = OneToSix.childCount;
        for(int i = 0; i < childCount; i++) Destroy(OneToSix.GetChild(i).gameObject);
        Destroy(Seven.GetChild(0).gameObject);
        Load();
    }
    public static string GenerateUUID()
    {
        Guid guid = Guid.NewGuid();
        string uuid = guid.ToString();
        uuid = uuid.Replace("-", "").ToLower();
        return uuid;
    }
    private void addChest(Rewards reward)
    {
        Chest tmpChest = Instantiate(reward.chest);
        Debug.Log(tmpChest.Name);
        //StorageManager.current.chests.Add(GenerateUUID(), tmpChest);
    }
    public void Collect(bool isDouble)
    {
        if (!gameObject.GetComponent<Timer>())
        {
            int multiplyer = 1;
            if (isDouble) { multiplyer = 2; }
            Rewards todayRewards = orderRewards[Strick];

            switch (todayRewards.rewardTypes)
            {
                case RewardTypes.Coin:
                    GameManager.current.GetCoins(multiplyer * todayRewards.amount);
                    break;

                case RewardTypes.GoldChest:
                    for (int i = 0; i < multiplyer; i++)
                    {
                        addChest(todayRewards);
                    }
                    break;

                case RewardTypes.PlatinumChest:
                    for (int i = 0; i < multiplyer; i++)
                    {
                        addChest(todayRewards);
                    }
                    break;

                case RewardTypes.Gem:
                    GameManager.current.GetCrystals(multiplyer * todayRewards.amount);
                    break;
            }

            if (Strick == 0)
            {
                CheckFirst();
            }
            else
            {
                Check();
            }
            if (Strick == 6)
            {
                Check();
                Strick = 0;
                Clear();
            }
            else
            {
                Strick++;
            }
            TimerInitialize();
        }
    }
    
    private void OnApplicationQuit()
    {
        dataDRD.strick = Strick;
        if (!gameObject.GetComponent<Timer>())
        {
            dataDRD.startTime = new DateTime(2000, 10, 10, 10, 20, 30);
            dataDRD.secondsLeft = 0;
            dataDRD.finishTime = new DateTime(2000,10,10,10,20,30);
        }
        else
        {
            dataDRD.startTime = gameObject.GetComponent<Timer>().startTime;
            dataDRD.secondsLeft = gameObject.GetComponent<Timer>().secondsLeft;
            dataDRD.finishTime = gameObject.GetComponent<Timer>().finishTime;
        }
        dataDRD.pauzeTime = DateTime.Now;
        GameManager.current.saveData.AddData(dataDRD);
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
            Collect(true);
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

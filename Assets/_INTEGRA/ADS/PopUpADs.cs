using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopUpADs : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public popUPbutton confirm;
    [SerializeField] private Button decline;

    System.Random rand = new System.Random();

    private void Awake()
    {
        confirm.adReward = generateRandomReward();
        confirm.parentModal = this;
        gameObject.transform.Find("PopUp").transform.Find("title").gameObject.GetComponent<TextMeshProUGUI>().text = $"WANNA GET A  BOUNS: {confirm.adReward.amount} {confirm.adReward.rewardType.ToString()}\r\n\r\nWATCH AD NOW";
    }
    private ADreward generateRandomReward()
    {
        //Quest[] qList = Resources.LoadAll<Quest>($"Quests/{LevelSystem.Level}");
        ADreward[] rewardsList = Resources.LoadAll<ADreward>("ADREWARDS");
        Debug.Log(rewardsList.Length);
        return rewardsList[rand.Next(rewardsList.Length)];
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            Destroy(gameObject);
        });
    }
    public void Destroy()
    {
        AdsInitializer.current.popUpAD = null;
        Destroy(gameObject);
    }
}

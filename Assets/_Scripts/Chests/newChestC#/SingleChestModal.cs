using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SingleChestModal : MonoBehaviour, IPointerClickHandler
{
    public int TAPS = 3;

    public TextMeshProUGUI TAPStext;
    public GameObject chestIMG;

    public Chest innerChest;
    public string innerChestKey;
    ChestDrop randomDrop;
    int randomDropAmount;

    System.Random rnd = new System.Random();

    private void Start()
    {
        chestIMG = gameObject.transform.Find("SingleChestBackground").
            transform.Find("ChestImage").gameObject;
        chestIMG.GetComponent<Image>().sprite = innerChest.Icon;

        TAPStext = gameObject.transform.Find("SingleChestBackground").
            transform.Find("Taps").GetComponent<TextMeshProUGUI>();
        TAPStext.text = $"TAP: {TAPS} TIMES TO OPEN";
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.GetComponent<Button>().onClick.AddListener(delegate
        {
            if(TAPS == 0) collectReward();
            else Destroy(gameObject);
        });
    }
    public void checkTaps()
    {
        switch(TAPS)
        {
            case 0:
                collectReward();

                break;
            case 1:
                LeanTween.rotateZ(chestIMG, 30f, 0.4f).setEaseInBounce();
                LeanTween.rotateZ(chestIMG, -30f, 0.4f).setEaseInBounce().setDelay(0.5f);
                LeanTween.rotateZ(chestIMG, 0f, 0.4f).setEaseInBounce().setDelay(0.5f);

                LeanTween.scale(chestIMG, new Vector3(1.5f, 1.5f, 1.5f), 0.5f);
                LeanTween.scale(chestIMG, new Vector3(0f, 0f, 0f), 0.5f).setDelay(0.1f);
                LeanTween.scale(chestIMG, new Vector3(3f, 3f, 3f), 0.5f).setDelay(0.2f);
                TAPS--;
                TAPStext.text = $"OPENING";

                LeanTween.scale(chestIMG, new Vector3(1f, 1f, 1f), 0.2f).setDelay(1f);
                LeanTween.moveY(chestIMG.GetComponent<RectTransform>(), 0.5f, 1.3f).setLoopPingPong();

                getRandomItem();

                Dictionary<CollectibleItem, int> itemS = new Dictionary<CollectibleItem, int>
                {
                    { randomDrop.drop, randomDropAmount }
                };
                GameManager.current.saveData.RemoveSpecial(innerChestKey, 2);
                StorageManager.current.UpdateItems(itemS, true);
                StorageManager.current.chests.Remove(innerChestKey);

                break;
            case 2:
                LeanTween.rotateZ(chestIMG, -30f, 0.2f).setEaseInBounce();
                LeanTween.scale(chestIMG, new Vector3(1.5f, 1.5f, 1.5f), 0.2f);
                LeanTween.scale(chestIMG, new Vector3(1.3f, 1.5f, 1f), 0.2f).setDelay(0.1f);
                TAPS--;
                TAPStext.text = $"TAP: {TAPS} TIMES TO OPEN";
                break;
            case 3:
                LeanTween.rotateZ(chestIMG, 30f, 0.2f).setEaseInBounce();
                LeanTween.scale(chestIMG, new Vector3(1.5f, 1.5f, 1.5f), 0.2f);
                LeanTween.scale(chestIMG, new Vector3(1.5f, 1.3f, 1f), 0.2f).setDelay(0.1f);
                TAPS--;
                TAPStext.text = $"TAP: {TAPS} TIMES TO OPEN";
                
                break;
        }
    
    }
    private void getRandomItem()
    {
        randomDrop = innerChest.drops[rnd.Next(innerChest.drops.Count)];
        randomDropAmount = rnd.Next(randomDrop.AmountMin, randomDrop.AmountMax);
        Debug.Log("Opend: " + randomDrop.drop.Name + " / " + randomDropAmount);

        TAPStext.text = $"{randomDrop.drop.Name}";

        GameObject amountText = gameObject.transform.Find("SingleChestBackground").
            transform.Find("Amount").gameObject;

        amountText.SetActive(true);
        amountText.GetComponent<TextMeshProUGUI>().text = $"x {randomDropAmount}";


        chestIMG.GetComponent<Image>().sprite = randomDrop.drop.Icon;
    }
    private void collectReward()
    {
        
        Destroy(gameObject);
    }
}

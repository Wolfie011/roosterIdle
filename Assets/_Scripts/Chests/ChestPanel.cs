using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestPanel : MonoBehaviour
{
    public GameObject text;
    public GameObject chestPrefab;
    public Transform chestTransform;
    public GameObject chestModalPrefab;

    private void OnEnable()
    {
        LoadChest();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        gameObject.transform.parent.gameObject.SetActive(false);
    }
    private void LoadChest()
    {
        int childCount = chestTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(chestTransform.GetChild(i).gameObject);
        }
        if (StorageManager.current.chests == null)
        {
        }
        else
        {
            text.gameObject.SetActive(false);
            foreach (var chest in StorageManager.current.chests)
            {
                GameObject chestHolder = Instantiate(chestPrefab, chestTransform);
                chestHolder.GetComponent<ChestHolder>().innerChest = chest.Value;
                chestHolder.GetComponent<ChestHolder>().innerChestKey = chest.Key;
                chestHolder.GetComponent<ChestHolder>().parentChestPanel = this;
                chestHolder.GetComponent<ChestHolder>().chestModalPrefab = chestModalPrefab;
            }
        }
        if (StorageManager.current.chests.Count == 0) { text.SetActive(true); }
        else { text.SetActive(false); }
    }
}

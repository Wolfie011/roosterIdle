using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChestHolder : MonoBehaviour
{
    public Chest innerChest;
    public string innerChestKey;
    public ChestPanel parentChestPanel;
    public GameObject chestModalPrefab;

    private void Start()
    {
        gameObject.transform.Find("NAME").GetComponent<TextMeshProUGUI>().text = innerChest.Name;
        gameObject.transform.Find("ICON").GetComponent<Image>().sprite = innerChest.Icon;
    }
    public void selectBoxOpen()
    {
        SingleChestModal modal = Instantiate(chestModalPrefab, GameManager.current.canvas.transform).GetComponent<SingleChestModal>();
        modal.innerChest = innerChest;
        modal.innerChestKey = innerChestKey;
        parentChestPanel.Hide();
    }
}

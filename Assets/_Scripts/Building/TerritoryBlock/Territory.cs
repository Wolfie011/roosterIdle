using System;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Territory : MonoBehaviour, IPointerClickHandler
{
    public ShopItem shopItem;
    //closed area in tiles
    public BoundsInt area;
    //open the area window
    public GameObject windowPrefab;
    //amount of currency needed to unlock the territory
    public int crystalsAmount;

    //starting point of the area (should be lower corner actually)
    private Vector3 upperCorner;
    //sprite renderer of the area
    private SpriteRenderer sr;

    [ReadOnly] public BlockData bData = new BlockData();

    private void Start()
    {
        //initialize the fields
        sr = GetComponent<SpriteRenderer>();
        Bounds bounds = GetComponent<PolygonCollider2D>().bounds;
        upperCorner = new Vector3(transform.position.x, bounds.min.y);

        //save the starting point of the area
        Vector3Int cellPosition = GridBuildingSystem.current.gridLayout.LocalToCell(upperCorner);
        area.position = cellPosition + new Vector3Int(1, 1, 0);
        //lock the territory
        GridBuildingSystem.current.TakeArea(area);
        //deselect it to set the opacity to the right value
        Deselect();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //instantiate the opening window
        GameObject holder = Instantiate(windowPrefab, GameManager.current.canvas.transform);
        Transform window = holder.transform.GetChild(0);

        //initialize button on the panel to close the window if the player clicks anywhere not on the window
        holder.GetComponent<Button>().onClick.AddListener(delegate
        {
            Destroy(holder);
            Deselect();
        });

        //initialize close button
        window.Find("Close Button").GetComponent<Button>().onClick.AddListener(delegate
        {
            Destroy(holder);
            Deselect();
        });

        //initialize buy button
        window.Find("Buy Button").GetComponent<Button>().onClick.AddListener(delegate
        {
            //make the object listen to both events
            EventManager.Instance.AddListenerOnce<EnoughCurrencyGameEvent>(OnEnoughCurrency);
            EventManager.Instance.AddListenerOnce<NotEnoughCurrencyGameEvent>(OnNotEnoughCurrency);

            //invoke currency change event to notify the currency system
            CurrencyChangeGameEvent info = new CurrencyChangeGameEvent(-crystalsAmount, CurrencyType.Crystals);
            EventManager.Instance.QueueEvent(info);

            //destroy the window
            Destroy(holder);
            Deselect();
        });

        //initialize amount of currency needed
        window.Find("Amount Text").GetComponent<TextMeshProUGUI>().text = crystalsAmount.ToString();

        //focus on the area
        PanZoom.current.Focus(transform.position);
        //select it to indicate
        Select();
    }
    private void UnlockTerritory()
    {
        //unlock territory - clear the area from tiles
        GridBuildingSystem.current.UnlockTerritory(area);
        //destroy the area object
        Destroy(gameObject);
        GameManager.current.saveData.RemoveSpecial(bData.ID, 3);
    }

    private void OnEnoughCurrency(EnoughCurrencyGameEvent info)
    {
        //unlock territory if the player has enough currency
        UnlockTerritory();
        //remove listener of the opposite event
        EventManager.Instance.RemoveListener<NotEnoughCurrencyGameEvent>(OnNotEnoughCurrency);
    }

    private void OnNotEnoughCurrency(NotEnoughCurrencyGameEvent info)
    {
        //remove listener of the opposite event
        EventManager.Instance.RemoveListener<EnoughCurrencyGameEvent>(OnEnoughCurrency);
    }
    private void Select()
    {
        //make the sprite more opaque
        Color color = sr.color;
        color.a = 1f;
        sr.color = color;
    }
    private void Deselect()
    {
        //make the sprite less opaque
        Color color = sr.color;
        color.a = 1f;
        sr.color = color;
    }
    private void OnApplicationQuit()
    {
        bData.assetName = shopItem.Name;
        bData.area = area;
        bData.position = transform.position;
        if (string.IsNullOrEmpty(bData.ID)) bData.ID = SaveData.GenerateId();
        GameManager.current.saveData.AddData(bData);
    }
}

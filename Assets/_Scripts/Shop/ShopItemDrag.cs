using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    //item we're dragging (to place)
    private ShopItem Item;
    public static Canvas canvas;

    //fields for dragging
    private RectTransform rt;
    private CanvasGroup cg;
    private Image img;

    //to return to the original position
    private Vector3 originPos;
    public bool drag;

    public void Initialize(ShopItem item)
    {
        //initialize Item
        Item = item;
    }
    
    private void Awake()
    {
        //initialize fields
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();

        img = GetComponent<Image>();
        originPos = rt.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        drag = true;
        cg.blocksRaycasts = false;
        img.maskable = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //move the icon object
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        drag = false;
        cg.blocksRaycasts = true;
        img.maskable = true;
        rt.anchoredPosition = originPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //hide the shop
        ShopManager.current.ShopButton_Click();
        //make the image invisible
        Color c = img.color;
        c.a = 0f;
        img.color = c;

        if (CurrencySystem.current.CurrencyAmounts[Item.Currency] >= Item.Price)
        {
            if (Item.building == null)
            {

            }
            else
            {
                //Building logic
                ShopManager.current.buildingPanel.SetActive(true);
                GridBuildingSystem.current.InitializeWithBuilding(Item.building);
            }
            
        }        
    }
    private void OnEnable()
    {
        drag = false;
        cg.blocksRaycasts = true;
        img.maskable = true;
        rt.anchoredPosition = originPos;
        
        //make the image visible
        Color c = img.color;
        c.a = 1f;
        img.color = c;
    }
}

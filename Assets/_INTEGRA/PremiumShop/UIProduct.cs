using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIProduct : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI NameText;
    [SerializeField]
    private TextMeshProUGUI DescriptionText;
    [SerializeField]
    private Image Icon;
    [SerializeField]
    private TextMeshProUGUI PriceText;
    [SerializeField]
    private Button PurchaseButton;

    private UnityEngine.Purchasing.Product Model;

    public delegate void PurchaseEvent(UnityEngine.Purchasing.Product Model, Action OnComplete);
    public event PurchaseEvent OnPurchase;

    public void Setup(UnityEngine.Purchasing.Product Product)
    {
        Model = Product;
        NameText.SetText(Product.metadata.localizedTitle);
        DescriptionText.SetText(Product.metadata.localizedDescription);
        PriceText.SetText($"{Product.metadata.localizedPriceString} "+
            $"{Product.metadata.isoCurrencyCode}");
        Texture2D texture = StoreIconProvider.GetIcon(Product.definition.id);
        if (texture != null)
        {
            Sprite sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                Vector2.one / 2f
            );
            Icon.sprite = sprite;
        }
        else
        {
            Debug.LogError($"No Sprite found for {Product.definition.id}!");
        }
    }
    public void Purchase()
    {
        PurchaseButton.enabled = false;
        OnPurchase?.Invoke(Model, HandlePurchaseComplete);
    }
    private void HandlePurchaseComplete()
    {
        PurchaseButton.enabled = true;
    }
}

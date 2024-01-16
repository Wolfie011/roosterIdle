using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class StorePage : MonoBehaviour, IStoreListener
{
    [SerializeField]
    private UIProduct ProductPrefab;
    [SerializeField]
    private HorizontalLayoutGroup ContentPanel;
    [SerializeField]
    private GameObject LoadingOverlay;

    private Action OnPurchaseCompleted;
    private IStoreController StoreController;
    private IExtensionProvider ExtensionProvider;

    [SerializeField] private bool UseFakeStore = true;

    private async void Awake()
    {
        InitializationOptions options = new InitializationOptions()
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            .SetEnvironmentName("Testowe");
#else
            .SetEnvironmentName("production");
#endif
        await UnityServices.InitializeAsync(options);
        ResourceRequest operation = Resources.LoadAsync<TextAsset>("IAPProductCatalog");
        operation.completed += HandleIAPCatalogLoaded;
    }
    private void HandleIAPCatalogLoaded(AsyncOperation operation)
    {
        ResourceRequest request = operation as ResourceRequest;
        Debug.Log($"Loaded Asset: {request.asset}");
        ProductCatalog catalog = JsonUtility.FromJson<ProductCatalog>((request.asset as TextAsset).text);
        Debug.Log($"Loaded catalog with {catalog.allProducts.Count} items");

        if (UseFakeStore) // Use bool in editor to control fake store behavior.
        {
            StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser; // Comment out this line if you are building the game for publishing.
            StandardPurchasingModule.Instance().useFakeStoreAlways = true; // Comment out this line if you are building the game for publishing.
        }

#if UNITY_ANDROID
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.GooglePlay)
        );
#elif UNITY_ISO
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.AppleAppStore)
        );
#else
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(
            StandardPurchasingModule.Instance(AppStore.NotSpecified)
        );
#endif

        foreach (ProductCatalogItem item in catalog.allProducts)
        {
            builder.AddProduct(item.id, item.type);
        }

        UnityPurchasing.Initialize(this, builder);
    }
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        StoreController = controller;
        ExtensionProvider = extensions;
        StoreIconProvider.Initialize(StoreController.products);
        StoreIconProvider.OnLoadComplete += HandleAllIconsLoaded;
    }
    private void HandleAllIconsLoaded()
    {
        Debug.Log($"TRIGGERED HandleAllIconsLoaded");
        StartCoroutine(CreateUI());
    }
    private IEnumerator CreateUI()
    {
        List<UnityEngine.Purchasing.Product> sortedProducts = StoreController.products.all
            .OrderBy(item => item.metadata.localizedPrice)
            .ToList();

        foreach(UnityEngine.Purchasing.Product product in sortedProducts)
        {
            Debug.Log($"Instatiate product: {product.definition.id}");
            UIProduct productUI = Instantiate(ProductPrefab, ContentPanel.transform);
            productUI.OnPurchase += HandlePurchase;
            productUI.Setup(product);
            //productUI.transform.SetParent(ContentPanel.transform, false);
            yield return null;
        }

        HorizontalLayoutGroup group = ContentPanel.GetComponent<HorizontalLayoutGroup>();
        float spacing = group.spacing;
        float horizontalPadding = group.padding.left + group.padding.right;
        float itemSize = ContentPanel.transform
            .GetChild(0)
            .GetComponent<RectTransform>()
            .sizeDelta.x;

        RectTransform contentPanelRectTransform = ContentPanel.GetComponent<RectTransform>();
        contentPanelRectTransform.sizeDelta = new(
            horizontalPadding + (spacing + itemSize) * sortedProducts.Count,
            contentPanelRectTransform.sizeDelta.y
        );
    }
    private void HandlePurchase(UnityEngine.Purchasing.Product Product, Action OnPurchaseCompleted)
    {
        //switch onpurchase completed on build
        LoadingOverlay.SetActive(true);
        this.OnPurchaseCompleted = OnPurchaseCompleted;
        StoreController.InitiatePurchase(Product);
        //this.OnPurchaseCompleted = OnPurchaseCompleted;
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        throw new System.NotImplementedException();
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log($"Error initializing IAP becouse of {error}." +
            $"\r\nShow a message to the player depending on the error.");
    }

    public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Failed to purchase {product.definition.id} becouse {failureReason}");
        OnPurchaseCompleted?.Invoke();
        OnPurchaseCompleted = null;
        LoadingOverlay.SetActive(false);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log($"Successfully purchased {purchaseEvent.purchasedProduct.definition.id}");
        OnPurchaseCompleted?.Invoke();
        OnPurchaseCompleted = null;
        LoadingOverlay.SetActive(false);

        //Give player thier items

        return PurchaseProcessingResult.Complete;
    }
}

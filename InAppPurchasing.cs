using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.UI;
using TMPro;

namespace Samples.Purchasing.Core.BuyingConsumables
{
    public class InAppPurchasings : MonoBehaviour, IDetailedStoreListener
    {
        public static InAppPurchasings instance;

        IStoreController m_StoreController; // The Unity Purchasing system.
        private IExtensionProvider m_ExtensionProvider;

        //Your products IDs. They should match the ids of your products in your store.
        public string tenSkipAdTokensId = "10_skip_ad_tokens";
        public string thirtySkipAdTokensId = "30_skip_ad_tokens";
        public string removeAdsId = "remove_ads";

        public string unlockGreenBirdId = "unlock_green_bird";
        public string unlockPinkBirdId = "unlock_pink_bird";
        public string unlockRedBirdId = "unlock_red_bird";

        public bool inAppPurchasingInitialized;

        void Start()
        {
            // Singleton pattern
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void UpdateStorePrices()
        {
            if (m_StoreController == null)
            {
                Debug.LogWarning("Store is not initialized yet!");
                return;
            }

            // Retrieve each product from the store controller and update UI text with the prices
            Product tenSkipAdTokens = m_StoreController.products.WithID(tenSkipAdTokensId);
            Product thirtySkipAdTokens = m_StoreController.products.WithID(thirtySkipAdTokensId);
            Product removeAds = m_StoreController.products.WithID(removeAdsId);
            Product unlockGreenBird = m_StoreController.products.WithID(unlockGreenBirdId);
            Product unlockPinkBird = m_StoreController.products.WithID(unlockPinkBirdId);
            Product unlockRedBird = m_StoreController.products.WithID(unlockRedBirdId);

            if (tenSkipAdTokens != null && tenSkipAdTokens.availableToPurchase)
                Shop.instance.tenAdTokensPrice.text = tenSkipAdTokens.metadata.localizedPriceString;

            if (thirtySkipAdTokens != null && thirtySkipAdTokens.availableToPurchase)
                Shop.instance.thirtyAdTokensPrice.text = thirtySkipAdTokens.metadata.localizedPriceString;

            if (removeAds != null && removeAds.availableToPurchase)
                Shop.instance.seventyAdTokensPrice.text = removeAds.metadata.localizedPriceString;

            if (unlockGreenBird != null && unlockGreenBird.availableToPurchase)
                Shop.instance.greenBirdPrice.text = unlockGreenBird.metadata.localizedPriceString; // Assuming all skins have the same price.

            if (unlockRedBird != null && unlockRedBird.availableToPurchase)
                Shop.instance.redBirdPrice.text = unlockRedBird.metadata.localizedPriceString;

            if (unlockPinkBird != null && unlockPinkBird.availableToPurchase)
                Shop.instance.pinkBirdPrice.text = unlockPinkBird.metadata.localizedPriceString;
        }

        public void InitializePurchasing()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            //Add products that will be purchasable and indicate its type.
            builder.AddProduct(tenSkipAdTokensId, ProductType.Consumable);
            builder.AddProduct(thirtySkipAdTokensId, ProductType.Consumable);

            builder.AddProduct(removeAdsId, ProductType.NonConsumable);
            builder.AddProduct(unlockGreenBirdId, ProductType.NonConsumable);
            builder.AddProduct(unlockPinkBirdId, ProductType.NonConsumable);
            builder.AddProduct(unlockRedBirdId, ProductType.NonConsumable);

            UnityPurchasing.Initialize(this, builder);
        }

        public void BuyTenSkipAdTokens()
        {
            m_StoreController.InitiatePurchase(tenSkipAdTokensId);
        }

        public void BuyThirtySkipAdTokens()
        {
            m_StoreController.InitiatePurchase(thirtySkipAdTokensId);
        }

        public void BuyRemoveAds()
        {
            m_StoreController.InitiatePurchase(removeAdsId);
        }

        public void BuyGreenBird()
        {
            m_StoreController.InitiatePurchase(unlockGreenBirdId);
        }

        public void BuyPinkBird()
        {
            m_StoreController.InitiatePurchase(unlockPinkBirdId);
        }

        public void BuyRedBird()
        {
            m_StoreController.InitiatePurchase(unlockRedBirdId);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            //Debug.Log("In-App Purchasing successfully initialized");
            inAppPurchasingInitialized = true;
            m_StoreController = controller;
            m_ExtensionProvider = extensions;  // Store the extension provider
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializeFailed(error, null);
            inAppPurchasingInitialized=false;
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            var errorMessage = $"Purchasing failed to initialize. Reason: {error}.";

            if (message != null)
            {
                errorMessage += $" More details: {message}";
            }

            Debug.Log(errorMessage);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            //Retrieve the purchased product
            var product = args.purchasedProduct;

            //Add the purchased product to the players inventory
            if (product.definition.id == tenSkipAdTokensId)
            {
                GameManager.instance.AddSkipAdTokens(10);
            }
            else if (product.definition.id == thirtySkipAdTokensId)
            {
                GameManager.instance.AddSkipAdTokens(30);
            }
            else if (product.definition.id == removeAdsId)
            {
                SaveSystem.SetInt("AdsRemoved", 1);
                AdsInitializer.instance.gameObject.SetActive(false);
                GameManager.instance.skipAdsTokensTopOfScreen.SetActive(false);
                GameManager.instance.StartScreenPlayAdBtn.SetActive(false);
                //unlock continue streak unlock as obsolete.
                GameManager.instance.UnlockPlayer("player10Unlocked", 10, false);
            }            
            else if (product.definition.id == unlockPinkBirdId)
            {
                GameManager.instance.UnlockPlayer("player6Unlocked", 6, true);
            }
            else if (product.definition.id == unlockRedBirdId)
            {
                GameManager.instance.UnlockPlayer("player5Unlocked", 5, true);
            }
            else if (product.definition.id == unlockGreenBirdId)
            {
                GameManager.instance.UnlockPlayer("player7Unlocked", 7, true);
            }

            CanvasManager.instance.UpdateShopUi();

            Debug.Log($"Purchase Complete - Product: {product.definition.id}");

            //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
                $" Purchase failure reason: {failureDescription.reason}," +
                $" Purchase failure details: {failureDescription.message}");
        }

        public void RestorePurchases()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (m_StoreController == null || m_ExtensionProvider == null)
                {
                    Debug.LogError("Restore Purchases failed: StoreController or ExtensionProvider is not initialized.");
                    return;
                }

                Debug.Log("Attempting to restore purchases...");

                // Get Apple extensions and restore transactions
                var appleExtensions = m_ExtensionProvider.GetExtension<IAppleExtensions>();
                appleExtensions.RestoreTransactions((success, error) =>
                {
                    if (success)
                    {
                        Debug.Log("Restore Purchases Successful.");
                        CanvasManager.instance.UpdateShopUi();
                    }
                    else
                    {
                        Debug.LogError($"Restore Purchases Failed: {error}");
                    }
                });
            }
            else
            {
                Debug.Log("Restore Purchases is only available on iOS.");
            }
        }



    }
}

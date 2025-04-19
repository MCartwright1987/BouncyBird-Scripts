using Samples.Purchasing.Core.BuyingConsumables;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public static Shop instance;

    public TextMeshProUGUI seventyAdTokensPrice;
    public TextMeshProUGUI thirtyAdTokensPrice;
    public TextMeshProUGUI tenAdTokensPrice;
    public TextMeshProUGUI redBirdPrice;
    public TextMeshProUGUI greenBirdPrice;
    public TextMeshProUGUI pinkBirdPrice;

    private void Start()
    {
        instance = this;
    }

    public void BuyTenSkipAdTokens() => InAppPurchasing.instance.BuyTenSkipAdTokens();
    public void BuyThirtySkipAdTokens() => InAppPurchasing.instance.BuyThirtySkipAdTokens();
    public void BuyRemoveAds() => InAppPurchasing.instance.BuyRemoveAds();
    public void BuyGreenBird() => InAppPurchasing.instance.BuyGreenBird();
    public void BuyPinkBird() => InAppPurchasing.instance.BuyPinkBird();
    public void BuyRedBird() => InAppPurchasing.instance.BuyRedBird();

    public void RestorePurchases() => InAppPurchasing.instance.RestorePurchases();
}

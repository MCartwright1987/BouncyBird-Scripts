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

    public void BuyTenSkipAdTokens() => InAppPurchasings.instance.BuyTenSkipAdTokens();
    public void BuyThirtySkipAdTokens() => InAppPurchasings.instance.BuyThirtySkipAdTokens();
    public void BuyRemoveAds() => InAppPurchasings.instance.BuyRemoveAds();
    public void BuyGreenBird() => InAppPurchasings.instance.BuyGreenBird();
    public void BuyPinkBird() => InAppPurchasings.instance.BuyPinkBird();
    public void BuyRedBird() => InAppPurchasings.instance.BuyRedBird();

    public void RestorePurchases() => InAppPurchasings.instance.RestorePurchases();
}

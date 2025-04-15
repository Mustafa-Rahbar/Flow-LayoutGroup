using System;
using System.Collections;
using System.Collections.Generic;
using Bazaar.Data;
using Bazaar.Poolakey;
using Bazaar.Poolakey.Data;
using UnityEngine;

public class BazaarStore : MonoBehaviour, IStorePlatform
{
    private Payment _payment;
    private const string RsaKey = "";

    private BazaarStore()
    { }

    public event Action billingSupportedEvent;
    public event Action<string> billingNotSupportedEvent;
    public event Action<List<PurchaseInfo>> queryPurchasesSucceededEvent;
    public event Action<string> queryPurchasesFailedEvent;
    public event Action<List<SKUDetails>> querySkuDetailsSucceededEvent;
    public event Action<List<PurchaseInfo>, List<SKUDetails>> queryInventorySucceededEvent;
    public event Action<string> queryInventoryFailedEvent;
    public event Action<string> querySkuDetailsFailedEvent;
    public event Action<PurchaseInfo> purchaseSucceededEvent;
    public event Action<string> purchaseFailedEvent;
    public event Action<bool> consumePurchaseSucceededEvent;
    public event Action<string> consumePurchaseFailedEvent;

    void Awake()
    {
        var securityCheck = SecurityCheck.Enable(RsaKey);
        var paymentConfiguration = new PaymentConfiguration(securityCheck);
        _payment = new Payment(paymentConfiguration);
    }

    void Start()
    {
        Connect();
    }

    void OnDestroy()
    {
        Disconnect();
    }

    public async void Connect()
    {
        var result = await _payment.Connect();

        if (result.status == Status.Success)
        {
            billingSupportedEvent?.Invoke();
            return;
        }

        billingNotSupportedEvent?.Invoke(result.message);
    }

    public void Disconnect()
    {
        _payment.Disconnect();
    }

    public async void QueryInventory(string[] skus)
    {
        var result1 = await _payment.GetPurchases();
        var result2 = await _payment.GetSkuDetails(skus);

        if (result1.status is Status.Success && result2.status == Status.Success)
        {
            var purchases = result1.data;
            var skuInfos = result2.data;
            queryInventorySucceededEvent?.Invoke(purchases, skuInfos);
            return;
        }

        queryInventoryFailedEvent?.Invoke($"{result1.message}\n{result2.message}");
    }

    public async void QueryPurchases()
    {
        var result = await _payment.GetPurchases();

        if (result.status is Status.Success)
        {
            var purchases = result.data;
            queryPurchasesSucceededEvent?.Invoke(purchases);
            return;
        }

        queryPurchasesFailedEvent?.Invoke(result.message);
    }

    public async void QuerySkuDetails(string[] skus)
    {
        var result = await _payment.GetSkuDetails(skus);

        if (result.status == Status.Success)
        {
            querySkuDetailsSucceededEvent?.Invoke(result.data);
            return;
        }

        querySkuDetailsFailedEvent?.Invoke(result.message);
    }

    public async void PurchaseProduct(string sku, string developerPayload = "")
    {
        var result = await _payment.Purchase(sku);

        if (result.status == Status.Success)
        {
            var purchase = result.data;
            purchaseSucceededEvent?.Invoke(purchase);

            return;
        }

        purchaseFailedEvent?.Invoke(result.message);
    }

    public async void ConsumeProduct(string token)
    {
        var result = await _payment.Consume(token);
        if (result.status == Status.Success)
        {
            var consumed = result.data;
            consumePurchaseSucceededEvent?.Invoke(consumed);
            return;
        }

        consumePurchaseFailedEvent?.Invoke(result.message);
    }

    public void ConsumeProducts(string[] tokens)
    {
        for (int i = 0; i < tokens.Length; i++)
        {
            ConsumeProduct(tokens[i]);
        }
    }
}
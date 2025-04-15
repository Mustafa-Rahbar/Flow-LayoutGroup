using System.Collections.Generic;
using Bazaar.Poolakey.Data;
using UnityEngine;

public class BazaarApiEventListener : AbstractIABEventListener<PurchaseInfo, SKUDetails, bool>
{
    private BazaarStore m_Listener;
    private BazaarStore listener => m_Listener ??= GetComponent<BazaarStore>();

    void OnEnable()
    {
        // Listen to all events for illustration purposes
        listener.billingSupportedEvent += billingSupportedEvent;
        listener.billingNotSupportedEvent += billingNotSupportedEvent;
        listener.queryInventorySucceededEvent += queryInventorySucceededEvent;
        listener.queryInventoryFailedEvent += queryInventoryFailedEvent;
        listener.querySkuDetailsSucceededEvent += querySkuDetailsSucceededEvent;
        listener.querySkuDetailsFailedEvent += querySkuDetailsFailedEvent;
        listener.queryPurchasesSucceededEvent += queryPurchasesSucceededEvent;
        listener.queryPurchasesFailedEvent += queryPurchasesFailedEvent;
        listener.purchaseSucceededEvent += purchaseSucceededEvent;
        listener.purchaseFailedEvent += purchaseFailedEvent;
        listener.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
        listener.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
    }

    void OnDisable()
    {

    }

    protected override void billingSupportedEvent()
    {
        Debug.Log("billingSupportedEvent");
    }

    protected override void billingNotSupportedEvent(string error)
    {
        Debug.Log("billingNotSupportedEvent: " + error);
    }

    protected override void queryInventorySucceededEvent(List<PurchaseInfo> purchases, List<SKUDetails> skus)
    {
        Debug.Log(string.Format("queryInventorySucceededEvent. total purchases: {0}, total skus: {1}", purchases.Count, skus.Count));

        for (int i = 0; i < purchases.Count; ++i)
        {
            Debug.Log(purchases[i].ToString());
        }

        Debug.Log("-----------------------------");

        for (int i = 0; i < skus.Count; ++i)
        {
            Debug.Log(skus[i].ToString());
        }
    }

    protected override void queryInventoryFailedEvent(string error)
    {
        Debug.Log("queryInventoryFailedEvent: " + error);
    }

    protected override void querySkuDetailsSucceededEvent(List<SKUDetails> skus)
    {
        Debug.Log(string.Format("querySkuDetailsSucceededEvent. total skus: {0}", skus.Count));

        for (int i = 0; i < skus.Count; ++i)
        {
            Debug.Log(skus[i].ToString());
        }
    }

    protected override void querySkuDetailsFailedEvent(string error)
    {
        Debug.Log("querySkuDetailsFailedEvent: " + error);
    }

    protected override void queryPurchasesSucceededEvent(List<PurchaseInfo> purchases)
    {
        Debug.Log(string.Format("queryPurchasesSucceededEvent. total purchases: {0}", purchases.Count));

        for (int i = 0; i < purchases.Count; ++i)
        {
            Debug.Log(purchases[i].ToString());
        }
    }

    protected override void queryPurchasesFailedEvent(string error)
    {
        Debug.Log("queryPurchasesFailedEvent: " + error);
    }

    protected override void purchaseSucceededEvent(PurchaseInfo purchase)
    {
        Debug.Log("purchaseSucceededEvent: " + purchase);
    }

    protected override void purchaseFailedEvent(string error)
    {
        Debug.Log("purchaseFailedEvent: " + error);
    }

    protected override void consumePurchaseSucceededEvent(bool purchase)
    {
        Debug.Log("consumePurchaseSucceededEvent: " + purchase);
    }

    protected override void consumePurchaseFailedEvent(string error)
    {
        Debug.Log("consumePurchaseFailedEvent: " + error);
    }
}
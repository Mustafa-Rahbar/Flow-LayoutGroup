using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractIABEventListener<PurchaseInfo, SkuInfo, ConsumeType> : MonoBehaviour
{
    protected abstract void billingSupportedEvent();

    protected abstract void billingNotSupportedEvent(string error);

    protected abstract void queryInventorySucceededEvent(List<PurchaseInfo> purchases, List<SkuInfo> skus);

    protected abstract void queryInventoryFailedEvent(string error);

    protected abstract void querySkuDetailsSucceededEvent(List<SkuInfo> skus);

    protected abstract void querySkuDetailsFailedEvent(string error);

    protected abstract void queryPurchasesSucceededEvent(List<PurchaseInfo> purchases);

    protected abstract void queryPurchasesFailedEvent(string error);

    protected abstract void purchaseSucceededEvent(PurchaseInfo purchase);

    protected abstract void purchaseFailedEvent(string error);

    protected abstract void consumePurchaseSucceededEvent(ConsumeType purchase);

    protected abstract void consumePurchaseFailedEvent(string error);
}
using MyketPlugin;
using UnityEngine;

public class MyketStore : MonoBehaviour, IStorePlatform
{
    private const string RsaKey = "";

    private MyketStore()
    { }

    void Awake()
    {
        Connect();
    }

    void OnDestroy()
    {
        Disconnect();
    }

    public void Connect()
    {
        MyketIAB.init(RsaKey);
        MyketIAB.enableLogging(Debug.isDebugBuild || Application.platform is RuntimePlatform.Android);
    }

    public void Disconnect()
    {
        MyketIAB.unbindService();
    }

    public void QueryInventory(string[] skus)
    {
        MyketIAB.queryInventory(skus);
    }

    public void QuerySkuDetails(string[] skus)
    {
        MyketIAB.querySkuDetails(skus);
    }

    public void QueryPurchases()
    {
        MyketIAB.queryPurchases();
    }

    public void PurchaseProduct(string sku, string developerPayload = "")
    {
        MyketIAB.purchaseProduct(sku, developerPayload);
    }

    public void ConsumeProduct(string sku)
    {
        MyketIAB.consumeProduct(sku);
    }

    public void ConsumeProducts(string[] skus)
    {
        MyketIAB.consumeProducts(skus);
    }
}
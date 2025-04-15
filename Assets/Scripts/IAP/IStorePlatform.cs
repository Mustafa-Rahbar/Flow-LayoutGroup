public interface IStorePlatform
{
    void Connect();

    void Disconnect();

    void QueryPurchases();

    void QuerySkuDetails(string[] skus);

    void QueryInventory(string[] skus);

    void PurchaseProduct(string sku, string developerPayload = "");

    void ConsumeProduct(string sku);

    void ConsumeProducts(string[] skus);
}
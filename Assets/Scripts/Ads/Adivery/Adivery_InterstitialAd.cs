using AdiveryUnity;
using UnityEngine;

public class Adivery_InterstitialAd : MonoBehaviour
{
    [SerializeField]
    private string PLACEMENT_ID = "38b301f2-5e0c-4776-b671-c6b04a612311";
    private AdiveryListener listener;

    private bool? m_Active = null;
    public bool active
    {
        get => m_Active ?? false;
        set
        {
            if (m_Active == value) return;

            m_Active = value;
            switch (value)
            {
                case true:
                    Adivery.PrepareInterstitialAd(PLACEMENT_ID);
                    listener = new AdiveryListener();
                    listener.OnError += OnError;
                    listener.OnInterstitialAdLoaded += OnInterstitialAdLoaded;

                    Adivery.AddListener(listener);
                    break;

                case false:
                    if (listener != null)
                    {
                        listener.OnError -= OnError;
                        listener.OnInterstitialAdLoaded -= OnInterstitialAdLoaded;

                        Adivery.RemoveListener(listener);
                        listener = null;
                    }
                    break;
            }
        }
    }

    private void Start()
    {
        active = true;
    }

    public void ShowAd()
    {
        if (listener == null) return;

        if (Adivery.IsLoaded(PLACEMENT_ID))
        {
            Adivery.Show(PLACEMENT_ID);
        }
    }

    private void OnInterstitialAdLoaded(object caller, string placementId)
    {
        // Interstitial ad loaded
    }

    private void OnError(object caller, AdiveryError error)
    {
        Debug.Log("placement: " + error.PlacementId + " error: " + error.Reason);
    }
}
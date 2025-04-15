using System;
using AdiveryUnity;
using UnityEngine;

public class Adivery_NativeAd : MonoBehaviour
{
    [SerializeField]
    private string PLACEMENT_ID = "ff454979-efaa-4ab8-b084-7db19e995d9b";
    private NativeAd native;

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
                    native = new NativeAd(PLACEMENT_ID);
                    native.OnAdLoaded += OnNativeAdLoaded;
                    native.OnAdClicked += OnNativeAdClicked;
                    native.OnAdShown += OnNativeAdShown;
                    native.OnError += OnNativeAdError;

                    native.LoadAd();
                    break;

                case false:
                    if (native != null)
                    {
                        native.OnAdLoaded -= OnNativeAdLoaded;
                        native.OnAdClicked -= OnNativeAdClicked;
                        native.OnAdShown -= OnNativeAdShown;
                        native.OnError -= OnNativeAdError;

                        native = null;
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
        if (native.IsLoaded())
        {
            Adivery.Show(PLACEMENT_ID);
        }
    }

    public void OnNativeAdLoaded(object caller, EventArgs args)
    {
        // Native ad loaded
        print("Native ad headline: " + native.GetHeadline());
    }

    private void OnNativeAdError(object sender, string error)
    {
        print($"Native ad error: " + error);
    }

    private void OnNativeAdShown(object sender, EventArgs args)
    {

    }

    private void OnNativeAdClicked(object sender, EventArgs args)
    {

    }
}
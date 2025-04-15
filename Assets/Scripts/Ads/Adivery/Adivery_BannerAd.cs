using System;
using AdiveryUnity;
using UnityEngine;

public class Adivery_BannerAd : MonoBehaviour
{
    [SerializeField]
    private string PLACEMENT_ID = "a355be22-970a-46b8-bc52-f0a59c4ded05";
    private BannerAd bannerAd;

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
                    bannerAd = new BannerAd(PLACEMENT_ID, BannerAd.TYPE_BANNER, BannerAd.POSITION_BOTTOM);
                    bannerAd.OnAdLoaded += OnBannerAdLoaded;
                    bannerAd.OnAdClicked += OnBannerAdClicked;
                    bannerAd.OnError += OnBannerAdError;

                    bannerAd.LoadAd();
                    break;

                case false:
                    if (bannerAd != null)
                    {
                        bannerAd.OnAdLoaded -= OnBannerAdLoaded;
                        bannerAd.OnAdClicked -= OnBannerAdClicked;
                        bannerAd.OnError -= OnBannerAdError;

                        bannerAd = null;
                    }
                    break;
            }
        }
    }

    private bool isShown = false;

    void Start()
    {
        active = true;
    }

    public void ShowAd()
    {
        if (!isShown && bannerAd.IsLoaded())
        {
            isShown = true;
            bannerAd.Show();
        }
    }

    public void HideAd()
    {
        if (isShown)
        {
            isShown = false;
            bannerAd.Hide();
        }
    }

    public void Destroy()
    {
        isShown = false;
        bannerAd.Destroy();
    }

    public void OnBannerAdLoaded(object caller, EventArgs args)
    {
    }

    private void OnBannerAdError(object sender, string error)
    {
        print($"Banner ad error: {error}");
    }

    private void OnBannerAdClicked(object sender, EventArgs args)
    {

    }
}
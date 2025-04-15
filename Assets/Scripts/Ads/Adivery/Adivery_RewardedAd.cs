using System;
using AdiveryUnity;
using UnityEngine;

public class Adivery_RewardedAd : MonoBehaviour
{
    [SerializeField]
    private string PLACEMENT_ID = "16414bae-368e-4904-b259-c5b89362206d";
    private AdiveryListener listener;

    public event Action<bool> OnAdClosed;

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
                    Adivery.PrepareRewardedAd(PLACEMENT_ID);

                    listener = new AdiveryListener();

                    listener.OnError += OnError;
                    listener.OnRewardedAdLoaded += OnRewardedLoaded;
                    listener.OnRewardedAdClosed += OnRewardedClosed;

                    Adivery.AddListener(listener);
                    break;

                case false:
                    if (listener != null)
                    {
                        listener.OnError -= OnError;
                        listener.OnRewardedAdLoaded -= OnRewardedLoaded;
                        listener.OnRewardedAdClosed -= OnRewardedClosed;

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

    private void OnRewardedLoaded(object caller, string placementId)
    {
        // Rewarded ad loaded
    }

    public void OnRewardedClosed(object caller, AdiveryReward reward)
    {
        // Check if User should receive the reward
        OnAdClosed?.Invoke(reward.IsRewarded);
    }

    private void OnError(object caller, AdiveryError error)
    {
        Debug.Log("placement: " + error.PlacementId + " error: " + error.Reason);
    }
}
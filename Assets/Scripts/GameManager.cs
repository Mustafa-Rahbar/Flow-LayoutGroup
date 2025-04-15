using System.Collections;
using System.Collections.Generic;
using AdiveryUnity;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    // Your adivery app id
    private const string APP_ID = "59c36ce3-7125-40a7-bd34-144e6906c796";

    void Awake()
    {
        if (!InitializeSingleton()) return;
    }

    void Start()
    {
        Adivery.Configure(APP_ID);
    }
}
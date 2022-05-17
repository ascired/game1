using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; private set; } // static singleton
    public Player Player;
    public float attackDistanceThreshold = 30f;

    public Dictionary<int, Vector3> portalLocations = new Dictionary<int, Vector3>();

    public ReactiveProperty<bool> IsPortalsReady { get; private set; } = new ReactiveProperty<bool>(false);

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        Player = FindObjectOfType<Player>();
    }

    void Start()
    {
        Portal[] portals = FindObjectsOfType<Portal>();
        Portal[] receivers = Array.FindAll(portals, p => p.IsReceiver);

        Debug.Log(portals.Length);
        Debug.Log(receivers.Length);
        this.UpdateAsObservable()
            .Select(_ => Array.FindAll(receivers, p => p.IsInitialized))
            .Select((Portal[] initialized) => initialized.Length == receivers.Length)
            .Where((bool isReady) => isReady)
            .Take(1)
            .Subscribe(_ => IsPortalsReady.Value = true)
            .AddTo(this);

    }

    public void nextLevel()
    {
        Player.Level++;
    }

}

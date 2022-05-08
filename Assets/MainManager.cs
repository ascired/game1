using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; private set; } // static singleton
    public Player player;
    public float attackDistanceThreshold = 30f;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        player = FindObjectOfType<Player>();

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; private set; } // static singleton
    public Player player;
    public float attackDistanceThreshold = 30f;

    public AudioMixer audioMixer;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        player = FindObjectOfType<Player>();

    }
}

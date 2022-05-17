using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; private set; } // static singleton
    public Player Player;
    public float attackDistanceThreshold = 30f;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        Player = FindObjectOfType<Player>();

    }

    public void nextLevel()
    {
        Player.level++;
    }

}

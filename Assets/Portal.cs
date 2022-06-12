using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private Player Player;
    public GameObject ExitPortal;
    public GameObject Effect;

    public bool IsReceiver = false;
    public int Level = 0;

    public bool IsInitialized = false;
    void Start()
    {
        if (IsReceiver & Level > 0)
        {
            MainManager.Instance.portalLocations.Add(Level, transform.position);
        }
        IsInitialized = true;

        Observable.Interval(TimeSpan.FromMilliseconds(2000))
            .Where(_ => gameObject.activeSelf)
            .Subscribe(_ => Instantiate(Effect, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity))
            .AddTo(this);

        if (!IsReceiver)
        {
            Player = MainManager.Instance.Player;
            NextMenu nextMenu = FindObjectOfType<NextMenu>();

            this.OnTriggerEnterAsObservable()
                .Where(_ => !IsReceiver)
                .Do(_ => Debug.Log("Enter trigger"))
                .Subscribe(_ => nextMenu.FinishPause(MoveToReceiver))
                .AddTo(this);
        }
       

    }

    public void MoveToReceiver()
    {
        Vector3 dest = ExitPortal.transform.position;
        Player.agent.Warp(dest);

        PlayerPrefs.SetInt("currentHealth", Player.Health);
        PlayerPrefs.SetInt("currentArmor", Player.Armor);
        PlayerPrefs.SetInt("currentLevel", Player.Level);
    }

}

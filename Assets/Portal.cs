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

    void Start()
    {

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
                .Subscribe(_ => nextMenu.FinishPause(MoveToReceiver))
                .AddTo(this);
        }
        else
        {
            this.OnTriggerExitAsObservable()
                .Subscribe(_ => gameObject.SetActive(false))
                .AddTo(this);
        }

    }

    public void MoveToReceiver()
    {
        Vector3 dest = ExitPortal.transform.position;
        Player.moveToDest(dest);
        Player.transform.position = dest;

        PlayerPrefs.SetInt("currentHealth", Player.Health);
        PlayerPrefs.SetInt("currentArmor", Player.Armor);
        PlayerPrefs.SetInt("currentLevel", Player.level);
    }

}

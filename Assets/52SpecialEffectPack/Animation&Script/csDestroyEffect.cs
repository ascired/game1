using System;
using UniRx;
using UnityEngine;

public class csDestroyEffect : MonoBehaviour {

    void Start()
    {
        Observable.Interval(TimeSpan.FromMilliseconds(3000))
            .Subscribe(_ => Destroy(gameObject))
            .AddTo(this);
    }

}

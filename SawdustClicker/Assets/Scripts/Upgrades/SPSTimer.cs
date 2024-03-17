using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPSTimer : MonoBehaviour
{
    public float TimerDuration = 1f;

    public double SawdustPerSecond { get; set; }

    private float _counter;

    private void Update()
    {
        _counter += Time.deltaTime;
        if (_counter >= TimerDuration)
        {
            SawdustManager.Instance.SimpleSawdustIncrease(SawdustPerSecond);
            _counter = 0;
        }
    }
}

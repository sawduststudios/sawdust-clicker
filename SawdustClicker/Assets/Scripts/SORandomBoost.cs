using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BoostSpawn", menuName = "SawdustClicker/BoostSpawn", order = 3)]
public class SORandomBoost : ScriptableObject
{
    public Sprite Sprite;
    public double CliclMultiplier;
    public double SPSMultiplier;
    public float Duration;
    public AudioClip Sound;

    public void ApplyBoost()
    {
        SawdustManager.Instance.PerSecondIncreaseFor(SPSMultiplier, Duration);
        SawdustManager.Instance.ClickIncreaseFor(CliclMultiplier, Duration);
    }
}

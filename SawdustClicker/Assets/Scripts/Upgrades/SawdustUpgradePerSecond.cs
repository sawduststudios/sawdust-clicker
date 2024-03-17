using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sawdust Upgrade/Sawdust Per Second", fileName = " SawdustPerSecond")]
public class SawdustUpgradePerSecond : SawdustUpgrade
{
    public override void ApplyUpgrade()
    {
        GameObject go = Instantiate(SawdustManager.Instance.SPSObjToSpawn, Vector3.zero, Quaternion.identity);
        go.GetComponent<SPSTimer>().SawdustPerSecond = UpgradeAmmount;

        SawdustManager.Instance.SimpleSPSIncrease(UpgradeAmmount);
    }
}

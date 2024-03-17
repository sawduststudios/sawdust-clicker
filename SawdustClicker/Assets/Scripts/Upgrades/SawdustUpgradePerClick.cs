using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sawdust Upgrade/Sawdust Per Click", fileName = " SawdustPerClick")]
public class SawdustUpgradePerClick : SawdustUpgrade
{
    public override void ApplyUpgrade()
    {
        SawdustManager.Instance.SawdustPerClickUpgrade += UpgradeAmmount;
    }
}

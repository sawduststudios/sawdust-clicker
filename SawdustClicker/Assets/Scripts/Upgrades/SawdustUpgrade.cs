using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SawdustUpgrade : ScriptableObject
{
    public string Name;
    [TextArea(3, 10)]
    public string Description;
    public int TimesPurchased = 1;

    public float UpgradeAmmount;

    public double OriginalUpdgradeCost = 100;
    public double CurrentUpgradeCost = 100;
    public double CostIncreasePerPurchase = 0.05f;

    

    public abstract void ApplyUpgrade();

    private void OnValidate()
    {
        CurrentUpgradeCost = OriginalUpdgradeCost;
    }
}

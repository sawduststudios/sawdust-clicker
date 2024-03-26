using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "SawdustClicker/Building", order = 1)]
public class BuildingUpgrade : ScriptableObject
{
    [Tooltip("The name displayed to the user")]
    public string Name;
    [Tooltip("Used in code to refference this")]
    public string InternalName;
    [Tooltip("The description displayed to the user")]
    [TextArea(3, 10)]
    public string Description;

    [HideInInspector]
    public int TimesPurchased = 0;

    [Tooltip("The ammount of sawdust per second one building generates (without multipliers)")]
    public float GainPerSecond;
    [HideInInspector]
    public float GainMultiplier = 1;

    [Tooltip("The original cost of the building")]
    public double OriginalUpdgradeCost = 100;
    [Tooltip("The ammount the cost increases per purchase in percent")]
    public double CostIncreasePerPurchase = 0.15f;

    [HideInInspector]
    public double CurrentUpgradeCost = 100;

    public double TotalSPS
    {
        get
        {
            return GainPerSecond * GainMultiplier * TimesPurchased;
        }
    }    

    public void BuildingPurchased(int ammount = 1)
    {
        for (int i = 0; i < ammount; i++)
        {
            TimesPurchased++;
            CurrentUpgradeCost *= 1 + CostIncreasePerPurchase;
            CurrentUpgradeCost = System.Math.Round(CurrentUpgradeCost, 2);
        }
    }
}

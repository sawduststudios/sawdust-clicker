using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClickUpgrade", menuName = "SawdustClicker/ClickUpgrade", order = 2)]
public class ClickUpgrade : ScriptableObject
{
    [Tooltip("The name displayed to the user")]
    public string Name;
    [Tooltip("The description displayed to the user")]
    [TextArea(3, 10)]
    public string Description;

    public bool Purchased = false;

    [Tooltip("The cost of the upgrade")]
    public double PurchaseCost = 69;
    [Tooltip("Click gains this percent of SPS")]
    public double ClickSPSPercentage = 0.01;

    public bool _isClick = true;

    public string TargetName;
    public float TargetMultiplier = 1;

    public void PurchaseUpgrade()
    {        
        if (_isClick) 
        {
            SawdustManager.Instance.ClickPercentFromSPS += ClickSPSPercentage;        
        }
        else
        {
            SawdustManager.Instance.AddBuildingMultiplier(TargetName, TargetMultiplier);
        }
        Purchased = true;
    }

    public void ResetUpgrade()
    {
        Purchased = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonRefferences : MonoBehaviour
{
    public Button UpgradeButton;
    public TextMeshProUGUI UpgradeButtonText;
    public TextMeshProUGUI UpgradeNameText;
    public TextMeshProUGUI UpgradeDescriptionText;

    private BuildingUpgrade _building;
    public BuildingUpgrade Building { get => _building; }

    public void AssignBuilding(BuildingUpgrade building)
    {
        _building = building;
    }

    public void UpdateBuildingUI()
    {
        UpgradeNameText.text = _building.TimesPurchased.ToString() + " " + _building.Name;
        UpgradeDescriptionText.text = _building.Description;

        UpgradeButtonText.text = _building.CurrentUpgradeCost.ToFormattedStr();
    }

    public void ShowBuildingInfo()
    {
        SawdustManager.Instance.ShowInfoBox(ToNiceString());
    }

    public string ToNiceString()
    {
        string result = "";
        result += _building.Name + "\n";
        result += "One generates: " + _building.GainPerSecond.ToFormattedStr() + " per second\n";
        result += "Times purchased: " + _building.TimesPurchased + "\n";
        result += "Current multiplier: " + _building.GainMultiplier + "\n";
        result += "\n";
        result += "Total SPS: " + _building.TotalSPS.ToFormattedStr() + "\n";
        result += "Making " + (100 * _building.TotalSPS/SawdustManager.Instance.SawdustPerSec).ToString("F2") + "% of your SPS";

        return result;
    }
}

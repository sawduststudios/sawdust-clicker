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

        UpgradeButtonText.text = _building.CurrentUpgradeCost.ToString();
    }
}

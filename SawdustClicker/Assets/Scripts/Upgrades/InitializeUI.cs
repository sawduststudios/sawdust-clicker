using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeUI : MonoBehaviour
{
    public void InitBuildingsUI(BuildingUpgrade[] buildings, GameObject buildingUIPrefab, Transform UIParent)
    {
        // Remove all children of UIParent
        foreach (Transform child in UIParent)
        {
            Destroy(child.gameObject);
        }

        foreach (BuildingUpgrade building in buildings)
        {

            GameObject upgradeUI = Instantiate(buildingUIPrefab, UIParent);
            
            // reset cost
            //building.CurrentUpgradeCost = building.CurrentUpgradeCost;

            // Asiign building to button
            UpgradeButtonRefferences buttonRef = upgradeUI.GetComponent<UpgradeButtonRefferences>();
            buttonRef.AssignBuilding(building);

            // update UI
            buttonRef.UpdateBuildingUI();

            // set OnClick
            buttonRef.UpgradeButton.onClick.AddListener(() => SawdustManager.Instance.OnUpgradeButtonClick(buttonRef));
        }
    }
}

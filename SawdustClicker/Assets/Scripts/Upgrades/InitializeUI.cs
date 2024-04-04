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
            BuildingUIRefferences buttonRef = upgradeUI.GetComponent<BuildingUIRefferences>();
            buttonRef.AssignBuilding(building);
            buttonRef.IconImage.sprite = SpriteProvider.Instance.GetSprite(building.Name);

            // update UI
            buttonRef.UpdateBuildingUI();

            // set OnClick
            buttonRef.BuyButton.onClick.AddListener(() => SawdustManager.Instance.OnBuildingPurchaseClick(buttonRef));
        }
    }

    public void InitUpgradesUI(ClickUpgrade[] upgrades, GameObject upgradeUIPrefab, Transform UIParent)
    {
        // Remove all children of UIParent
        foreach (Transform child in UIParent)
        {
            Destroy(child.gameObject);
        }
        foreach (ClickUpgrade upgrade in upgrades)
        {
            if (upgrade.Purchased)
                continue;

            GameObject upgradeUI = Instantiate(upgradeUIPrefab, UIParent);
            // Asiign building to button
            UpgradeUIRefferences buttonRef = upgradeUI.GetComponent<UpgradeUIRefferences>();
            buttonRef.AssignUpgrade(upgrade);
            // update UI
            buttonRef.UpdateUpgradeUI();
            buttonRef.IconImage.sprite = SpriteProvider.Instance.GetSprite(upgrade.Name);
            // set OnClick
            buttonRef.BuyButton.onClick.AddListener(() => SawdustManager.Instance.OnUpgradePurchaseClick(buttonRef));
        }
    }
}

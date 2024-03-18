using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeUpgrades : MonoBehaviour
{
    public void InitializeGivenUpgrades(SawdustUpgrade[] upgrades, GameObject upgradeUIPrefab, Transform upgradeUIParent)
    {
        foreach (SawdustUpgrade upgrade in upgrades)
        {
            GameObject upgradeUI = Instantiate(upgradeUIPrefab, upgradeUIParent);
            
            // reset cost
            upgrade.CurrentUpgradeCost = upgrade.OriginalUpdgradeCost;

            // set text
            UpgradeButtonRefferences buttonRef = upgradeUI.GetComponent<UpgradeButtonRefferences>();
            buttonRef.UpgradeNameText.text = upgrade.TimesPurchased.ToString() + " " + upgrade.Name;
            buttonRef.UpgradeDescriptionText.text = upgrade.Description;

            buttonRef.UpgradeButtonText.text = upgrade.CurrentUpgradeCost.ToString();

            // set OnClick
            buttonRef.UpgradeButton.onClick.AddListener(() => SawdustManager.Instance.OnUpgradeButtonClick(upgrade, buttonRef));
        }
    }
}

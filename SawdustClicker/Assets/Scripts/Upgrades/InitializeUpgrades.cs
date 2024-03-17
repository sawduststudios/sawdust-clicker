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
            buttonRef.UpgradeButtonText.text = upgrade.UpgradeButtonText;
            buttonRef.UpgradeDescriptionText.SetText(upgrade.UpgradeButtonDescription, upgrade.UpgradeAmmount);
            buttonRef.UpgradeCostText.text = "Cost: " + upgrade.CurrentUpgradeCost.ToString();

            // set OnClick
            buttonRef.UpgradeButton.onClick.AddListener(() => SawdustManager.Instance.OnUpgradeButtonClick(upgrade, buttonRef));
        }
    }
}

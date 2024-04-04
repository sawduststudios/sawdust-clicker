using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIRefferences : MonoBehaviour
{
    public Button BuyButton;
    public TextMeshProUGUI BuyButtonText;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;
    public Image IconImage;

    private ClickUpgrade _upgrade;
    public ClickUpgrade Upgrade { get => _upgrade; }

    public void AssignUpgrade(ClickUpgrade upgrade)
    {
        _upgrade = upgrade;
    }

    public void UpdateUpgradeUI()
    {
        NameText.text = _upgrade.Name;
        DescriptionText.text = _upgrade.Description;

        BuyButtonText.text = _upgrade.PurchaseCost.ToFormattedStr();
    }

    public void ShowUpgradeInfo()
    {
        SawdustManager.Instance.ShowInfoBox(ToNiceString());
    }

    public string ToNiceString()
    {
        string result = "";
        result += _upgrade.Name + "\n\n";

        if (_upgrade._isClick)
        {
            result += "Buying this improves your SAWS! (=clicks)\n";
            result += "A click gains " + (_upgrade.ClickSPSPercentage * 100).ToString("F2") + "% of your SPS\n";
        }
        else
        {
            result += "Buying this multiplies the production of " + _upgrade.TargetName + " by x" + _upgrade.TargetMultiplier + "\n";
        }

        return result;
    }
}

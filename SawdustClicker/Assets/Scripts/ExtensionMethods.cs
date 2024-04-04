using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static string ToFormattedStr(this double number)
    {
        string[] suffixes = new string[] { "", "k", "M", "B", "T", "Q", "QQ", "S", "SS", "O", "N", "D", "UN", "DD", "TR", "QT", "QN", "SD", "SPD", "OD", "ND", "VG" };
        int suffixIndex = 0;
        while (number >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            number /= 1000;
            suffixIndex++;

            if (suffixIndex >= suffixes.Length - 1 && number >= 1000)
            {
                break;
            }
        }

        if (suffixIndex == 0)
            return number.ToString("F0");

        return number.ToString("F1") + suffixes[suffixIndex];
    }

    public static string ToFormattedStr(this float number)
    {
        string[] suffixes = new string[] { "", "k", "M", "B", "T", "Q", "QQ", "S", "SS", "O", "N", "D", "UN", "DD", "TR", "QT", "QN", "SD", "SPD", "OD", "ND", "VG" };
        int suffixIndex = 0;
        while (number >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            number /= 1000;
            suffixIndex++;

            if (suffixIndex >= suffixes.Length - 1 && number >= 1000)
            {
                break;
            }
        }

        if (suffixIndex == 0)
            return number.ToString("F0");

        return number.ToString("F1") + suffixes[suffixIndex];
    }

    public static string Serialize(this BuildingUpgrade[] buildings)
    {
        List<BuildingData> list = new List<BuildingData>();
        foreach (BuildingUpgrade building in buildings) 
        {
            BuildingData buildingData = new BuildingData();

            buildingData.Name = building.Name;
            buildingData.InternalName = building.InternalName;
            buildingData.Description = building.Description;
            buildingData.TimesPurchased = building.TimesPurchased;
            buildingData.GainPerSecond = building.GainPerSecond;
            buildingData.GainMultiplier = building.GainMultiplier;
            buildingData.OriginalUpdgradeCost = building.OriginalUpdgradeCost;
            buildingData.CostIncreasePerPurchase = building.CostIncreasePerPurchase;
            buildingData.CurrentUpgradeCost = building.CurrentUpgradeCost;

            list.Add(buildingData);
        }
        return JsonUtility.ToJson(new BuildingDataArray() { Buildings = list.ToArray() }, prettyPrint: true);
    }

    public static BuildingUpgrade[] DeserializeToBuildingUpgradeArr (this string buildingsStr)
    {
        BuildingDataArray buildingDataArray = JsonUtility.FromJson<BuildingDataArray>(buildingsStr);
        List<BuildingUpgrade> buildingList = new List<BuildingUpgrade>();
        foreach (BuildingData buildingData in buildingDataArray.Buildings)
        {
            BuildingUpgrade building = ScriptableObject.CreateInstance<BuildingUpgrade>();

            building.Name = buildingData.Name;
            building.InternalName = buildingData.InternalName;
            building.Description = buildingData.Description;
            building.TimesPurchased = buildingData.TimesPurchased;
            building.GainPerSecond = buildingData.GainPerSecond;
            building.GainMultiplier = buildingData.GainMultiplier;
            building.OriginalUpdgradeCost = buildingData.OriginalUpdgradeCost;
            building.CostIncreasePerPurchase = buildingData.CostIncreasePerPurchase;
            building.CurrentUpgradeCost = buildingData.CurrentUpgradeCost;

            buildingList.Add(building);
        }
        return buildingList.ToArray();
    }

    public static void LoadBuildings(string buildingsStr)
    {
        BuildingData[] buildingsData = JsonUtility.FromJson<BuildingDataArray>(buildingsStr).Buildings;

        if (buildingsData.Length != SawdustManager.Instance.Buildings.Length)
        {
            Debug.LogError("Building data length does not match GameManager.Buildings length");
            return;
        }

        for (int i = 0; i < buildingsData.Length; i++)
        {
            BuildingUpgrade building = SawdustManager.Instance.Buildings[i];
            building.Name = buildingsData[i].Name;
            building.InternalName = buildingsData[i].InternalName;
            building.Description = buildingsData[i].Description;
            building.TimesPurchased = buildingsData[i].TimesPurchased;
            building.GainPerSecond = buildingsData[i].GainPerSecond;
            building.GainMultiplier = buildingsData[i].GainMultiplier;
            building.OriginalUpdgradeCost = buildingsData[i].OriginalUpdgradeCost;
            building.CostIncreasePerPurchase = buildingsData[i].CostIncreasePerPurchase;
            building.CurrentUpgradeCost = buildingsData[i].CurrentUpgradeCost;
        }
    }



    public static string Serialize(this ClickUpgrade[] clickUpgrades)
    {
        List<ClickUpgradeData> list = new List<ClickUpgradeData>();
        foreach (ClickUpgrade upgrade in clickUpgrades)
        {
            ClickUpgradeData upgradeData = new ClickUpgradeData();

            upgradeData.Name = upgrade.Name;
            upgradeData.Description = upgrade.Description;
            upgradeData.Purchased = upgrade.Purchased;
            upgradeData.PurchaseCost = upgrade.PurchaseCost;
            upgradeData.ClickSPSPercentage = upgrade.ClickSPSPercentage;

            list.Add(upgradeData);            
        }
        return JsonUtility.ToJson(new ClickUpgradeDataArray() { ClickUpgrades = list.ToArray() }, prettyPrint: true);
    }

    public static void LoadClickUpgrades(string clickUpgradesStr)
    {
        ClickUpgradeData[] clickUpgradesData = JsonUtility.FromJson<ClickUpgradeDataArray>(clickUpgradesStr).ClickUpgrades;
        if (clickUpgradesData.Length != SawdustManager.Instance.ClickUpgrades.Length)
        {
            Debug.LogError("Click upgrade data length does not match GameManager.ClickUpgrades length");
            return;
        }
        for (int i = 0; i < clickUpgradesData.Length; i++)
        {
            ClickUpgrade upgrade = SawdustManager.Instance.ClickUpgrades[i];
            upgrade.Name = clickUpgradesData[i].Name;
            upgrade.Description = clickUpgradesData[i].Description;
            upgrade.Purchased = clickUpgradesData[i].Purchased;
            upgrade.PurchaseCost = clickUpgradesData[i].PurchaseCost;
            upgrade.ClickSPSPercentage = clickUpgradesData[i].ClickSPSPercentage;
        }
    }

}

// Data classes for serialization
[System.Serializable]
public class BuildingData
{
    public string Name;
    public string InternalName;
    public string Description;
    public int TimesPurchased = 0;
    public float GainPerSecond;
    public float GainMultiplier = 1;
    public double OriginalUpdgradeCost = 7;
    public double CostIncreasePerPurchase = 0.15f;
    public double CurrentUpgradeCost = 7;
}

[System.Serializable]
public class BuildingDataArray
{
    public BuildingData[] Buildings;
}


[System.Serializable]
public class ClickUpgradeData
{
    public string Name;
    public string Description;
    public bool Purchased;
    public double PurchaseCost;
    public double ClickSPSPercentage = 0.069;
}

[System.Serializable]
public class ClickUpgradeDataArray
{
    public ClickUpgradeData[] ClickUpgrades;
}
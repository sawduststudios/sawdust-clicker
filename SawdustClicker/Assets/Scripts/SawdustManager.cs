using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using DG;
using DG.Tweening;

public class SawdustManager : MonoBehaviour
{
    public static SawdustManager Instance;

    public GameObject MainCanvas;
    [SerializeField] private GameObject _upgradeCanvas;
    [SerializeField] private TextMeshProUGUI _sawdustCountText;
    [SerializeField] private TextMeshProUGUI _spsText;
    [SerializeField] private GameObject _sawdustTrunkObj;
    public GameObject SawdustTextPopUp;
    [SerializeField] private GameObject _backgroundObj;

    [Space]
    public SawdustUpgrade[] Upgrades;
    [SerializeField] private GameObject _upgradeUIToSpawn;
    [SerializeField] private Transform _upgradeUIParent;
    public GameObject SPSObjToSpawn;

    public double CurrentSawdustCount { get; set; }
    public double SPS { get; set; }

    // upgrades
    public double SawdustPerClickUpgrade { get; set; }

    private InitializeUpgrades _initializeUpgrades;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("SawdustManager instance already exists");
        }

        UpdateUI();

        _upgradeCanvas.SetActive(false);
        MainCanvas.SetActive(true);

        SawdustPerClickUpgrade = 1;

        _initializeUpgrades = GetComponent<InitializeUpgrades>();
        _initializeUpgrades.InitializeGivenUpgrades(Upgrades, _upgradeUIToSpawn, _upgradeUIParent);
    }

    #region UI Updates

    public void UpdateUI()
    {
        UpdateSawdustCountUI();
        UpdateSPSUI();
    }

    private void UpdateSawdustCountUI()
    {
        _sawdustCountText.text = CurrentSawdustCount.ToString() + " Sawdust";
    }
    private void UpdateSPSUI()
    {
        _spsText.text = SPS.ToString() + " SPS";
    }

    #endregion

    #region On Trunk Click
    
    public void OnTrunkClicked()
    {
        IncreaseSawdust();
    }

    private void IncreaseSawdust()
    {
        CurrentSawdustCount += SawdustPerClickUpgrade;
        UpdateUI();

        _sawdustTrunkObj.transform.DOBlendableScaleBy(new Vector3(0.05f, 0.05f, 0.05f), 0.05f).OnComplete(TrunkScaleBack);
        _backgroundObj.transform.DOBlendableScaleBy(new Vector3(0.03f, 0.03f, 0.03f), 0.08f).SetDelay(0.03f).OnComplete(BGScaleBack);
    }

    private void TrunkScaleBack()
    {
        _sawdustTrunkObj.transform.DOBlendableScaleBy(new Vector3(-0.05f, -0.05f, -0.05f), 0.05f);
    }
    private void BGScaleBack()
    {
        _backgroundObj.transform.DOBlendableScaleBy(new Vector3(-0.03f, -0.03f, -0.03f), 0.08f);
    }

    #endregion

    #region Button Presses

    public void OnUpgradeButtonPressed()
    {
        _upgradeCanvas.SetActive(true);
        MainCanvas.SetActive(false);
    }

    public void OnMainButtonPressed()
    {
        _upgradeCanvas.SetActive(false);
        MainCanvas.SetActive(true);
    }

    #endregion

    #region Direct Increases

    public void SimpleSawdustIncrease(double ammount)
    {
        CurrentSawdustCount += ammount;
        UpdateUI();
    }

    public void SimpleSPSIncrease(double ammount) 
    {
        CurrentSawdustCount += ammount;
        UpdateUI();
    }

    #endregion

    #region Events

    public void OnUpgradeButtonClick(SawdustUpgrade upgrade, UpgradeButtonRefferences buttonRefs)
    {
        if (CurrentSawdustCount > upgrade.CurrentUpgradeCost) 
        {
            upgrade.ApplyUpgrade();

            CurrentSawdustCount -= upgrade.CurrentUpgradeCost;
            UpdateUI();

            upgrade.CurrentUpgradeCost *= (1 + upgrade.CostIncreasePerPurchase);
            upgrade.CurrentUpgradeCost = System.Math.Round(upgrade.CurrentUpgradeCost, 2);

            buttonRefs.UpgradeButtonText.text = upgrade.CurrentUpgradeCost.ToString();
            
            upgrade.TimesPurchased++;
            buttonRefs.UpgradeNameText.text = upgrade.TimesPurchased.ToString() + " " + upgrade.Name;
        }
    }

    #endregion
}

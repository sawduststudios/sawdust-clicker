using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using DG;
using DG.Tweening;
using Unity.VisualScripting;

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
    public BuildingUpgrade[] Buildings;
    [SerializeField] private GameObject _upgradeUIToSpawn;
    [SerializeField] private Transform _upgradeUIParent;

    [HideInInspector]
    public double CurrentSawdustCount { get; set; }

    [HideInInspector]
    public double PerSecMultiplier = 1;
    [HideInInspector]
    public double SawdustPerSec {
        get 
        {
            double sps = 0;
            foreach (var building in Buildings)
            {
                sps += building.TotalSPS;
            }
            return sps * PerSecMultiplier;
        }
    }

    [Header("Per Click")]
    [HideInInspector]
    [Tooltip("The base ammount of sawdust per click")]
    public double PerClickBase = 1;
    [HideInInspector]
    [Tooltip("Multiplies per click")]
    public double PerClickMultiplier = 1;
    [HideInInspector]
    [Tooltip("Add SawdustPerSec% to each click (before multiplier gets applied)")]
    public double ClickPercentFromSPS { get; set; }
    [HideInInspector]
    public double PerClickAmmount {
        get
        {
            return (PerClickBase + SawdustPerSec * ClickPercentFromSPS) * PerClickMultiplier;
        }
    }

    private InitializeUI _initializeUI;

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

        _initializeUI = GetComponent<InitializeUI>();
        _initializeUI.InitBuildingsUI(Buildings, _upgradeUIToSpawn, _upgradeUIParent);
    }

    private void Start()
    {
        StartCoroutine(AddSPSCoroutine());
    }

    IEnumerator AddSPSCoroutine(float updatesPerSec = 5)
    {
        while (true)
        {
            CurrentSawdustCount += SawdustPerSec / updatesPerSec;
            UpdateUI();
            yield return new WaitForSeconds(1f / updatesPerSec);
        }
    }

    #region UI Updates

    public void UpdateUI()
    {
        UpdateSawdustCountUI();
        UpdateSPSUI();
    }

    private void UpdateSawdustCountUI()
    {
        _sawdustCountText.text = CurrentSawdustCount.ToString("F0") + " Sawdust";
    }

    private void UpdateSPSUI()
    {
        _spsText.text = SawdustPerSec.ToString("F0") + " SawdustPerSec";
    }


    #endregion

    #region On Trunk Click

    public void OnTrunkClicked()
    {
        IncreaseSawdust();
    }

    private void IncreaseSawdust()
    {
        CurrentSawdustCount += PerClickAmmount;
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

    #region Screen Switching

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

    #endregion

    #region Increase for duration

    public void PerSecondIncreaseFor(double multiplier, float duration)
    {
        StartCoroutine(IncreasePerSecFor(multiplier, duration));
    }

    IEnumerator IncreasePerSecFor(double multiplier, float duration)
    {
        Debug.Log("Increasing PerSec for " + duration + " seconds by x" + multiplier);
        PerSecMultiplier *= multiplier;
        UpdateUI();
        yield return new WaitForSeconds(duration);
        PerSecMultiplier /= multiplier;
        UpdateUI();
        Debug.Log("PerSec multiplier x" + multiplier + " ran out!");
    }

    #endregion

    #region Events

    public void OnUpgradeButtonClick(UpgradeButtonRefferences buttonRefs)
    {
        BuildingUpgrade building = buttonRefs.Building;
        if (CurrentSawdustCount > building.CurrentUpgradeCost) 
        {
            Debug.Log($"{building.Name} purchased!!");
            building.BuildingPurchased(1);

            // take away sawdust
            CurrentSawdustCount -= building.CurrentUpgradeCost;
            UpdateUI();

            buttonRefs.UpdateBuildingUI();
        }
        else
        {
            Debug.Log("Not enough sawdust!");
        }
    }

    #endregion
}

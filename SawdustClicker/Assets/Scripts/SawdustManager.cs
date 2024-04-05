using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using DG;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.SceneManagement;
using System;

using System.Linq;

public class SawdustManager : MonoBehaviour
{
    public static SawdustManager Instance;

    public float PopUpVelocity = 750f;

    public float TotalTime = 900f;
    private bool _shouldReset = false;
    private bool _didTimeRunOut = false;

    // GO Refferences
    public GameObject MainCanvas;
    [SerializeField] private GameObject _buildingsCanvas;
    [SerializeField] private GameObject _buildingsScreenButton;
    [SerializeField] private GameObject _upgradesCanvas;
    [SerializeField] private GameObject _upgradesScreenButton;

    [SerializeField] private TextMeshProUGUI _sawdustCountText;
    [SerializeField] private TextMeshProUGUI _spsText;
    [SerializeField] private GameObject _sawdustTrunkObj;
    public GameObject SawdustTextPopUp;
    [SerializeField] private GameObject _backgroundObj;

    // Info Box
    [SerializeField] private GameObject _infoBox;
    [SerializeField] private TextMeshProUGUI _infoBoxText;

    [SerializeField] private GameObject _restartBox;
    [SerializeField] private TextMeshProUGUI _restartBoxText;


    [Space]
    public BuildingUpgrade[] Buildings; // All buildings in the game
    public ClickUpgrade[] ClickUpgrades; // All click upgrades in the game
    // GO Refferences
    [SerializeField] private GameObject _buildingUIToSpawn;
    [SerializeField] private Transform _buildingUIParent;

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

        _buildingsCanvas.SetActive(false);
        _upgradesCanvas.SetActive(false);
        _restartBox.SetActive(false);
        MainCanvas.SetActive(true);

        _initializeUI = GetComponent<InitializeUI>();
    }

    private void Start()
    {
        LoadGame();

        UpdateUI();
        //_initializeUI.InitBuildingsUI(Buildings, _buildingUIToSpawn, _buildingUIParent);

        StartCoroutine(AddSPSCoroutine());
    }

    private void OnDestroy()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGame();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SimpleSawdustIncrease(10000);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGame();
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SaveGame();
                Application.Quit();
            }
        }

        TotalTime -= Time.deltaTime;
        if (TotalTime <= 0)
        {
            TimeRanOut();
        }
    }

    private void TimeRanOut()
    {
        if (_didTimeRunOut) return;
        Debug.Log("TimeRanOut");
        Time.timeScale = 0;
        _shouldReset = true;

        _restartBox.SetActive(true);
        string text = "";
        text += "The time ran out!\n\n";
        text += "Final SPS: " + SawdustPerSec.ToFormattedStr() + "\n\n";
        text += "Write your result onto the leaderboard!";
        _restartBoxText.text = text;
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

    #region Save/Load

    private bool _shouldSave = true;

    #region Saving Keys
    private const string saveKey_TotalTime = "TotalTime";
    private const string saveKey_CurrentSawdustCount = "CurrentSawdustCount";
    private const string saveKey_PerSecMultiplier = "PerSecMultiplier";
    private const string saveKey_PerClickBase = "PerClickBase";
    private const string saveKey_PerClickMultiplier = "PerClickMultiplier";
    private const string saveKey_ClickPercentFromSPS = "ClickPercentFromSPS";
    private const string saveFileName_Buildings = "buildings.json";
    private const string saveFileName_ClickUpgrades = "click_upgrades.json";
    #endregion
    public void SaveGame()
    {
        if (_shouldReset) 
        {
            ResetGame();
        }
        if (!_shouldSave)
        {
            Debug.LogWarning("Saving disabled...");
            return;
        }
        Debug.Log("SAVING GAME");

        // Saving Manager state
        PlayerPrefs.SetFloat(saveKey_TotalTime, TotalTime);
        PlayerPrefs.SetString(saveKey_CurrentSawdustCount, CurrentSawdustCount.ToString());
        PlayerPrefs.SetString(saveKey_PerSecMultiplier, PerSecMultiplier.ToString());
        PlayerPrefs.SetString(saveKey_PerClickBase, PerClickBase.ToString());
        PlayerPrefs.SetString(saveKey_PerClickMultiplier, PerClickMultiplier.ToString());
        PlayerPrefs.SetString(saveKey_ClickPercentFromSPS, ClickPercentFromSPS.ToString());

        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs saved");


        // Saving Buildings
        string buildingsJson = Buildings.Serialize();
        Debug.Log("Saving buildings:" + buildingsJson);
        SaveToFile(saveFileName_Buildings, buildingsJson);

        // Saving ClickUpgrades
        string clickUpgradesJson = ClickUpgrades.Serialize();
        Debug.Log("Saving click upgrades:" + clickUpgradesJson);
        SaveToFile(saveFileName_ClickUpgrades, clickUpgradesJson);
    }

    public void LoadGame()
    {
        Debug.Log("LOADING GAME");

        if (PlayerPrefs.HasKey(saveKey_TotalTime))
            TotalTime = PlayerPrefs.GetFloat(saveKey_TotalTime);
        if (PlayerPrefs.HasKey(saveKey_CurrentSawdustCount))
            CurrentSawdustCount = double.Parse(PlayerPrefs.GetString(saveKey_CurrentSawdustCount));
        if (PlayerPrefs.HasKey(saveKey_PerSecMultiplier))
            PerSecMultiplier = double.Parse(PlayerPrefs.GetString(saveKey_PerSecMultiplier));
        if (PlayerPrefs.HasKey(saveKey_PerClickBase))
            PerClickBase = double.Parse(PlayerPrefs.GetString(saveKey_PerClickBase));
        if (PlayerPrefs.HasKey(saveKey_PerClickMultiplier))
            PerClickMultiplier = double.Parse(PlayerPrefs.GetString(saveKey_PerClickMultiplier));
        if (PlayerPrefs.HasKey(saveKey_ClickPercentFromSPS))
            ClickPercentFromSPS = double.Parse(PlayerPrefs.GetString(saveKey_ClickPercentFromSPS));
        
        // LOAD BUILDINGS

        // if buildings file exists, load it
        if (File.Exists(Path.Combine(Application.persistentDataPath, saveFileName_Buildings)))
        {
            //string buildingsJson = PlayerPrefs.GetString(saveKey_Buildings);
            //Buildings = buildingsJson.DeserializeToBuildingUpgradeArr();

            string buildingsJson = LoadFromFile(saveFileName_Buildings);
            Debug.Log("Loaded buildings: " + buildingsJson);
            ExtensionMethods.LoadBuildings(buildingsJson);

            double sps = 0;
            foreach (var building in Buildings)
            {
                sps += building.TotalSPS;
            }
            sps = sps * PerSecMultiplier;
            Debug.Log("Sps computed as " + sps);
        }


        // LOAD CLICK UPGRADES

        // if click upgrades file exists, load it
        if (File.Exists(Path.Combine(Application.persistentDataPath, saveFileName_ClickUpgrades)))
        {
            string clickUpgradesJson = LoadFromFile(saveFileName_ClickUpgrades);
            Debug.Log("Loaded click upgrades: " + clickUpgradesJson);
            ExtensionMethods.LoadClickUpgrades(clickUpgradesJson);
        }

        UpdateUI();
        _initializeUI.InitBuildingsUI(Buildings, _buildingUIToSpawn, _buildingUIParent);
        _initializeUI.InitUpgradesUI(ClickUpgrades, _upgradeUIToSpawn, _upgradeUIParent);
    }

    private void SaveToFile(string fileName, string data)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(filePath, data);
        Debug.Log("File saved to " + filePath);
    }

    private string LoadFromFile(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        else
        {
            return null;
        }
    }

    public void ResetGame()
    {
        _shouldReset = false;
        Time.timeScale = 1;
        _didTimeRunOut = false;

        Debug.LogWarning("RESETTING THE WHOLE GAME");
        _shouldSave = false;
        PlayerPrefs.DeleteAll();
        File.Delete(Path.Combine(Application.persistentDataPath, saveFileName_Buildings));
        foreach (var building in Buildings)
        {
            building.ResetBuilding();
        }
        File.Delete(Path.Combine(Application.persistentDataPath, saveFileName_ClickUpgrades));
        foreach (var clickUpgrade in ClickUpgrades)
        {
            clickUpgrade.ResetUpgrade();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    #region UI Updates

    public void UpdateUI()
    {
        UpdateSawdustCountUI();
        UpdateSPSUI();
    }

    private void UpdateSawdustCountUI()
    {
        _sawdustCountText.text = CurrentSawdustCount.ToFormattedStr() + " Sawdust";
    }

    private void UpdateSPSUI()
    {
        _spsText.text = SawdustPerSec.ToFormattedStr() + " SawdustPerSec";
    }

    public void ShowInfoBox(string msg)
    {
        _infoBoxText.text = msg;
        _infoBox.SetActive(true);
    }

    #endregion

    #region On Trunk Click

    public void OnTrunkClicked(PointerEventData pointerEventData)
    {
        IncreaseSawdust();
        
        PopUpText.Create(PerClickAmmount, pointerEventData.position);
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

    public void OnBuildingsButtonPress()
    {
        // activate buildings canvas
        _buildingsCanvas.SetActive(true);
        _upgradesCanvas.SetActive(false);

        // deactivate main screen buttons
        _buildingsScreenButton.SetActive(false);
        _upgradesScreenButton.SetActive(false);
    }

    public void OnUpgradesButtonPress()
    {
        // activate upgrades canvas
        _buildingsCanvas.SetActive(false);
        _upgradesCanvas.SetActive(true);

        // deactivate main screen buttons
        _buildingsScreenButton.SetActive(false);
        _upgradesScreenButton.SetActive(false);
    }

    public void OnMainButtonPressed()
    {
        // deactivate canvases
        _buildingsCanvas.SetActive(false);
        _upgradesCanvas.SetActive(false);
        MainCanvas.SetActive(true);

        // activate main screen buttons
        _buildingsScreenButton.SetActive(true);
        _upgradesScreenButton.SetActive(true);
    }

    #endregion

    #region Increases

    public void SimpleSawdustIncrease(double ammount)
    {
        CurrentSawdustCount += ammount;
        UpdateUI();
    }

    public void AddBuildingMultiplier(string buildingName, float multiplier)
    {
        BuildingUpgrade building = Buildings.First(x => x.Name == buildingName);
        building.GainMultiplier *= multiplier;
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
        Debug.Log("PerSec multiplier x" + multiplier + " ran out! Now at " + PerSecMultiplier);
    }

    public void ClickIncreaseFor(double multiplier, float duration)
    {
        StartCoroutine(IncreaseClickFor(multiplier, duration));
    }

    IEnumerator IncreaseClickFor(double multiplier, float duration)
    {
        Debug.Log("Increasing Click for " + duration + " seconds by x" + multiplier);
        PerClickMultiplier *= multiplier;
        PopUpVelocity *= 1.6f;
        UpdateUI();
        yield return new WaitForSeconds(duration);
        PerClickMultiplier /= multiplier;
        PopUpVelocity /= 1.6f;
        UpdateUI();
        Debug.Log("Click multiplier x" + multiplier + " ran out! Now at " + PerClickMultiplier);
    }

    #endregion

    #region Events

    public void OnBuildingPurchaseClick(BuildingUIRefferences buttonRefs)
    {
        BuildingUpgrade building = buttonRefs.Building;
        if (CurrentSawdustCount >= building.CurrentUpgradeCost) 
        {
            CurrentSawdustCount -= building.CurrentUpgradeCost;

            Debug.Log($"{building.Name} purchased!!");
            building.BuildingPurchased(1);

            // take away sawdust
            UpdateUI();

            buttonRefs.UpdateBuildingUI();
        }
        else
        {
            Debug.Log("Not enough sawdust!");
        }
    }

    public void OnUpgradePurchaseClick(UpgradeUIRefferences buttonRefs)
    {
        ClickUpgrade upgrade = buttonRefs.Upgrade;

        if (CurrentSawdustCount >= upgrade.PurchaseCost)
        {
            CurrentSawdustCount -= upgrade.PurchaseCost;
            upgrade.PurchaseUpgrade();
            Debug.Log($"{upgrade.Name} purchased!!");

            UpdateUI();
            _initializeUI.InitUpgradesUI(ClickUpgrades, _upgradeUIToSpawn, _upgradeUIParent);
            buttonRefs.UpdateUpgradeUI();
        }
        else
        {
            Debug.Log("Not enough sawdust!");
        }
    }

    #endregion
}

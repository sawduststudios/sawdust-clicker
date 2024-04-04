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

public class SawdustManager : MonoBehaviour
{
    public static SawdustManager Instance;

    // GO Refferences
    public GameObject MainCanvas;
    [SerializeField] private GameObject _upgradeCanvas;
    [SerializeField] private GameObject _upgradeScreenButton;
    [SerializeField] private TextMeshProUGUI _sawdustCountText;
    [SerializeField] private TextMeshProUGUI _spsText;
    [SerializeField] private GameObject _sawdustTrunkObj;
    public GameObject SawdustTextPopUp;
    [SerializeField] private GameObject _backgroundObj;

    // Info Box
    [SerializeField] private GameObject _infoBox;
    [SerializeField] private TextMeshProUGUI _infoBoxText;


    [Space]
    public BuildingUpgrade[] Buildings; // All buildings in the game
    // GO Refferences
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
    }

    private void Start()
    {
        LoadGame();

        UpdateUI();
        _initializeUI.InitBuildingsUI(Buildings, _upgradeUIToSpawn, _upgradeUIParent);

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
    private const string saveKey_CurrentSawdustCount = "CurrentSawdustCount";
    private const string saveKey_PerSecMultiplier = "PerSecMultiplier";
    private const string saveKey_PerClickBase = "PerClickBase";
    private const string saveKey_PerClickMultiplier = "PerClickMultiplier";
    private const string saveKey_ClickPercentFromSPS = "ClickPercentFromSPS";
    private const string saveKey_Buildings = "Buildings";
    private const string saveFileName_Buildings = "buildings.json";
    #endregion
    public void SaveGame()
    {
        if (!_shouldSave)
        {
            Debug.LogWarning("Saving disabled...");
            return;
        }
        Debug.Log("SAVING GAME");

        PlayerPrefs.SetString(saveKey_CurrentSawdustCount, CurrentSawdustCount.ToString());
        PlayerPrefs.SetString(saveKey_PerSecMultiplier, PerSecMultiplier.ToString());
        PlayerPrefs.SetString(saveKey_PerClickBase, PerClickBase.ToString());
        PlayerPrefs.SetString(saveKey_PerClickMultiplier, PerClickMultiplier.ToString());
        PlayerPrefs.SetString(saveKey_ClickPercentFromSPS, ClickPercentFromSPS.ToString());

        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs saved");

        // Serialize Buildings array into JSON and save it as a string
        string buildingsJson = Buildings.Serialize();
        Debug.Log("Saving buildings:" + buildingsJson);
        //PlayerPrefs.SetString(saveKey_Buildings, buildingsJson);
        SaveToFile(saveFileName_Buildings, buildingsJson);
    }

    public void LoadGame()
    {
        Debug.Log("LOADING GAME");

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

        // Deserialize Buildings array from JSON
        //if (PlayerPrefs.HasKey(saveKey_Buildings))
        
        // if buildings file exists, load it
        if (File.Exists(Path.Combine(Application.persistentDataPath, saveFileName_Buildings)))
        {
            //string buildingsJson = PlayerPrefs.GetString(saveKey_Buildings);
            //Buildings = buildingsJson.DeserializeToBuildingUpgradeArr();

            string buildingsJson = LoadFromFile(saveFileName_Buildings);
            Debug.Log("Loaded buildings: " + buildingsJson);
            ExtensionMethods.LoadBuildings(buildingsJson);

            Debug.Log("Loaded from file buildings of len" + Buildings.Length);
            Debug.Log("Loaded buildings, computing SPS");

            double sps = 0;
            foreach (var building in Buildings)
            {
                sps += building.TotalSPS;
            }
            sps = sps * PerSecMultiplier;
            Debug.Log("Sps computed as " + sps);
        }

        UpdateUI();
        _initializeUI.InitBuildingsUI(Buildings, _upgradeUIToSpawn, _upgradeUIParent);
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
        Debug.LogWarning("RESETTING THE WHOLE GAME");
        _shouldSave = false;
        PlayerPrefs.DeleteAll();
        File.Delete(Path.Combine(Application.persistentDataPath, saveFileName_Buildings));
        foreach (var building in Buildings)
        {
            building.ResetBuilding();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        //StartCoroutine(ResetGameCoroutine());
    }

    IEnumerator ResetGameCoroutine()
    {
        _shouldSave = false;
        PlayerPrefs.DeleteAll();
        File.Delete(Path.Combine(Application.persistentDataPath, saveFileName_Buildings));
        foreach (var building in Buildings)
        {
            building.ResetBuilding();
        }
        yield return new WaitForSeconds(2f);
        Debug.LogWarning("RESETTING THE WHOLE GAME");
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

    public void OnUpgradeButtonPressed()
    {
        _upgradeCanvas.SetActive(true);
        //MainCanvas.SetActive(false);
        _upgradeScreenButton.SetActive(false);
    }

    public void OnMainButtonPressed()
    {
        _upgradeCanvas.SetActive(false);
        MainCanvas.SetActive(true);
        _upgradeScreenButton.SetActive(true);
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

    #endregion
}

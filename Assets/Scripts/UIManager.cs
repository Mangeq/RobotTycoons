using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("--- GENEL ---")]
    public TMP_Text moneyText;
    public Button travelButton;
    public TMP_Text travelButtonText;

    // Panellerin açık olup olmadığını kontrol eder
    public bool IsUIOpen
    {
        get
        {
            return (upgradePanelObject != null && upgradePanelObject.activeSelf) ||
                   (storagePanelObject != null && storagePanelObject.activeSelf) ||
                   (converterPanel != null && converterPanel.activeSelf) ||
                   (buyFactoryPanel != null && buyFactoryPanel.activeSelf) ||
                   (settingsPanel != null && settingsPanel.activeSelf);
        }
    }

    [Header("--- PANELLER (Panelleri Buraya Sürükle) ---")]
    public GameObject upgradePanelObject;
    public GameObject storagePanelObject;
    public GameObject converterPanel;
    public GameObject buyFactoryPanel; // Satın alma paneli (Bağımsız)
    public GameObject settingsPanel;

    // NOT: Kapatma butonları koddan silindi. 
    // Sen Unity Inspector'dan butonların OnClick özelliğine "CloseActivePanel" atayacaksın.

    [Header("--- ANA EKRAN BUTONLARI ---")]
    public Button openSettingsButton;
    public Button openBuyFactoryPanelButton; // Elmas Fabrika Satın Alma Butonu (Ana Ekranda)

    [Header("Upgrade Panel İçeriği")]
    public TMP_Text[] machineNameTexts;
    public TMP_Text[] machinePriceTexts;
    public Button[] upgradeButtons;

    [Header("Storage Panel İçeriği")]
    public Button[] unlockButtons;
    public TMP_Text[] unlockPriceTexts;
    public TMP_Text[] unlockNameTexts;
    public GameObject[] lockIcons;

    [Header("Converter Panel İçeriği")]
    public Button openConverterButton;
    public Button convertButton;
    public TMP_Text converterInfoText;
    public TMP_Text currentGoldText;
    public TMP_Text currentPlasmaText;

    [Header("Buy Factory Panel İçeriği")]
    public Button buyFactoryButton; // Panelin içindeki "Satın Al" butonu

    [Header("Settings Panel İçeriği")]
    public Button musicToggleButton;
    public GameObject musicCrossIcon;
    public Button sfxToggleButton;
    public GameObject sfxCrossIcon;

    [Header("Dünya / Mars Sekmeleri")]
    public GameObject earthTabs;
    public Button goldTabButton;
    public Button diamondTabButton;

    // WEB LİNKLERİ
    public string supportLink = "mailto:destek@sirketadi.com";
    public string instagramLink = "https://instagram.com/";
    public string androidRateLink = "market://details?id=com.sirket";

    private ProductionLine currentSelectedLine;
    private string currentStorageTab = "Gold";
    private float goldCost = 10000f;
    private float plasmaGain = 1000f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // --- ANA EKRAN BUTONLARI ---
        if (openSettingsButton) openSettingsButton.onClick.AddListener(OpenSettingsPanel);

        // Fabrika Satın Alma Panelini Açan Ana Buton
        if (openBuyFactoryPanelButton) openBuyFactoryPanelButton.onClick.AddListener(OpenBuyFactoryPanel);

        // --- İŞLEV BUTONLARI ---
        if (unlockButtons != null)
        {
            for (int i = 0; i < unlockButtons.Length; i++)
            {
                int index = i;
                if (unlockButtons[i]) unlockButtons[i].onClick.AddListener(() => OnUnlockButtonClicked(index));
            }
        }

        if (upgradeButtons != null)
        {
            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                int index = i;
                if (upgradeButtons[i]) upgradeButtons[i].onClick.AddListener(() => TryUpgradeMachine(index));
            }
        }

        if (goldTabButton) goldTabButton.onClick.AddListener(() => SwitchTab("Gold"));
        if (diamondTabButton) diamondTabButton.onClick.AddListener(() => SwitchTab("Diamond"));

        // Panelin içindeki asıl satın alma işlemini yapan buton
        if (buyFactoryButton) buyFactoryButton.onClick.AddListener(() => {
            FactoryManager.Instance.BuyDiamondFactory();
            CloseAllPanels(); // Satın alınca paneli kapat
        });

        if (travelButton) travelButton.onClick.AddListener(() => GameManager.Instance.ToggleTravel());

        if (openConverterButton) openConverterButton.onClick.AddListener(OpenConverterPanel);
        if (convertButton) convertButton.onClick.AddListener(ConvertCurrency);

        // Ayarlar
        if (musicToggleButton) musicToggleButton.onClick.AddListener(() => AudioManager.Instance.ToggleMusic());
        if (sfxToggleButton) sfxToggleButton.onClick.AddListener(() => AudioManager.Instance.ToggleSFX());

        UpdateWorldUI(false);
        RefreshSettingsUI();
    }

    // --- SENİN İSTEDİĞİN METOD GERİ GELDİ ---
    // Unity Inspector'dan tüm X butonlarına bunu ata
    public void CloseActivePanel()
    {
        CloseAllPanels();
    }

    public void CloseAllPanels()
    {
        if (upgradePanelObject) upgradePanelObject.SetActive(false);
        if (storagePanelObject) storagePanelObject.SetActive(false);
        if (converterPanel) converterPanel.SetActive(false);
        if (buyFactoryPanel) buyFactoryPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    // --- PANEL AÇMA FONKSİYONLARI ---

    public void OpenBuyFactoryPanel()
    {
        CloseAllPanels();
        // Eğer fabrika zaten alınmışsa açma
        if (FactoryManager.Instance != null && FactoryManager.Instance.isDiamondFactoryUnlocked)
        {
            return;
        }
        if (buyFactoryPanel) buyFactoryPanel.SetActive(true);
    }

    public void OpenUpgradePanel(ProductionLine line)
    {
        CloseAllPanels();
        currentSelectedLine = line;
        if (upgradePanelObject) upgradePanelObject.SetActive(true);
        UpdateUpgradePanelUI();
    }

    public void OpenStoragePanel()
    {
        CloseAllPanels();

        if (GameManager.Instance.isMarsActive)
        {
            currentStorageTab = "Mars";
            if (earthTabs) earthTabs.SetActive(false);
        }
        else
        {
            if (currentStorageTab == "Mars") currentStorageTab = "Gold";
            if (earthTabs) earthTabs.SetActive(true);
        }

        if (storagePanelObject) storagePanelObject.SetActive(true);
        UpdateStoragePanelUI();
    }

    public void OpenSettingsPanel()
    {
        CloseAllPanels();
        if (settingsPanel)
        {
            settingsPanel.SetActive(true);
            RefreshSettingsUI();
        }
    }

    public void OpenConverterPanel()
    {
        CloseAllPanels();
        if (converterPanel)
        {
            converterPanel.SetActive(true);
            UpdateConverterUI();
        }
    }

    // --- UI GÜNCELLEME ---

    void Update()
    {
        UpdateMoneyUI();

        if (IsUIOpen)
        {
            if (storagePanelObject.activeSelf) UpdateStoragePanelUI();
            else if (upgradePanelObject.activeSelf) UpdateUpgradePanelUI();
            else if (converterPanel.activeSelf) UpdateConverterUI();
        }
    }

    public static void UpdateMoneyUI()
    {
        if (Instance == null || Instance.moneyText == null || EconomyManager.Instance == null) return;

        bool isMars = (GameManager.Instance != null && GameManager.Instance.isMarsActive);

        if (isMars)
        {
            Instance.moneyText.color = Color.red;
            Instance.moneyText.text = FormatMoney(EconomyManager.Instance.Plasma);
        }
        else
        {
            Instance.moneyText.color = Color.white;
            string goldText = FormatMoney(EconomyManager.Instance.Gold);
            string speedText = FormatMoney(EconomyManager.Instance.GoldPerSecond);
            Instance.moneyText.text = $"{goldText}\n<size=70%>+{speedText}/sec</size>";
        }
    }

    public void UpdateWorldUI(bool isMars)
    {
        if (EconomyManager.Instance == null) return;

        CloseAllPanels();

        if (isMars)
        {
            if (travelButtonText) travelButtonText.text = "GO TO EARTH";
            if (earthTabs) earthTabs.SetActive(false);
            currentStorageTab = "Mars";
            if (openConverterButton) openConverterButton.gameObject.SetActive(true);

            if (openBuyFactoryPanelButton) openBuyFactoryPanelButton.gameObject.SetActive(false);
        }
        else
        {
            if (travelButtonText) travelButtonText.text = "GO TO MARS";
            if (earthTabs) earthTabs.SetActive(true);
            currentStorageTab = "Gold";
            if (openConverterButton) openConverterButton.gameObject.SetActive(true);

            if (openBuyFactoryPanelButton)
            {
                bool isOwned = FactoryManager.Instance.isDiamondFactoryUnlocked;
                openBuyFactoryPanelButton.gameObject.SetActive(!isOwned);
            }
        }
        UpdateMoneyUI();
    }

    void UpdateStoragePanelUI()
    {
        if (FactoryManager.Instance == null) return;

        var filteredProducts = FactoryManager.Instance.products.FindAll(x => x.factoryID == currentStorageTab);

        for (int i = 0; i < unlockButtons.Length; i++)
        {
            if (i < filteredProducts.Count)
            {
                var prod = filteredProducts[i];
                if (unlockButtons[i]) unlockButtons[i].gameObject.SetActive(true);

                if (unlockNameTexts != null && i < unlockNameTexts.Length) unlockNameTexts[i].text = prod.displayName;
                if (lockIcons != null && i < lockIcons.Length && lockIcons[i] != null) lockIcons[i].SetActive(!prod.isUnlocked);

                if (prod.isUnlocked)
                {
                    if (unlockPriceTexts[i]) unlockPriceTexts[i].text = "OWNED";
                    if (unlockButtons[i]) unlockButtons[i].interactable = false;
                }
                else
                {
                    if (unlockPriceTexts[i]) unlockPriceTexts[i].text = FormatMoney(prod.unlockCost);
                    float wallet = (prod.factoryID == "Mars") ? EconomyManager.Instance.Plasma : EconomyManager.Instance.Gold;
                    if (unlockButtons[i]) unlockButtons[i].interactable = (wallet >= prod.unlockCost);
                }
            }
            else
            {
                if (unlockButtons[i]) unlockButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void SwitchTab(string tabID)
    {
        currentStorageTab = tabID;
        UpdateStoragePanelUI();
    }

    public void OnUnlockButtonClicked(int buttonIndex)
    {
        var currentProducts = FactoryManager.Instance.products.FindAll(x => x.factoryID == currentStorageTab);
        if (buttonIndex < currentProducts.Count) FactoryManager.Instance.UnlockProduct(currentProducts[buttonIndex].id);
    }

    void UpdateUpgradePanelUI()
    {
        if (currentSelectedLine == null) return;
        var machines = currentSelectedLine.machines;
        bool isMars = GameManager.Instance.isMarsActive;

        for (int i = 0; i < machines.Count && i < 4; i++)
        {
            if (machineNameTexts[i]) machineNameTexts[i].text = machines[i].name + " LVL " + machines[i].lvl;
            if (machinePriceTexts[i]) machinePriceTexts[i].text = FormatMoney(machines[i].price);

            if (upgradeButtons[i])
            {
                float wallet = isMars ? EconomyManager.Instance.Plasma : EconomyManager.Instance.Gold;
                upgradeButtons[i].interactable = (wallet >= machines[i].price);
            }
        }
    }

    void UpdateConverterUI()
    {
        if (converterInfoText) converterInfoText.text = $"EXCHANGE:\n{FormatMoney(goldCost)} \n▼\n{FormatMoney(plasmaGain)} ";
        if (currentGoldText) currentGoldText.text = FormatMoney(EconomyManager.Instance.Gold);
        if (currentPlasmaText) currentPlasmaText.text = FormatMoney(EconomyManager.Instance.Plasma);

        if (convertButton) convertButton.interactable = (EconomyManager.Instance.Gold >= goldCost);
    }

    public void ConvertCurrency()
    {
        if (EconomyManager.Instance.TrySpendCurrency(CurrencyType.Gold, goldCost))
        {
            EconomyManager.Instance.AddCurrency(CurrencyType.Plasma, plasmaGain);
            FindObjectOfType<SaveManager>().SavePlayerData();
            UpdateConverterUI();
        }
    }

    void TryUpgradeMachine(int index)
    {
        if (currentSelectedLine != null)
        {
            var machine = currentSelectedLine.machines[index];
            bool isMars = GameManager.Instance.isMarsActive;
            float wallet = isMars ? EconomyManager.Instance.Plasma : EconomyManager.Instance.Gold;

            if (wallet >= machine.price)
            {
                currentSelectedLine.UpgradeMachine(index);
                FindObjectOfType<SaveManager>().SavePlayerData();
                UpdateUpgradePanelUI();
                if (AudioManager.Instance) AudioManager.Instance.PlayUpgradeSound();
            }
        }
    }

    public static string FormatMoney(float amount)
    {
        if (amount >= 1000000) return (amount / 1000000f).ToString("0.0") + "M";
        if (amount >= 1000) return (amount / 1000f).ToString("0.0") + "K";
        return amount.ToString("0");
    }

    public void RefreshSettingsUI()
    {
        if (AudioManager.Instance == null) return;
        if (musicCrossIcon) musicCrossIcon.SetActive(AudioManager.Instance.isMusicMuted);
        if (sfxCrossIcon) sfxCrossIcon.SetActive(AudioManager.Instance.isSfxMuted);
    }

    // --- LİNKLER ---
    public void OpenSupport() { if (!string.IsNullOrEmpty(supportLink)) Application.OpenURL(supportLink); }
    public void OpenInstagram() { if (!string.IsNullOrEmpty(instagramLink)) Application.OpenURL(instagramLink); }
    public void OpenRateUs() { if (!string.IsNullOrEmpty(androidRateLink)) Application.OpenURL(androidRateLink); }
}
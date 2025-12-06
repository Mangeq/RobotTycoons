using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AssemblyManager : MonoBehaviour
{
    public static AssemblyManager Instance;

    [Header("UI Referanslarý")]
    public GameObject assemblyPanel;
    public TMP_Text robotCountText;
    public TMP_Text bonusText;

    [Header("Gereksinim Metinleri")]
    public TMP_Text headReqText;
    public TMP_Text bodyReqText;
    public TMP_Text armReqText;
    public TMP_Text legReqText;

    [Header("Butonlar")]
    public Button assembleButton;
    public Button closeButton;
    public Button scrapButton; // YENÝ: Parça Satma Butonu (Sahneye ekle ve sürükle)

    // YENÝ DENGE: Bacaklar çok biriktiði için robot artýk "4 Bacaklý" (Tank/Örümcek) modelinde.
    private int reqHead = 1;
    private int reqBody = 1;
    private int reqArm = 2;
    private int reqLeg = 4; // ESKÝSÝ 2 ÝDÝ, ÞÝMDÝ 4. Bacaklar daha hýzlý tükenecek.

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (assembleButton) assembleButton.onClick.AddListener(AssembleRobot);
        if (closeButton) closeButton.onClick.AddListener(ClosePanel);

        // Eðer hurda butonu atadýysan çalýþtýr
        if (scrapButton) scrapButton.onClick.AddListener(ScrapAllParts);
    }

    public void OpenAssemblyPanel()
    {
        assemblyPanel.SetActive(true);
        UIManager.Instance.CloseAllPanels();
        UpdateAssemblyUI();
    }

    public void ClosePanel()
    {
        assemblyPanel.SetActive(false);
    }

    public void UpdateAssemblyUI()
    {
        if (FactoryManager.Instance == null) return;

        int currentHead = GetStock("Head");
        int currentBody = GetStock("Body");
        int currentArm = GetStock("Arm");
        int currentLeg = GetStock("Leg");

        // UI Metinlerini Güncelle
        if (headReqText) headReqText.text = $"HEAD: {currentHead}/{reqHead}";
        if (bodyReqText) bodyReqText.text = $"BODY: {currentBody}/{reqBody}";
        if (armReqText) armReqText.text = $"ARM: {currentArm}/{reqArm}";
        if (legReqText) legReqText.text = $"LEG: {currentLeg}/{reqLeg}";

        // Renklendirme: Yeterli sayýdaysa Yeþil, deðilse Kýrmýzý yap
        if (headReqText) headReqText.color = currentHead >= reqHead ? Color.green : Color.red;
        if (armReqText) armReqText.color = currentArm >= reqLeg ? Color.green : Color.red;
        if (bodyReqText) bodyReqText.color = currentBody >= reqLeg ? Color.green : Color.red;
        if (legReqText) legReqText.color = currentLeg >= reqLeg ? Color.green : Color.red;
        // Diðerleri için de yapabilirsin...

        // Robot Sayýsý ve Bonus
        // Her robot %10 yerine artýk %25 bonus versin (çünkü üretimi zorlaþtý)
        float currentBonus = EconomyManager.Instance.TotalRobots * 25f;

        if (robotCountText) robotCountText.text = "ROBOTS: " + EconomyManager.Instance.TotalRobots;
        if (bonusText) bonusText.text = "INCOME BONUS: +%" + currentBonus;

        // Üretim Butonu Kontrolü
        bool canAssemble = (currentHead >= reqHead && currentBody >= reqBody && currentArm >= reqArm && currentLeg >= reqLeg);
        if (assembleButton) assembleButton.interactable = canAssemble;

        // Hurda Butonu Kontrolü (Stokta en az 1 parça varsa satabilsin)
        if (scrapButton)
        {
            bool hasAnyStock = (currentHead > 0 || currentBody > 0 || currentArm > 0 || currentLeg > 0);
            scrapButton.interactable = hasAnyStock;
        }
    }

    public void AssembleRobot()
    {
        ReduceStock("Head", reqHead);
        ReduceStock("Body", reqBody);
        ReduceStock("Arm", reqArm);
        ReduceStock("Leg", reqLeg);

        EconomyManager.Instance.AddRobot();
        FindObjectOfType<SaveManager>().SavePlayerData();

        // Efektif ses veya partikül eklenebilir
        Debug.Log("Robot Üretildi! Bonus arttý.");

        UpdateAssemblyUI();
    }

    // YENÝ FONKSÝYON: Fazla parçalarý nakite çevir
    // Robot yapamýyorsan biriken bacaklarý satýp fabrikalarý geliþtirebilirsin.
    public void ScrapAllParts()
    {
        float totalScrapValue = 0;

        // Her parçanýn bir hurda deðeri var
        totalScrapValue += ScrapProduct("Leg", 50f);   // Tanesi 50 Gold
        totalScrapValue += ScrapProduct("Arm", 150f);  // Tanesi 150 Gold
        totalScrapValue += ScrapProduct("Body", 500f);
        totalScrapValue += ScrapProduct("Head", 1000f);

        if (totalScrapValue > 0)
        {
            EconomyManager.Instance.AddCurrency(CurrencyType.Gold, totalScrapValue);
            FindObjectOfType<SaveManager>().SavePlayerData();
            UpdateAssemblyUI();
            Debug.Log("Tüm parçalar hurdaya satýldý: " + totalScrapValue);
        }
    }

    private float ScrapProduct(string id, float pricePerUnit)
    {
        var prod = FactoryManager.Instance.products.Find(x => x.id == id);
        if (prod != null && prod.inventoryAmount > 0)
        {
            float income = prod.inventoryAmount * pricePerUnit;
            prod.inventoryAmount = 0; // Hepsini sil
            return income;
        }
        return 0;
    }

    int GetStock(string id)
    {
        var prod = FactoryManager.Instance.products.Find(x => x.id == id);
        return prod != null ? prod.inventoryAmount : 0;
    }

    void ReduceStock(string id, int amount)
    {
        var prod = FactoryManager.Instance.products.Find(x => x.id == id);
        if (prod != null) prod.inventoryAmount -= amount;
    }

    public float GetPrestigeMultiplier()
    {
        if (EconomyManager.Instance == null) return 1.0f;
        // Robot baþýna çarpaný da artýrdýk (0.1 -> 0.25)
        return 1.0f + (EconomyManager.Instance.TotalRobots * 0.25f);
    }
}
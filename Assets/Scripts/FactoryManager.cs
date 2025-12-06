using System.Collections.Generic;
using UnityEngine;

public class FactoryManager : MonoBehaviour
{
    public static FactoryManager Instance;

    // Aktif çalýþan hatlarýn listesi
    public List<ProductionLine> activeLines = new List<ProductionLine>();

    [Header("Bina Referanslarý")]
    public GameObject goldFactoryRoot;
    public GameObject diamondFactoryRoot;
    public GameObject marsFactoryRoot;

    [Header("Veriler")]
    public List<ProductData> products; // Ürün listesi (Inspector'da görünmez, Awake'de dolar)
    public bool isDiamondFactoryUnlocked = false;
    public float diamondFactoryCost = 50000f;

    void Awake()
    {
        Instance = this;
        InitializeProducts();
    }

    private void InitializeProducts()
    {
        products = new List<ProductData>
        {
            // --- GOLD FACTORY (DÜNYA) ---
            // Strateji: Ýlk ürün bedava veya çok ucuz, diðerleri katlanarak artar.
            
            new ProductData { id = "Leg",  displayName = "LEG",  factoryID = "Gold", unlockCost = 0 },      // ÝLK ÜRÜN BEDAVA OLSUN
            new ProductData { id = "Arm",  displayName = "ARM",  factoryID = "Gold", unlockCost = 2500 },   // Hedef 1
            new ProductData { id = "Body", displayName = "BODY", factoryID = "Gold", unlockCost = 15000 },  // Hedef 2
            new ProductData { id = "Head", displayName = "HEAD", factoryID = "Gold", unlockCost = 50000 },  // Büyük Hedef

            // --- DIAMOND FACTORY (ZORLU SEVÝYE) ---
            new ProductData { id = "D_Leg",  displayName = "D-LEG",  factoryID = "Diamond", unlockCost = 250000 },
            new ProductData { id = "D_Arm",  displayName = "D-ARM",  factoryID = "Diamond", unlockCost = 500000 },
            new ProductData { id = "D_Body", displayName = "D-BODY", factoryID = "Diamond", unlockCost = 1000000 }, // 1M
            new ProductData { id = "D_Head", displayName = "D-HEAD", factoryID = "Diamond", unlockCost = 5000000 }, // 5M

            // --- MARS FACTORY (PLASMA) ---
            new ProductData { id = "M_Leg",  displayName = "GRAVITY LEG",factoryID = "Mars", unlockCost = 1000 },
            new ProductData { id = "M_Arm",  displayName = "LASER ARM",  factoryID = "Mars", unlockCost = 5000 },
            new ProductData { id = "M_Body", displayName = "LASER BODY", factoryID = "Mars", unlockCost = 25000 },
            new ProductData { id = "M_Head", displayName = "AI BRAIN",   factoryID = "Mars", unlockCost = 100000 }
        };
    }

    // SaveManager'dan gelen veriyi iþler
    public void LoadFactoryData(SaveData data)
    {
        isDiamondFactoryUnlocked = data.isDiamondFactoryUnlocked;

        if (data.products != null)
        {
            foreach (var saved in data.products)
            {
                var current = products.Find(p => p.id == saved.id);
                if (current != null)
                {
                    current.isUnlocked = saved.isUnlocked;
                    current.inventoryAmount = saved.inventoryAmount;
                }
            }
        }

        // Hatlarýn levellerini yükle
        if (data.lines != null)
        {
            foreach (var savedLine in data.lines)
            {
                var line = activeLines.Find(l => l.productID == savedLine.lineID);
                if (line != null)
                {
                    line.LoadMachineData(savedLine.machines);
                }
            }
        }

        RefreshVisuals();
    }

    // Kayýt için veriyi paketler
    public void FillSaveData(SaveData data)
    {
        data.isDiamondFactoryUnlocked = isDiamondFactoryUnlocked;
        data.products = products;
        data.lines = new List<LineSaveModel>();

        foreach (var line in activeLines)
        {
            data.lines.Add(new LineSaveModel
            {
                lineID = line.productID,
                machines = line.machines
            });
        }
    }

    public void RefreshVisuals()
    {
        // 1. Diamond Binasý
        if (diamondFactoryRoot) diamondFactoryRoot.SetActive(isDiamondFactoryUnlocked);

        // 2. Hatlarý Aç/Kapa
        foreach (var prod in products)
        {
            Transform t = null;
            if (prod.factoryID == "Gold" && goldFactoryRoot) t = FindDeepChild(goldFactoryRoot.transform, prod.id);
            else if (prod.factoryID == "Diamond" && diamondFactoryRoot) t = FindDeepChild(diamondFactoryRoot.transform, prod.id);
            else if (prod.factoryID == "Mars" && marsFactoryRoot) t = FindDeepChild(marsFactoryRoot.transform, prod.id);

            if (t != null) t.gameObject.SetActive(prod.isUnlocked);
        }

        // UI'ya haber ver
        GameEvents.OnUnlockUpdate?.Invoke();
    }

    public void UnlockProduct(string id)
    {
        var prod = products.Find(p => p.id == id);
        if (prod == null || prod.isUnlocked) return;

        CurrencyType costType = (prod.factoryID == "Mars") ? CurrencyType.Plasma : CurrencyType.Gold;

        if (EconomyManager.Instance.TrySpendCurrency(costType, prod.unlockCost))
        {
            prod.isUnlocked = true;
            RefreshVisuals();
            FindObjectOfType<SaveManager>().SavePlayerData();
        }
    }

    public void BuyDiamondFactory()
    {
        if (!isDiamondFactoryUnlocked && EconomyManager.Instance.TrySpendCurrency(CurrencyType.Gold, diamondFactoryCost))
        {
            isDiamondFactoryUnlocked = true;
            RefreshVisuals();
            // UI.Instance.SwitchTab("Diamond"); // Bunu UI tarafý event ile yapmalý
            FindObjectOfType<SaveManager>().SavePlayerData();
        }
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            var result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }
}
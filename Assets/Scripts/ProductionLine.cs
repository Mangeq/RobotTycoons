using System.Collections.Generic;
using UnityEngine;

public class ProductionLine : MonoBehaviour
{
    [Header("Ayarlar")]
    public string productID;
    public bool isMarsLine;

    [Header("Durum")]
    public List<MachineData> machines;

    private float productionTimer;
    private float productionSpeed;
    private int boost = 1;

    // TEK VE GÜNCEL AWAKE FONKSÝYONU
    void Awake()
    {
        if (string.IsNullOrEmpty(productID)) productID = gameObject.name;

        // YENÝ EKONOMÝ AYARLARI (Tycoon Standardý)
        machines = new List<MachineData>
        {
            // Ýsim, Baþlangýç Fiyatý, Fiyat Artýþ Çarpaný
            new MachineData("Smelter", 100, 1.4f),      // Temel Gelir
            new MachineData("Pattern", 500, 1.5f),      // Çarpan 1 (%10 artýþ)
            new MachineData("Electronics", 1500, 1.6f), // Çarpan 2 (%25 artýþ)
            new MachineData("Work Speed", 2000, 2.5f)   // Hýz
        };
    }

    void OnEnable()
    {
        if (FactoryManager.Instance) FactoryManager.Instance.activeLines.Add(this);
        RecalculateStats();
    }

    void OnDisable()
    {
        if (FactoryManager.Instance) FactoryManager.Instance.activeLines.Remove(this);
    }

    void Update()
    {
        if (AssemblyManager.Instance == null) return;
        productionTimer += Time.deltaTime;
        if (productionTimer >= productionSpeed)
        {
            Produce();
            productionTimer = 0f;
        }
    }

    void Produce()
    {
        // --- YENÝ GERÇEKÇÝ EKONOMÝ FORMÜLÜ ---

        // 1. Makine (Smelter): Temel Para (Level * 15)
        float baseIncome = machines[0].lvl * 15f;

        // 2. Makine (Pattern): Her level geliri %10 katlar (1.1, 1.2, 1.3...)
        float multiplier1 = 1.0f + (machines[1].lvl * 0.10f);

        // 3. Makine (Electronics): Her level geliri %25 katlar (1.25, 1.50...)
        float multiplier2 = 1.0f + (machines[2].lvl * 0.25f);

        // Ana Hesap: Temel * Çarpan1 * Çarpan2
        float totalIncome = baseIncome * multiplier1 * multiplier2;

        // Prestige Bonusu
        float prestigeMult = 1.0f;
        if (AssemblyManager.Instance != null) prestigeMult = AssemblyManager.Instance.GetPrestigeMultiplier();

        // --- BURAYI DEÐÝÞTÝRÝYORUZ ---

        // AdManager'dan reklam bonusunu al (Yoksa 1 gelir, varsa 2 gelir)
        float adBonus = 1.0f;
        if (AdManager.Instance != null)
        {
            adBonus = AdManager.Instance.GetIncomeMultiplier();
        }

        // Hepsini çarp: Normal Gelir * Robot Bonusu * Reklam Bonusu
        float finalIncome = totalIncome * prestigeMult * adBonus;

        // Ödeme Yap
        CurrencyType type = isMarsLine ? CurrencyType.Plasma : CurrencyType.Gold;

        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.AddCurrency(type, finalIncome);
        }

        // Stok Ekle
        if (FactoryManager.Instance != null)
        {
            var prod = FactoryManager.Instance.products.Find(p => p.id == productID);
            if (prod != null)
            {
                prod.inventoryAmount++;
                GameEvents.OnInventoryChanged?.Invoke();
            }
        }
    }

    public void RecalculateStats()
    {
        // Hýz hesaplamasý (Maksimum hýz sýnýrý: 0.1 saniye)
        int speedLvl = machines[3].lvl;
        // Baþlangýç hýzý 3.0 saniye. Her level 0.1 saniye düþürür.
        productionSpeed = Mathf.Max(0.1f, 3.0f - (speedLvl * 0.1f)) / boost;
    }

    public void LoadMachineData(List<MachineData> loadedMachines)
    {
        for (int i = 0; i < machines.Count; i++)
        {
            if (i < loadedMachines.Count)
            {
                machines[i].lvl = loadedMachines[i].lvl;
                machines[i].price = loadedMachines[i].price;
            }
        }
        RecalculateStats();
    }

    public void UpgradeMachine(int index)
    {
        var machine = machines[index];
        CurrencyType type = isMarsLine ? CurrencyType.Plasma : CurrencyType.Gold;

        if (EconomyManager.Instance.TrySpendCurrency(type, machine.price))
        {
            machine.lvl++;
            machine.price *= machine.multiplier; // Fiyat katlanarak artar
            RecalculateStats();
        }
    }
}
using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    [Header("Ekonomi Verileri")]
    public float Gold;
    public float Plasma;
    public int TotalRobots;

    // --- YENÝ EKLENEN DEÐÝÞKENLER ---
    [Header("Gelir Hýzý Takibi")]
    public float GoldPerSecond;    // Ekranda görünecek deðer
    private float goldAccumulator; // Arkada biriken gizli sayaç
    private float timer;           // 1 saniyelik zamanlayýcý
    // --------------------------------

    public event Action OnCurrencyChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // --- YENÝ UPDATE FONKSÝYONU ---
    void Update()
    {
        // Her saniye, biriken parayý "Hýz" deðiþkenine aktar ve sýfýrla
        timer += Time.deltaTime;
        if (timer >= 1.0f)
        {
            GoldPerSecond = goldAccumulator; // Son 1 saniyede toplananý kaydet
            goldAccumulator = 0;             // Sayacý sýfýrla
            timer = 0;

            // UI'ý güncelle ki yeni hýzý görelim
            UIManager.UpdateMoneyUI();
        }
    }
    // -----------------------------

    public void LoadEconomy(float money, float plasma, int robots)
    {
        Gold = money;
        Plasma = plasma;
        TotalRobots = robots;
        OnCurrencyChanged?.Invoke();
        UIManager.UpdateMoneyUI();
    }

    public void AddCurrency(CurrencyType type, float amount)
    {
        if (type == CurrencyType.Gold)
        {
            Gold += amount;
            goldAccumulator += amount; // --- BURASI YENÝ: Hýz ölçümü için biriktir ---
        }
        else
        {
            Plasma += amount;
            // Ýstersen Plasma için de aynýsýný yapabilirsin
        }

        OnCurrencyChanged?.Invoke();
        UIManager.UpdateMoneyUI();
    }

    public bool TrySpendCurrency(CurrencyType type, float amount)
    {
        if (type == CurrencyType.Gold)
        {
            if (Gold >= amount)
            {
                Gold -= amount;
                OnCurrencyChanged?.Invoke();
                UIManager.UpdateMoneyUI();
                return true;
            }
        }
        else
        {
            if (Plasma >= amount)
            {
                Plasma -= amount;
                OnCurrencyChanged?.Invoke();
                UIManager.UpdateMoneyUI();
                return true;
            }
        }
        return false;
    }

    public void AddRobot()
    {
        TotalRobots++;
    }
}
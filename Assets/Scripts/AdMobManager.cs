using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdMobManager : MonoBehaviour
{
    public static AdMobManager Instance;

    [Header("Ayarlar")]
    // Test ID'si (Gerçek yayýnlamada kendi ID'ni koy)
    private string adUnitId = "ca-app-pub-3940256099942544/5224354917";
    private RewardedAd _rewardedAd;

    [Header("Boost Durumu")]
    public bool isSpeedBoostActive = false;
    public bool isIncomeBoostActive = false;

    private float boostTimer = 0f;
    private const float BOOST_DURATION = 180f; // 3 Dakika (180 saniye)

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Google Ads SDK Baþlat
        MobileAds.Initialize(initStatus => { });
        LoadRewardedAd();
    }

    void Update()
    {
        // Sayaç Mantýðý
        if (boostTimer > 0)
        {
            boostTimer -= Time.deltaTime;

            // Süre bittiðinde kapat
            if (boostTimer <= 0)
            {
                DeactivateBoosts();
            }
        }
    }

    public void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        AdRequest adRequest = new AdRequest();

        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Reklam Yükleme Hatasý: " + error);
                    return;
                }
                _rewardedAd = ad;
                Debug.Log("Reklam Yüklendi!");
            });
    }

    // Bu fonksiyonu Butonlara baðlayacaðýz. 
    // type: "Speed" veya "Income"
    public void ShowRewardedAd(string type)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                // Reklam izlendi, ödülü ver
                ActivateBoost(type);
            });

            // Bir sonraki için yenisini yükle
            LoadRewardedAd();
        }
        else
        {
            Debug.Log("Reklam hazýr deðil, tekrar yükleniyor...");
            LoadRewardedAd();
        }
    }

    private void ActivateBoost(string type)
    {
        boostTimer = BOOST_DURATION;

        if (type == "Speed")
        {
            isSpeedBoostActive = true;
            Debug.Log("HIZ BOOSTU AKTÝF!");
        }
        else if (type == "Income")
        {
            isIncomeBoostActive = true;
            Debug.Log("GELÝR BOOSTU AKTÝF!");
        }

        // Tüm hatlara haber ver ki hýzlarýný güncellesinler
        RefreshAllLines();
    }

    private void DeactivateBoosts()
    {
        isSpeedBoostActive = false;
        isIncomeBoostActive = false;
        boostTimer = 0;
        RefreshAllLines();
        Debug.Log("BOOST SÜRESÝ BÝTTÝ.");
    }

    private void RefreshAllLines()
    {
        // FactoryManager'daki tüm aktif hatlarý güncelle
        if (FactoryManager.Instance != null)
        {
            foreach (var line in FactoryManager.Instance.activeLines)
            {
                line.RecalculateStats();
            }
        }
    }

    // ProductionLine bu fonksiyonlarý kullanacak
    public int GetSpeedMultiplier()
    {
        return isSpeedBoostActive ? 2 : 1; // Aktifse 2 kat hýz
    }

    public int GetIncomeMultiplier()
    {
        return isIncomeBoostActive ? 2 : 1; // Aktifse 2 kat para
    }

    // UI'da kalan süreyi göstermek istersen
    public float GetRemainingTime()
    {
        return boostTimer;
    }
}
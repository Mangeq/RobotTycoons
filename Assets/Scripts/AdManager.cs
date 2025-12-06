using UnityEngine;
using GoogleMobileAds.Api; // AdMob Kütüphanesi
using System;
using TMPro; // Eðer butonda geri sayým yapacaksan gerekli
using UnityEngine.UI;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    [Header("Ayarlar")]
    // TEST ID'leridir. Yayýnlarken deðiþtir!
    // Android Interstitial Test ID: ca-app-pub-3940256099942544/1033173712
    // Android Rewarded Test ID: ca-app-pub-3940256099942544/5224354917
    private string interstitialAdId = "ca-app-pub-3940256099942544/1033173712";
    private string rewardedAdId = "ca-app-pub-3940256099942544/5224354917";

    [Header("UI Referanslarý")]
    public Button watchAdButton;
    public TMP_Text bonusTimerText; // Butonun üzerindeki yazý (Opsiyonel)

    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    // Zamanlayýcýlar
    private float autoAdTimer = 0f;
    private float autoAdInterval = 300f; // 5 Dakika (300 saniye)

    private float bonusTimer = 0f;
    private bool isBonusActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        // AdMob'u Baþlat
        MobileAds.Initialize(initStatus => {
            LoadInterstitialAd();
            LoadRewardedAd();
        });

        if (watchAdButton)
        {
            watchAdButton.onClick.AddListener(ShowRewardedAd);
        }
    }

    void Update()
    {
        // 1. OTOMATÝK REKLAM SAYACI (5 Dakika)
        autoAdTimer += Time.deltaTime;
        if (autoAdTimer >= autoAdInterval)
        {
            ShowInterstitialAd();
            autoAdTimer = 0f; // Sayacý sýfýrla
        }

        // 2. BONUS (2x) SAYACI (1 Dakika)
        if (isBonusActive)
        {
            bonusTimer -= Time.deltaTime;

            // UI Güncelleme (Opsiyonel)
            if (bonusTimerText)
                bonusTimerText.text = "2x ACTIVE: " + Mathf.Ceil(bonusTimer).ToString();

            if (bonusTimer <= 0)
            {
                isBonusActive = false;
                if (bonusTimerText) bonusTimerText.text = "WATCH AD (2x)";
                if (watchAdButton) watchAdButton.interactable = true;
                Debug.Log("Bonus süresi bitti.");
            }
        }
    }

    // --- INTERSTITIAL (GEÇÝÞ) REKLAM ---

    void LoadInterstitialAd()
    {
        // Eski reklamý temizle
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        var adRequest = new AdRequest();
        InterstitialAd.Load(interstitialAdId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial yüklenemedi: " + error);
                return;
            }
            interstitialAd = ad;

            // Reklam kapanýnca yenisini yükle
            interstitialAd.OnAdFullScreenContentClosed += () => { LoadInterstitialAd(); };
        });
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial hazýr deðil, yenisi yükleniyor...");
            LoadInterstitialAd();
        }
    }

    // --- REWARDED (ÖDÜLLÜ) REKLAM ---

    void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        var adRequest = new AdRequest();
        RewardedAd.Load(rewardedAdId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded yüklenemedi: " + error);
                return;
            }
            rewardedAd = ad;

            // Reklam kapanýnca yenisini yükle
            rewardedAd.OnAdFullScreenContentClosed += () => { LoadRewardedAd(); };
        });
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                // KULLANICI REKLAMI TAMAMLADI -> ÖDÜLÜ VER
                ActivateBonus();
            });
        }
        else
        {
            Debug.Log("Ödüllü reklam hazýr deðil.");
            LoadRewardedAd();
        }
    }

    void ActivateBonus()
    {
        isBonusActive = true;
        bonusTimer = 60f; // 1 Dakika (60 saniye)
        if (watchAdButton) watchAdButton.interactable = false; // Süre bitene kadar tekrar izleyemesin
        Debug.Log("2x Gelir Bonusu Aktif!");
    }

    // Diðer scriptlerin bonusu kontrol etmesi için fonksiyon
    public float GetIncomeMultiplier()
    {
        return isBonusActive ? 2.0f : 1.0f;
    }
}
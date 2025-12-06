using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Ses Kaynaklarý (Audio Source Ekle ve Sürükle)")]
    public AudioSource musicSource; // Arka plan müziði için
    public AudioSource sfxSource;   // Efektler (Upgrade sesi vb.) için

    [Header("Ses Dosyalarý (Klipleri Sürükle)")]
    public AudioClip upgradeSound;  // Upgrade yapýnca çýkacak ses
    public AudioClip clickSound;    // Buton týklama sesi (Opsiyonel)

    // Durumlar (Mute durumu)
    public bool isMusicMuted = false;
    public bool isSfxMuted = false;

    void Awake()
    {
        // Singleton yapýsý
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahne geçiþinde müzik kesilmesin
        }
        else
        {
            Destroy(gameObject);
        }

        // Kayýtlý ayarlarý yükle
        isMusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        isSfxMuted = PlayerPrefs.GetInt("SfxMuted", 0) == 1;
    }

    void Start()
    {
        UpdateSoundStates();
    }

    // --- MÜZÝK KONTROL ---
    public void ToggleMusic()
    {
        isMusicMuted = !isMusicMuted;
        // Ayarý kaydet (1: Mute, 0: Açýk)
        PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
        UpdateSoundStates();
    }

    // --- EFEKT (SFX) KONTROL ---
    public void ToggleSFX()
    {
        isSfxMuted = !isSfxMuted;
        PlayerPrefs.SetInt("SfxMuted", isSfxMuted ? 1 : 0);
        UpdateSoundStates();
    }

    void UpdateSoundStates()
    {
        // Müzik kaynaðýný sustur veya aç
        if (musicSource) musicSource.mute = isMusicMuted;
        if (sfxSource) sfxSource.mute = isSfxMuted;

        // UI'daki çarpý iþaretlerini güncellemesi için UIManager'a haber ver
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshSettingsUI();
        }
    }

    // --- SES ÇALMA FONKSÝYONLARI ---
    public void PlayUpgradeSound()
    {
        if (!isSfxMuted && sfxSource && upgradeSound)
        {
            sfxSource.PlayOneShot(upgradeSound);
        }
    }

    public void PlayClickSound()
    {
        if (!isSfxMuted && sfxSource && clickSound)
        {
            sfxSource.PlayOneShot(clickSound);
        }
    }
}
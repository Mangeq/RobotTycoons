using System; // Tarih işlemleri için gerekli
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class SaveManager : MonoBehaviour
{
    [Header("Firebase Ayarları")]
    [SerializeField] private string firebaseUrl = "https://robottycoon-default-rtdb.europe-west1.firebasedatabase.app/";
    [SerializeField] private string apiKey = "AIzaSyAMco75-nyBYd8Bm8KlDHrbrrXcKP2xMZ8";

    private string playerId;
    private float autoSaveTimer = 0f;
    private const float AUTO_SAVE_INTERVAL = 60f;

    void Start()
    {
        playerId = SystemInfo.deviceUniqueIdentifier;
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            StartCoroutine(LoadPlayerData());
        }
    }

    IEnumerator LoadPlayerData()
    {
        string url = $"{firebaseUrl}players/{playerId}.json?auth={apiKey}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                if (string.IsNullOrEmpty(json) || json == "null")
                {
                    CreateNewUser();
                }
                else
                {
                    ProcessLoadedData(json);
                }
            }
            else
            {
                // İnternet hatası varsa veya veri yoksa yeni kullanıcı gibi davranılabilir
                // Veya offline mod eklenebilir. Şimdilik pas geçiyoruz.
                Debug.LogError("Veri yüklenemedi: " + request.error);
            }
        }
    }

    void CreateNewUser()
    {
        Debug.Log("Yeni Kullanıcı Oluşturuluyor...");

        // Başlangıç Parası (100 Gold)
        if (EconomyManager.Instance)
            EconomyManager.Instance.LoadEconomy(100f, 0f, 0);

        // Fabrika görsellerini sıfırla
        if (FactoryManager.Instance)
            FactoryManager.Instance.RefreshVisuals();

        // Hemen kaydet ki tarih bilgisi veritabanına işlensin
        SavePlayerData();
    }

    void ProcessLoadedData(string json)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // --- 90 GÜN KURALI KONTROLÜ ---
        if (!string.IsNullOrEmpty(data.lastLoginDate))
        {
            // Kayıtlı tarihi parse et (String -> DateTime çevirimi)
            DateTime lastLogin;
            if (DateTime.TryParse(data.lastLoginDate, out lastLogin))
            {
                // Bugün ile son giriş arasındaki farkı bul
                TimeSpan difference = DateTime.Now - lastLogin;

                // Eğer 90 günden fazlaysa
                if (difference.TotalDays > 90)
                {
                    Debug.LogWarning("Kullanıcı 90 gündür girmedi. Veriler siliniyor ve sıfırlanıyor.");
                    CreateNewUser(); // Her şeyi sıfırla ve üzerine yaz
                    return; // Fonksiyonu burada kes, eski veriyi yükleme
                }
            }
        }
        // -----------------------------

        // Veri güncelse yüklemeye devam et
        if (EconomyManager.Instance)
            EconomyManager.Instance.LoadEconomy(data.money, data.plasma, data.assembledRobots);

        if (FactoryManager.Instance)
            FactoryManager.Instance.LoadFactoryData(data);

        // Oyuna girdiği için tarihi hemen güncelle ve kaydet
        SavePlayerData();
    }

    public void SavePlayerData()
    {
        StartCoroutine(SaveCoroutine());
    }

    IEnumerator SaveCoroutine()
    {
        if (EconomyManager.Instance == null) yield break;

        SaveData data = new SaveData
        {
            money = EconomyManager.Instance.Gold,
            plasma = EconomyManager.Instance.Plasma,
            assembledRobots = EconomyManager.Instance.TotalRobots,

            // --- TARİHİ KAYDET ---
            // Şu anki zamanı String olarak kaydet
            lastLoginDate = DateTime.Now.ToString()
            // ---------------------
        };

        if (FactoryManager.Instance) FactoryManager.Instance.FillSaveData(data);

        string json = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        string url = $"{firebaseUrl}players/{playerId}.json?auth={apiKey}";

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
        }
    }

    void Update()
    {
        autoSaveTimer += Time.deltaTime;
        if (autoSaveTimer >= AUTO_SAVE_INTERVAL)
        {
            SavePlayerData();
            autoSaveTimer = 0;
        }
    }

    void OnApplicationQuit() => SavePlayerData();
}
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("KAMERA REFERANSLARI (Sürükle!)")]
    public GameObject earthCamera;
    public GameObject marsCamera;

    [Header("Durum")]
    public bool isMarsActive = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Oyuna Dünya'da baþla
        GoToEarth();
    }

    public void ToggleTravel()
    {
        if (isMarsActive) GoToEarth();
        else GoToMars();
    }

    public void GoToMars()
    {
        isMarsActive = true;

        // Kameralarý deðiþtir
        if (earthCamera) earthCamera.SetActive(false);
        if (marsCamera) marsCamera.SetActive(true);

        // Olay Yayýnla: "Dünya Deðiþti! Yeni yer Mars mý? -> EVET (true)"
        // Bu sayede UI, Ses, Efektler vb. her þey bu haberi alýp kendini ayarlar.
        GameEvents.OnWorldChanged?.Invoke(true);
    }

    public void GoToEarth()
    {
        isMarsActive = false;

        // Kameralarý deðiþtir
        if (earthCamera) earthCamera.SetActive(true);
        if (marsCamera) marsCamera.SetActive(false);

        // Olay Yayýnla: "Dünya Deðiþti! Yeni yer Mars mý? -> HAYIR (false)"
        GameEvents.OnWorldChanged?.Invoke(false);
    }
}
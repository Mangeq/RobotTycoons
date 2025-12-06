using System;

public static class GameEvents
{
    // Para deðiþtiðinde tetiklenir (Hangi para, Yeni miktar)
    public static Action<CurrencyType, float> OnCurrencyChanged;

    // Ürün stoðu deðiþtiðinde tetiklenir
    public static Action OnInventoryChanged;

    // Sahne/Gezegen deðiþtiðinde tetiklenir (Mars mý?)
    public static Action<bool> OnWorldChanged;

    // Fabrika veya Hat açýldýðýnda tetiklenir
    public static Action OnUnlockUpdate;
}
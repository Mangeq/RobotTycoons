using System.Collections.Generic;

// BU ENUM EKSÝK OLDUÐU ÝÇÝN HATA ALIYORSUN
public enum CurrencyType
{
    Gold,
    Plasma
}

[System.Serializable]
public class ProductData
{
    public string id;
    public string displayName;
    public string factoryID;
    public bool isUnlocked;
    public float unlockCost;
    public int inventoryAmount;
}

[System.Serializable]
public class MachineData
{
    public string name;
    public int lvl; // lvl OLARAK SABÝTLEDÝK
    public float price;
    public float multiplier;

    public MachineData(string name, float startPrice, float multiplier)
    {
        this.name = name;
        this.lvl = 1;
        this.price = startPrice;
        this.multiplier = multiplier;
    }
}

[System.Serializable]
public class SaveData
{
    public string lastLoginDate;
    public float money;
    public float plasma;
    public int assembledRobots;
    public bool isDiamondFactoryUnlocked;
    public List<ProductData> products;
    public List<LineSaveModel> lines;
}

[System.Serializable]
public class LineSaveModel
{
    public string lineID;
    public List<MachineData> machines;
}
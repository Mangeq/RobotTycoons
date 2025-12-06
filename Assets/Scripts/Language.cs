using TMPro;
using UnityEngine;

public class Language : MonoBehaviour
{
    [SerializeField] TextAsset currentJson;
    [SerializeField] TextAsset[] languages;
    [SerializeField] TMP_Text comeBackPanel;
    [SerializeField] TMP_Text takeButton;
    [SerializeField] TMP_Text settingsMenuTitle;
    [SerializeField] TMP_Text switchLanguageButton;
    [SerializeField] TMP_Text privacyPoliceButton;
    [SerializeField] TMP_Text upgradeMenuTitle;
    [SerializeField] TMP_Text smelterText;
    [SerializeField] TMP_Text patternText;
    [SerializeField] TMP_Text electronicsText;
    [SerializeField] TMP_Text lineSpeedText;
    [SerializeField] TMP_Text storageMenuTitle;
    [SerializeField] TMP_Text armProductName;
    [SerializeField] TMP_Text legProductName;
    [SerializeField] TMP_Text headProductName;
    [SerializeField] TMP_Text bodyProductName;
    [SerializeField] TMP_Text robotProductName;
    [SerializeField] TMP_Text storeMenuTitle;
    [SerializeField] TMP_Text goldSubTittle;
    [SerializeField] TMP_Text goldFistful;
    [SerializeField] TMP_Text goldBox;
    [SerializeField] TMP_Text goldBucket;
    [SerializeField] TMP_Text goldBarrel;
    [SerializeField] TMP_Text goldWagon;
    [SerializeField] TMP_Text goldMountain;
    [SerializeField] TMP_Text diamondSubTittle;
    [SerializeField] TMP_Text diamondFistful;
    [SerializeField] TMP_Text diamondBox;
    [SerializeField] TMP_Text diamondBucket;
    [SerializeField] TMP_Text diamondBarrel;
    [SerializeField] TMP_Text diamondWagon;
    [SerializeField] TMP_Text diamondMountain;
    public void ChangeLanguage(GameObject ga)
    {
        switch (ga.name)
        {
            case "TurkishButton": currentJson = languages[0]; break;
            case "EnglishButton": currentJson = languages[1]; break;
            case "DeutschButton": currentJson = languages[2]; break;
            case "RussianButton": currentJson = languages[3]; break;
            case "ChineseButton": currentJson = languages[4]; break;
            case "FrenchButton": currentJson = languages[5]; break;
        }
        LanguageData data = new LanguageData();
        data = JsonUtility.FromJson<LanguageData>(currentJson.text);
        UpdateDisplay(data);
    }
    public void UpdateDisplay(LanguageData data)
    {
        if (data != null)
        {
            comeBackPanel.text = data.comeBackPanel;
            takeButton.text = data.takeButton;
            settingsMenuTitle.text = data.settingsMenuTitle;
            switchLanguageButton.text = data.switchLanguageButton;
            privacyPoliceButton.text = data.privacyPoliceButton;
            upgradeMenuTitle.text = data.upgradeMenuTitle;
            smelterText.text = data.smelterUpButton;
            patternText.text = data.patternUpButton;
            electronicsText.text = data.electronicsUpButton;
            lineSpeedText.text = data.lineSpeedUpButton;
            storageMenuTitle.text = data.storageMenuTitle;
            armProductName.text = data.armProductName;
            legProductName.text = data.legProductName;
            headProductName.text = data.headProductName;
            bodyProductName.text = data.bodyProductName;
            robotProductName.text = data.robotProductName;
            storeMenuTitle.text = data.storeMenuTitle;
            goldSubTittle.text = data.goldSubTittle;
            goldFistful.text = data.goldFistful;
            goldBox.text = data.goldBox;
            goldBucket.text = data.goldBucket;
            goldBarrel.text = data.goldBarrel;
            goldWagon.text = data.goldWagon;
            goldMountain.text = data.goldMountain;
            diamondSubTittle.text = data.diamondSubTittle;
            diamondFistful.text = data.diamondFistful;
            diamondBox.text = data.diamondBox;
            diamondBucket.text = data.diamondBucket;
            diamondBarrel.text = data.diamondBarrel;
            diamondWagon.text = data.diamondWagon;
            diamondMountain.text = data.diamondMountain;
        } 
    }
}
public class LanguageData
{
    public string comeBackPanel;
    public string takeButton;
    public string settingsMenuTitle;
    public string switchLanguageButton;
    public string privacyPoliceButton;
    public string upgradeMenuTitle;
    public string smelterUpButton;
    public string patternUpButton;
    public string electronicsUpButton;
    public string lineSpeedUpButton;
    public string upgrade;
    public string storageMenuTitle;
    public string armProductName;
    public string legProductName;
    public string headProductName;
    public string bodyProductName;
    public string robotProductName;
    public string storeMenuTitle;
    public string goldSubTittle;
    public string goldFistful;
    public string goldBox;
    public string goldBucket;
    public string goldBarrel;
    public string goldWagon;
    public string goldMountain;
    public string diamondSubTittle;
    public string diamondFistful;
    public string diamondBox;
    public string diamondBucket;
    public string diamondBarrel;
    public string diamondWagon;
    public string diamondMountain;
}
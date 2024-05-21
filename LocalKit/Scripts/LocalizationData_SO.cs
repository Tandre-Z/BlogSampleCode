using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationData_SO", menuName = "Inventory/LocalizationData_SO")]
public class LocalizationData_SO : ScriptableObject
{
    public List<LocalizationItem> localizationItemList;

    public LocalizationItem GetlocalizationItem(string key)
    {
        return localizationItemList.Find(i => i.Name == key);
    }
    private void OnValidate()
    {
        UpdateSentenceNames();
    }
    private void UpdateSentenceNames()
    {
        foreach (var localizationItem in localizationItemList)
        {
            foreach (var sentence in localizationItem.localizedSentence)
            {
                sentence.Name = sentence.language + " - " + sentence.Text;
            }
        }
    }
}

[Serializable]
public class LocalizationItem
{
    public string Name;
    public List<LocalizedSentence> localizedSentence;

    public string GetSentence(Language language)
    {
        return localizedSentence.Find(i => i.language == language).Text;
    }
}

[Serializable]
public class LocalizedSentence
{
    [HideInInspector]
    public string Name;
    public Language language;

    [TextArea(0, int.MaxValue)]
    public string Text;
}
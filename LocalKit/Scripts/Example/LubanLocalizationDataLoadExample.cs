using UnityEngine;
using SimpleJSON;
using System.IO;
public class LubanLocalizationDataLoadExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var tables = new cfg.Tables(fileName => JSON.Parse(File.ReadAllText($"Assets/GameDesign/13.LocalKit/StreamingAssets/GenData/{fileName}.json")));
        var about = tables.Localization.DataMap[Language.Chinese.ToString()].LocalizationItems.About;
        Debug.Log(about);
        var menu = tables.Localization.DataMap[Language.English.ToString()].LocalizationItems.Menu;
        Debug.Log(menu);
    }
}

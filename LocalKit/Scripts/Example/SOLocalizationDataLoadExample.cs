using System.Collections;
using System.Collections.Generic;
using LocalKit;
using UnityEngine;

public class SOLocalizationDataLoadExample : MonoBehaviour
{
    [SerializeField] private LocalizationData_SO localizationData_SO;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(localizationData_SO.GetlocalizationItem(LocalKitKeys.MENU).GetSentence(Language.Chinese));
        Debug.Log(localizationData_SO.GetlocalizationItem(LocalKitKeys.ABOUT).GetSentence(Language.English));
    }

    // Update is called once per frame
    void Update()
    {

    }
}

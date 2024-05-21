using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using System.IO;


public class LocalKitConfigEditor : EditorWindow
{
    private readonly string LocalKitConfigEditorPath = "Assets/GameDesign/13.LocalKit/Editor/Builder/LocalKitConfigEditor.uxml";
    private readonly string LocalizationItemTemplatePath = "Assets/GameDesign/13.LocalKit/Editor/Builder/LocalizationItem.uxml";
    private readonly string LocalizedSentenceTemplatePath = "Assets/GameDesign/13.LocalKit/Editor/Builder/LocalizedSentence.uxml";
    private LocalizationData_SO dataBase;
    private List<LocalizationItem> localizationItemList;//实际数据库中的列表
    private List<LocalizationItem> localizationItemListSearched;//被搜索工具条筛选后的列表
    private List<LocalizationItem> currentlocalizationItemListSource;//缓存下来的当前要展示的列表

    private VisualTreeAsset localizationItemTemplate;
    private VisualTreeAsset localizedSentenceTemplate;

    private LocalizationItem activeLocalizationItem;

    //VisualElement
    private VisualElement localizationItemListViewElement;
    private ListView localizationItemListView;
    private ToolbarSearchField toolbarSearchFieldView;
    private VisualElement sentenceListViewElement;
    private TextField sentenceTextField;
    private ListView sentenceListView;

    [MenuItem("UIEditor/LocalKitConfigEditor")]
    public static void ShowExample()
    {
        LocalKitConfigEditor wnd = GetWindow<LocalKitConfigEditor>();
        wnd.titleContent = new GUIContent("LocalKitConfigEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LocalKitConfigEditorPath);
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        //拿列表模板数据
        localizationItemTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LocalizationItemTemplatePath);
        localizedSentenceTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(LocalizedSentenceTemplatePath);
        if (localizationItemTemplate == null)
        {
            Debug.LogError("LocalizationItemListTemplate is null");
            return;
        }

        localizationItemListViewElement = root.Q<VisualElement>("LocalizationItemList");
        localizationItemListView = localizationItemListViewElement.Q<ListView>("ListView");
        toolbarSearchFieldView = localizationItemListViewElement.Q<ToolbarSearchField>("SearchField");
        toolbarSearchFieldView.RegisterValueChangedCallback(OnToolbarSearchFieldChanged);
        sentenceListViewElement = root.Q<VisualElement>("SentenceList");
        sentenceTextField = sentenceListViewElement.Q<VisualElement>("Header").Q<TextField>("TextField");
        sentenceListView = sentenceListViewElement.Q<ListView>("ListView");



        root.Q<Button>("BtnAddLocalizationItem").clicked += AddLocalizationItem;
        root.Q<Button>("BtnDeleteLocalizationItem").clicked += DelectLocalizationItem;
        root.Q<Button>("BtnAddLocalizedSentence").clicked += AddLocalizedSentence;
        root.Q<Button>("BtnDeleteLocalizedSentence").clicked += DeleteLocalizedSentence;
        root.Q<Button>("BtnSave").clicked += SaveLocalizationItemKey;

        LadDateBase();

        GenerateListView();
    }



    private void OnToolbarSearchFieldChanged(ChangeEvent<string> evt)
    {
        if (string.IsNullOrEmpty(evt.newValue))
        {
            currentlocalizationItemListSource = localizationItemList;
            localizationItemListView.itemsSource = localizationItemList;
            return;
        }
        localizationItemListSearched = new List<LocalizationItem>();
        for (int i = 0; i < localizationItemList.Count; i++)
        {
            if (localizationItemList[i].Name.Contains(evt.newValue))
            {
                localizationItemListSearched.Add(localizationItemList[i]);
            }
        }
        currentlocalizationItemListSource = localizationItemListSearched;
        localizationItemListView.itemsSource = localizationItemListSearched;
    }

    #region 按钮事件
    private void DelectLocalizationItem()
    {
        localizationItemList.Remove(activeLocalizationItem);
        localizationItemListView.Rebuild();
        sentenceListViewElement.visible = false;
    }

    private void AddLocalizationItem()
    {
        LocalizationItem localizationItem = new LocalizationItem();
        localizationItem.Name = "New LocalizationItem";
        localizationItem.localizedSentence = new List<LocalizedSentence>();
        localizationItemList.Add(localizationItem);
        localizationItemListView.Rebuild();
    }
    private void AddLocalizedSentence()
    {
        LocalizedSentence sentence = new LocalizedSentence();
        sentence.Text = "New Sentence";
        sentence.language = Language.English;
        activeLocalizationItem.localizedSentence.Add(sentence);
        sentenceListView.Rebuild();
    }


    private void DeleteLocalizedSentence()
    {
        activeLocalizationItem.localizedSentence.Remove(activeSentence);
        Debug.Log("DeleteLocalizedSentence:" + activeSentence.Text);
        sentenceListView.Rebuild();
    }
    private void SaveLocalizationItemKey()
    {
        activeLocalizationItem.Name = sentenceTextField.text;
        localizationItemListView.Rebuild();
        var dabeBasePath = AssetDatabase.GetAssetPath(dataBase);
        var directory = Path.GetDirectoryName(dabeBasePath);
        LocalKitKeysCodeGenerator.GeneratorKeyClassFile(localizationItemList.Select(x => x.Name).ToList(), Path.Combine(directory, "LocalKitKeys.cs"));
    }

    #endregion
    private void LadDateBase()
    {
        var assets = AssetDatabase.FindAssets("LocalizationData_SO");
        foreach (var item in assets)
        {
            var path = AssetDatabase.GUIDToAssetPath(item);
            var possibleSO = AssetDatabase.LoadAssetAtPath(path, typeof(LocalizationData_SO)) as LocalizationData_SO;
            if (possibleSO != null)
            {
                dataBase = possibleSO;
                break;
            }
        }
        localizationItemList = dataBase.localizationItemList;
        currentlocalizationItemListSource = localizationItemList;
    }

    private void GenerateListView()
    {
        Func<VisualElement> makelocalizationItem = () => localizationItemTemplate.CloneTree();
        Action<VisualElement, int> bindlocalizationItem = (e, index) =>
        {
            if (index < currentlocalizationItemListSource.Count)
            {
                e.Q<Label>("Label").text = currentlocalizationItemListSource[index] == null ? "NO localizationItem" : currentlocalizationItemListSource[index].Name;
            }
        };

        localizationItemListView.fixedItemHeight = 30;
        localizationItemListView.itemsSource = localizationItemList;
        localizationItemListView.makeItem = makelocalizationItem;
        localizationItemListView.bindItem = bindlocalizationItem;

        localizationItemListView.onSelectionChange += OnListSelectionChang;

        sentenceListViewElement.visible = false;
    }

    private void OnListSelectionChang(IEnumerable<object> enumerable)
    {
        activeLocalizationItem = (LocalizationItem)enumerable.First();
        GetLocalizationItem();
        localizationItemListView.visible = true;
    }

    private void GetLocalizationItem()
    {
        localizationItemListView.MarkDirtyRepaint();
        sentenceListViewElement.visible = true;

        sentenceTextField.value = activeLocalizationItem.Name;

        Func<VisualElement> makeSentence = () => localizedSentenceTemplate.CloneTree();
        Action<VisualElement, int> bindSentence = (e, index) =>
        {
            if (index < activeLocalizationItem.localizedSentence.Count)
            {
                e.Q<TextField>("Content").value = activeLocalizationItem.localizedSentence[index].Text;
                e.Q<TextField>("Content").RegisterValueChangedCallback(evt =>
                {
                    if (activeLocalizationItem.localizedSentence[index].Text != evt.newValue)
                    {
                        activeLocalizationItem.localizedSentence[index].Text = evt.newValue;
                    }
                });
                e.Q<EnumField>("Language").Init(activeLocalizationItem.localizedSentence[index].language);
                e.Q<EnumField>("Language").value = activeLocalizationItem.localizedSentence[index].language;
                e.Q<EnumField>("Language").RegisterValueChangedCallback(evt =>
                {
                    Debug.Log("Language:" + evt.newValue);
                    activeLocalizationItem.localizedSentence[index].language = (Language)evt.newValue;
                });
            }
        };
        sentenceListView.fixedItemHeight = 80;
        sentenceListView.itemsSource = activeLocalizationItem.localizedSentence;
        sentenceListView.makeItem = makeSentence;
        sentenceListView.bindItem = bindSentence;

        sentenceListView.onSelectionChange += OnSentenceListSelectionChange;
    }

    LocalizedSentence activeSentence;
    private void OnSentenceListSelectionChange(IEnumerable<object> enumerable)
    {
        activeSentence = (LocalizedSentence)enumerable.First();
    }

}
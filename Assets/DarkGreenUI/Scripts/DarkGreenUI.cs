using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DarkGreenUI : MonoBehaviour{

    public static string assetsPath = "Assets/DarkGreenUI/Prefabs/";

    public static GameObject textBackground = AssetDatabase.LoadAssetAtPath(assetsPath + "TextBackground.prefab", typeof(GameObject)) as GameObject;

    public static GameObject button_red = AssetDatabase.LoadAssetAtPath(assetsPath + "Button_red.prefab", typeof(GameObject)) as GameObject;
    public static GameObject button_dark_red = AssetDatabase.LoadAssetAtPath(assetsPath + "Button_dark_red.prefab", typeof(GameObject)) as GameObject;
    public static GameObject button_yellow = AssetDatabase.LoadAssetAtPath(assetsPath + "Button_yellow.prefab", typeof(GameObject)) as GameObject;
    public static GameObject button_green = AssetDatabase.LoadAssetAtPath(assetsPath + "Button_green.prefab", typeof(GameObject)) as GameObject;

    public static GameObject toggle = AssetDatabase.LoadAssetAtPath(assetsPath + "Toggle.prefab", typeof(GameObject)) as GameObject;
    public static GameObject slider = AssetDatabase.LoadAssetAtPath(assetsPath + "Slider.prefab", typeof(GameObject)) as GameObject;
    public static GameObject scrollbar = AssetDatabase.LoadAssetAtPath(assetsPath + "Scrollbar.prefab", typeof(GameObject)) as GameObject;
    public static GameObject dropdown = AssetDatabase.LoadAssetAtPath(assetsPath + "Dropdown.prefab", typeof(GameObject)) as GameObject;

    public static GameObject inputfield = AssetDatabase.LoadAssetAtPath(assetsPath + "InputField.prefab", typeof(GameObject)) as GameObject;
    public static GameObject inputfield_password = AssetDatabase.LoadAssetAtPath(assetsPath + "InputFieldPassword.prefab", typeof(GameObject)) as GameObject;
    public static GameObject inputfield_password_wrong = AssetDatabase.LoadAssetAtPath(assetsPath + "InputFieldPassword_wrong.prefab", typeof(GameObject)) as GameObject;
    public static GameObject inputfield_password_warning = AssetDatabase.LoadAssetAtPath(assetsPath + "InputFieldPassword_warning.prefab", typeof(GameObject)) as GameObject;
    public static GameObject inputfield_search = AssetDatabase.LoadAssetAtPath(assetsPath + "SearchField.prefab", typeof(GameObject)) as GameObject;

    public static GameObject switchButton = AssetDatabase.LoadAssetAtPath(assetsPath + "SwitchLabel.prefab", typeof(GameObject)) as GameObject;
    public static GameObject scrollView = AssetDatabase.LoadAssetAtPath(assetsPath + "ScrollView.prefab", typeof(GameObject)) as GameObject;
    public static GameObject toggleGroup = AssetDatabase.LoadAssetAtPath(assetsPath + "ToggleGroup.prefab", typeof(GameObject)) as GameObject;

    public static GameObject message_correct = AssetDatabase.LoadAssetAtPath(assetsPath + "Correct.prefab", typeof(GameObject)) as GameObject;
    public static GameObject message_error = AssetDatabase.LoadAssetAtPath(assetsPath + "Error.prefab", typeof(GameObject)) as GameObject;
    public static GameObject message_warning = AssetDatabase.LoadAssetAtPath(assetsPath + "Warning.prefab", typeof(GameObject)) as GameObject;

    public static GameObject message_under = AssetDatabase.LoadAssetAtPath(assetsPath + "InformationUnderText.prefab", typeof(GameObject)) as GameObject;

    public static GameObject loadingBar_green = AssetDatabase.LoadAssetAtPath(assetsPath + "LoadingBar_green.prefab", typeof(GameObject)) as GameObject;
    public static GameObject loadingBar_red = AssetDatabase.LoadAssetAtPath(assetsPath + "LoadingBar_red.prefab", typeof(GameObject)) as GameObject;

    public static GameObject window_layout1 = AssetDatabase.LoadAssetAtPath(assetsPath + "Window_layout1.prefab", typeof(GameObject)) as GameObject;
    public static GameObject window_layout2 = AssetDatabase.LoadAssetAtPath(assetsPath + "Window_layout2.prefab", typeof(GameObject)) as GameObject;

    [MenuItem("GameObject/DarkGreen UI/Text background", false, 0)]
    static void Add1(MenuCommand menuCommand) {CreateElement(textBackground, menuCommand);}

    [MenuItem("GameObject/DarkGreen UI/Buttons/Red", false, 0)]
    static void Add2(MenuCommand menuCommand) { CreateElement(button_red, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Buttons/Dark Red", false, 0)]
    static void Add3(MenuCommand menuCommand) { CreateElement(button_dark_red, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Buttons/Yellow", false, 0)]
    static void Add4(MenuCommand menuCommand) { CreateElement(button_yellow, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Buttons/Green", false, 0)]
    static void Add5(MenuCommand menuCommand) { CreateElement(button_green, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Toggle", false, 0)]
    static void Add6(MenuCommand menuCommand) { CreateElement(toggle, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Slider", false, 0)]
    static void Add7(MenuCommand menuCommand) { CreateElement(slider, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Scrollbar", false, 0)]
    static void Add8(MenuCommand menuCommand) { CreateElement(scrollbar, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Dropdown", false, 0)]
    static void Add9(MenuCommand menuCommand) { CreateElement(dropdown, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Input Fields/Standard", false, 0)]
    static void Add10(MenuCommand menuCommand) { CreateElement(inputfield, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Input Fields/Password", false, 0)]
    static void Add11(MenuCommand menuCommand) { CreateElement(inputfield_password, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Input Fields/Wrong Password", false, 0)]
    static void Add12(MenuCommand menuCommand) { CreateElement(inputfield_password_wrong, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Input Fields/Warning Password", false, 0)]
    static void Add13(MenuCommand menuCommand) { CreateElement(inputfield_password_warning, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Input Fields/Search", false, 0)]
    static void Add14(MenuCommand menuCommand) { CreateElement(inputfield_search, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Switch", false, 0)]
    static void Add15(MenuCommand menuCommand) { CreateElement(switchButton, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Scroll View", false, 0)]
    static void Add16(MenuCommand menuCommand) { CreateElement(scrollView, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Toggle Group", false, 0)]
    static void Add17(MenuCommand menuCommand) { CreateElement(toggleGroup, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Messages/Correct", false, 0)]
    static void Add18(MenuCommand menuCommand) { CreateElement(message_correct, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Messages/Error", false, 0)]
    static void Add19(MenuCommand menuCommand) { CreateElement(message_error, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Messages/Warning", false, 0)]
    static void Add20(MenuCommand menuCommand) { CreateElement(message_warning, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Messages/Under Label", false, 0)]
    static void Add21(MenuCommand menuCommand) { CreateElement(message_under, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Loading Bars/Green", false, 0)]
    static void Add22(MenuCommand menuCommand) { CreateElement(loadingBar_green, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Loading Bars/Red", false, 0)]
    static void Add23(MenuCommand menuCommand) { CreateElement(loadingBar_red, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Windows/Layout1", false, 0)]
    static void Add24(MenuCommand menuCommand) { CreateElement(window_layout1, menuCommand); }

    [MenuItem("GameObject/DarkGreen UI/Windows/Layout2", false, 0)]
    static void Add25(MenuCommand menuCommand) { CreateElement(window_layout2, menuCommand); }

    public static void CreateElement(GameObject gameObject, MenuCommand menuCommand) {
        GameObject go = Instantiate(gameObject) as GameObject;
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        go.name = go.name.Split('(')[0];
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}

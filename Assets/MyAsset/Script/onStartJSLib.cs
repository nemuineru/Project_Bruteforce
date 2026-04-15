using System.Runtime.InteropServices;
using UnityEngine;

public class onStartJSLib : MonoBehaviour
{
    [DllImport("__Internal")]
    public static extern void InjectionJs(string url);

    [DllImport("__Internal")]
    public static extern void InjectionCSS(string url);

    [DllImport("__Internal")]
    private static extern void SwalFireBase(string body);

    void Awake()
    {
        //setInGL();
    }

    void setInGL()
    {
        #if !UNITY_EDITOR && UNITY_WEBGL
        {
            var url = "https://cdn.jsdelivr.net/gh/nemuineru/Project_Bruteforce@main/Assets/MyAsset/Script/runtimeResources/puerts-runtime.js";
            InjectionJs(url);
            url = "https://cdn.jsdelivr.net/gh/nemuineru/Project_Bruteforce@main/Assets/MyAsset/Script/runtimeResources/puerts_browser_js_resources.js";
            InjectionJs(url);
        }
        #endif
    }

    public void ShowMessage(string body)
    {
        SwalFireBase(body);
    }
}
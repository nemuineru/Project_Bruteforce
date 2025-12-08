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
        #if !UNITY_EDITOR && UNITY_WEBGL
        {
            var url = "https://cdn.jsdelivr.net/npm/sweetalert2@11";
            InjectionJs(url);
        }
        #endif
    }

    public void ShowMessage(string body)
    {
        SwalFireBase(body);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShaderNameHash  {
    public static int MainTex;
    public static int RectValue;
    public static int ExpectedRect;
    public static int LightColor0;
    public static int WorldSpaceLightPos0;
    public static int TintColor;

    [RuntimeInitializeOnLoadMethod]
    public static void Initialize()
    {
        MainTex = Shader.PropertyToID("_MainTex");
        RectValue = Shader.PropertyToID("_RectValue");
        ExpectedRect = Shader.PropertyToID("_ExpectedRect");
        LightColor0 = Shader.PropertyToID("_LightColor0");
        WorldSpaceLightPos0 = Shader.PropertyToID("_WorldSpaceLightPos0");
        TintColor = Shader.PropertyToID("_Color");


    }

    public static Color[] ColorList = new Color[] { Color.white, Color.red, Color.blue, Color.yellow, Color.cyan, Color.grey };

}

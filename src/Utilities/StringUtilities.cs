using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StringUtilities
{
    public static string AsPercent<T>(this T s) => string.Format("{0:P}", s);

    public static string AsColor(this string s, Color color) =>
        $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{s}</color>";

    public static string FormatTime(this float time) =>
        TimeSpan.FromSeconds(time).ToString("mm':'ss");

    public static string AsSign(this float f) => f >= 0.0 ? "" : "-";

    public static string AsSign(this int i) => i >= 0 ? "" : "-";
}

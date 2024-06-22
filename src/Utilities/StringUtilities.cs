using System;
using UnityEngine;

public static class StringUtilities
{
    public static string AsPercent<T>(this T s) => string.Format("{0:P}", s);

    // courtesy https://stackoverflow.com/a/2412387
    public static string AsShorthand(this int num)
    {
        if (num >= 100000000)
            return (num / 1000000).ToString("#,0M");

        if (num >= 10000000)
            return (num / 1000000).ToString("0.#") + "M";

        if (num >= 100000)
            return (num / 1000).ToString("#,0K");

        if (num >= 10000)
            return (num / 1000).ToString("0.#") + "K";

        return num.ToString("#,0");
    }

    public static string AsColor(this string s, Color color) =>
        $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{s}</color>";

    public static string FormatTime(this float time) =>
        TimeSpan.FromSeconds(time).ToString("mm':'ss");

    public static string AsSign(this float f) => f >= 0.0 ? "" : "-";

    public static string AsSign(this int i) => i >= 0 ? "" : "-";
}

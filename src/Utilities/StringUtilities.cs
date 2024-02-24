using System;

public static class StringUtilities
{
    public static string AsPercent<T>(this T s) => string.Format("{0:P}%", s);
}

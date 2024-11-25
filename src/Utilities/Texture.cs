using UnityEngine;

public static class TextureUtilities
{
    public static Texture2D Create(int width, int height, Color color)
    {
        var pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = color;
        }

        var result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}

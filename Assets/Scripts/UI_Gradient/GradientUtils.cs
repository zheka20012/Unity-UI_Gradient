using UnityEngine;

public sealed class GradientUtils
{
    public static Texture CreateTexture(UnityEngine.Gradient gradient, int width, int height, bool invert)
    {
        if (gradient == null) return Texture2D.whiteTexture;
        var gradTex = new Texture2D(1, height, TextureFormat.ARGB32, false);
        gradTex.hideFlags = HideFlags.DontSave;
        for (int y = 0; y < height; y++)
        {
            float t = Mathf.Clamp01((float)(y / (float)height));
            Color col = gradient.Evaluate(t);
            gradTex.SetPixel(0, invert ? height - y : y, col);
        }
        gradTex.Apply();
        gradTex.filterMode = FilterMode.Bilinear;
        return gradTex;
    }
}

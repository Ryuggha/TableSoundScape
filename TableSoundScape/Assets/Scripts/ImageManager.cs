using System.IO;
using UnityEngine;

public static class ImageManager
{
    public static Texture2D LoadTextureFromFile(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        return ResizeAndCrop(tex);
    }

    public static Texture2D ResizeAndCrop(Texture2D source, int size = 120)
    {
        int minDimension = Mathf.Min(source.width, source.height);

        // Crop to square (centered)
        int x = (source.width - minDimension) / 2;
        int y = (source.height - minDimension) / 2;
        Color[] pixels = source.GetPixels(x, y, minDimension, minDimension);

        Texture2D cropped = new Texture2D(minDimension, minDimension);
        cropped.SetPixels(pixels);
        cropped.Apply();

        // Scale to 250x250
        Texture2D scaled = new Texture2D(size, size, TextureFormat.RGBA32, false);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float u = (float)i / (size - 1);
                float v = (float)j / (size - 1);
                scaled.SetPixel(i, j, cropped.GetPixelBilinear(u, v));
            }
        }
        scaled.Apply();

        return scaled;
    }

    public static Sprite TextureToSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    public static string EncodeToJson(Texture2D tex)
    {
        byte[] pngData = tex.EncodeToPNG();
        string base64 = System.Convert.ToBase64String(pngData);

        return base64;
    }

    public static Sprite DecodeFromJson(string base64)
    {
        byte[] pngData = System.Convert.FromBase64String(base64);

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(pngData);
        return TextureToSprite(tex);
    }
}


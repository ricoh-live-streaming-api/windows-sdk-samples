using System;
using System.IO;
using UnityEngine;

public class Utils
{
    private Utils() { }

    public static string CreateFilePath(string path)
    {
        var dt = DateTime.Now;
        var fileName = dt.ToString("yyyyMMdd'T'HHmm") + ".log";

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path + "/" + fileName;
    }

    public static Texture2D CreateTexture(int width, int height)
    {
        return new Texture2D(width, height, TextureFormat.RGBA32, false);
    }

    public static void AdjustAspect(GameObject videoPlane, int width, int height)
    {
        float plane_x = Mathf.Abs(videoPlane.transform.lossyScale.x) * 10.0f;
        float plane_y = Mathf.Abs(videoPlane.transform.lossyScale.z) * 10.0f;
        float image_x = (float)width;
        float image_y = (float)height;

        float planeAspect = plane_x / plane_y;
        float imageAspect = image_x / image_y;

        var tiling_x = 1.0f;
        var tiling_y = 1.0f;
        var Offset_x = 1.0f;
        var Offset_y = 1.0f;

        if (imageAspect > planeAspect)
        {
            tiling_x = planeAspect / imageAspect;
            Offset_x = (image_x - (plane_x * image_y / plane_y)) / (2 * image_x);
        }
        else
        {
            tiling_y = imageAspect / planeAspect;
            Offset_y = (image_y - (plane_y * image_x / plane_x)) / (2 * image_y);
        }

        var material = videoPlane.GetComponent<Renderer>().material;
        material.SetTextureScale("_MainTex", new Vector2(tiling_x, tiling_y));
        material.SetTextureOffset("_MainTex", new Vector2(Offset_x, Offset_y));
    }
}

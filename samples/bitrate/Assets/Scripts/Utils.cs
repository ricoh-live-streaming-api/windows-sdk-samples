using UnityEngine;
using UnityEngine.UI;

public class Utils
{
    private Utils() { }

    public static Texture2D CreateTexture(int width, int height)
    {
        return new Texture2D(width, height, TextureFormat.RGBA32, false);
    }

    public static void DestroyTexture(Texture texture)
    {
        if (texture != null)
        {
            MonoBehaviour.Destroy(texture);
        }
    }

    public static Vector2 GetRectSize(GameObject gameObject)
    {
        RectTransform rt = gameObject.transform as RectTransform;
        return rt.sizeDelta;
    }

    public static void AdjustAspect(RawImage image, Vector2 originalSize, int width, int height)
    {
        Vector2 textureSize = new Vector2(width, height);

        var heightScale = originalSize.y / height;
        var widthScale = originalSize.x / width;
        Vector2 rectSize = textureSize * Mathf.Min(heightScale, widthScale);

        Vector2 anchorDiff = image.rectTransform.anchorMax - image.rectTransform.anchorMin;
        Vector2 parentSize = (image.transform.parent as RectTransform).rect.size;
        Vector2 anchorSize = parentSize * anchorDiff;

        image.rectTransform.sizeDelta = rectSize - anchorSize;
    }
}

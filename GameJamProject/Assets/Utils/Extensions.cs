using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public static class ToggleGroupExtension
{
    public static Toggle GetActive(this ToggleGroup aGroup)
    {
        return aGroup.ActiveToggles().FirstOrDefault();
    }
}

public static class Vector2MapInUIRectExtension {
    public static Vector2 PercentPosInUIRect(this Vector2 pos, Rect ScreenRect) {
        return new Vector2(pos.x.Map(ScreenRect.xMin, ScreenRect.xMax, 0.0f, 1.0f), pos.y.Map(ScreenRect.yMin, ScreenRect.yMax, 0.0f, 1.0f));
    }

    public static float Map(this float num, float minA, float maxA, float minB, float maxB) {
        float percent = (num-minA)/(maxA - minA);
        return (percent * (maxB-minB)) + minB;
    }
}

public static class RectTransformExtensions
{
    public static void SetDefaultScale(this RectTransform trans) {
        trans.localScale = new Vector3(1, 1, 1);
    }
    public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec) {
        trans.pivot = aVec;
        trans.anchorMin = aVec;
        trans.anchorMax = aVec;
    }

    public static Vector2 GetSize(this RectTransform trans) {
        return trans.rect.size;
    }
    public static float GetWidth(this RectTransform trans) {
        return trans.rect.width;
    }
    public static float GetHeight(this RectTransform trans) {
        return trans.rect.height;
    }

    public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos) {
        trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
    }

    public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos) {
        trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
    }
    public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos) {
        trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
    }
    public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos) {
        trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
    }
    public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos) {
        trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
    }

    public static void SetSize(this RectTransform trans, Vector2 newSize) {
        Vector2 oldSize = trans.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
        trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
    }
    public static void SetWidth(this RectTransform trans, float newSize) {
        SetSize(trans, new Vector2(newSize, trans.rect.size.y));
    }
    public static void SetHeight(this RectTransform trans, float newSize) {
        SetSize(trans, new Vector2(trans.rect.size.x, newSize));
    }

    public static Rect GetScreenRect(this RectTransform rectTransform, Canvas canvas) {
        
        Vector3[] corners = new Vector3[4];
        Vector3[] screenCorners = new Vector3[2];
 
        rectTransform.GetWorldCorners(corners);
 
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
        {
            screenCorners[0] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
            screenCorners[1] = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);
        }
        else
        {
            screenCorners[0] = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
            screenCorners[1] = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
        }
 
        screenCorners[0].y = Screen.height - screenCorners[0].y;
        screenCorners[1].y = Screen.height - screenCorners[1].y;
 
        return new Rect(screenCorners[0], screenCorners[1] - screenCorners[0]);
    }
}
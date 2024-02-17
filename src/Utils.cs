using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate bool SelectWhereAction<T>(T inVal, out T var);
public delegate K SelectedEnumerable<T, K>(T input, int idx);

public static class Utilities
{
    public static void SetBool(this Material m, string name, bool value) =>
        m.SetFloat(name, Convert.ToSingle(value));

    public static void PositionFrom(this GameObject go, GameObject target)
    {
        go.transform.position = target.transform.position;
    }

    public static void PositionFrom(this GameObject go, Component target)
    {
        go.transform.position = target.transform.position;
    }

    public static void PositionFrom(this Component go, Component target)
    {
        go.transform.position = target.transform.position;
    }

    public static void PositionFrom(this Component go, GameObject target)
    {
        go.transform.position = target.transform.position;
    }

    public static T Random<T>(this IEnumerable<T> list)
    {
        var l = list.ToList();
        return l[UnityEngine.Random.Range(0, l.Count)];
    }

    public static float Randf() => UnityEngine.Random.Range(0.0f, 1.0f);

    public static void WaitThen(this MonoBehaviour t, float time, Action then)
    {
        t.StartCoroutine(WaitAndThen(time, then));
    }

    private static IEnumerator WaitAndThen(float time, Action then)
    {
        yield return new WaitForSeconds(time);
        then();
    }

    public static Vector3 ToVec3(this Vector2 v) => new Vector3(v.x, 0.0f, v.y);

    public static IEnumerable<T> Tap<T>(this IEnumerable<T> tapped, Action<T> cb)
    {
        foreach (var el in tapped)
        {
            cb(el);
        }
        return tapped;
    }

    public static IEnumerable<T> Enumerate<T>(this IEnumerable<T> enumerator, Action<T, int> cb)
    {
        int count = 0;
        foreach (var el in enumerator)
        {
            cb(el, count);
            count++;
        }

        return enumerator;
    }

    public static IEnumerable<K> SelectEnumerate<T, K>(
        this IEnumerable<T> enumerator,
        SelectedEnumerable<T, K> cb
    )
    {
        int count = 0;
        List<K> enumerated = new();
        foreach (var el in enumerator)
        {
            enumerated.Add(cb(el, count));
            count++;
        }

        return enumerated;
    }

    public static void ForEach<T>(this IEnumerable<T> e, Action<T> cb)
    {
        foreach (var el in e)
        {
            cb(el);
        }
    }

    public static void Defer(this MonoBehaviour m, Action cb)
    {
        m.StartCoroutine(_Defer(cb));
    }

    private static IEnumerator _Defer(Action cb)
    {
        yield return new WaitForEndOfFrame();
        cb();
    }

    public static List<T> SelectWhere<T>(this IEnumerable<T> en, SelectWhereAction<T> cb)
    {
        List<T> final = new();

        foreach (T v in en)
        {
            if (cb(v, out T outVar))
            {
                final.Add(outVar);
            }
        }

        return final;
    }

    public static Bounds CalculateBounds(this GameObject go)
    {
        Bounds bounds = new();

        if (go.TryGetComponent(out Renderer r))
        {
            bounds = r.bounds;
        }

        foreach (var renderer in go.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds;
    }

    public static void SetLayerForMeAndChildren(this GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            child.gameObject.layer = layer;
            SetLayerForMeAndChildren(child.gameObject, layer);
        }
    }

    public static void LocalReset(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public static Vector3 FieldMultiply(this Vector3 v, Vector3 o) =>
        new Vector3(v.x * o.x, v.y * o.y, v.z * o.z);

    public static Vector3Int FloorToInt(this Vector3 v) =>
        new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));

    public static Vector3Int RoundToInt(this Vector3 v) =>
        new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));

    public static Vector3Int CeilToInt(this Vector3 v) =>
        new Vector3Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));

    public static Vector2 To2D(this Vector3 v) => new Vector2(v.x, v.z);

    // yoinked from here: https://discussions.unity.com/t/is-there-an-easy-way-to-get-on-screen-render-size-bounds/15884/4
    public static Rect GetScreenRect(this MeshRenderer mesh)
    {
        Vector3 cen = mesh.bounds.center;
        Vector3 ext = mesh.bounds.extents;
        Camera cam = Camera.main;

        Vector2 min = cam.WorldToScreenPoint(
            new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z - ext.z)
        );
        Vector2 max = min;

        //0
        Vector2 point = min;
        min = new Vector2(min.x >= point.x ? point.x : min.x, min.y >= point.y ? point.y : min.y);
        max = new Vector2(max.x <= point.x ? point.x : max.x, max.y <= point.y ? point.y : max.y);

        //1
        point = cam.WorldToScreenPoint(new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z - ext.z));
        min = new Vector2(min.x >= point.x ? point.x : min.x, min.y >= point.y ? point.y : min.y);
        max = new Vector2(max.x <= point.x ? point.x : max.x, max.y <= point.y ? point.y : max.y);

        //2
        point = cam.WorldToScreenPoint(new Vector3(cen.x - ext.x, cen.y - ext.y, cen.z + ext.z));
        min = new Vector2(min.x >= point.x ? point.x : min.x, min.y >= point.y ? point.y : min.y);
        max = new Vector2(max.x <= point.x ? point.x : max.x, max.y <= point.y ? point.y : max.y);

        //3
        point = cam.WorldToScreenPoint(new Vector3(cen.x + ext.x, cen.y - ext.y, cen.z + ext.z));
        min = new Vector2(min.x >= point.x ? point.x : min.x, min.y >= point.y ? point.y : min.y);
        max = new Vector2(max.x <= point.x ? point.x : max.x, max.y <= point.y ? point.y : max.y);

        //4
        point = cam.WorldToScreenPoint(new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z - ext.z));
        min = new Vector2(min.x >= point.x ? point.x : min.x, min.y >= point.y ? point.y : min.y);
        max = new Vector2(max.x <= point.x ? point.x : max.x, max.y <= point.y ? point.y : max.y);

        //5
        point = cam.WorldToScreenPoint(new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z - ext.z));
        min = new Vector2(min.x >= point.x ? point.x : min.x, min.y >= point.y ? point.y : min.y);
        max = new Vector2(max.x <= point.x ? point.x : max.x, max.y <= point.y ? point.y : max.y);

        //6
        point = cam.WorldToScreenPoint(new Vector3(cen.x - ext.x, cen.y + ext.y, cen.z + ext.z));
        min = new Vector2(min.x >= point.x ? point.x : min.x, min.y >= point.y ? point.y : min.y);
        max = new Vector2(max.x <= point.x ? point.x : max.x, max.y <= point.y ? point.y : max.y);

        //7
        point = cam.WorldToScreenPoint(new Vector3(cen.x + ext.x, cen.y + ext.y, cen.z + ext.z));
        min = new Vector2(min.x >= point.x ? point.x : min.x, min.y >= point.y ? point.y : min.y);
        max = new Vector2(max.x <= point.x ? point.x : max.x, max.y <= point.y ? point.y : max.y);

        return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    }
}

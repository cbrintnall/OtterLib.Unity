using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public delegate bool SelectWhereAction<T, K>(T inVal, out K var);
public delegate K SelectedEnumerable<T, K>(T input, int idx);

public static class Utilities
{
#if UNITY_EDITOR
    public static IEnumerable<T> FindAssetsByType<T>()
        where T : UnityEngine.Object
    {
        var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        foreach (var t in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(t);
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                yield return asset;
            }
        }
    }
#endif

    public static List<MeshRenderer> CollectMesh(this GameObject gameObject)
    {
        var renderers = new List<MeshRenderer> { gameObject.GetComponent<MeshRenderer>() };

        renderers.AddRange(gameObject.GetComponentsInChildren<MeshRenderer>());

        return renderers.Where(renderer => renderer != null).ToList();
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        var buffer = source.ToList();
        for (int i = 0; i < buffer.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, buffer.Count);
            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }

    public static T CreateAtMe<T>(this GameObject obj, T prefab)
        where T : Component
    {
        var child = GameObject.Instantiate(prefab);
        child.transform.position = obj.transform.position;
        return child;
    }

    public static void PlayThenDestroy(this ParticleSystem ps)
    {
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
        ps.Play();
    }

    public static void TODO()
    {
        var frame = new StackFrame(1, true);
        UnityEngine.Debug.LogWarning(
            $"Unhandled TODO: [line={frame.GetFileLineNumber()},file={frame.GetFileName()}]"
        );
    }

    public static int PingPongInt(int val, int max)
    {
        return Mathf.RoundToInt(Mathf.PingPong(val, max));
    }

    public static Vector3 WithX(this Vector3 vec, float x)
    {
        return new Vector3(x, vec.y, vec.z);
    }

    public static Vector3 WithZ(this Vector3 vec, float z)
    {
        return new Vector3(vec.x, vec.y, z);
    }

    public static Vector3 WithY(this Vector3 vec, float y)
    {
        return new Vector3(vec.x, y, vec.z);
    }

    public static Color FromHex(string input)
    {
        string hex = input.StartsWith("#") ? input.Substring(1) : input;

        if (hex.Length < 6)
        {
            throw new System.FormatException("Needs a string with a length of at least 6");
        }

        var r = hex.Substring(0, 2);
        var g = hex.Substring(2, 2);
        var b = hex.Substring(4, 2);
        string alpha;
        if (hex.Length >= 8)
            alpha = hex.Substring(6, 2);
        else
            alpha = "FF";

        return new Color(
            (int.Parse(r, NumberStyles.HexNumber) / 255f),
            (int.Parse(g, NumberStyles.HexNumber) / 255f),
            (int.Parse(b, NumberStyles.HexNumber) / 255f),
            (int.Parse(alpha, NumberStyles.HexNumber) / 255f)
        );
    }

    public static bool Chance(this float f) => Randf() < f;

    public static IEnumerable<Transform> Children(this Transform t)
    {
        List<Transform> children = new();
        foreach (Transform child in t)
        {
            children.Add(child);
        }
        return children;
    }

    public static IEnumerable<T> ComponentsInChildren<T>(this GameObject g)
    {
        List<T> children = new();
        foreach (T child in g.GetComponentsInChildren<T>())
        {
            children.Add(child);
        }
        return children;
    }

    public static T GameObjectWith<T>()
        where T : MonoBehaviour => new GameObject(nameof(T)).AddComponent<T>();

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

    public static int RandomIndex<T>(this IEnumerable<T> list)
    {
        return UnityEngine.Random.Range(0, list.Count());
    }

    public static float Randf() => UnityEngine.Random.Range(0.0f, 1.0f);

    public static T GetOrCreateComponent<T>(this GameObject go)
        where T : Component => go.TryGetComponent(out T c) ? c : go.AddComponent<T>();

    public static T GetOrCreateComponent<T>(this MonoBehaviour comp)
        where T : Component => comp.gameObject.GetOrCreateComponent<T>();

    public static void WaitThen(this MonoBehaviour t, float time, Action then)
    {
        t.StartCoroutine(WaitAndThen(time, then));
    }

    public static IEnumerator WaitAndThen(float time, Action then)
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

    public static List<K> SelectWhere<T, K>(this IEnumerable<T> en, SelectWhereAction<T, K> cb)
    {
        List<K> final = new();

        foreach (T v in en)
        {
            if (cb(v, out K outVar))
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

    public static Bounds CalculateBoundsWithPoints(this GameObject go)
    {
        Bounds bounds = new();

        foreach (Transform t in go.GetComponentsInChildren<Transform>())
        {
            bounds.Encapsulate(t.transform.position);
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

    public static float ClampedBetween(this float f, Vector2 values) =>
        Mathf.Clamp(f, values.x, values.y);

    public static float ClampedBetween(this int i, Vector2Int values) =>
        Mathf.Clamp(i, values.x, values.y);
}

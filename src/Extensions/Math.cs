using UnityEngine;

public static class MathExtensions
{
    public static Vector3 ToTangent(this Vector3 v)
    {
        var t1 = Vector3.Cross(v, Vector3.up);
        return Vector3.Cross(v, t1).normalized;
    }
}

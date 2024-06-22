using UnityEngine;

public static class Vector
{
    public static readonly Vector2Int[] CardinalDirections = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public static readonly Vector2Int[] Directions = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        new Vector2Int(1, 1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
    };

    public static Vector2Int ToTile(this Vector3 vec) =>
        Vector3.ProjectOnPlane(vec, Vector3.up).To2D().FloorToInt();

    public static bool Within(this Vector3 a, Vector3 b, float dist) =>
        Vector3.Distance(a, b) <= dist;

    public static Vector2Int Up(this Vector2Int v) => v + Vector2Int.up;

    public static Vector2Int Left(this Vector2Int v) => v + Vector2Int.left;

    public static Vector2Int Right(this Vector2Int v) => v + Vector2Int.right;

    public static Vector2Int Down(this Vector2Int v) => v + Vector2Int.down;

    public static Vector2Int RandomCardinalDirectionInt() => CardinalDirections.Random();

    public static Vector2Int RandomDirectionInt() => Directions.Random();

    public static Vector3 ToVec3(this Vector2Int v) => new Vector3(v.x, 0f, v.y);

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

    public static Vector2Int FloorToInt(this Vector2 v) =>
        new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));

    public static Vector2Int RoundToInt(this Vector2 v) =>
        new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));

    public static Vector2Int CeilToInt(this Vector2 v) =>
        new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));

    public static Vector2 To2D(this Vector3 v) => new Vector2(v.x, v.z);
}

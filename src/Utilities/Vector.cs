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

    public static Vector2Int Up(this Vector2Int v) => v + Vector2Int.up;

    public static Vector2Int Left(this Vector2Int v) => v + Vector2Int.left;

    public static Vector2Int Right(this Vector2Int v) => v + Vector2Int.right;

    public static Vector2Int Down(this Vector2Int v) => v + Vector2Int.down;

    public static Vector2Int RandomCardinalDirectionInt() => CardinalDirections.Random();

    public static Vector2Int RandomDirectionInt() => Directions.Random();

    public static Vector3 ToVec3(this Vector2Int v) => new Vector3(v.x, 0f, v.y);
}

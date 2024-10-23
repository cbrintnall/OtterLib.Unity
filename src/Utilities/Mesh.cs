using UnityEngine;

public static class MeshUtilities
{
    public static Mesh GetMesh(this GameObject obj)
    {
        var filter = obj.GetComponent<MeshFilter>();

        if (filter == null)
        {
            filter = obj.GetComponentInChildren<MeshFilter>();
        }

        if (filter == null)
        {
            return obj.GetComponent<SkinnedMeshRenderer>()?.sharedMesh;
        }

        return filter.sharedMesh;
    }

    public static Mesh GetMesh(this Component comp) => comp.gameObject.GetMesh();
}

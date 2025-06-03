using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class ExportMeshToOBJ
{
    [MenuItem("Tools/Export Mesh to OBJ")]
    static void ExportSelectedMesh()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("No GameObject selected!");
            return;
        }

        MeshFilter mf = selected.GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning("Selected GameObject does not have a MeshFilter with a mesh.");
            return;
        }

        string path = EditorUtility.SaveFilePanel("Save OBJ", "", selected.name + ".obj", "obj");
        if (string.IsNullOrEmpty(path)) return;

        Mesh mesh = mf.sharedMesh;
        ExportMesh(mesh, selected.transform, path);
        Debug.Log("Exported mesh to: " + path);
    }

    static void ExportMesh(Mesh mesh, Transform transform, string path)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("# Exported from Unity");
        
        foreach (Vector3 v in mesh.vertices)
        {
            Vector3 wv = transform.TransformPoint(v);
            sb.AppendLine($"v {wv.x} {wv.y} {wv.z}");
        }

        foreach (Vector3 n in mesh.normals)
        {
            Vector3 wn = transform.TransformDirection(n);
            sb.AppendLine($"vn {wn.x} {wn.y} {wn.z}");
        }

        foreach (Vector2 uv in mesh.uv)
        {
            sb.AppendLine($"vt {uv.x} {uv.y}");
        }

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] triangles = mesh.GetTriangles(i);
            for (int t = 0; t < triangles.Length; t += 3)
            {
                int i1 = triangles[t] + 1;
                int i2 = triangles[t + 1] + 1;
                int i3 = triangles[t + 2] + 1;
                sb.AppendLine($"f {i1}/{i1}/{i1} {i2}/{i2}/{i2} {i3}/{i3}/{i3}");
            }
        }

        File.WriteAllText(path, sb.ToString());
    }
}

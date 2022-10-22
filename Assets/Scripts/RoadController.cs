using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]

public class RoadController : MonoBehaviour
{
    //Road Point Info
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private Transform firstControlPoint;
    [SerializeField] private Transform secondControlPoint;

    [Range(0, 1)]
    [SerializeField] private float t = 0;

    [SerializeField] private Road_2DShape roadShape;
    [SerializeField] private int frameCount; 
    private Mesh mesh;
    private MeshFilter meshFilter;

    [SerializeField] private Transform pathFollower;

    private void Awake()
    {
        mesh = new Mesh();

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
    }

    private void Update()
    {
        SetPath();
        SetPathFollowerTransform();
    }

    private void SetPathFollowerTransform() 
    {
        pathFollower.position = GetPointOnCurve(t).pos + 2 * transform.TransformDirection(pathFollower.up);
        pathFollower.rotation = GetPointOnCurve(t).rot;
    }

    private void SetPath()
    {
        mesh.Clear();

        //Vertices
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < frameCount; i++)
        {
            float t = i / (frameCount - 1f);
            PointOnRoad op = GetPointOnCurve(t);

            for (int j = 0; j < roadShape.vertices.Length; j++)
            {
                vertices.Add(op.WorldPosition(roadShape.vertices[j].vertexPos));
                normals.Add(op.WorldAxis(roadShape.vertices[j].vertexNormal));
                uvs.Add(new Vector2((float)j / (float)roadShape.vertices.Length, (float)i % 2 / 2f));
            }
        }

        //Triangles
        List<int> triangleIndices = new List<int>();
        for (int i = 0; i < frameCount - 1; i++)
        {
            int firstShapeIndex = i * roadShape.vertices.Length;
            int secondShapeIndex = (i + 1) * roadShape.vertices.Length;

            for (int j = 0; j < roadShape.indices.Length; j += 2)
            {
                int currentFirst = roadShape.indices[j];
                int currentSecond = roadShape.indices[j + 1];

                triangleIndices.Add(firstShapeIndex + currentFirst);
                triangleIndices.Add(secondShapeIndex + currentFirst);
                triangleIndices.Add(secondShapeIndex + currentSecond);
                triangleIndices.Add(firstShapeIndex + currentFirst);
                triangleIndices.Add(secondShapeIndex + currentSecond);
                triangleIndices.Add(firstShapeIndex + currentSecond);

            }
        }
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangleIndices, 0);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
    }

    private PointOnRoad GetPointOnCurve(float t)  
    {
        Vector3 A = (1 - t) * startPoint.position + t * firstControlPoint.position;
        Vector3 B = (1 - t) * firstControlPoint.position + t * secondControlPoint.position;
        Vector3 C = (1 - t) * secondControlPoint.position + t * endPoint.position;

        Vector3 D = (1 - t) * A + t * B;
        Vector3 E = (1 - t) * B + t * C;

        Vector3 pos = (1 - t) * D + t * E;

        Vector3 tangent = (E - D).normalized;

        Vector3 up = Vector3.Lerp(startPoint.up, endPoint.up, t).normalized;
        Quaternion rot = Quaternion.LookRotation(tangent, up);

        return new PointOnRoad(pos, rot);
    }
}

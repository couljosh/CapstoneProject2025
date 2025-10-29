using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GroundMeshGeneration : MonoBehaviour
{
    //Mesh Variables
    public Mesh mesh;
    private MeshCollider meshCollider;
    public List<Vector3> vertexPosition = new List<Vector3>(); //List of vertex positions of mesh

    [Header("Chunk Variables")]
    [Range(10, 1000)]
    public int chunkSize;
    private int chunkWidth;
    private int chunkHeight;

    [Range(1, 4)]
    public int subdivisions;
    private int subdivisionsX;
    private int subdivisionsZ;

    //Arrays for triangles and Verticies
    private int[] triangles;
    private Vector3[] verticesArray;

    void Start()
    {
        //Chunk & subdivisions always even values
        chunkWidth = chunkSize;
        chunkHeight = chunkSize;
        subdivisionsX = chunkWidth / subdivisions;
        subdivisionsZ = chunkHeight / subdivisions;

        //Creates starting mesh
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();


        StartCoroutine(GenerateMap());

    }

    public IEnumerator GenerateMap()
    {
        Debug.Log("generating mesh");
        CalculateMesh();

        yield return null;

        Debug.Log("updating mesh");
        UpdateMesh();

        yield return null;

        Debug.Log("Generating Pits");


    }

    public void CalculateMesh()
    {
        //Assigns the calculation to make a quad
        triangles = new int[subdivisionsX * subdivisionsZ * 6];
        verticesArray = new Vector3[(subdivisionsX + 1) * (subdivisionsZ + 1)];


        //Loops through and makes each vertex depending on the chosen world size, chosing their positions based on perlin noise, and added to a list
        int i = 0;
        for (int z = 0; z <= subdivisionsZ; z++)
        {
            for (int x = 0; x <= subdivisionsX; x++)
            {

                float worldX = x + this.transform.position.x;
                float worldY = z + this.transform.position.z;

                Vector3 vertex = new Vector3(x * subdivisions, 0, z * subdivisions);
                verticesArray[i] = vertex;
                vertexPosition.Add(vertex);
                i++;
            }
        }


        //tracks # of triangles and vertices
        int tri = 0;
        int vert = 0;

        //Loops through each vertex and creates the triangles for the quads, going in a certain order 
        for (int z = 0; z < subdivisionsZ; z++)
        {
            for (int x = 0; x < subdivisionsX; x++)
            {
                //triangle 1 of quad
                triangles[tri + 0] = vert + 0;
                triangles[tri + 1] = vert + subdivisionsZ + 1;
                triangles[tri + 2] = vert + 1;

                //triangle 2 of quad
                triangles[tri + 3] = vert + 1;
                triangles[tri + 4] = vert + subdivisionsZ + 1;
                triangles[tri + 5] = vert + subdivisionsZ + 2;

                //increase value to the vertex and triangle to go to their next quad in the row 
                vert++;
                tri += 6;
            }
            //additional increment to get the vertex to get to the next row of quads
            vert++;
        }
    }


    //WILL BE USED TO SET PIT DEPTHS ************
    //Clears the mesh and sets the proper vertecies, triangles, color, and normals
    public void UpdateMesh()
    {

        mesh.Clear();
        mesh.vertices = verticesArray;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
    }
}

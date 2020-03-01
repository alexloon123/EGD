using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject PREFAB_vertex;
    [SerializeField] GameObject PREFAB_line;
    [SerializeField] Vector2[] positions;
    Vertex[] vertices;
    Line[] lines;
    List<int>[] adjacency;

    // Start is called before the first frame update
    void Start() {
        //Initialize vertecies array
        vertices = new Vertex[positions.Length];
        //Create vertex[i] at position[i]
        for (int i = 0; i < positions.Length; i++) {
            vertices[i] = Instantiate(PREFAB_vertex, new Vector3(positions[i].x, positions[i].y, 2.5f), Quaternion.identity).GetComponent<Vertex>();
        }
        //Initialize adjacency array
        bool[,] adjacency = new bool[positions.Length, positions.Length];
        //Adjacency range
        float _range = .75f;
        //Record edges to be created
        List<(Vertex, Vertex)> edges = new List<(Vertex, Vertex)>();
        //Set vertex[i] adjacent to all vertecies within a distance of _range
        for (int i = 0; i < vertices.Length; i++) {
            for (int j = i + 1; j < vertices.Length; j++) {
                //Test distance between vertex[i] and vertex[j]
                if (Mathf.Sqrt(
                    Mathf.Pow(Mathf.Abs(positions[i].x - positions[j].x), 2) +
                    Mathf.Pow(Mathf.Abs(positions[i].y - positions[j].y), 2)
                ) <= _range) {
                    edges.Add((vertices[i], vertices[j]));
                }
            }
        }
        //Initialize lines array
        lines = new Line[edges.Count];
        //Create lines
        for (int i = 0; i < edges.Count; i++) {
            lines[i] = Instantiate(PREFAB_line, new Vector3(0, 0, 2.5f), Quaternion.identity).GetComponent<Line>();
            lines[i].Setup(edges[i].Item1, edges[i].Item2);
            print("Line[" + i + "] had verts (" + edges[i].Item1.GetPosition() + "),(" + edges[i].Item2.GetPosition() + ")");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

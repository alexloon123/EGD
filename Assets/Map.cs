﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject PREFAB_vertex;
    [SerializeField] GameObject PREFAB_line;
    [SerializeField] Vector2[] positions;
    [SerializeField] List<int> path;

    public float z_depth;
    public float l_range;
    public float l_chance;
    public float l_curve;

    Vertex[] vertices;
    Line[] lines;
    List<int>[] adjacency;

    // Start is called before the first frame update
    void Start() {
        //Initialize vertecies array
        vertices = new Vertex[positions.Length];
        //Create vertex[i] at position[i]
        for (int i = 0; i < positions.Length; i++) {
            vertices[i] = Instantiate(PREFAB_vertex, new Vector3(positions[i].x, positions[i].y, z_depth), Quaternion.identity).GetComponent<Vertex>();
            if (path.Contains(i)) {//Enable vertex collider
                vertices[i].GetComponent<SphereCollider>().enabled = true;
                vertices[i].GetComponent<Vertex>().OnPath = true;
            }
        }
        //Initialize adjacency array
        bool[,] adjacency = new bool[positions.Length, positions.Length];
        //Adjacency range
        
        //Record edges to be created
        List<(Vertex, Vertex)> edges = new List<(Vertex, Vertex)>();
        //Set vertex[i] adjacent to all vertecies within a distance of _range
        for (int i = 0; i < vertices.Length; i++) {
            for (int j = i + 1; j < vertices.Length; j++) {
                //Test distance between vertex[i] and vertex[j]
                if (Mathf.Sqrt(
                    Mathf.Pow(Mathf.Abs(positions[i].x - positions[j].x), 2) +
                    Mathf.Pow(Mathf.Abs(positions[i].y - positions[j].y), 2)
                ) <= l_range && Random.Range(0,1)<l_chance) {
                    edges.Add((vertices[i], vertices[j]));
                }
            }
        }
        //Initialize lines array
        lines = new Line[edges.Count];
        //Create lines
        for (int i = 0; i < edges.Count; i++) {
            lines[i] = Instantiate(PREFAB_line, new Vector3(0, 0, z_depth), Quaternion.identity).GetComponent<Line>();
            lines[i].Setup(edges[i].Item1, edges[i].Item2, l_curve);
            if(edges[i].Item1.OnPath && edges[i].Item2.OnPath) {
                lines[i].GetComponent<MeshCollider>().enabled = true;
            }
            //print("Line[" + i + "] had verts (" + edges[i].Item1.GetPosition() + "),(" + edges[i].Item2.GetPosition() + ")");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

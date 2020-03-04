using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{

    public bool OnPath = false;

    List<Vertex> connected = new List<Vertex>();
    List<Line> connection = new List<Line>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddConnection(Vertex v, Line l) {
        connected.Add(v);
        connection.Add(l);
    }

    public Vector2 GetPosition() {
        return new Vector2(transform.position.x, transform.position.y);
    }
}

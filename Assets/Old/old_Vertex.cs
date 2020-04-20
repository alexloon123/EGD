using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class old_Vertex : MonoBehaviour
{

    public bool OnPath = false;

    List<old_Vertex> connected = new List<old_Vertex>();
    List<old_Line> connection = new List<old_Line>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddConnection(old_Vertex v, old_Line l) {
        connected.Add(v);
        connection.Add(l);
    }

    public Vector2 GetPosition() {
        return new Vector2(transform.position.x, transform.position.y);
    }
}

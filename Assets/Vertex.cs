using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{

    public bool OnPath = false;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 GetPosition() {
        return new Vector2(transform.position.x, transform.position.y);
    }
}

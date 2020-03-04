using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{

    //Index finger bone to track
    public Transform index_finger;
    //Map to communicate with
    public Map map;
    //Danger level
    public float danger = 0;

    //Last vertex visited
    Vertex last_vert = null;
    //Last line visited
    Line last_line = null;

    //Number of colliders currently in contact with
    int c = 0;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(index_finger.position.x - transform.position.x, index_finger.position.y- transform.position.y, 0));

        if(c > 0) { //SAFE
            danger = 0;
        } else { //DANGER
            danger += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider col) {
        if (col.gameObject.CompareTag("Line")){
            last_line = col.GetComponent<Line>();
            c++;
        }
        if (col.gameObject.CompareTag("Vertex")) {
            last_vert = col.GetComponent<Vertex>();
            c++;
        }
    }

    private void OnTriggerExit(Collider collision) {
        if (collision.gameObject.CompareTag("Line") || collision.gameObject.CompareTag("Vertex")) {
            c--;
        }
    }
}

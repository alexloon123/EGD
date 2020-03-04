using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{

    public Transform index_finger;
    public float danger = 0;

    int c = 0;
    Vector2 last_pos = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(index_finger.position.x - last_pos.x, index_finger.position.y- last_pos.y, 0));
        last_pos.x = index_finger.position.x;
        last_pos.y = index_finger.position.y;

        if(c > 0) { //SAFE
            danger = 0;
        } else { //DANGER
            danger += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.CompareTag("Line") || collision.gameObject.CompareTag("Vertex")) {
            c++;
        }
    }

    private void OnTriggerExit(Collider collision) {
        if (collision.gameObject.CompareTag("Line") || collision.gameObject.CompareTag("Vertex")) {
            c--;
        }
    }
}

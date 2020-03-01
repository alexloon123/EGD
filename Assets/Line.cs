using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{

    Vector3[] line_points;
    Vertex A;
    Vertex B;

    // Update is called once per frame
    public void Setup(Vertex a, Vertex b)
    {
        //Setup vertex references
        A = a;
        B = b;
        //Step value 0<s<1 determines how many points are interpolated for each curve
        double s = 0.1;
        //Total number of points to be interpolated
        int n = (int)(1 / s) + 1;
        //Setup position A
        Vector2 p_a = new Vector2(a.transform.position.x, a.transform.position.y);
        //Setup position C
        Vector2 p_c = new Vector2(b.transform.position.x, b.transform.position.y);
        //Setup position B
        Vector2 p_b = new Vector2(
            ((a.transform.position.x + b.transform.position.x) / 2) + Random.Range(-.35f, .35f), 
            ((a.transform.position.y + b.transform.position.y) / 2) + Random.Range(-.35f, .35f));
        //GeneratePoints call
        line_points = GeneratePoints(p_a, p_b, p_c, s, n);
        //Render the line
        GetComponent<LineRenderer>().positionCount = n;
        GetComponent<LineRenderer>().SetPositions(line_points);
    }

    //Given a list of input vectors, interpolates curves based on step and returns array of all generated points
    Vector3[] GeneratePoints(Vector2 A, Vector2 B, Vector2 C, double step, int n) {
        //Storage of points between AB and BC
        Vector2 AB = new Vector2();
        Vector2 BC = new Vector2();
        //Current index in output
        int i = 0;
        //Setup output
        Vector3[] output = new Vector3[n];
        //Interpolate 3 points
        for (double t = 0; t <= 1; t = t + step) {
            //Calc point on line A-->B
            AB.x = (float)((1 - t) * A.x + (t) * B.x);
            AB.y = (float)((1 - t) * A.y + (t) * B.y);
            //Calc point on line B-->C
            BC.x = (float)((1 - t) * B.x + (t) * C.x);
            BC.y = (float)((1 - t) * B.y + (t) * C.y);
            //Calc point on line AB-->BC
            output[i] = new Vector3(0, 0, 0);
            output[i].x = (float)((1 - t) * AB.x + (t) * BC.x);
            output[i].y = (float)((1 - t) * AB.y + (t) * BC.y);
            //Move to next index in output
            i++;
        }
        return output;
    }
}

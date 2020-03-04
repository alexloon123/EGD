using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{

    Vertex A;
    Vertex B;

    // Update is called once per frame
    public void Setup(Vertex a, Vertex b, float l_curve)
    {
        //Setup vertex references
        A = a;
        B = b;
        //Step value 0<s<1 determines how many points are interpolated for each curve
        double s = 0.1;
        //Total number of points to be interpolated
        int n = (int)(1 / s) + 1;
        //Total number of triangles in mesh
        int t = ((n - 1) * 2) + ((n - 2) * 2);
        //Setup position A
        Vector2 p_a = new Vector2(a.transform.position.x, a.transform.position.y);
        //Setup position C
        Vector2 p_c = new Vector2(b.transform.position.x, b.transform.position.y);
        //Setup position B
        Vector2 p_b = new Vector2(
            ((a.transform.position.x + b.transform.position.x) / 2) + Random.Range(-l_curve, l_curve), 
            ((a.transform.position.y + b.transform.position.y) / 2) + Random.Range(-l_curve, l_curve));
        //Generate Line Points
        Vector3[] line_points = GenerateLinePoints(p_a, p_b, p_c, s, n);
        //Generate Mesh Points
        Vector3[] mesh_points = GenerateMeshPoints(line_points, .01f);

        //Compute triangles
        int[] triangles = new int[t*3];
        int c = 0;
        for(int i = 0; i < mesh_points.Length; i += 3) {
            //IF NOT LAST POINT
            if(i < mesh_points.Length - 1) {
                triangles[c] = i;c++;
                triangles[c] = i+1; c++;
                triangles[c] = i+3; c++;
                triangles[c] = i; c++;
                triangles[c] = i + 3; c++;
                triangles[c] = i + 2; c++;

            }
            //IF MIDDLE POINT
            if(i != 0 && i < mesh_points.Length - 1) {
                triangles[c] = i; c++;
                triangles[c] = i + 2; c++;
                triangles[c] = i - 1; c++;
                triangles[c] = i; c++;
                triangles[c] = i - 2; c++;
                triangles[c] = i + 1; c++;
            }
        }

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = mesh_points;
        mesh.triangles = triangles;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        //Render the line
        /*
        GetComponent<LineRenderer>().positionCount = n;
        GetComponent<LineRenderer>().SetPositions(line_points);
        */
    }

    //Given a list of input vectors, interpolates curves based on step and returns array of all generated points
    Vector3[] GenerateLinePoints(Vector2 A, Vector2 B, Vector2 C, double step, int n) {
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

    Vector3[] GenerateMeshPoints(Vector3[] points, float width) {
        int m = points.Length;//Points on curve
        int n = (m - 1) * 2;  //Additional points to add

        List<Vector3> temp = new List<Vector3>();
        //For each pair of points i and i+1, generate two points that are equidistant and width away from the closest points between them
        for(int i = 0; i < m - 1; i++) {
            //Calculate slope perpendicular to line between points
            float slope = (points[i].y - points[i + 1].y) / (points[i].x - points[i + 1].x);
            slope = -1 / slope;
            //Calculate midpoint
            Vector2 midpoint = new Vector2((points[i].x + points[i + 1].x) / 2, (points[i].y + points[i + 1].y) / 2);
            //Calculate change in X and Y from midpoint
            float angle = Mathf.Atan(slope);
            float x = width * Mathf.Cos(angle);
            float y = width * Mathf.Sin(angle);
            //Calculate two new points
            Vector3 p1 = new Vector3(midpoint.x + x, midpoint.y + y,0);
            Vector3 p2 = new Vector3(midpoint.x - x, midpoint.y - y,0);
            //Add line point and next two offline points
            temp.Add(points[i]);
            temp.Add(p1);
            temp.Add(p2);
        }
        //Add last point
        temp.Add(points[m-1]);
        //Convert to array
        Vector3[] output = new Vector3[m + n];
        for(int i = 0; i < m + n; i++) {
            output[i] = temp[i];
        }
        return output;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{

    public Vertex A;
    public Vertex B;

    // Update is called once per frame
    public void Setup(Vertex a, Vertex b, float l_curve)
    {
        //Setup vertex references
        A = a;
        B = b;
        //Setup vertex connections
        a.AddConnection(b, this);
        b.AddConnection(a, this);
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
        List<Vector3> line_points = GenerateLinePoints(p_a, p_b, p_c, s, n);
        //Generate Mesh Points
        (List<Vector3>,List<Vector3>) mesh_points = GenerateMeshPoints(line_points, .01f);
        List<Vector3> left = mesh_points.Item1;
        List<Vector3> right = mesh_points.Item2;

        //Compute triangles
        int[] triangles = new int[t*3];
        int c = 0;
        for(int i = 0; i < n; i ++) {
            //IF NOT LAST POINT
            if(i < n - 1) {
                triangles[c] = i;c++;
                triangles[c] = i+n; c++;
                triangles[c] = i+1; c++;
                triangles[c] = i; c++;
                triangles[c] = i+1; c++;
                triangles[c] = i+(2*n)-1; c++;
            }
            //IF MIDDLE POINT
            if(i != 0 && i < n - 1) {
                triangles[c] = i; c++;
                triangles[c] = i-1+n; c++;
                triangles[c] = i+n; c++;
                triangles[c] = i; c++;
                triangles[c] = i+(2*n)-1; c++;
                triangles[c] = i+(2*n)-2; c++;
            }
        }

        //Create final vert array
        Vector3[] all_verts = new Vector3[(3 * n) - 2];
        for(int i = 0; i < n; i++) {
            all_verts[i] = line_points[i];
        }
        for (int i = 0; i < n-1; i++) {
            all_verts[i+n] = left[i];
        }
        for (int i = 0; i < n-1; i++) {
            all_verts[i+(2*n)-1] = right[i];
        }

        //Create mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = all_verts;
        mesh.triangles = triangles;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    //Given a list of input vectors, interpolates curves based on step and returns array of all generated points
    List<Vector3> GenerateLinePoints(Vector2 A, Vector2 B, Vector2 C, double step, int n) {
        //Storage of points between AB and BC
        Vector2 AB = new Vector2();
        Vector2 BC = new Vector2();
        //Setup output
        List<Vector3> output = new List<Vector3>();
        //Interpolate 3 points
        for (double t = 0; t <= 1; t = t + step) {
            //Calc point on line A-->B
            AB.x = (float)((1 - t) * A.x + (t) * B.x);
            AB.y = (float)((1 - t) * A.y + (t) * B.y);
            //Calc point on line B-->C
            BC.x = (float)((1 - t) * B.x + (t) * C.x);
            BC.y = (float)((1 - t) * B.y + (t) * C.y);
            //Calc point on line AB-->BC
            output.Add(new Vector3(
                (float)((1 - t) * AB.x + (t) * BC.x), 
                (float)((1 - t) * AB.y + (t) * BC.y), 0));
        }
        return output;
    }

    (List<Vector3>,List<Vector3>) GenerateMeshPoints(List<Vector3> points, float width) {
        int m = points.Count;//Points on curve
        int n = (m - 1) * 2;  //Additional points to add

        List<Vector3> left = new List<Vector3>();
        List<Vector3> right = new List<Vector3>();
        //For each pair of points i and i+1, generate two points that are equidistant and width away from the closest points between them
        for (int i = 0; i < m - 1; i++) {
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

            //Are the points on (left and right) or (right and left) sides of the line
            float d1 = ((p1.x - points[i].x) * (points[i + 1].y - points[i].y)) - ((p1.y - points[i].y) * (points[i + 1].x - points[i].x));
            if(d1 > 0) {
                left.Add(p1);
                right.Add(p2);
            } else {
                left.Add(p2);
                right.Add(p1);
            }
            
        }
        return (left,right);
    }

    float DistanceToLine(Vector3 pos) {
        //Find closest point
        Vector3 temp = GetComponent<MeshCollider>().ClosestPointOnBounds(pos);
        //Return distance between
        return Vector3.Distance(pos, temp);
    }
}

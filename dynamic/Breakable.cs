/*
 * Author: Brent Barbachem
 * Date:   January 2, 2019
 *
 * Get the shape, mesh, and all triangles that make up the mesh
 * for an object. After we have all of the pieces, then we can
 * break the object into pieces. 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Breakable : MonoBehaviour
{
    // mesh that makes up our object
    Mesh m_mesh;

    void Start()
    {
        /* Print out the all of the trianles including the x,y,z
         * points of the object that they correspond to.
         */
        MeshFilter meshFilter = (MeshFilter)gameObject.GetComponent("MeshFilter");
        Mesh mesh = meshFilter.mesh;
        int[] triangles = mesh.triangles;

        for(int i = 0; i < triangles.Length; i+=3)
        {
            //Debug.Log(mesh.vertices[triangles[i+0]] + ", " + mesh.vertices[triangles[i+1]] + ", " + mesh.vertices[triangles[i+2]]);
        }
    }

    /**
     * 
     */
    void Update()
    {

    }

    /** 
     * Collision occurred with another object. Get the collision point for this object. 
     * The collision point will then become the origin of the crack/break event. 
     * @param other - Collider object that this game object collided with.
     */
    private void OnCollisionEnter(Collision other)
    {
        // print information about the gameObject and the Collision
        //Debug.Log("Tranformation = " + transform.position + " Rotation = " + transform.rotation);
        //Debug.Log("Number of contact points = " + other.contacts.Length);

        Vector3 crack = new Vector3(0.0f, 0.0f, 0.0f);

        // get the contact point[s] in the local space accounting for translation and orientation
        for(int i = 0; i < other.contacts.Length; i++)
        {
            // get the local space coordinate
            crack += transform.InverseTransformPoint(other.contacts[i].point);
            //Debug.Log("Contact point [" + i + "] = " + transform.InverseTransformPoint(other.contacts[i].point));
        }

        crack /= other.contacts.Length;

        crack.x = (float)System.Math.Round((float)crack.x, 1);
        crack.y = (float)System.Math.Round((float)crack.y, 1);
        crack.z = (float)System.Math.Round((float)crack.z, 1);

        CreateTheCrack(crack);
    }

    /**
     * Crack from the starting position until the edge is reached
     * @param crack - [x,y,z] local coordinates of the crack starting position 
     */
    private void CreateTheCrack(Vector3 crack)
    {
        //Debug.Log("Start location of the crack = " + crack);

        /// get all world points of the object
        /// crack through and transform
        //GetFaceVertices(crack);

        Debug.Log("Crack start = " + crack);
        Debug.Log("Estimated End = " + GetOppositePoint(crack));
    }

    private Vector3 GetOppositePoint(Vector3 point)
    {
        return point * -1.0f;
    }

    private void GetFaceVertices(Vector3 crack)
    {
        Debug.Log(crack.x + ", " + crack.y + ", " + crack.z);

        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;

        int face = 1;
        for(int i = 0; i < vertices.Length; i+=4)
        {
            Debug.Log("Face[" + face + "] = " + vertices[i] + ", " + vertices[i+1] + ", " + vertices[i+2] + ", " + vertices[i+3]);
            bool sameX = isSame(vertices[i].x, vertices[i+1].x, vertices[i+2].x, vertices[i+3].x, crack.x);
            bool sameY = isSame(vertices[i].y, vertices[i+1].y, vertices[i+2].y, vertices[i+3].y, crack.y);
            bool sameZ = isSame(vertices[i].z, vertices[i+1].z, vertices[i+2].z, vertices[i+3].z, crack.z);

            if(sameX)
            {
                Debug.Log("Same X!!!");
            }

            if(sameY)
            {
                Debug.Log("Same Y!!!");
            }

            if(sameZ)
            {
                Debug.Log("Same Z!!!");
            }
            face ++;
        }

        /// -4 is for the size of the face since this is a cube object
        // for(int i = 0; i < vertices.Length; i++)
        // {
        //     Debug.Log("Vector[" + i + "] = " + vertices[i]);
        // }
    }

    private bool isSame(double a, double b, double c, double d, double e)
    {
        Debug.Log(a + ", " + b + ", " + c + ", " + d + ", " + e);
        return a == b && a == c && a == d && a == e;
    }

    /**
     * @return area of a triangle
     */
    private static float Area(double x1, double y1, double x2, double y2, double x3, double y3) 
    { 
        return (float)Mathf.Abs( (float)(x1 * ((float)y2 - (float)y3) + (float)x2 * ((float)y3 - (float)y1) + (float)x3 * ((float)y1 - (float)y2)) / 2.0f); 
    } 

    private static bool InArea(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double x, double y)
    {
                // Calculate area of rectangle ABCD  
        float A = Area(x1, y1, x2, y2, x3, y3) + Area(x1, y1, x4, y4, x3, y3); 
      
        // Calculate area of triangle PAB  
        float A1 = Area(x, y, x1, y1, x2, y2); 
      
        // Calculate area of triangle PBC  
        float A2 = Area(x, y, x2, y2, x3, y3); 
      
        // Calculate area of triangle PCD  
        float A3 = Area(x, y, x3, y3, x4, y4); 
      
        // Calculate area of triangle PAD 
        float A4 = Area(x, y, x1, y1, x4, y4); 
      
        // Check if sum of A1, A2, A3   
        // and A4is same as A  
        return (A == A1 + A2 + A3 + A4);
    }

    private float Modulus(Vector3 point)
    {
        return Mathf.Sqrt(point.x * point.x + point.y * point.y + point.z * point.z);
    }

    private double CalcAngleSum(Vector3 point, Vector3[] polygon)
    {
        float anglesum = 0;

        for(int i = 0; i < polygon.Length; i++)
        {
            Vector3 p1 = polygon[i] - point;
            Vector3 p2 = polygon[(i+1)%polygon.Length] - point;

            float m1 = Modulus(p1);
            float m2 = Modulus(p2);

            float val = 0.0f;

            if(m1 * m2 <= 0.0000001f) // EPSILON
            {
                return(2 * Mathf.PI);
            }
            else
            {
                anglesum += Mathf.Acos((p1.x*p2.x + p1.y*p2.y + p1.z*p2.z) / (m1*m2));
            }
        }

        return anglesum;
    }
}


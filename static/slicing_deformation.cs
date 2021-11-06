/*
 * Author: Brent Barbachem
 * Date:   November 17, 2015
 * 
 * Description: Dynamically cut a cubic object. Pick a 
 * 				random point on the bottom of the object
 * 				and create a random-walk crack through the 
 * 				object.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class slicing_deformation : MonoBehaviour 
{
	Mesh mesh1, mesh2;
	Material mat;
	List<Vector3> Points1, Points2;
	List<Vector3> Verts1, Verts2;
	List<int> tris1, tris2;
	List<Vector2> UVs1, UVs2;
	List<int> reg1, reg2;
	GameObject ob1, ob2;

	/* number of points in the crack */
	private int crack_length;
	/* Local Position Values */
	private float maxx = float.NegativeInfinity;
	private float minx = float.PositiveInfinity;
	private float maxy = float.NegativeInfinity;
	private float miny = float.PositiveInfinity;
	private float maxz = float.NegativeInfinity;
	private float minz = float.PositiveInfinity;
	/* World Position Values */
	private float worldx, worldy, worldz;
	
	/*
	 * 
	 * Called at the start of the simulation
	 * 
	 */
	void Start () 
	{
		/* get the dimensions of the game object */
		dimension_setup ();
		/* 
		 * Take the original material so the new 
		 * objects look similar
		 */
		mat = GetComponent<Renderer>().material;
		Destroy (gameObject);
		
		ob1 = new GameObject ();
		ob2 = new GameObject ();
		
		/* Initialize the points */
		Points1 = new List<Vector3>();
		Points2 = new List<Vector3>();
		
		/* Initialize the verts */
		Verts1 = new List<Vector3> ();
		Verts2 = new List<Vector3> ();
		
		/* initialize the triangles */
		tris1 = new List<int> ();
		tris2 = new List<int> ();
		
		/* Initialize the UV Points */
		UVs1 = new List<Vector2>();
		UVs2 = new List<Vector2>();
		
		/* initialize the regions */
		reg1 = new List<int> ();
		reg2 = new List<int> ();
		
		/* Call the create mesh method */
		CreateMesh ();
		
	}	


	/* 
	 * 
	 * Called once per frame
	 * 
	 */
	void Update () 
	{
		
	}
	
	private void CreateMesh()
	{
		ob1.AddComponent <MeshFilter>();
		ob1.AddComponent <MeshRenderer>();
		ob1.AddComponent <MeshCollider>();
		
		ob2.AddComponent <MeshFilter>();
		ob2.AddComponent <MeshRenderer>();
		ob2.AddComponent <MeshCollider>();

		MeshFilter meshFilter1 = ob1.GetComponent<MeshFilter> ();
		MeshFilter meshFilter2 = ob2.GetComponent<MeshFilter> ();
		if (meshFilter1 == null || meshFilter2 == null) 
		{
			Debug.LogError("MeshFilter not Found!");
			return;
		}
		
		mesh1 = ob1.GetComponent<MeshFilter>().sharedMesh;
		mesh2 = ob2.GetComponent<MeshFilter>().sharedMesh;
		if (mesh1 == null) 
		{
			ob1.GetComponent<MeshFilter>().mesh = new Mesh();
			mesh1 = ob1.GetComponent<MeshFilter>().sharedMesh;
		}
		
		if (mesh2 == null) 
		{
			ob2.GetComponent<MeshFilter>().mesh = new Mesh();
			mesh2 = ob2.GetComponent<MeshFilter>().sharedMesh;
		}
		
		MeshCollider meshCollider1 = ob1.GetComponent<MeshCollider> ();
		MeshCollider meshCollider2 = ob2.GetComponent<MeshCollider> ();
		if (meshCollider1 == null) 
		{
			Debug.LogError("MeshCollider not Found!");
			return;
		}
		else if (meshCollider2 == null) 
		{
			Debug.LogError("MeshCollider not Found!");
			return;
		}
		


		/*
		 * 
		 * Make sure that we have a few points for our objects
		 * 
		 */
		while (Points1.Count < 5 || Points2.Count < 5) 
		{
			/* clear out all of the points */
			Points1.Clear();
			Points2.Clear ();

			/* Add in all of the points for the new objects */
			add_points ();
		}
		
		mesh1.Clear ();
		mesh2.Clear ();
		UpdateMesh ();
	}
	
	/*
	 * 
	 * Go through the process of adding the points to
	 * the points list 
	 * 
	 */
	private void add_points()
	{
		int count1 = 0;
		int count2 = 0;
		
		/* start of the front planes */
		reg1.Add (count1);
		reg2.Add (count2);
		
		List<Vector3> the_crack = crack (minz);
		crack_length = the_crack.Count; /* store the length of the crack */
		
		the_crack.Reverse (); /* we need to start at the top for Points1 */
		
		/* 
		 * 
		 * Left Side of the Crack
		 * 
		 */
		
		Points1.Add (new Vector3 (minx, miny, minz)); /* bottom left */
		count1++;
		
		/* Did we crack through the right  */
		if (the_crack [0].x >= maxx) 
		{
			/* Add the top Left Point */
			Points1.Add (new Vector3 (minx, maxy, minz));
			/* Add the top Right point */
			Points1.Add (new Vector3 (maxx, maxy, minz));
			
			count1 += 2;
		}
		/* Did we crack through the Left */
		else if (the_crack [0].x <= minx) 
		{
			/* no op */
		} 
		else 
		{
			/* Add the top Left Point */
			Points1.Add (new Vector3 (minx, maxy, minz));
			count1++;
		}
		
		foreach (Vector3 vec in the_crack) 
		{
			Points1.Add (vec);
			count1++;
		}
		
		/* starting place for back plane */
		reg1.Add (count1); 
		
		
		the_crack.Reverse ();
		
		foreach (Vector3 vec in the_crack)
		{
			Points1.Add (new Vector3(vec.x, vec.y, maxz));
			count1++;
		}
		
		
		/* Did we crack through the right  */
		if (the_crack [the_crack.Count - 1].x >= maxx) 
		{
			/* Add the top Right point */
			Points1.Add (new Vector3 (maxx, maxy, maxz));
			/* Add the top Left Point */
			Points1.Add (new Vector3 (minx, maxy, maxz));
			
			count1 += 2;
		}
		/* Did we crack through the Left */
		else if (the_crack [the_crack.Count - 1].x <= minx) 
		{
			/* no op */
		} 
		else 
		{
			/* Add the top Left Point */
			Points1.Add (new Vector3 (minx, maxy, maxz));
			
			count1++;
		}
		
		/* add the bottom left point */
		Points1.Add (new Vector3 (minx, miny, maxz));
		count1++;
		
		/* Starting point for the left plane */
		reg1.Add (count1);
		
		/* 
		 * 
		 * Right Side of the Crack
		 * 
		 */
		
		foreach (Vector3 vec in the_crack) 
		{
			Points2.Add (vec);
			count2++;
		}
		
		/* Did we crack through the right  */
		if (the_crack [the_crack.Count-1].x >= maxx) 
		{
			/* no op */
			
		}
		/* Did we crack through the Left */
		else if (the_crack [the_crack.Count-1].x <= minx) 
		{
			/* Add the top Left Point */
			Points1.Add (new Vector3 (minx, maxy, minz));
			/* Add the top Right point */
			Points2.Add (new Vector3 (maxx, maxy, minz));
			
			count2 += 2;
		} 
		else 
		{
			/* Add the top right Point */
			Points2.Add (new Vector3 (maxx, maxy, minz));
			
			count2++;
		}
		/* Add in the bottom right point */
		Points2.Add (new Vector3 (maxx, miny, minz));
		count2++;
		
		/* starting place for the back plane */
		reg2.Add (count2);
		
		/* the list must be reversed because the back back face points are reversed */
		the_crack.Reverse (); 
		
		/* Add in the bottom right point */
		Points2.Add (new Vector3 (maxx, miny, maxz));
		count2++;
		
		/* Did we crack through the right  */
		if (the_crack [the_crack.Count-1].x >= maxx) 
		{
			/* no op */
			
		}
		/* Did we crack through the Left */
		else if (the_crack [the_crack.Count-1].x <= minx) 
		{
			/* Add the top Left Point */
			Points1.Add (new Vector3 (minx, maxy, maxz));
			/* Add the top Right point */
			Points2.Add (new Vector3 (maxx, maxy, maxz));
			
			count2 += 2;
		} 
		else 
		{
			/* Add the top right Point */
			Points2.Add (new Vector3 (maxx, maxy, maxz));
			
			count2++;
		}
		
		foreach (Vector3 vec in the_crack) 
		{
			Points2.Add (new Vector3(vec.x,vec.y, maxz));
			count2++;
		}
		
		/* Starting point for the left plane */
		reg2.Add (count2);
		
	}
	
	/*
	 * 
	 * Create a random walk to simulate the crack of the object 
	 * 
	 */
	private List<Vector3> crack(float zpos)
	{
		List<Vector3> the_crack = new List<Vector3>();
		
		float xpos = Random.Range (minx, maxx); /* random starting position */
		float ypos = miny; /* bottom of the object */
		the_crack.Add (new Vector3 (xpos, ypos, zpos));
		
		
		while ((ypos <= maxy) && (xpos > minx) && (xpos < maxx)) 
		{
			float val = Random.Range (0.0f, 1.0f);
			
			if (val > 0.5f) 
			{
				xpos += 0.01f;
			}
			else 
			{
				xpos -= 0.01f;
			}
			
			the_crack.Add (new Vector3(xpos, ypos, zpos));
			ypos += 0.01f;
		}
		return the_crack;
		
	}
	
	/*
	 * 
	 * Add the vertices for each game object
	 * and each plane for that object
	 * 
	 */
	private void add_vertices()
	{
		/* 
		 * 
		 * Left Side of the Crack
		 * 
		 */
		
		/* front plane */
		for(int i = 0; i < reg1[1]; i++)
		{
			Verts1.Add (Points1[i]);
		}
		
		/* back plane */
		for(int i = reg1[1]; i < reg1[2]; i++)
		{
			Verts1.Add (Points1[i]);
		}
		
		/* left plane */
		Verts1.Add (Points1 [reg1 [2] - 2]);
		Verts1.Add (Points1 [reg1 [0] + 1]);
		Verts1.Add (Points1 [reg1 [0]]);
		Verts1.Add (Points1 [reg1 [2] - 1]);
		
		/* 
		 * Right Plane
		 * 
		 * This plane is interesting, we are going
		 * to split this into quite a few pieces.
		 * 
		 */
		int left = reg1 [1] - 1;
		int right = reg1 [1];
		
		while ((left > (reg1[0] + 2)) && (right <  reg1[2] - 2)) 
		{
			/* Add in a lot of rectangular areas */
			Verts1.Add (Points1[left]);
			Verts1.Add (Points1[left - 1]);
			Verts1.Add (Points1[right + 1]);
			Verts1.Add (Points1[right]);
			
			left --;
			right ++;
		}
		
		
		/* top plane */
		Verts1.Add (Points1 [reg1 [2] - 2]);
		Verts1.Add (Points1 [reg1 [2] - 3]);
		Verts1.Add (Points1 [reg1 [0] + 2]);
		Verts1.Add (Points1 [reg1 [0] + 1]);
		
		/* bottom plane */
		Verts1.Add (Points1 [reg1 [0]]);
		Verts1.Add (Points1 [reg1 [1] - 1]);
		Verts1.Add (Points1 [reg1 [1]]);
		Verts1.Add (Points1 [reg1 [2] - 1]);
		
		/* 
		 * 
		 * Right Side of the Crack
		 * 
		 */
		
		/* Top Plane */
		for(int i = 0; i < reg2[1]; i++)
		{
			Verts2.Add (Points2[i]);
		}
		
		/* Back Plane */
		for(int i = reg2[1]; i < reg2[2]; i++)
		{
			Verts2.Add (Points2[i]);
		}
		
		/* 
		 * Left Plane
		 * 
		 * This plane is interesting, we are going
		 * to split this into quite a few pieces.
		 * 
		 */
		left = reg2 [2] - 2;
		right = reg2 [0];
		
		while ((left > reg2[1]+2) && (right < (reg2[0]+2+(crack_length-1)))) 
		{
			Verts2.Add (Points2[left]);
			Verts2.Add (Points2[left - 1]);
			Verts2.Add (Points2[right + 1]);
			Verts2.Add (Points2[right]);
			
			left --;
			right ++;
		}
		
		/* Right Plane */
		Verts2.Add (Points2 [reg2 [1] - 2]);
		Verts2.Add (Points2 [reg2 [1] + 1]);
		Verts2.Add (Points2 [reg2 [1]]);
		Verts2.Add (Points2 [reg2 [1] - 1]);
		
		/* Top Plane */
		Verts2.Add (Points2 [reg2 [1] - 3]);
		Verts2.Add (Points2 [reg2 [1] - 2]);
		Verts2.Add (Points2 [reg2 [1] + 1]);
		Verts2.Add (Points2 [reg2 [1] + 2]);
		
		/* Bottom Plane */
		Verts2.Add (Points2 [reg2 [0]]);
		Verts2.Add (Points2 [reg2 [1] - 1]);
		Verts2.Add (Points2 [reg2 [1]]);
		Verts2.Add (Points2 [reg2 [2] - 1]); 
		
	}
	
	
	/*
	 * 
	 * Add the triangles for each game object
	 * and each plane for that object
	 * 
	 */
	private void add_triangles()
	{
		/* 
		 * 
		 * Left Side of the Crack
		 * 
		 */
		
		/* front plane */
		int mid = 1;
		while ((mid+1) < reg1[1]) 
		{
			tris1.Add (reg1[0]); tris1.Add (mid); tris1.Add (mid+1);
			mid++;
		}
		
		/* back plane */
		mid = reg1[1];
		while ((mid+1) < reg1[2]) 
		{
			tris1.Add (reg1[2]-1); tris1.Add (mid); tris1.Add (mid+1);
			mid++;
		}
		
		
		/* left plane */
		tris1.Add (reg1 [2] - 2); tris1.Add (reg1 [0] + 1); tris1.Add (reg1 [0]);
		tris1.Add (reg1 [0]); tris1.Add (reg1 [2] - 1); tris1.Add (reg1 [2] - 2);
		
		/* 
		 * Right Plane
		 * 
		 * This plane is interesting, we are going
		 * to split this into quite a few pieces.
		 * 
		 */
		int left = reg1 [1] - 1;
		int right = reg1 [1];
		
		while ((left > (reg1[0] + 2)) && (right <  reg1[2] - 2)) 
		{
			tris1.Add (left); tris1.Add (left - 1); tris1.Add (right + 1);
			tris1.Add (right + 1); tris1.Add (right); tris1.Add (left);
			
			left --;
			right ++;
		}
		
		/* Top plane */
		tris1.Add (reg1 [2] - 2); tris1.Add (reg1 [2] - 3); tris1.Add (reg1 [0] + 2);
		tris1.Add (reg1 [0] + 2); tris1.Add (reg1 [0] + 1); tris1.Add (reg1 [2] - 2);
		
		/* Bottom plane */
		tris1.Add (reg1 [0]); tris1.Add (reg1 [1] - 1); tris1.Add (reg1 [1]);
		tris1.Add (reg1 [1]); tris1.Add (reg1 [2] - 1); tris1.Add (reg1 [0]);
		
		/* 
		 * 
		 * Right Side of the Crack
		 * 
		 */
		mid = 0;
		while ((mid+2) < reg2[1]) 
		{
			tris2.Add (mid); tris2.Add (mid + 1); tris2.Add (reg2[1]-1);
			mid++;
		}
		
		mid = reg2[1]+1;
		while ((mid+1) < reg2[2]) 
		{
			tris2.Add (reg2[1]); tris2.Add (mid); tris2.Add (mid+1);
			mid++;
		}
		
		/* 
		 * Left Plane
		 * 
		 * This plane is interesting, we are going
		 * to split this into quite a few pieces.
		 * 
		 */
		left = reg2 [2] - 2;
		right = reg2 [0];
		
		while ((left > reg2[1]+2) && (right < (reg2[0]+2+(crack_length-1))))
		{
			tris2.Add (left); tris2.Add (left - 1); tris2.Add (right + 1);
			tris2.Add (right + 1); tris2.Add (right); tris2.Add (left);
			
			left --;
			right ++;
		}
		
		
		/* Right plane */
		tris2.Add (reg2 [1] - 2); tris2.Add (reg2 [1] + 1); tris2.Add (reg2 [1]);
		tris2.Add (reg2 [1]); tris2.Add (reg2[1] - 1); tris2.Add (reg2 [1] - 2);
		
		/* Top plane */
		tris2.Add (reg2[1] + 2);tris2.Add (reg2 [1] + 1);tris2.Add (reg2 [1] - 2);
		tris2.Add (reg2 [1] - 2);tris2.Add (reg2 [1] - 3);tris2.Add (reg2[1] + 2);
		
		/* Bottom plane */
		tris2.Add (reg2 [0]); tris2.Add (reg2 [1] - 1); tris2.Add (reg2 [1]);
		tris2.Add (reg2 [1]); tris2.Add (reg2 [2] - 1); tris2.Add (reg2 [0]); 
		
	}
	
	
	/*
	 * 
	 * Add the UVs for each game object
	 * and each plane for that object
	 * 
	 */
	private void add_uvs()
	{
		/* 
		 * 
		 * Left Side of the Crack
		 * 
		 */
		
		/* front plane */
		UVs1.Add (new Vector2 (0, 0));
		UVs1.Add (new Vector2 (0, 1));
		
		for (int i = 2; i < reg1[1] ; i++) 
		{
			UVs1.Add (new Vector2(Points1[i].x, Points1[i].y));
		}
		
		/* back plane */ 
		for (int i = reg1[1]; i < reg1[2]-2; i++)
		{
			UVs1.Add (new Vector2(Points1[i].x, Points1[i].y));
		}
		
		UVs1.Add (new Vector2 (1, 0));
		UVs1.Add (new Vector2 (0, 0));
		
		
		/* Left Plane */
		UVs1.Add (new Vector2 (0, 1));
		UVs1.Add (new Vector2 (1, 1));
		UVs1.Add (new Vector2 (1, 0));
		UVs1.Add (new Vector2 (0, 0));
		
		/* 
		 * Right Plane
		 * 
		 * This plane is interesting, we are going
		 * to split this into quite a few pieces.
		 * 
		 */
		int left = reg1 [1] - 1;
		int right = reg1 [1];
		
		while ((left > (reg1[0] + 2)) && (right <  reg1[2] - 2)) 
		{
			/* Add in a lot of rectangular areas */
			UVs1.Add (new Vector2(Points1[left].x, Points1[left].y));
			UVs1.Add (new Vector2(Points1[left - 1].x, Points1[left - 1].y));
			UVs1.Add (new Vector2(Points1[right + 1].x, Points1[right + 1].y));
			UVs1.Add (new Vector2(Points1[right].x, Points1[right].y));
			
			left --;
			right ++;
		}
		
		
		/* Top Plane */
		UVs1.Add (new Vector2 (0, 1));
		UVs1.Add (new Vector2 (1, 1));
		UVs1.Add (new Vector2 (1, 0));
		UVs1.Add (new Vector2 (0, 0));
		
		/* Bottom Plane */
		UVs1.Add (new Vector2 (0, 1));
		UVs1.Add (new Vector2 (1, 1));
		UVs1.Add (new Vector2 (1, 0));
		UVs1.Add (new Vector2 (0, 0));
		
		/* 
		 * 
		 * Right Side of the Crack
		 * 
		 */
		/* Front Plane */
		for (int i = 0; i < reg2[1] - 2; i++) 
		{
			UVs2.Add (new Vector2(Points2[i].x, Points2[i].y));
		}
		
		UVs2.Add (new Vector2(1,1));
		UVs2.Add(new Vector2(1, 0));
		
		
		/* Back Plane */
		UVs2.Add (new Vector2(1, 0));
		UVs2.Add (new Vector2(1, 1));
		
		for (int i = reg2[1]+2; i < reg2[2]; i++)
		{
			UVs2.Add (new Vector2(Points2[i].x, Points2[i].y));
		}
		
		
		/* 
		 * Left Plane
		 * 
		 * This plane is interesting, we are going
		 * to split this into quite a few pieces.
		 * 
		 */
		left = reg2 [2] - 2;
		right = reg2 [0];
		
		while ((left > reg2[1]+2) && (right < (reg2[0]+2+(crack_length-1)))) 
		{
			/* Add in a lot of rectangular areas */
			UVs2.Add (new Vector2(Points2[left].x,Points2[left].y));
			UVs2.Add (new Vector2(Points2[left - 1].x, Points2[left - 1].y));
			UVs2.Add (new Vector2(Points2[right + 1].x,Points2[right + 1].y));
			UVs2.Add (new Vector2(Points2[right].x, Points2[right].y));
			
			left --;
			right ++;
		}
		
		/* Right Plane */
		UVs2.Add (new Vector2 (0, 1));
		UVs2.Add (new Vector2 (1, 1));
		UVs2.Add (new Vector2 (1, 0));
		UVs2.Add (new Vector2 (0, 0));
		
		/* Top Plane */
		UVs2.Add (new Vector2 (0, 1));
		UVs2.Add (new Vector2 (1, 1));
		UVs2.Add (new Vector2 (1, 0));
		UVs2.Add (new Vector2 (0, 0));
		
		/* Bottom Plane */
		UVs2.Add (new Vector2 (0, 1));
		UVs2.Add (new Vector2 (1, 1));
		UVs2.Add (new Vector2 (1, 0));
		UVs2.Add (new Vector2 (0, 0));
		
	}
	
	/*
	 * 
	 * Update the Mesh by setting all of the 
	 * vertices, triangles, and UVs for each 
	 * game object.
	 * 
	 */
	private void UpdateMesh()
	{
		/* set the positions of the new objects */
		ob1.transform.position = new Vector3(worldx, worldy, worldz);
		ob2.transform.position = new Vector3(worldx, worldy, worldz);
		
		/*
		 * 
		 * Add in our vertices, triangles and UVs
		 * 
		 */
		add_vertices ();
		add_triangles ();
		add_uvs ();
		
		/* Turn the list into an array */
		mesh1.vertices = Verts1.ToArray ();
		mesh1.triangles = tris1.ToArray ();
		mesh1.uv = UVs1.ToArray ();
		
		/* Clear out all of the stored information - so we can reuse add*/
		Verts1.Clear ();
		tris1.Clear ();
		UVs1.Clear ();
		
		MeshCollider mc1 = ob1.GetComponent<MeshCollider> ();
		mesh1.RecalculateNormals ();
		mesh1.RecalculateBounds ();
		RecalculateTangents (mesh1);
		/* set to null first - or it wont update in unity */
		mc1.sharedMesh = null;
		mc1.sharedMesh = mesh1;
		ob1.GetComponent<Renderer>().material = mat;
		;
		
		
		/* Turn the list into an array */
		mesh2.vertices = Verts2.ToArray ();
		mesh2.triangles = tris2.ToArray ();
		mesh2.uv = UVs2.ToArray ();
		
		/* Clear out all of the stored information - so we can reuse add*/
		Verts2.Clear ();
		tris2.Clear ();
		UVs2.Clear ();
		
		MeshCollider mc2 = ob2.GetComponent<MeshCollider> ();
		mesh2.RecalculateNormals ();
		mesh2.RecalculateBounds ();
		RecalculateTangents (mesh2);
		/* set to null first - or it wont update in unity */
		mc2.sharedMesh = null;
		mc2.sharedMesh = mesh2;
		ob2.GetComponent<Renderer>().material = mat;
		;
		
	}
	
	
	/*
	 * 
	 * Recalculate the tangents for the meshes 
	 * 
	 * NOTE: this was constructed through the help of
	 * Blackmodule studio
	 * 
	 * https://www.youtube.com/watch?v=hL0TXUY6Vl0
	 * 
	 */
	private static void RecalculateTangents(Mesh mesh)
	{
		/* get all of the data from the mesh */
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;
		
		int triCount = triangles.Length;
		int vertCount = vertices.Length;
		
		Vector3[] tan1 = new Vector3[vertCount];
		Vector3[] tan2 = new Vector3[vertCount];
		
		Vector4[] tangents = new Vector4[vertCount];
		
		for (long a = 0; a < triCount; a+= 3) 
		{
			long i1 = triangles[a+0];
			long i2 = triangles[a+1];
			long i3 = triangles[a+2];
			
			Vector3 v1 = vertices[i1];
			Vector3 v2 = vertices[i2];
			Vector3 v3 = vertices[i3];
			
			Vector2 w1 = uv[i1];
			Vector2 w2 = uv[i2];
			Vector2 w3 = uv[i3];
			
			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;
			
			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;
			
			float div = s1 * t2 - s2 * t1;
			float r = div == 0.0f ? 0.0f : 1.0f/div;
			/* s direction */
			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, 
			                           (t2 * y1 - t1 * y2) * r,
			                           (t2 * z1 - t1 * z2) * r);
			/* t direction */
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, 
			                           (s1 * y2 - s2 * y1) * r,
			                           (s1 * z2 - s2 * z1) * r);
			
			/* set values in tangent array */
			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;
			
			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}
		
		for (long a = 0; a < vertCount; ++a) 
		{
			Vector3 n = normals[a];
			Vector3 t = tan1[a];
			
			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;
			tangents[a].w = (Vector3.Dot (Vector3.Cross (n,t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
			
			mesh.tangents = tangents;
		}
	}
	
	
	/*
	 * 
	 * Get all of the correct points for the cube
	 * 
	 */
	private void dimension_setup()
	{
		/* Get the world coordinates */
		worldx = transform.position.x;
		worldy = transform.position.y;
		worldz = transform.position.z;
		
		Mesh mesh = gameObject.GetComponent<MeshFilter> ().mesh;
		foreach (Vector3 vec in mesh.vertices) 
		{
			if(vec.x > maxx) { maxx = vec.x; }
			if(vec.x < minx) { minx = vec.x; }
			if(vec.y > maxy) { maxy = vec.y; }
			if(vec.y < miny) { miny = vec.y; }
			if(vec.z > maxz) { maxz = vec.z; }
			if(vec.x < minz) { minz = vec.z; }
		}
		
		/*
		 * 
		 * The points are in local coordinate space
		 * so mulitply them by the scaled values
		 *
		 */
		maxx *= transform.localScale.x;
		minx *= transform.localScale.x;
		maxy *= transform.localScale.y;
		miny *= transform.localScale.y;
		maxz *= transform.localScale.z;
		minz *= transform.localScale.z;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class MeshBuilder {

	private List<Vector3> _vertices;
	private List<Vector2> _uvs;
	private List<int> _triangles;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="MeshBuilder"/> class.
	/// </summary>
	public MeshBuilder() {
		_vertices = new List<Vector3>();
		_uvs = new List<Vector2>();
		_triangles = new List<int>();
	}
	
	/// <summary>
	/// Clear all internal lists. You can create a new mesh definition after this.
	/// </summary>
	public void Clear() {
		_vertices.Clear ();
		_uvs.Clear ();
		_triangles.Clear ();
	}

	/// <summary>
	/// Adds a vertex to the list, based on a Vector3f position and an optional Vector2f uv set
	/// </summary>
	/// <returns>The vertex.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="u">The u coordinate</param>
	/// <param name="v">The v coordinate</param>
	public int AddVertex(Vector3 position, Vector2 uv = new Vector2()) {
		int newVertexIndex = _vertices.Count;
		_vertices.Add (new Vector3 (position.x, position.y, position.z));
		_uvs.Add (new Vector2 (uv.x, uv.y));
		return newVertexIndex;
	}

	/// <summary>
	/// Adds a triangle to the list, based on three vertex indices.
	/// </summary>
	/// <param name="vertexIndex1">Vertex 1 index.</param>
	/// <param name="vertexIndex2">Vertex 2 index.</param>
	/// <param name="vertexIndex3">Vertex 3 index.</param>
	public void AddTriangle(int vertexIndex1, int vertexIndex2, int vertexIndex3) {
		_triangles.Add (vertexIndex1);
		_triangles.Add (vertexIndex2);
		_triangles.Add (vertexIndex3);
	}

	/// <summary>
	/// Creates the mesh. Note: this will not reset any of the internal lists. (Use Clear to do that)
	/// </summary>
	/// <returns>The mesh.</returns>
	/// <param name="shouldRecalculateNormals">If set to <c>true</c> should recalculate normals.</param>
	public Mesh CreateMesh(bool shouldRecalculateNormals=true) {
		Mesh mesh = new Mesh ();
		mesh.vertices = this._vertices.ToArray ();
		mesh.uv = this._uvs.ToArray ();
		mesh.triangles = this._triangles.ToArray ();
		if (shouldRecalculateNormals == true) {
			mesh.RecalculateNormals ();
		}
		mesh.RecalculateBounds ();
		return mesh;
	}

	/// <summary>
	/// Saves the mesh as an OBJ file.
	/// </summary>
	/// <param name="mesh">Mesh.</param>
	/// <param name="filename">Filename. Does not automatically set the extension.</param>
	static public void SaveMeshToObj(Mesh mesh, string filename) {
		StreamWriter writer = new StreamWriter (filename);

		//object name
		writer.WriteLine ("#Mesh\n");
		writer.WriteLine ("g mesh\n");

		//vertices
		for (int i=0; i<mesh.vertices.Length; i++) {
			float x = mesh.vertices[i].x;
			float y = mesh.vertices[i].y;
			float z = mesh.vertices[i].z;
			writer.WriteLine (String.Format ("v {0:F3} {1:F3} {2:F3}", x, y, z));
		}
		writer.WriteLine ("");

		//uv-set
		for (int i=0; i<mesh.uv.Length; i++) {
			float u = mesh.uv[i].x;
			float v = mesh.uv[i].y;
			writer.WriteLine (String.Format ("vt {0:F3} {1:F3}", u, v));
		}
		writer.WriteLine ("");

		//normals
		for (int i=0; i<mesh.normals.Length; i++) {
			float x = mesh.normals[i].x;
			float y = mesh.normals[i].y;
			float z = mesh.normals[i].z;
			writer.WriteLine (String.Format ("vn {0:F3} {1:F3} {2:F3}", x, y, z));
		}
		writer.WriteLine ("");

		//triangles
		for (int i=0; i<mesh.triangles.Length / 3; i++) {
			int v0 = mesh.triangles[i * 3 + 0]+1;
			int v1 = mesh.triangles[i * 3 + 1]+1;
			int v2 = mesh.triangles[i * 3 + 2]+1;
			writer.WriteLine(String.Format ("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", v0, v1, v2));
		}

		writer.Close ();
	}
}

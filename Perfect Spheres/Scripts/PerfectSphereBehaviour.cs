using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerfectSphereBehaviour : MonoBehaviour
{
	private const int _numVertices = 4;
	public Texture2D _texture;
	
	public float _radius = 0.5f;
	
	private MeshFilter _meshFilter;
	private MeshRenderer _meshRenderer;

	private Vector3[] _vertices;
	private Vector3[] _normals;
	private Vector4[] _tangents;
	
	private void Start()
	{
		_meshFilter = gameObject.AddComponent<MeshFilter>();
		_meshRenderer = gameObject.AddComponent<MeshRenderer>();

		if (_texture != null)
		{
			_meshRenderer.material = new Material(Shader.Find("Sphere Texture"));
			_meshRenderer.material.SetTexture("_Texture", _texture);
		}
		else
		{
			_meshRenderer.material = new Material(Shader.Find("Sphere"));
		}
		
		_vertices = new Vector3[_numVertices];
		_normals = new Vector3[_numVertices];
		_tangents = new Vector4[_numVertices];
		
		_meshFilter.mesh.vertices = _vertices;
		_meshFilter.mesh.normals = _normals;
		_meshFilter.mesh.tangents = _tangents;
		
		int[] _triangles = new int[3*(_numVertices - 2)];

		for (int i = 0; i < _numVertices - 2; i++)
		{
			_triangles[3 * i + 1] = i + 2;
			_triangles[3 * i + 2] = i + 1;
		}

		_meshFilter.mesh.triangles = _triangles;

	}

	void Update ()
	{
		//transform.rotation = Quaternion.Euler(0, 20*Time.realtimeSinceStartup, 0);

		Camera mainCamera = Camera.main;
		Vector3 direction;
		float alpha;
		
		if (mainCamera.orthographic)
		{
			direction = mainCamera.transform.forward;
			alpha = 0;
		}
		else
		{
			direction = transform.position - mainCamera.transform.position;
			float distance = direction.magnitude;
			direction /= distance;

			if (distance < _radius)
			{
				_meshRenderer.enabled = false;
				return;
			}
			
			alpha = Mathf.Asin(_radius / distance);
		}

		_meshRenderer.enabled = true;
		Quaternion rotation = Quaternion.LookRotation(direction, mainCamera.transform.up);
		rotation = new Quaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w) * transform.rotation;
		float offsetXY = _radius * Mathf.Cos(alpha) / Mathf.Cos(Mathf.PI / _numVertices);
		float offsetZ = _radius * Mathf.Sin(alpha);
		for (int i = 0; i < _numVertices; i++)
		{
			float theta = 2 * Mathf.PI * (i + 0.5f) / _numVertices;
			_vertices[i] = offsetXY * new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) + offsetZ * Vector3.back;
			_vertices[i] = Rotate(_vertices[i], rotation);
			_normals[i] = -direction;
			_tangents[i] = mainCamera.transform.up;
			_tangents[i].w = 1;
		}

		_meshFilter.mesh.vertices = _vertices;
		_meshFilter.mesh.normals = _normals;
		_meshFilter.mesh.tangents = _tangents;
		
		_meshFilter.mesh.RecalculateBounds();

		_meshRenderer.material.SetFloat("_Radius", _radius);
		_meshRenderer.material.SetVector("_Position", transform.position);
		if (_texture != null)
		{
			Quaternion transformRotation = transform.rotation;
			_meshRenderer.material.SetVector("_Rotation",
				new Vector4(transformRotation.x, transformRotation.y, transformRotation.z, transformRotation.w));
		}

		_meshRenderer.material.SetInt("_Orthographic", mainCamera.orthographic ? 1 : 0);
	}

	static Vector3 Rotate(Vector3 V, Quaternion Q)
	{
		Quaternion ret = new Quaternion(-Q.x, -Q.y, -Q.z, Q.w) * new Quaternion(V.x, V.y, V.z, 0) * Q;
		return new Vector3(ret.x, ret.y, ret.z);
	}
}

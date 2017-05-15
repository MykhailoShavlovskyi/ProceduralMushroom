using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class LatheSpline : MonoBehaviour {
	
	bool DontUpdateSliders = false;

	const int NUMSPLINES = 15;

	public Slider rootHeightSlider;
	float rootHeight = 1; 

	public Slider rootWidthSlider;
	float rootWidth = 1; 

	public Slider stemWidthSlider;
	float stemWidth = 1; 

	public Slider stemHeightSlider;
	float stemHeight = 1; 

	public Slider capAHeightSlider;
	float capAHeight = 1; 

	public Slider capAWidthSlider;
	float capAWidth = 1; 

	public Toggle toggle;
	bool shapeOne = true;

	public Slider capBWidthSlider;
	float capBWidth = 1;

	public Slider capBOverhangSlider;
	float capBOverhang = 1;

	public Slider capBOverhangWidthSlider;
	float capBOverhangWidth = 1;

	public Slider capbHeightSlider;
	float capbHeight = 1; 

	System.Random rand = new  System.Random ();

	//[SerializeField]
	private Vector2[] _points = new Vector2[8];

	private bool _dirty = true;
	
	//------------------------------------------------------------------------------------------------------------------
	//													Start()
	//------------------------------------------------------------------------------------------------------------------
	private void Start() 
	{ 
		Randomize ();
	}
	
	//------------------------------------------------------------------------------------------------------------------
	//													GetPoint()
	//------------------------------------------------------------------------------------------------------------------
	public Vector2 GetPoint(int index) 
	{
		if (index < 0) return Vector2.zero;
		if (index >= _points.Length) return Vector2.zero;
		if (_points == null) return Vector2.zero;
		return _points[index];
	}

	//------------------------------------------------------------------------------------------------------------------
	//													SetPoint()
	//------------------------------------------------------------------------------------------------------------------
	public void SetPoint(int index, Vector2 point)
	{
		if (index < 0) return;
		if (index >= _points.Length) return;
		if (_points == null) return;
		_points[index] = point;
		_dirty = true;
	}
	
	//------------------------------------------------------------------------------------------------------------------
	//													NumberOfPoints()
	//------------------------------------------------------------------------------------------------------------------
	public int NumberOfPoints 
	{
		get { return (_points != null)?_points.Length:0; }
	}
	
	//------------------------------------------------------------------------------------------------------------------
	//													getIndex()
	//------------------------------------------------------------------------------------------------------------------
	private int getIndex(int x, int y, int height)
	{
		return y + x * height;
	}

	//------------------------------------------------------------------------------------------------------------------
	//													UpdateMesh()
	//------------------------------------------------------------------------------------------------------------------
	public void UpdateMesh()
	{	
		//nothing changed
		if (_dirty == false) return;
		
		//start building mesh
		MeshBuilder meshBuilder = new MeshBuilder();
		
		int splineVertexCount = _points.Length;
		int splineCount = NUMSPLINES;

		//generate vertices
		for (int splineIndex=0; splineIndex<=splineCount; splineIndex++)
		{
			Matrix4x4 mat = Matrix4x4.TRS (Vector3.zero, Quaternion.Euler (0, splineIndex * 360.0f / splineCount, 0), new Vector3(1, 1, 1));
			for (int j=0; j<splineVertexCount; j++) 
			{
				Vector3 vertex = new Vector3(_points[j].x, _points[j].y);
				vertex = mat.MultiplyPoint (vertex);
				meshBuilder.AddVertex (vertex);
			}
		}
		
		//generate triangles
		for (int splineIndex=1; splineIndex<=splineCount; splineIndex++)
		{ //start at 1, because we need to access spline at splineIndex-1
			for (int vertexIndex=1; vertexIndex<splineVertexCount; vertexIndex++)
			{ //start at 1, because we need to access vertex at vertexIndex-1
				int v0 = getIndex (splineIndex - 1, vertexIndex - 1, splineVertexCount);
				int v1 = getIndex (splineIndex, vertexIndex - 1, splineVertexCount);
				int v2 = getIndex (splineIndex, vertexIndex, splineVertexCount);
				int v3 = getIndex (splineIndex - 1, vertexIndex, splineVertexCount);
				meshBuilder.AddTriangle (v0, v1, v2);
			    meshBuilder.AddTriangle (v0, v2, v3);
			}
		}
		
		//generate mesh and apply it to meshfilter
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		meshFilter.sharedMesh = meshBuilder.CreateMesh ();
		
		//unset dirty flag
		_dirty = false;
	}

	public void Randomize()
	{
		rootHeight = rand.Next (1,4);
		rootWidth = rand.Next (1,4);
		stemWidth = rand.Next (1,6);
		stemHeight = rand.Next (10,31);
		capAHeight = rand.Next (2, 11);
		capAWidth = rand.Next (4,16);
		int shapeint = rand.Next (0, 2);
		if (shapeint == 0)
			shapeOne = true;
		else
			shapeOne = false;
		capBWidth = rand.Next (4,16);
		capBOverhang = rand.Next (1,6);
		capBOverhangWidth = rand.Next (1,11);
		capbHeight = rand.Next (2, 16);

		DontUpdateSliders = true;
		rootHeightSlider.value = rootHeight;
		rootWidthSlider.value=rootWidth;
		stemWidthSlider.value=stemWidth;
		stemHeightSlider.value=stemHeight;
		capAHeightSlider.value=capAHeight;
		capAWidthSlider.value=capAWidth;
		toggle.isOn=shapeOne;
		capBWidthSlider.value=capBWidth;
		capBOverhangSlider.value=capBOverhang;
		capBOverhangWidthSlider.value=capBOverhangWidth;
		capbHeightSlider.value=capbHeight;
		DontUpdateSliders = false;
	    

		UpdateSplinePointsPositions ();

		_dirty = true;
		UpdateMesh ();
	}

	public void UpdateSplinePointsPositions()
	{
		_points [0] = new Vector2 (rootWidth+stemWidth, 0);
		_points [1] = new Vector2 (stemWidth, rootHeight);
		_points [2] = new Vector2 (stemWidth, rootHeight+stemHeight);
		if (shapeOne) 
		{
			_points [3] = new Vector2 (stemWidth + capAWidth / 1.2f, rootHeight + stemHeight + (capAHeight / 3));
			_points [4] = new Vector2 (stemWidth + capAWidth, rootHeight + stemHeight + (capAHeight / 3 * 2));
			_points [5] = new Vector2 (stemWidth + capAWidth / 1.2f, rootHeight + stemHeight + (capAHeight / 6 * 5));
			_points [6] = new Vector2 (stemWidth, rootHeight + stemHeight + capAHeight);
			_points [7] = new Vector2 (0, rootHeight + stemHeight + capAHeight);
		} 
		else 
		{
			_points [3] = new Vector2 (stemWidth + capBWidth/3*2,rootHeight+stemHeight);
			_points [4] = new Vector2 (stemWidth + capBWidth,rootHeight+stemHeight-capBOverhang);
			_points [5] = new Vector2 (stemWidth + capBWidth+capBOverhangWidth,rootHeight+stemHeight-capBOverhang);
			_points [6] = new Vector2 (stemWidth + capBWidth/6*5,rootHeight+stemHeight+capbHeight/10*9);
			_points [7] = new Vector2 (0,rootHeight+stemHeight+capbHeight);
		}
	}

	public void UpdateSliders()
	{
		if (!DontUpdateSliders) 
		{
			rootHeight = rootHeightSlider.value;
			rootWidth = rootWidthSlider.value;
			stemWidth = stemWidthSlider.value;
			stemHeight = stemHeightSlider.value;
			capAHeight = capAHeightSlider.value;
			capAWidth = capAWidthSlider.value;
			shapeOne = toggle.isOn;
			capBWidth = capBWidthSlider.value;
			capBOverhang = capBOverhangSlider.value;
			capBOverhangWidth = capBOverhangWidthSlider.value;
			capbHeight = capbHeightSlider.value;
			UpdateSplinePointsPositions ();

			_dirty = true;
			UpdateMesh ();
		} 
	}

}
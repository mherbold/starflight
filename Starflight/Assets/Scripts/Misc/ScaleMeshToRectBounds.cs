
using UnityEngine;

public class ScaleMeshToRectBounds : MonoBehaviour
{
	public bool m_keepAspectRato;

	// unity start
	void Start()
	{
		// force the canvas to update
		Canvas.ForceUpdateCanvases();

		// grab the mesh
		var meshFilter = GetComponent<MeshFilter>();

		// grab the rect transform
		var rectTransform = GetComponent<RectTransform>();

		// get the mesh
		var mesh = meshFilter.sharedMesh;

		// get the mesh bounds
		var bounds = mesh.bounds;

		// calculate the mesh size in X and Y
		var xMeshSize = bounds.extents.x * 2;
		var yMeshSize = bounds.extents.y * 2;

		// grab the rect bounds in world space
		var fourCornersArray = new Vector3[ 4 ];

		rectTransform.GetWorldCorners( fourCornersArray );

		// calculate the rect size in X and Y
		var xRectSize = fourCornersArray[ 1 ].x - fourCornersArray[ 2 ].x;
		var yRectSize = fourCornersArray[ 1 ].y - fourCornersArray[ 0 ].y;

		// calculate the scale
		var xScale = xRectSize / xMeshSize * 108.0f; // TODO: figure out why 108 - where does that come from?  it's exactly right... but why?
		var yScale = yRectSize / yMeshSize * 108.0f; // TODO: figure out why 108 - where does that come from?  it's exactly right... but why?

		if ( m_keepAspectRato )
		{
			if ( xScale > yScale )
			{
				xScale = yScale;
			}

			if ( yScale > xScale )
			{
				yScale = xScale;
			}
		}

		// apply the scale to the mesh
		rectTransform.localScale = new Vector3( xScale, yScale, 1 );
	}
}

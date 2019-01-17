
using UnityEngine;
using UnityEditor;

using System.IO;

[ExecuteInEditMode]
public class BuildAtmosphereMaps : EditorWindow
{
	const int c_size = 512;
	const float c_atmosphereThickness = 0.05f;
	const float c_insideThickness = 0.95f;

	string m_filename = "";

	private void OnEnable()
	{
		m_filename = Application.dataPath + "/Image.png";
	}

	[MenuItem( "Starflight Remake/Build Atmosphere Maps" )]
	public static void ShowWindow()
	{
		EditorWindow editorWindow = EditorWindow.GetWindow( typeof( BuildAtmosphereMaps ) );

		editorWindow.autoRepaintOnSceneChange = true;

		editorWindow.Show();

		editorWindow.titleContent = new GUIContent( "Build Atmosphere Maps" );
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Build Atmosphere Maps", GUILayout.MinHeight( 60 ) ) )
		{
			m_filename = EditorUtility.SaveFilePanel( "Image File Name", Path.GetDirectoryName( m_filename ), Path.GetFileName( m_filename ), "png" );

			if ( m_filename.Length > 0 )
			{
				var baseFileName = Path.GetDirectoryName( m_filename ) + "/" + Path.GetFileNameWithoutExtension( m_filename );

				Debug.Log( "baseFileName = " + baseFileName );

				GenerateMaps( out var normalMap, out var opacityMap );

				PG_Tools.SaveAsPNG( normalMap, baseFileName + " - Normal.png" );
				PG_Tools.SaveAsPNG( opacityMap, baseFileName + " - Opacity.png" );
			}
		}
	}

	void GenerateMaps( out Color[,] normalMap, out Color[,] opacityMap )
	{
		normalMap = new Color[ c_size, c_size ];
		opacityMap = new Color[ c_size, c_size ];

		for ( var y = 0; y < c_size; y++ )
		{
			var dy = ( y - c_size * 0.5f ) / ( c_size * 0.5f );

			for ( var x = 0; x < c_size; x++ )
			{
				var dx = ( x - c_size * 0.5f ) / ( c_size * 0.5f );

				var vector = new Vector3( dx, dy, 0.0f );

				var pointOnRay = new Vector3( dx, dy, -10.0f );

				if ( IntersectRaySphere( pointOnRay, Vector3.forward, Vector3.zero, 1.0f - c_atmosphereThickness, out var collisionPoint ) )
				{
					collisionPoint.Normalize();

					normalMap[ y, x ] = new Color( collisionPoint.x * 0.5f + 0.5f, collisionPoint.y * 0.5f + 0.5f, collisionPoint.z * -0.5f + 0.5f );

					var opacity = Mathf.Lerp( 1.0f, 0.0f, ( ( 1.0f - c_atmosphereThickness ) - vector.magnitude ) / c_insideThickness );

					opacity = Mathf.Pow( opacity, 2.0f );

					opacityMap[ y, x ] = new Color( opacity, opacity, opacity );
				}
				else
				{
					var normal = Vector3.Normalize( vector );

					normalMap[ y, x ] = new Color( normal.x * 0.5f + 0.5f, normal.y * 0.5f + 0.5f, normal.z * 0.5f + 0.5f );

					var opacity = Mathf.SmoothStep( 1.0f, 0.0f, ( vector.magnitude - ( 1.0f - c_atmosphereThickness ) ) / c_atmosphereThickness );

					opacityMap[ y, x ] = new Color( opacity, opacity, opacity );
				}
			}
		}
	}

	bool IntersectRaySphere( Vector3 pointOnRay, Vector3 rayDirection, Vector3 sphereCenter, float sphereRadius, out Vector3 collisionPoint )
	{
		var m = pointOnRay - sphereCenter;

		float b = Vector3.Dot( m, rayDirection );
		float c = Vector3.Dot( m, m ) - sphereRadius * sphereRadius;

		if ( ( c > 0.0f ) && ( b > 0.0f ) )
		{
			collisionPoint = Vector3.zero;
			return false;
		}

		float d = b * b - c;

		if ( d < 0.0f )
		{
			collisionPoint = Vector3.zero;
			return false;
		}

		var t = -b - Mathf.Sqrt( d );

		if ( t < 0.0f )
		{
			t = 0.0f;
		}

		collisionPoint = pointOnRay + t * rayDirection;

		return true;
	}
}

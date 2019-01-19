
using UnityEngine;
using UnityEditor;

using System.IO;

[ExecuteInEditMode]
public class BuildAtmosphereSkybox : EditorWindow
{
	const int c_size = 32;
	const float c_atmosphereStartFadeAngle = 0.0f;
	const float c_atmosphereEndFadeAngle = 45.0f;

	private void OnEnable()
	{
	}

	[MenuItem( "Starflight Remake/Build Atmosphere Skybox" )]
	public static void ShowWindow()
	{
		EditorWindow editorWindow = EditorWindow.GetWindow( typeof( BuildAtmosphereSkybox ) );

		editorWindow.autoRepaintOnSceneChange = true;

		editorWindow.Show();

		editorWindow.titleContent = new GUIContent( "Build Atmosphere Skybox" );
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Build Atmosphere Skybox", GUILayout.MinHeight( 60 ) ) )
		{
			GenerateMaps( out var bottomMap, out var sideMap, out var topMap );

			PG_Tools.SaveAsPNG( bottomMap, Application.dataPath + "/Maps/Skyboxes/Planet/planet_bottom.png" );
			PG_Tools.SaveAsPNG( sideMap, Application.dataPath + "/Maps/Skyboxes/Planet/planet_sides.png" );
			PG_Tools.SaveAsPNG( topMap, Application.dataPath + "/Maps/Skyboxes/Planet/planet_top.png" );
		}
	}

	void GenerateMaps( out Color[,] bottomMap, out Color[,] sideMap, out Color[,] topMap )
	{
		// calculate scale and offset
		var scale = 2.0f / c_size;
		var offset = -1.0f + scale / 2.0f;

		// bottom
		bottomMap = new Color[ c_size, c_size ];

		for ( var y = 0; y < c_size; y++ )
		{
			for ( var x = 0; x < c_size; x++ )
			{
				var sx = x * scale + offset;
				var sy = y * scale + offset;

				var v1 = new Vector3( sx, -1.0f, sy );
				var v2 = new Vector3( sx, 0.0f, sy );
				var axis = new Vector3( sy, 0.0f, -sx );

				var angle = ( v2.sqrMagnitude < 0.0001f ) ? -90.0f : Vector3.SignedAngle( v1, v2, axis );

				angle = ( angle - c_atmosphereStartFadeAngle ) / ( c_atmosphereEndFadeAngle - c_atmosphereStartFadeAngle );

				var alpha = Mathf.SmoothStep( 1.0f, 0.0f, angle );

				bottomMap[ y, x ] = new Color( alpha, alpha, alpha, 1.0f );
			}
		}

		// side
		sideMap = new Color[ c_size, c_size ];

		for ( var y = 0; y < c_size; y++ )
		{
			for ( var x = 0; x < c_size; x++ )
			{
				var sx = x * scale + offset;
				var sy = y * scale + offset;

				var v1 = new Vector3( sx, sy, 1.0f );
				var v2 = new Vector3( sx, 0.0f, 1.0f );
				var axis = new Vector3( 1.0f, 0.0f, sx );

				var angle = ( v2.sqrMagnitude < 0.0001f ) ? 0.0f : Vector3.SignedAngle( v1, v2, axis );

				angle = ( angle - c_atmosphereStartFadeAngle ) / ( c_atmosphereEndFadeAngle - c_atmosphereStartFadeAngle );

				var alpha = Mathf.SmoothStep( 1.0f, 0.0f, angle );

				sideMap[ y, x ] = new Color( alpha, alpha, alpha, 1.0f );
			}
		}

		// top
		topMap = new Color[ c_size, c_size ];

		for ( var y = 0; y < c_size; y++ )
		{
			for ( var x = 0; x < c_size; x++ )
			{
				var sx = x * scale + offset;
				var sy = y * scale + offset;

				var v1 = new Vector3( sx, 1.0f, sy );
				var v2 = new Vector3( sx, 0.0f, sy );
				var axis = new Vector3( sy, 0.0f, -sx );

				var angle = ( v2.sqrMagnitude < 0.0001f ) ? 90.0f : Vector3.SignedAngle( v1, v2, axis );

				angle = ( angle - c_atmosphereStartFadeAngle ) / ( c_atmosphereEndFadeAngle - c_atmosphereStartFadeAngle );

				var alpha = Mathf.SmoothStep( 1.0f, 0.0f, angle );

				topMap[ y, x ] = new Color( alpha, alpha, alpha, 1.0f );
			}
		}
	}
}

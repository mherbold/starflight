
using UnityEngine;
using UnityEditor;

using System.IO;

[ExecuteInEditMode]
public class BuildPerlinNoiseTextureMaps : EditorWindow
{
	const int c_size = 1024;

	private void OnEnable()
	{
	}

	[MenuItem( "Starflight Remake/Build Perlin Noise Texture Maps" )]
	public static void ShowWindow()
	{
		EditorWindow editorWindow = EditorWindow.GetWindow( typeof( BuildPerlinNoiseTextureMaps ) );

		editorWindow.autoRepaintOnSceneChange = true;

		editorWindow.Show();

		editorWindow.titleContent = new GUIContent( "Build Perlin Noise Texture Maps" );
	}

	void OnGUI()
	{
		if ( GUILayout.Button( "Build Perlin Noise Texture Maps", GUILayout.MinHeight( 60 ) ) )
		{
			var perlinNoiseMap = GenerateMap( 1.0f / 8.0f );

			PG_Tools.SaveAsPNG( perlinNoiseMap, Application.dataPath + "/Maps/Perlin Noise/perlin_noise.png" );
		}
	}

	Color[,] GenerateMap( float scale )
	{
		var seamlessNoise = new SeamlessNoise( Random.Range( 0, 50000 ), c_size, 1 );

		var noiseMap = new Color[ c_size, c_size ];

		for ( var y = 0; y < c_size; y++ )
		{
			for ( var x = 0; x < c_size; x++ )
			{
				var alpha = seamlessNoise.Perlin( 0, c_size / 8, x * scale, y * scale ) * 0.5f + 0.5f;
				//var alpha = Mathf.PerlinNoise( x * scale, y * scale );

				noiseMap[ y, x ] = new Color( alpha, alpha, alpha, 1.0f );
			}
		}

		return noiseMap;
	}
}

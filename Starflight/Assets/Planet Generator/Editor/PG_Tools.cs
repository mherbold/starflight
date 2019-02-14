
using UnityEngine;
using System.IO;
using System.Threading;

class PG_Tools
{
	// thread safe floating point add
	public static float ThreadSafeAdd( ref float location, float value )
	{
		float newCurrentValue = location;

		while ( true )
		{
			float currentValue = newCurrentValue;

			float newValue = currentValue + value;

			newCurrentValue = Interlocked.CompareExchange( ref location, newValue, currentValue );

			if ( newCurrentValue == currentValue )
			{
				return newValue;
			}
		}
	}

	// saves a texture map to file as a png image
	public static void SaveAsPNG( Texture2D textureMap, string filename )
	{
		var data = textureMap.EncodeToPNG();

		var directory = Path.GetDirectoryName( filename );

		Directory.CreateDirectory( directory );

		File.WriteAllBytes( filename, data );
	}

	// saves a float array to file as a png image
	public static void SaveAsPNG( float[,] values, string filename )
	{
		var width = values.GetLength( 1 );
		var height = values.GetLength( 0 );

		var textureMap = new Texture2D( width, height, TextureFormat.RGB24, false );

		for ( var y = 0; y < height; y++ )
		{
			for ( var x = 0; x < width; x++ )
			{
				float value = values[ y, x ];

				textureMap.SetPixel( x, y, new Color( value, value, value, 1.0f ) );
			}
		}

		textureMap.Apply();

		SaveAsPNG( textureMap, filename );
	}

	// saves a color array to file as a png image
	public static void SaveAsPNG( Color[,] colors, string filename, bool withAlpha = false )
	{
		var width = colors.GetLength( 1 );
		var height = colors.GetLength( 0 );

		var textureMap = new Texture2D( width, height, withAlpha ? TextureFormat.RGBA32 : TextureFormat.RGB24, false );

		for ( var y = 0; y < height; y++ )
		{
			for ( var x = 0; x < width; x++ )
			{
				textureMap.SetPixel( x, y, new Color( colors[ y, x ].r, colors[ y, x ].g, colors[ y, x ].b, colors[ y, x ].a ) );
			}
		}

		textureMap.Apply();

		SaveAsPNG( textureMap, filename );
	}

	// saves a texture map to file as an exr images
	public static void SaveAsEXR( Texture2D textureMap, string filename )
	{
		var data = textureMap.EncodeToEXR();

		var directory = Path.GetDirectoryName( filename );

		Directory.CreateDirectory( directory );

		File.WriteAllBytes( filename, data );
	}

	// saves a float array to file as an exr image
	public static void SaveAsEXR( float[,] values, string filename )
	{
		var width = values.GetLength( 1 );
		var height = values.GetLength( 0 );

		var textureMap = new Texture2D( width, height, TextureFormat.RGBAFloat, false );

		for ( var y = 0; y < height; y++ )
		{
			for ( var x = 0; x < width; x++ )
			{
				var value = values[ y, x ] * 0.25f;

				textureMap.SetPixel( x, y, new Color( value, value, value, 1.0f ) );
			}
		}

		textureMap.Apply();

		SaveAsEXR( textureMap, filename );
	}

	// saves a color array to file as an exr image
	public static void SaveAsEXR( Color[,] colors, string filename )
	{
		var width = colors.GetLength( 1 );
		var height = colors.GetLength( 0 );

		var textureMap = new Texture2D( width, height, TextureFormat.RGBAFloat, false );

		for ( var y = 0; y < height; y++ )
		{
			for ( var x = 0; x < width; x++ )
			{
				textureMap.SetPixel( x, y, new Color( colors[ y, x ].r, colors[ y, x ].g, colors[ y, x ].b, colors[ y, x ].a ) );
			}
		}

		textureMap.Apply();

		SaveAsEXR( textureMap, filename );
	}
}

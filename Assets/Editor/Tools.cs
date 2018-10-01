
using UnityEngine;
using System.IO;
using System.Threading;

class Tools
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

	// saves a texture map to file as png images (color and alpha)
	public static void SaveAsPNG( Texture2D textureMap, string filename )
	{
		var data = textureMap.EncodeToPNG();

		var filenameWithExtension = filename + "png";

		var directory = Path.GetDirectoryName( filenameWithExtension );

		Directory.CreateDirectory( directory );

		File.WriteAllBytes( filename + ".png", data );
	}

	// saves a float array to file as png images
	public static void SaveAsPNG( float[,] alpha, string filename )
	{
		var width = alpha.GetLength( 1 );
		var height = alpha.GetLength( 0 );

		var textureMap = new Texture2D( width, height, TextureFormat.RGB24, false );

		for ( var y = 0; y < height; y++ )
		{
			for ( var x = 0; x < width; x++ )
			{
				float a = alpha[ y, x ];

				textureMap.SetPixel( x, y, new Color( a, a, a, 1.0f ) );
			}
		}

		textureMap.Apply();

		SaveAsPNG( textureMap, filename );
	}

	// saves a color array to file as png images (color and alpha)
	public static void SaveAsPNG( Color[,] colors, string filename )
	{
		var hasAlpha = false;

		var width = colors.GetLength( 1 );
		var height = colors.GetLength( 0 );

		var textureMap = new Texture2D( width, height, TextureFormat.RGB24, false );

		for ( var y = 0; y < height; y++ )
		{
			for ( var x = 0; x < width; x++ )
			{
				textureMap.SetPixel( x, y, new Color( colors[ y, x ].r, colors[ y, x ].g, colors[ y, x ].b, 1.0f ) );

				if ( colors[ y, x ].a < 1.0f )
				{
					hasAlpha = true;
				}
			}
		}

		textureMap.Apply();

		SaveAsPNG( textureMap, filename );

		if ( hasAlpha )
		{
			for ( var y = 0; y < height; y++ )
			{
				for ( var x = 0; x < width; x++ )
				{
					textureMap.SetPixel( x, y, new Color( colors[ y, x ].a, colors[ y, x ].a, colors[ y, x ].a, 1.0f ) );
				}
			}

			textureMap.Apply();

			SaveAsPNG( textureMap, filename + " Alpha" );
		}
	}
}

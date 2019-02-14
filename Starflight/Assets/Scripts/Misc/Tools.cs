
using UnityEngine;

using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

class Tools
{
	// convert from unity world coordinates to game coordinates
	public static Vector3 WorldToGameCoordinates( Vector3 worldCoordinates )
	{
		return new Vector3( worldCoordinates.x / 256.0f + 128.0f, 0.0f, worldCoordinates.z / 256.0f + 128.0f );
	}

	// convert from game coordinates to unity world coordinates
	public static Vector3 GameToWorldCoordinates( Vector3 gameCoordinates )
	{
		return new Vector3( ( gameCoordinates.x - 128.0f ) * 256.0f, 0.0f, ( gameCoordinates.z - 128.0f ) * 256.0f );
	}

	// convert from unity world coordinates to latlong coordinates
	public static void WorldToLatLongCoordinates( Vector3 worldCoordinates, out float x, out float z )
	{
		// unscale from world coordinates (2048x2048 = 1/8 of map surface)
		x = worldCoordinates.x / ( 2048.0f * 4.0f );
		z = worldCoordinates.z / ( 2048.0f * 2.0f );

		// convert from -0.5,0.5 -180,180 
		x *= 360.0f;

		// convert from -0.375,0.375 to -90,90
		z *= 180.0f / 0.75f;
	}

	// convert from latlong coordinates to unity world coordinates
	public static Vector3 LatLongToWorldCoordinates( float latitude, float longitude )
	{
		// convert from -180,180 to -0.5,0.5
		var x = latitude / 360.0f;

		// convert from -90,90 to -0.375,0.375
		var z = longitude / 180.0f * 0.75f;

		// scale to world coordinates (2048x2048 = 1/8 of map surface)
		x *= 2048.0f * 4.0f;
		z *= 2048.0f * 2.0f;

		// wrap it in a vector
		return new Vector3( x, 0.0f, z );
	}

	// convert from world coordinates to map coordinates
	public static void WorldToMapCoordinates( Vector3 worldCoordinates, out float mapX, out float mapY, int mapWidth, int mapHeight )
	{
		mapX = ( worldCoordinates.x * 0.25f ) + ( mapWidth * 0.5f ) - 0.5f;
		mapY = ( worldCoordinates.z * 0.25f ) + ( mapHeight * 0.5f ) - 0.5f;
	}

	// convert from map to world coordinates
	public static Vector3 MapToWorldCoordinates( float mapX, float mapY, int mapWidth, int mapHeight )
	{
		Vector3 worldCoordinates;

		worldCoordinates.x = ( ( mapX + 0.5f ) - ( mapWidth * 0.5f ) ) * 4.0f;
		worldCoordinates.z = ( ( mapY + 0.5f ) - ( mapHeight * 0.5f ) ) * 4.0f;

		worldCoordinates.y = 0.0f;

		return worldCoordinates;
	}

	// dump an object to the debug log
	public static void Dump( string name, object someObject, int maxDepth = -1 )
	{
		var dumpString = ObjectToString( name, someObject, 1, maxDepth );

		Debug.Log( dumpString );
	}

	// converts an object to a string
	static string ObjectToString( string name, object someObject, int currentDepth, int maxDepth )
	{
		var dumpString = "";

		foreach ( PropertyDescriptor descriptor in TypeDescriptor.GetProperties( someObject ) )
		{
			var combinedName = name + "." + descriptor.Name;

			object value = null;

			try
			{
				value = descriptor.GetValue( someObject );
			}
			catch
			{
			}

			dumpString += combinedName + " = " + value + Environment.NewLine;

			if ( ( maxDepth == -1 ) || ( currentDepth < maxDepth ) )
			{
				if ( value != null )
				{
					if ( value.GetType().IsClass )
					{
						dumpString += ObjectToString( combinedName, value, currentDepth + 1, maxDepth );
					}
				}
			}
		}

		return dumpString;
	}

	// perform a deep clone of a serializable object
	public static T CloneObject<T>( T source )
	{
		// make the sure object is serializable
		if ( !typeof( T ).IsSerializable )
		{
			throw new ArgumentException( "The type must be serializable.", "source" );
		}

		// if the object is null the return the default
		if ( ReferenceEquals( source, null ) )
		{
			return default( T );
		}

		// create the binary formatter
		var binaryFormatter = new BinaryFormatter();

		// add support for serializing / deserializing Unity.Vector3
		var surrogateSelector = new SurrogateSelector();
		var vector3SerializationSurrogate = new Vector3SerializationSurrogate();
		surrogateSelector.AddSurrogate( typeof( Vector3 ), new StreamingContext( StreamingContextStates.All ), vector3SerializationSurrogate );
		binaryFormatter.SurrogateSelector = surrogateSelector;

		// create a new memory stream
		var stream = new MemoryStream();

		// copy the object by serializing and then deserializing it
		using ( stream )
		{
			binaryFormatter.Serialize( stream, source );
			stream.Seek( 0, SeekOrigin.Begin );
			return (T) binaryFormatter.Deserialize( stream );
		}
	}

	// find if one FP number is close to another FP number (within the given threshold)
	public static bool IsApproximatelyEqual( float a, float b, float threshold )
	{
		return Mathf.Abs( a - b ) <= threshold;
	}

	// get the opacity of a material using a SF shader
	public static float GetOpacity( Material material )
	{
		var color = material.GetColor( "SF_AlbedoColor" );

		return Mathf.LinearToGammaSpace( color.a );
	}

	// set the opacity of a material using a SF shader
	public static void SetOpacity( Material material, float opacity )
	{
		var color = material.GetColor( "SF_AlbedoColor" );

		color.a = Mathf.GammaToLinearSpace( opacity );

		material.SetColor( "SF_AlbedoColor", color );
	}

	// destroy all of the children of a game object
	public static void DestroyChildrenOf( GameObject gameObject )
	{
		var children = new List<GameObject>();

		foreach ( Transform child in gameObject.transform )
		{
			children.Add( child.gameObject );
		}

		children.ForEach( child => GameObject.Destroy( child ) );
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
}

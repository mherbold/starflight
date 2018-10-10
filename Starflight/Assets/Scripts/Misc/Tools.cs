
using UnityEngine;
using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
}

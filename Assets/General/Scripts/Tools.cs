
using System;
using System.ComponentModel;
using UnityEngine;

class Tools
{
	// convert from unity world coordinates to game coordinates
	static public Vector3 WorldToGameCoordinates( Vector3 worldCoordinates )
	{
		return new Vector3( worldCoordinates.x / 256.0f + 128.0f, 0.0f, worldCoordinates.z / 256.0f + 128.0f );
	}

	// convert from game coordinates to unity world coordinates
	static public Vector3 GameToWorldCoordinates( Vector3 gameCoordinates )
	{
		return new Vector3( ( gameCoordinates.x - 128.0f ) * 256.0f, 0.0f, ( gameCoordinates.z - 128.0f ) * 256.0f );
	}

	// call this to dump an object to the debug log
	static public void Dump( string name, object someObject, int maxDepth = -1 )
	{
		string dumpString = ObjectToString( name, someObject, 1, maxDepth );

		Debug.Log( dumpString );
	}

	// converts an object to a string
	static private string ObjectToString( string name, object someObject, int currentDepth, int maxDepth )
	{
		string dumpString = "";

		foreach ( PropertyDescriptor descriptor in TypeDescriptor.GetProperties( someObject ) )
		{
			string combinedName = name + "." + descriptor.Name;
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
}


using System;
using System.ComponentModel;
using UnityEngine;

class Tools
{
	static public void Dump( string name, object someObject, int maxDepth = -1 )
	{
		string dumpString = GetDumpString( name, someObject, 1, maxDepth );

		Debug.Log( dumpString );
	}

	static private string GetDumpString( string name, object someObject, int currentDepth, int maxDepth )
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
						dumpString += GetDumpString( combinedName, value, currentDepth + 1, maxDepth );
					}
				}
			}
		}

		return dumpString;
	}
}

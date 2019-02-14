
using UnityEngine;

public class PG_ContourMap
{
	public Color[,] Process( float[,] sourceElevation, float waterHeight )
	{
		var outputColorWidth = sourceElevation.GetLength( 1 );
		var outputColorHeight = sourceElevation.GetLength( 0 );

		var outputColor = new Color[ outputColorHeight, outputColorWidth ];

		var minHeight = float.MaxValue;
		var maxHeight = float.MinValue;

		for ( var y = 0; y < outputColorHeight; y++ )
		{
			for ( var x = 0; x < outputColorWidth; x++ )
			{
				var height = sourceElevation[ y, x ];

				if ( height < minHeight )
				{
					minHeight = height;
				}

				if ( height > maxHeight )
				{
					maxHeight = height;
				}
			}
		}

		for ( var y = 0; y < outputColorHeight; y++ )
		{
			for ( var x = 0; x < outputColorWidth; x++ )
			{
				var height = sourceElevation[ y, x ];

				outputColor[ y, x ] = new Color( height * 16.0f % 1.0f, height, ( height <= waterHeight ) ? 1.0f : 0.0f );
			}
		}

		return outputColor;
	}
}

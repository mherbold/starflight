<?php

ob_end_flush();

ob_implicit_flush();

echo 'Converting planet screenshots...<br>';

$planetMapList = [];

for ( $i = 1; $i < 812; $i++ )
{
	$filename = "images/$i.png";

	$planetMap = [ 'm_key' => [], 'm_map' => [] ];
	
	if ( file_exists( $filename ) )
	{
		echo "Processing image file '$filename'...<br>";
		
		$image = imagecreatefrompng( $filename );
		
		$key = imagecrop( $image, [ 'x' => 296, 'y' => 8, 'width' => 8, 'height' => 48 ] );
		$key = imagescale( $key, 1, 8, IMG_NEAREST_NEIGHBOUR );
		
		for ( $y = 0; $y < 8; $y++ )
		{
			$rgb = imagecolorat( $key, 0, $y );
			
			$r = ( $rgb >> 16 ) & 0xFF;
			$g = ( $rgb >> 8 ) & 0xFF;
			$b = ( $rgb >> 0 ) & 0xFF;
			
			$planetMap[ 'm_key' ][ $y ] = $rgb;
		}
		
		$map = imagecrop( $image, [ 'x' => 192, 'y' => 7, 'width' => 96, 'height' => 48 ] );
		$map = imagescale( $map, 48, 24, IMG_NEAREST_NEIGHBOUR );
		
		for ( $y = 0; $y < 24; $y++ )
		{
			for ( $x = 0; $x < 48; $x++ )
			{
				$rgb = imagecolorat( $map, $x, $y );
				
				$r = ( $rgb >> 16 ) & 0xFF;
				$g = ( $rgb >> 8 ) & 0xFF;
				$b = ( $rgb >> 0 ) & 0xFF;
				
				$planetMap[ 'm_map' ][ $y * 48 + $x ] = $rgb;
			}
		}
	}
	else
	{
		echo "Image file '$filename' not found.<br>";
	}
	
	$planetMapList[] = (object) $planetMap;
	
	$planetData = (object) [ 'm_planetMapList' => $planetMapList ];
}

file_put_contents( 'Starflight Planet Data.json', json_encode( $planetData, JSON_PRETTY_PRINT ) );

echo 'All done!<br>';

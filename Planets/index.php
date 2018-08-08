<?php

class Planet
{
	public $index;
	public $systemX;
	public $systemY;
	public $planetNumber;
	public $type;
	public $captured;
	public $yourName;
	
	function __construct( int $index, int $x, int $y, int $number, string $type, bool $captured )
	{
		$this->index = $index;
		$this->systemX = $x;
		$this->systemY = $y;
		$this->planetNumber = $number;
		$this->type = $type;
		$this->captured = $captured;
		$this->yourName = '';
	}
}

/** @var Planet[] $planetList */
$planetList = file_get_contents( 'Planets.dat' );

if ( $planetList === false )
{
	$planetList = [];
	
	$handle = fopen( 'Planets.csv', 'r' );
	
	fgets( $handle );
	
	while ( true )
	{
		$data = fgetcsv( $handle );
		
		if ( $data === false )
		{
			break;
		}
		
		$captured = file_exists( "images/{$data[0]}.png" );
		
		$planetList[] = new Planet( (int)$data[ 0 ], (int)$data[ 1 ], (int)$data[ 2 ], (int)$data[ 3 ], $data[ 4 ], $captured );
	}
	
	fclose( $handle );
	
	file_put_contents( 'Planets.dat', serialize( $planetList ) );
}
else
{
	$planetList = unserialize( $planetList );
}

$numPlanetsCaptured = 0;

foreach ( $planetList as $planet )
{
	if ( $planet->captured )
	{
		$numPlanetsCaptured++;
	}
}

$viewList = filter_input( INPUT_GET, 'viewList', FILTER_VALIDATE_BOOLEAN ) ?? false;
$submit = filter_input( INPUT_POST, 'submit', FILTER_VALIDATE_BOOLEAN ) ?? false;

$yourName = '';
$systemX = '';
$systemY = '';
$planetNumber = '';

$messages = [];
$messagesType = 'warning';

if ( $submit )
{
	$yourName = filter_input( INPUT_POST, 'yourName', FILTER_SANITIZE_STRING ) ?? null;
	
	if ( empty( $yourName ) )
	{
		$yourName = '(anonymous)';
	}
	
	$systemX = filter_input( INPUT_POST, 'systemX', FILTER_VALIDATE_INT ) ?? -1;
	
	if ( ( $systemX < 2 ) || ( $systemX > 249 ) )
	{
		$messages[] = 'The system X coordinate must be between 2 and 249.';
	}
	
	$systemY = filter_input( INPUT_POST, 'systemY', FILTER_VALIDATE_INT ) ?? -1;
	
	if ( ( $systemY < 0 ) || ( $systemY > 214 ) )
	{
		$messages[] = 'The system Y coordinate must be between 0 and 214.';
	}
	
	$planetNumber = filter_input( INPUT_POST, 'planetNumber', FILTER_VALIDATE_INT ) ?? -1;
	
	if ( ( $planetNumber < 1 ) || ( $planetNumber > 8 ) )
	{
		$messages[] = 'The planet number must be between 1 and 8.';
	}
	
	if ( empty( $messages ) )
	{
		$systemFound = false;
		$numPlanetsInSystem = 0;
		$matchingPlanet = null;
		
		foreach ( $planetList as $planet )
		{
			if ( ( $planet->systemX == $systemX ) && ( $planet->systemY == $systemY ) )
			{
				$systemFound = true;
				
				if ( $planet->planetNumber > $numPlanetsInSystem )
				{
					$numPlanetsInSystem = $planet->planetNumber;
				}
				
				if ( $planet->planetNumber == $planetNumber )
				{
					$matchingPlanet = $planet;
					break;
				}
			}
		}
		
		if ( $matchingPlanet === null )
		{
			if ( $systemFound )
			{
				$messages[] = "There are only {$numPlanetsInSystem} planets in system {$systemX}x{$systemY}.";
			}
			else
			{
				$messages[] = "There is no system at {$systemX}x{$systemY}. Check your system coordinates.";
			}
		}
		else if ( $matchingPlanet->captured )
		{
			$messages[] = 'Thank you, but we already have a screenshot for that planet.';
		}
		
		if ( empty( $messages ) )
		{
			$mimeType = mime_content_type( $_FILES[ 'screenshotFile' ][ 'tmp_name' ] );
			
			if ( $mimeType != 'image/png' )
			{
				$messages[] = 'The uploaded file is not a PNG image.';
			}
			else
			{
				$data = getimagesize( $_FILES[ 'screenshotFile' ][ 'tmp_name' ] );
				
				if ( $data === false )
				{
					$messages[] = 'The uploaded file is not a valid image.';
				}
				else if ( ( $data[ 0 ] != 320 ) || ( $data[ 1 ] != 200 ) )
				{
					$messages[] = 'The uploaded PNG image is not 320x200.';
				}
				else
				{
					if ( !move_uploaded_file( $_FILES[ 'screenshotFile' ][ 'tmp_name' ], "images/{$planet->index}.png" ) )
					{
						$messages[] = 'Something went wrong moving the uploaded PNG image.';
					}
					else
					{
						$messages[] = 'Thank you for uploading a screenshot! It has been added to the database. More, please!';
						$messagesType = 'success';
						
						$matchingPlanet->captured = true;
						$matchingPlanet->yourName = $yourName;
						
						file_put_contents( 'Planets.dat', serialize( $planetList ) );
					}
				}
			}
		}
	}
}

?>
<!doctype html>
<html lang="en">
	<head>
		<meta charset="utf-8">
		<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
		<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/css/bootstrap.min.css" integrity="sha384-Smlep5jCw/wG7hdkwQ/Z5nLIefveQRIY9nfy6xoR1uRYBtpZgI6339F5dgvm/e9B" crossorigin="anonymous">
		<title>Starflight Planets</title>
	</head>
	<body>
		<div class="container">
			<p>
				<img src="Starflight.png" width="100%">
			</p>
			<h3>Welcome to the Starflight Planets project!</h3>
			<p>The goal of this project is to collect all of the texture maps for planets to be used in the
				<a href="http://bravearmy.com/starflight/">Starflight 1 Remake Project</a>.
			</p>
			<?php if ( $viewList ) { ?>
				<p>Click
					<a href="index.php">here</a> to upload a screenshot.
				</p>
				<p>This is a list of all of the planets in Starflight:</p>
				<table class="table">
					<thead>
						<tr>
							<th>Index</th>
							<th>X</th>
							<th>Y</th>
							<th>#</th>
							<th>Type</th>
							<th>Captured</th>
							<th>By Who</th>
							<th>Screenshot</th>
						</tr>
					</thead>
					<tbody>
						<?php foreach ( $planetList as $planet )
						{
							?>
							<tr>
								<td><?php echo $planet->index; ?></td>
								<td><?php echo $planet->systemX; ?></td>
								<td><?php echo $planet->systemY; ?></td>
								<td><?php echo $planet->planetNumber; ?></td>
								<td><?php echo $planet->type; ?></td>
								<td><?php echo $planet->captured ? 'Yes' : 'No'; ?></td>
								<td><?php echo $planet->yourName; ?></td>
								<td><?php echo $planet->captured ? "<img src=\"images/{$planet->index}.png\">" : 'None'; ?></td>
							</tr>
							<?php
						}
						?>
					</tbody>
				</table>
				<p>End of list.</p>
			<?php } else { ?>
			<p>There are a total of <?php echo count( $planetList ); ?> planets in Starflight.</p>
			<p>So far, the community (you!) have uploaded screen shots for <?php echo $numPlanetsCaptured; ?> of them.</p>
			<p>Click
				<a href="index.php?viewList=1">here</a> to see a list of planets we still need screenshots for, as well as to see the screenshots that have already been uploaded.
			</p>
			<p>So, this means we are <?php echo number_format( $numPlanetsCaptured * 100 / count( $planetList ), 1 ); ?>% complete!</p>
			<p>To take a screenshot:</p>
			<ol>
				<li>Play Starflight 1 in
					<b>EGA</b> mode using DosBox
				</li>
				<li>Orbit a planet</li>
				<li>Go to
					<b>Captain</b> &rarr;
					<b>Land</b>
				</li>
				<li>Do not select a site or descend</li>
				<li>Hit Ctrl+F5 in DosBox to take the screenshot</li>
				<li>If you got Starflight from GOG your screenshots are located in
					<i>C:\Program Files (x86)\GalaxyClient\Games\Starflight\capture</i>
				</li>
				<li>Click
					<a href="images/95.png">here</a> for an example
				</li>
			</ol>
			<p>Use this form below to upload your screenshots:</p>
			<div class="card">
				<div class="card-body">
					<?php if ( !empty( $messages ) ) { ?>
						<div class="alert alert-<?php echo $messagesType; ?>">
							<?php echo implode( '<br>', $messages ); ?>
						</div>
					<?php } ?>
					<form enctype="multipart/form-data" method="post">
						<div class="form-group">
							<label for="yourName">Your name</label>
							<input type="text" class="form-control" name="yourName" id="yourName" value="<?php echo $yourName; ?>">
							<small class="form-text text-muted">This is completely optional. You can use an alias.</small>
						</div>
						<div class="row">
							<div class="col">
								<div class="form-group">
									<label for="systemX">System X coordinate</label>
									<input type="number" class="form-control" name="systemX" id="systemX" min="2" max="249" value="<?php echo htmlspecialchars( $systemX ); ?>" required>
									<small class="form-text text-muted">This is the X coordinate of the system.</small>
								</div>
							</div>
							<div class="col">
								<div class="form-group">
									<label for="systemY">System Y coordinate</label>
									<input type="number" class="form-control" name="systemY" id="systemY" min="0" max="214" value="<?php echo htmlspecialchars( $systemY ); ?>" required>
									<small class="form-text text-muted">This is the Y coordinate of the system.</small>
								</div>
							</div>
							<div class="col">
								<div class="form-group">
									<label for="planetNumber">Planet number</label>
									<input type="number" class="form-control" name="planetNumber" id="planetNumber" min="1" max="8" value="<?php echo htmlspecialchars( $planetNumber ); ?>" required>
									<small class="form-text text-muted">This is the planet number (n<sup>th</sup> from the sun).
									</small>
								</div>
							</div>
						</div>
						<div class="form-group">
							<label for="screenshotFile">Screenshot file</label>
							<input type="hidden" name="MAX_FILE_SIZE" value="10000">
							<input type="file" class="form-control" name="screenshotFile" id="screenshotFile" accept="image/png" required>
							<small class="form-text text-muted">This is the screenshot file - it must be 320x200 and in PNG format.</small>
						</div>
						<button type="submit" name="submit" value="1" class="btn btn-primary">Submit</button>
					</form>
				</div>
			</div>
		</div>
		<?php } ?>
		<script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
		<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js" integrity="sha384-ZMP7rVo3mIykV+2+9J3UJ46jBk0WLaUAdn689aCwoqbBJiSnjAK/l8WvCWPIPm49" crossorigin="anonymous"></script>
		<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.2/js/bootstrap.min.js" integrity="sha384-o+RDsa0aLu++PJvFqy8fFScvbHFLtbvScb8AjopnFD+iEQ7wo/CG0xlczd+2O/em" crossorigin="anonymous"></script>
	</body>
</html>

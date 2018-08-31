
using UnityEngine;

public class Player : MonoBehaviour
{
	// the main camera
	public Camera m_camera;

	// the ship
	public Transform m_ship;

	// the infinite starfield
	public InfiniteStarfield m_infiniteStarfield;

	// the space warp effect
	SpaceWarp m_spaceWarp;

	// keep track of the skybox rotation
	Matrix4x4 m_skyboxRotation;

	// set to true to prevent the player from moving (temporarily)
	bool m_freezePlayer;

	// convenient access to the spaceflight controller
	SpaceflightController m_spaceflightController;

	// unity awake
	private void Awake()
	{
		// get the spaceflight controller
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );
		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();

		// get the space warp effect
		m_spaceWarp = m_camera.GetComponent<SpaceWarp>();
	}

	// unity start
	void Start()
	{
		// reset the skybox rotation
		m_skyboxRotation = Matrix4x4.identity;

		// make sure the player can move
		m_freezePlayer = false;
	}

	// unity update
	void Update()
	{
		// update the skybox rotation on the material
		Material skyboxMaterial = RenderSettings.skybox;
		skyboxMaterial.SetMatrix( "_Rotation", m_spaceflightController.m_player.m_skyboxRotation );
	}

	// call this to hide the player (ship)
	public void Hide()
	{
		m_ship.gameObject.SetActive( false );
	}

	// call this to show the player (ship)
	public void Show()
	{
		m_ship.gameObject.SetActive( true );
	}

	// call this to change the height of the camera above the zero plane
	public void DollyCamera( float distance )
	{
		// calculate the camera vector
		Vector3 cameraPosition = new Vector3( 0.0f, distance, 0.0f );

		// set it on the camera
		m_camera.transform.localPosition = cameraPosition;
	}

	// call this to get the current position of the player
	public Vector3 GetPosition()
	{
		return transform.position;
	}

	// call this to instantly change the position of the player
	public void SetPosition( Vector3 newPosition )
	{
		transform.position = newPosition;
	}

	// call this to instantly change the rotation of the player
	public void SetRotation( Quaternion newRotation )
	{
		m_ship.rotation = newRotation;
	}

	// call this to change when the infinite starfield becomes fully visible
	public void SetStarfieldFullyVisibleSpeed( float newSpeed )
	{
		m_infiniteStarfield.m_fullyVisibleSpeed = newSpeed;
	}

	// call this to find out if the player is currently frozen or not
	public bool IsFrozen()
	{
		return m_freezePlayer;
	}

	// call this to temporarily stop the player from moving (e.g. while travelling in flux)
	public void Freeze()
	{
		m_freezePlayer = true;
	}

	// call this to allow the player to move again
	public void Unfreeze()
	{
		m_freezePlayer = false;
	}

	// start the space warp effect
	public void StartSpaceWarp()
	{
		m_spaceWarp.EnterWarp();
	}

	// stop the space warp effect
	public void StopSpaceWarp()
	{
		m_spaceWarp.ExitWarp();
	}

	// call this to rotate the skybox by a certain amount in the given direction
	public void RotateSkybox( Vector3 direction, float amount )
	{
		// compute the rotation quaternion
		Quaternion rotation = Quaternion.LookRotation( direction, Vector3.up );

		// we want to rotate the skybox by the right vector
		Vector3 currentRightVector = rotation * Vector3.right;

		// compute the skybox rotation delta
		rotation = Quaternion.AngleAxis( amount, currentRightVector );

		// compute the new skybox rotation
		m_skyboxRotation = Matrix4x4.Rotate( rotation ) * m_skyboxRotation;

		// update the skybox rotation parameter on the material
		Material skyboxMaterial = RenderSettings.skybox;
		skyboxMaterial.SetMatrix( "_Rotation", m_skyboxRotation );
	}
}

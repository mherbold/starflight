
using UnityEngine;

public class Player : MonoBehaviour
{
	// the main camera
	public Camera m_camera;

	// the ship
	public Transform m_ship;

	// the infinite starfield
	public InfiniteStarfield m_infiniteStarfield;

	// convenient access to the spaceflight controller
	SpaceflightController m_spaceflightController;

	// unity awake
	private void Awake()
	{
		// get the spaceflight controller
		GameObject controllersGameObject = GameObject.FindWithTag( "Spaceflight Controllers" );
		m_spaceflightController = controllersGameObject.GetComponent<SpaceflightController>();
	}

	// unity start
	void Start()
	{

	}

	// unity update
	void Update()
	{

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
}

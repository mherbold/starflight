
using UnityEngine;

public class Starmap : MonoBehaviour
{
	public float m_scrollVelocity;

	public GameObject m_starmap;
	public GameObject m_playerShip;

	public Vector3 m_originalPosition;

	Vector3 m_positionOffset;

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
		// if the player hits cancel (esc) or submit (enter) close the starmap
		if ( InputController.m_instance.m_cancel || InputController.m_instance.m_submit )
		{
			Hide();

			InputController.m_instance.Debounce();

			SpaceflightController.m_instance.m_buttonController.DeactivateButton();
		}
		else
		{
			// get the controller stick position
			float controllerX = InputController.m_instance.m_x;
			float controllerY = InputController.m_instance.m_y;

			// create our 3d move vector from the controller position
			Vector3 scrollVector = new Vector3( controllerX, controllerY, 0.0f );

			// check if the move vector will actually move the starmap (that the controller is not centered)
			if ( scrollVector.magnitude > 0.5f )
			{
				// normalize the move vector to a length of 1.0
				scrollVector.Normalize();

				// apply the desired scroll velocity
				scrollVector *= m_scrollVelocity * Time.deltaTime;

				// update the position offset
				m_positionOffset += scrollVector;

				// make sure we don't go out of bounds
				if ( m_positionOffset.x < 0.0f )
				{
					m_positionOffset.x = 0.0f;
				}
				else if ( m_positionOffset.x > 250.0f * m_starmap.transform.localScale.x )
				{
					m_positionOffset.x = 250.0f * m_starmap.transform.localScale.x;
				}

				if ( m_positionOffset.y < 0.0f )
				{
					m_positionOffset.y = 0.0f;
				}
				else if ( m_positionOffset.y > 220.0f * m_starmap.transform.localScale.y )
				{
					m_positionOffset.y = 220.0f * m_starmap.transform.localScale.y;
				}

				// move the starmap
				m_starmap.transform.localPosition = m_originalPosition - m_positionOffset;

				// update the viewport label
				var x = Mathf.RoundToInt( m_positionOffset.x / m_starmap.transform.localScale.x );
				var y = Mathf.RoundToInt( m_positionOffset.y / m_starmap.transform.localScale.y );

				var text = x.ToString() + "   " + y.ToString();

				SpaceflightController.m_instance.m_viewport.UpdateLabel( text );
			}
		}
	}

	// hide the starmap
	public void Hide()
	{
		// unpause the game
		SpaceflightController.m_instance.m_gameIsPaused = false;

		// make this game object not active
		gameObject.SetActive( false );
	}

	// show the starmap
	public void Show()
	{
		// pause the game
		SpaceflightController.m_instance.m_gameIsPaused = true;

		// make this game object active
		gameObject.SetActive( true );

		// play a sound
		SoundController.m_instance.PlaySound( SoundController.Sound.Beep );

		// get to the player data
		PlayerData playerData = DataController.m_instance.m_playerData;

		// get the current hyperspace coordinates of the player
		var worldHyperspaceCoordinates = playerData.m_general.m_lastHyperspaceCoordinates;

		// convert to game coordinates
		var gameHyperspaceCoordinates = Tools.WorldToGameCoordinates( worldHyperspaceCoordinates );

		// scale the coordinates (and move z to y)
		m_positionOffset = new Vector3( gameHyperspaceCoordinates.x * m_starmap.transform.localScale.x, gameHyperspaceCoordinates.z * m_starmap.transform.localScale.y, 0.0f );

		// update the starmap position
		m_starmap.transform.localPosition = m_originalPosition - m_positionOffset;

		// update the player ship position
		m_playerShip.transform.localPosition = new Vector3( -gameHyperspaceCoordinates.x, gameHyperspaceCoordinates.z, 15.0f );
	}

	// returns true if the starmap is open
	public bool IsOpen()
	{
		return gameObject.activeInHierarchy;
	}
}

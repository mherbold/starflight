
using UnityEngine;

public class Scanner : MonoBehaviour
{
	float m_rotation;

	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
		m_rotation += Time.deltaTime * 100.0f;

		if ( m_rotation > 360.0f )
		{
			m_rotation -= 360.0f;
		}

		transform.localRotation = Quaternion.Euler( -90.0f, m_rotation, 0.0f );
	}

	// hide the scanner
	public void Hide()
	{
		// make this game object not active
		gameObject.SetActive( false );
	}

	// show the scanner
	public void Show()
	{
		// make this game object active
		gameObject.SetActive( true );
	}

	// update the position of the scanner ring
	public void UpdatePosition( Vector3 newPosition )
	{
		newPosition.y = -16;

		transform.position = newPosition;
	}
}

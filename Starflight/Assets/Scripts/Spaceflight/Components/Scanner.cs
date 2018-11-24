
using UnityEngine;

public class Scanner : MonoBehaviour
{
	// unity start
	void Start()
	{
	}

	// unity update
	void Update()
	{
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
		gameObject.transform.position = newPosition;
	}
}

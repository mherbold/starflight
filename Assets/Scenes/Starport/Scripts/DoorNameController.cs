
using UnityEngine;
using TMPro;

public class DoorNameController : MonoBehaviour
{
	// the door name text
	public TextMeshProUGUI m_doorNameText;

	// this is called by unity once at the start of the level
	void Start()
	{
		// hide the door name when we load this scene
		Hide();
	}

	// change the door name and show it
	public void SetText( string newDoorName )
	{
		m_doorNameText.text = newDoorName;

		Show();
	}

	// show the door name
	public void Show()
	{
		gameObject.SetActive( true );
	}

	// hide the door name
	public void Hide()
	{
		gameObject.SetActive( false );
	}

	// activeSelf is true if we are showing the door name
	public bool IsShowingDoorName()
	{
		return gameObject.activeSelf;
	}

	// return the current door name
	public string GetCurrentDoorName()
	{
		return m_doorNameText.text;
	}
}

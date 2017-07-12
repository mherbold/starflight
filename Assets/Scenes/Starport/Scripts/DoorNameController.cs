
using UnityEngine;
using TMPro;

public class DoorNameController : MonoBehaviour
{
	// public stuff we want to set using the editor
	public GameObject m_doorNameGameObject;

	// private stuff we don't want the editor to see
	private TextMeshProUGUI m_doorNameText;

	// this is called by unity before start
	private void Awake()
	{
		// get to the text mesh pro component
		m_doorNameText = m_doorNameGameObject.transform.Find( "Door Name Text" ).gameObject.GetComponent<TextMeshProUGUI>();
	}

	// this is called by unity once at the start of the level
	private void Start()
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
		m_doorNameGameObject.SetActive( true );
	}

	// hide the door name
	public void Hide()
	{
		m_doorNameGameObject.SetActive( false );
	}

	// activeSelf is true if we are showing the door name
	public bool IsShowingDoorName()
	{
		return m_doorNameGameObject.activeSelf;
	}

	// return the current door name
	public string GetCurrentDoorName()
	{
		return m_doorNameText.text;
	}
}

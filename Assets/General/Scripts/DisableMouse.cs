
using UnityEngine;
using UnityEngine.EventSystems;

class DisableMouse : MonoBehaviour
{
	// private stuff we don't want the editor to see
	private GameObject m_lastSelectedGameObject;

	// this is called by unity before start
	void Awake()
	{
		m_lastSelectedGameObject = new GameObject();
	}

	// this is called by unity every frame
	void Update()
	{
		// check if we have an active ui
		if ( false && ( EventSystem.current != null ) )
		{
			// check if we have a currently selected game object
			if ( EventSystem.current.currentSelectedGameObject == null )
			{
				// nope - the mouse may have stolen it - give it back to the last selected game object
				EventSystem.current.SetSelectedGameObject( m_lastSelectedGameObject );
			}
			else if ( EventSystem.current.currentSelectedGameObject != m_lastSelectedGameObject )
			{
				// we changed our selection - remember it
				m_lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			}
		}
	}
}

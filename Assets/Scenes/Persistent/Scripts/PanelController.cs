
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelController : MonoBehaviour
{
	// static reference to this instance
	public static PanelController m_instance;

	// the global save game panel
	public SaveGamePanel m_saveGamePanel;

	// the currently active panel (null if none)
	Panel m_activePanel;

	// unity awake
	void Awake()
	{
		// remember this instance to this
		m_instance = this;
	}

	// unity start
	void Start()
	{
		m_saveGamePanel.gameObject.SetActive( false );
	}

	// unity update
	void Update()
	{
		if ( m_activePanel != null )
		{
			m_activePanel.Tick();
		}
	}

	// call this to find out if a panel is active (opened)
	public bool HasActivePanel()
	{
		return ( m_activePanel != null );
	}

	// call this to open the panel
	public void Open( Panel panel )
	{
		// make sure we don't already have an active panel
		if ( m_activePanel != null )
		{
			Debug.Log( "Whoops - we already have an active panel!" );
		}
		else
		{
			// try to open the panel
			if ( panel.Open() )
			{
				// the panel has opened - so remember this panel
				m_activePanel = panel;
			}
		}
	}

	// call this to close the panel
	public void Close()
	{
		// make sure we have an active panel
		if ( m_activePanel == null )
		{
			Debug.Log( "Whoops - trying to close the active panel when we don't have one!" );
		}
		else
		{
			// turn off controller navigation of the UI
			EventSystem.current.sendNavigationEvents = false;

			// close the panel
			m_activePanel.Close();
		}
	}

	// this is called by the ui animation callback
	public void Opened()
	{
		// let the currently active panel know we have finished opening it
		m_activePanel.Opened();

		// turn on controller navigation of the UI
		EventSystem.current.sendNavigationEvents = true;
	}

	// this is called by the ui animation callback
	public void Closed()
	{
		// let the currently active panel know we have finished closing it
		m_activePanel.Closed();

		// forget this panel
		m_activePanel = null;
	}
}


using System;

[Serializable]

public class ElementReference
{
	public int m_elementId;
	public int m_volume;

	// constructor
	public ElementReference( int elementId, int volume )
	{
		m_elementId = elementId;
		m_volume = volume;
	}

	// get access to the element game data for this element
	public ElementGameData GetElementGameData()
	{
		// get access to the game data
		GameData gameData = PersistentController.m_instance.m_gameData;

		// return the element game data
		return gameData.m_elementList[ m_elementId ];
	}

	// get the number of cubic meters stored
	public int GetVolume()
	{
		return m_volume;
	}

	// add more volume to this element
	public void AddVolume( int volume )
	{
		m_volume += volume;
	}
}

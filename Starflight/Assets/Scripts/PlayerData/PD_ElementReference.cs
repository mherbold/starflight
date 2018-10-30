
using System;

[Serializable]

public class PD_ElementReference
{
	public int m_elementId;
	public int m_volume;

	// constructor
	public PD_ElementReference( int elementId, int volume )
	{
		m_elementId = elementId;
		m_volume = volume;
	}

	// get access to the element game data for this element
	public GD_Element GetElementGameData()
	{
		// get access to the game data
		var gameData = DataController.m_instance.m_gameData;

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

	// remove some volume from this element
	public void RemoveVolume( int volume )
	{
		m_volume -= volume;
	}
}

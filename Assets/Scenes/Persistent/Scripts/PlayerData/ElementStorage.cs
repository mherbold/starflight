
using System;
using System.Collections.Generic;

[Serializable]

public class ElementStorage
{
	public int m_volumeUsed;
	public List<ElementReference> m_elementList;

	public void Reset()
	{
		// reset the volume used
		m_volumeUsed = 0;

		// create a new element list
		m_elementList = new List<ElementReference>();
	}

	public ElementReference Find( int elementId )
	{
		// see if we can find this element in storage
		foreach ( ElementReference elementReference in m_elementList )
		{
			if ( elementReference.m_elementId == elementId )
			{
				// we found it so return it
				return elementReference;
			}
		}

		// we could not find it in storage so return null
		return null;
	}

	// search for an element in storage by name
	public ElementReference Find( string name )
	{
		GameData gameData = DataController.m_instance.m_gameData;

		int elementId = gameData.FindElementId( name );

		return Find( elementId );
	}

	// add the specified amount of the element to storage
	public void Add( int elementId, int volume )
	{
		ElementReference elementReference = Find( elementId );

		if ( elementReference == null )
		{
			// we didn't find it in storage so create a new element reference
			elementReference = new ElementReference( elementId, volume );

			// add the element to the storage
			m_elementList.Add( elementReference );
		}
		else
		{
			// we found it - update the volume of the element already in storage
			elementReference.AddVolume( volume );
		}

		// update the volume used
		m_volumeUsed += volume;
	}

	// remove the specified amount of the element from storage
	public void Remove( int elementId, int volume )
	{
		// find the element in storage
		ElementReference elementReference = Find( elementId );

		// if we didn't find it in storage then do nothing
		if ( elementReference != null )
		{
			// check if we want to remove it all or just some of it
			if ( elementReference.m_volume == volume )
			{
				// we want to remove the entire amount so kick the whole thing out of the list
				m_elementList.Remove( elementReference );
			}
			else
			{
				// remove only some of the element from the storage
				elementReference.RemoveVolume( volume );
			}

			// update the volume used
			m_volumeUsed -= volume;
		}
	}
}

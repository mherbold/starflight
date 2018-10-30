
using System;
using System.Collections.Generic;

[Serializable]

public class PD_ElementStorage
{
	public int m_volumeUsed;
	public List<PD_ElementReference> m_elementList;

	public void Reset()
	{
		// reset the volume used
		m_volumeUsed = 0;

		// create a new element list
		m_elementList = new List<PD_ElementReference>();
	}

	public PD_ElementReference Find( int elementId )
	{
		// see if we can find this element in storage
		foreach ( var elementReference in m_elementList )
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
	public PD_ElementReference Find( string name )
	{
		var gameData = DataController.m_instance.m_gameData;

		var elementId = gameData.FindElementId( name );

		return Find( elementId );
	}

	// add the specified amount of the element to storage
	public void Add( int elementId, int volume )
	{
		var elementReference = Find( elementId );

		if ( elementReference == null )
		{
			// we didn't find it in storage so create a new element reference
			elementReference = new PD_ElementReference( elementId, volume );

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
		var elementReference = Find( elementId );

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

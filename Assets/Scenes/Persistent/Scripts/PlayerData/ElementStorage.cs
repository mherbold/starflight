
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

	public ElementReference Find( string name )
	{
		GameData gameData = PersistentController.m_instance.m_gameData;

		int elementId = gameData.FindElementId( name );

		return Find( elementId );
	}

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
}

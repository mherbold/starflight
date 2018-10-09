
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using System;
using System.Collections.Generic;
using System.Reflection;

public class PG_ContractResolver : DefaultContractResolver
{
	private readonly Dictionary<Type, HashSet<string>> m_includes;
	private readonly Dictionary<Type, HashSet<string>> m_ignores;
	private readonly Dictionary<Type, Dictionary<string, string>> m_renames;

	public PG_ContractResolver()
	{
		m_includes = new Dictionary<Type, HashSet<string>>();
		m_ignores = new Dictionary<Type, HashSet<string>>();
		m_renames = new Dictionary<Type, Dictionary<string, string>>();
	}

	public void IncludeProperty( Type type, params string[] jsonPropertyNames )
	{
		if ( !m_ignores.ContainsKey( type ) )
		{
			m_includes[ type ] = new HashSet<string>();
		}

		foreach ( var prop in jsonPropertyNames )
		{
			m_includes[ type ].Add( prop );
		}
	}

	public void IgnoreProperty( Type type, params string[] jsonPropertyNames )
	{
		if ( !m_ignores.ContainsKey( type ) )
		{
			m_ignores[ type ] = new HashSet<string>();
		}

		foreach ( var prop in jsonPropertyNames )
		{
			m_ignores[ type ].Add( prop );
		}
	}

	public void RenameProperty( Type type, string propertyName, string newJsonPropertyName )
	{
		if ( !m_renames.ContainsKey( type ) )
		{
			m_renames[ type ] = new Dictionary<string, string>();
		}

		m_renames[ type ][ propertyName ] = newJsonPropertyName;
	}

	protected override JsonProperty CreateProperty( MemberInfo member, MemberSerialization memberSerialization )
	{
		var property = base.CreateProperty( member, memberSerialization );

		if ( IsIgnored( property.DeclaringType, property.PropertyName ) )
		{
			property.ShouldSerialize = i => false;
			property.Ignored = true;
		}

		string newJsonPropertyName;

		if ( IsRenamed( property.DeclaringType, property.PropertyName, out newJsonPropertyName ) )
		{
			property.PropertyName = newJsonPropertyName;
		}

		return property;
	}

	private bool IsIgnored( Type type, string jsonPropertyName )
	{
		if ( m_includes.ContainsKey( type ) )
		{
			if ( m_includes[ type ].Contains( jsonPropertyName ) )
			{
				return false;
			}

			return true;
		}

		if ( !m_ignores.ContainsKey( type ) )
		{
			return false;
		}

		return m_ignores[ type ].Contains( jsonPropertyName );
	}

	private bool IsRenamed( Type type, string jsonPropertyName, out string newJsonPropertyName )
	{
		Dictionary<string, string> renames;

		if ( !m_renames.TryGetValue( type, out renames ) || !renames.TryGetValue( jsonPropertyName, out newJsonPropertyName ) )
		{
			newJsonPropertyName = null;

			return false;
		}

		return true;
	}
}

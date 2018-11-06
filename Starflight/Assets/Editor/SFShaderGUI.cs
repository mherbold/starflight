
using UnityEngine;
using UnityEditor;

class SFShaderGUI : ShaderGUI
{
	private static class Styles
	{
		public static readonly GUIContent albedoText = EditorGUIUtility.TrTextContent( "Albedo", "UV1; RGB=Color, Alpha (Transparency)" );
		public static readonly GUIContent specularText = EditorGUIUtility.TrTextContent( "Specular", "UV1; R=Intensity, G=Smoothness" );
		public static readonly GUIContent occlusionText = EditorGUIUtility.TrTextContent( "Occlusion", "UV2; R=Intensity; Albedo Occlusion Switch" );
		public static readonly GUIContent normalText = EditorGUIUtility.TrTextContent( "Normal", "UV1; RGB=Uncompressed Normal, GA=DXT5 Compressed Normal; XY=Scale, ZW=Offset; Orthonormalize Switch" );
		public static readonly GUIContent emissiveText = EditorGUIUtility.TrTextContent( "Emissive", "UV1; RGB=Color" );
		public static readonly GUIContent waterText = EditorGUIUtility.TrTextContent( "Water", "UV1; RGB=Uncompressed Normal, GA=DXT5 Compressed Normal; XY=Scale, Z=Bumpiness, W=Speed" );
		public static readonly GUIContent waterMaskText = EditorGUIUtility.TrTextContent( "Water Mask", "UV1; R=Opacity" );
		public static readonly GUIContent scatterMapText = EditorGUIUtility.TrTextContent( "Scatter Map", "UV1; RGB=Height" );
		public static readonly GUIContent densityMapText = EditorGUIUtility.TrTextContent( "Density Map", "UV1; RGB=Height" );
		public static readonly GUIContent densityText = EditorGUIUtility.TrTextContent( "Density", "" );
		public static readonly GUIContent speedText = EditorGUIUtility.TrTextContent( "Speed", "" );
		public static readonly GUIContent blendSrcText = EditorGUIUtility.TrTextContent( "Source Blend", "" );
		public static readonly GUIContent blendDstText = EditorGUIUtility.TrTextContent( "Destination Blend", "" );
		public static readonly GUIContent zWriteText = EditorGUIUtility.TrTextContent( "Z Write", "" );
		public static readonly GUIContent forwardShadowsText = EditorGUIUtility.TrTextContent( "Forward Shadows", "" );
		public static readonly GUIContent renderQueueOffsetText = EditorGUIUtility.TrTextContent( "Render Queue Offset", "" );

		public static readonly string mainText = "Starflight Shader Properties";
	}

	MaterialProperty m_albedoMap = null;
	MaterialProperty m_albedoColor = null;

	MaterialProperty m_specularMap = null;
	MaterialProperty m_specularColor = null;
	MaterialProperty m_smoothness = null;

	MaterialProperty m_occlusionMap = null;
	MaterialProperty m_occlusionPower = null;
	MaterialProperty m_albedoOcclusionOn = null;

	MaterialProperty m_normalMap = null;
	MaterialProperty m_normalMapScaleOffset = null;
	MaterialProperty m_orthonormalizeOn = null;

	MaterialProperty m_emissiveMap = null;
	MaterialProperty m_emissiveColor = null;

	MaterialProperty m_waterMap = null;
	MaterialProperty m_waterScale = null;

	MaterialProperty m_waterMaskMap = null;

	MaterialProperty m_scatterMapA = null;
	MaterialProperty m_scatterMapB = null;
	MaterialProperty m_densityMap = null;

	MaterialProperty m_density = null;
	MaterialProperty m_speed = null;

	MaterialProperty m_blendSrc = null;
	MaterialProperty m_blendDst = null;

	MaterialProperty m_zWriteOn = null;

	MaterialProperty m_forwardShadowsOn = null;

	MaterialProperty m_renderQueueOffset = null;

	MaterialEditor m_materialEditor;

	bool m_firstTimeApply = true;

	string[] m_debugLogMessage;

	public SFShaderGUI()
	{
		m_debugLogMessage = new string[ 2 ];
	}

	public override void OnGUI( MaterialEditor materialEditor, MaterialProperty[] materialPropertyList )
	{
		FindProperties( materialPropertyList );

		m_materialEditor = materialEditor;

		Material material = materialEditor.target as Material;

		if ( m_firstTimeApply )
		{
			m_firstTimeApply = false;

			MaterialChanged( material );
		}

		ShaderPropertiesGUI( material );
	}

	public void ShaderPropertiesGUI( Material material )
	{
		// use default label width
		EditorGUIUtility.labelWidth = 0.0f;

		// fetect any changes to the material
		EditorGUI.BeginChangeCheck();

		// primary properties
		GUILayout.Label( Styles.mainText, EditorStyles.boldLabel );

		if ( m_albedoMap != null && m_albedoColor != null )
		{
			m_materialEditor.TexturePropertySingleLine( Styles.albedoText, m_albedoMap, m_albedoColor );
		}
		else if ( m_albedoColor != null )
		{
			m_materialEditor.ShaderProperty( m_albedoColor, Styles.albedoText );
		}

		if ( m_specularMap != null && m_specularColor != null && m_smoothness != null )
		{
			m_materialEditor.TexturePropertySingleLine( Styles.specularText, m_specularMap, m_specularColor, m_smoothness );
		}

		if ( m_occlusionMap != null && m_occlusionPower != null && m_albedoOcclusionOn != null )
		{
			m_materialEditor.TexturePropertySingleLine( Styles.occlusionText, m_occlusionMap, m_occlusionPower, m_albedoOcclusionOn );
		}

		if ( m_normalMap != null && m_normalMapScaleOffset != null && m_orthonormalizeOn != null )
		{
			m_materialEditor.TexturePropertySingleLine( Styles.normalText, m_normalMap, m_normalMapScaleOffset, m_orthonormalizeOn );
		}

		if ( m_emissiveMap != null && m_emissiveColor != null )
		{
			m_materialEditor.TexturePropertySingleLine( Styles.emissiveText, m_emissiveMap, m_emissiveColor );
		}

		if ( m_waterMap != null && m_waterScale != null )
		{
			m_materialEditor.TexturePropertySingleLine( Styles.waterText, m_waterMap, m_waterScale );
		}

		if ( m_waterMaskMap != null )
		{
			if ( HasTextureMap( material, "SF_WaterMap" ) )
			{
				m_materialEditor.TexturePropertySingleLine( Styles.waterMaskText, m_waterMaskMap );
			}
		}

		if ( m_scatterMapA != null )
		{
			m_materialEditor.TexturePropertySingleLine( Styles.scatterMapText, m_scatterMapA );
		}

		if ( m_scatterMapB != null )
		{
			m_materialEditor.TexturePropertySingleLine( Styles.scatterMapText, m_scatterMapB );
		}

		if ( m_densityMap != null )
		{
			m_materialEditor.TexturePropertySingleLine( Styles.densityMapText, m_densityMap );
		}

		if ( m_density != null )
		{
			m_materialEditor.ShaderProperty( m_density, Styles.densityText );
		}

		if ( m_speed != null )
		{
			m_materialEditor.ShaderProperty( m_speed, Styles.speedText );
		}

		if ( m_blendSrc != null && m_blendDst != null )
		{
			m_materialEditor.ShaderProperty( m_blendSrc, Styles.blendSrcText );
			m_materialEditor.ShaderProperty( m_blendDst, Styles.blendDstText );
		}

		if ( m_zWriteOn != null )
		{
			m_materialEditor.ShaderProperty( m_zWriteOn, Styles.zWriteText );
		}

		if ( m_forwardShadowsOn != null )
		{
			m_materialEditor.ShaderProperty( m_forwardShadowsOn, Styles.forwardShadowsText );
		}

		m_materialEditor.ShaderProperty( m_renderQueueOffset, Styles.renderQueueOffsetText );

		if ( EditorGUI.EndChangeCheck() )
		{
			MaterialChanged( material );
		}
	}

	void FindProperties( MaterialProperty[] materialPropertyList )
	{
		m_albedoMap = FindProperty( "SF_AlbedoMap", materialPropertyList, false );
		m_albedoColor = FindProperty( "SF_AlbedoColor", materialPropertyList, false );

		m_specularMap = FindProperty( "SF_SpecularMap", materialPropertyList, false );
		m_specularColor = FindProperty( "SF_SpecularColor", materialPropertyList, false );
		m_smoothness = FindProperty( "SF_Smoothness", materialPropertyList, false );

		m_occlusionMap = FindProperty( "SF_OcclusionMap", materialPropertyList, false );
		m_occlusionPower = FindProperty( "SF_OcclusionPower", materialPropertyList, false );
		m_albedoOcclusionOn = FindProperty( "SF_AlbedoOcclusionOn", materialPropertyList, false );

		m_normalMap = FindProperty( "SF_NormalMap", materialPropertyList, false );
		m_normalMapScaleOffset = FindProperty( "SF_NormalMapScaleOffset", materialPropertyList, false );
		m_orthonormalizeOn = FindProperty( "SF_OrthonormalizeOn", materialPropertyList, false );

		m_emissiveMap = FindProperty( "SF_EmissiveMap", materialPropertyList, false );
		m_emissiveColor = FindProperty( "SF_EmissiveColor", materialPropertyList, false );

		m_waterMap = FindProperty( "SF_WaterMap", materialPropertyList, false );
		m_waterScale = FindProperty( "SF_WaterScale", materialPropertyList, false );

		m_waterMaskMap = FindProperty( "SF_WaterMaskMap", materialPropertyList, false );

		m_scatterMapA = FindProperty( "SF_ScatterMapA", materialPropertyList, false );
		m_scatterMapB = FindProperty( "SF_ScatterMapB", materialPropertyList, false );
		m_densityMap = FindProperty( "SF_DensityMap", materialPropertyList, false );

		m_density = FindProperty( "SF_Density", materialPropertyList, false );
		m_speed = FindProperty( "SF_Speed", materialPropertyList, false );

		m_blendSrc = FindProperty( "SF_BlendSrc", materialPropertyList, false );
		m_blendDst = FindProperty( "SF_BlendDst", materialPropertyList, false );

		m_zWriteOn = FindProperty( "SF_ZWriteOn", materialPropertyList, false );

		m_forwardShadowsOn = FindProperty( "SF_ForwardShadowsOn", materialPropertyList, false );

		m_renderQueueOffset = FindProperty( "SF_RenderQueueOffset", materialPropertyList );
	}

	void MaterialChanged( Material material )
	{
		// texture maps on/off
		bool albedoMapOn = HasTextureMap( material, "SF_AlbedoMap" );
		bool specularMapOn = HasTextureMap( material, "SF_SpecularMap" );
		bool occlusionMapOn = HasTextureMap( material, "SF_OcclusionMap" );
		bool normalMapOn = HasTextureMap( material, "SF_NormalMap" );
		bool emissiveMapOn = HasTextureMap( material, "SF_EmissiveMap" );
		bool waterMapOn = HasTextureMap( material, "SF_WaterMap" );
		bool waterMaskMapOn = HasTextureMap( material, "SF_WaterMaskMap" );

		// texture compression
		bool normalMapIsCompressed = TextureIsCompressed( material, "SF_NormalMap" );
		bool waterMapIsCompressed = TextureIsCompressed( material, "SF_WaterMap" );

		// toggle switches on/off
		bool albedoOcclusionOn = IsSwitchedOn( material, "SF_AlbedoOcclusionOn" );
		bool orthonormalizeOn = IsSwitchedOn( material, "SF_OrthonormalizeOn" );
		bool forwardShadowsOn = IsSwitchedOn( material, "SF_ForwardShadowsOn" );

		// enable blending if blendsrc != one or blenddst != zero
		bool blendOn = false;

		if ( material.HasProperty( "SF_BlendSrc" ) && material.HasProperty( "SF_BlendDst" ) )
		{
			blendOn = ( ( (int) material.GetFloat( "SF_BlendSrc" ) != (int) UnityEngine.Rendering.BlendMode.One ) || ( (int) material.GetFloat( "SF_BlendDst" ) != (int) UnityEngine.Rendering.BlendMode.Zero ) );
		}

		// enable specular if specular color is not black
		bool specularOn = false;

		if ( material.HasProperty( "SF_SpecularColor" ) )
		{
			specularOn = ( material.GetColor( "SF_SpecularColor" ) != Color.black );
		}

		// if specular is not on then there is no point in having a specular map
		if ( !specularOn )
		{
			specularMapOn = false;
		}

		// if water is not on then there is no point in having a water mask map
		if ( !waterMapOn )
		{
			waterMaskMapOn = false;
		}

		// for debug log
		m_debugLogMessage[ 1 ] = "Enabled keywords:";

		// set all the material keywords
		SetKeyword( material, "SF_ALBEDOMAP_ON", albedoMapOn );
		SetKeyword( material, "SF_ALPHA_ON", blendOn );
		SetKeyword( material, "SF_SPECULARMAP_ON", specularMapOn );
		SetKeyword( material, "SF_SPECULAR_ON", specularOn );
		SetKeyword( material, "SF_OCCLUSIONMAP_ON", occlusionMapOn );
		SetKeyword( material, "SF_ALBEDOOCCLUSION_ON", albedoOcclusionOn );
		SetKeyword( material, "SF_NORMALMAP_ON", normalMapOn );
		SetKeyword( material, "SF_NORMALMAP_ISCOMPRESSED", normalMapIsCompressed );
		SetKeyword( material, "SF_ORTHONORMALIZE_ON", orthonormalizeOn );
		SetKeyword( material, "SF_EMISSIVEMAP_ON", emissiveMapOn );
		SetKeyword( material, "SF_WATERMAP_ON", waterMapOn );
		SetKeyword( material, "SF_WATERMAP_ISCOMPRESSED", waterMapIsCompressed );
		SetKeyword( material, "SF_WATERMASKMAP_ON", waterMaskMapOn );
		SetKeyword( material, "SF_FORWARDSHADOWS_ON", forwardShadowsOn );

		// if blending is on then force material to be transparent (this also disables the deferred pass automatically)
		if ( blendOn )
		{
			material.SetOverrideTag( "RenderType", "Transparent" );
			material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent + Mathf.RoundToInt( m_renderQueueOffset.floatValue );

			m_debugLogMessage[ 1 ] += " TYPE:TRANSPARENT QUEUE:" + material.renderQueue;
		}
		else
		{
			material.SetOverrideTag( "RenderType", "" );
			material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Geometry + Mathf.RoundToInt( m_renderQueueOffset.floatValue );

			m_debugLogMessage[ 1 ] += " TYPE:OPAQUE QUEUE:" + material.renderQueue;
		}

		// debug log
		if ( ( m_debugLogMessage[ 0 ] == null ) || ( m_debugLogMessage[ 0 ] != m_debugLogMessage[ 1 ] ) )
		{
			m_debugLogMessage[ 0 ] = m_debugLogMessage[ 1 ];

			Debug.Log( m_debugLogMessage[ 0 ] );
		}
	}

	void SetKeyword( Material material, string keyword, bool state )
	{
		if ( state )
		{
			material.EnableKeyword( keyword );

			m_debugLogMessage[ 1 ] += " " + keyword;
		}
		else
		{
			material.DisableKeyword( keyword );
		}
	}

	bool HasTextureMap( Material material, string propertyName )
	{
		if ( material.HasProperty( propertyName ) )
		{
			return material.GetTexture( propertyName );
		}

		return false;
	}

	bool IsSwitchedOn( Material material, string propertyName )
	{
		if ( material.HasProperty( propertyName ) )
		{
			return ( material.GetFloat( propertyName ) == 1.0f );
		}

		return false;
	}

	bool TextureIsCompressed( Material material, string propertyName )
	{
		if ( HasTextureMap( material, propertyName ) )
		{
			Texture2D texture = material.GetTexture( propertyName ) as Texture2D;

			if ( texture.format == TextureFormat.DXT5 )
			{
				return true;
			}
		}

		return false;
	}
}

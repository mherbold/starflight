
using UnityEngine;
using UnityEditor;

using System.IO;

[ExecuteInEditMode]
public class MassCreateGameObjects : EditorWindow
{
	GameObject m_sourceGameObject;
	GameObject m_parentGameObject;

	int m_numberOfGameObjects;

	string m_baseGameObjectName;

	private void OnEnable()
	{
	}

	[MenuItem( "Starflight Remake/Mass Create GameObjects" )]
	public static void ShowWindow()
	{
		EditorWindow editorWindow = EditorWindow.GetWindow( typeof( MassCreateGameObjects ) );

		editorWindow.autoRepaintOnSceneChange = true;

		editorWindow.Show();

		editorWindow.titleContent = new GUIContent( "Mass Create GameObjects" );
	}

	void OnGUI()
	{
		EditorGUILayout.LabelField( "Parameters", EditorStyles.boldLabel );

		m_sourceGameObject = (GameObject) EditorGUILayout.ObjectField( "Source GameObject", m_sourceGameObject, typeof( GameObject ), true );
		m_parentGameObject = (GameObject) EditorGUILayout.ObjectField( "Parent GameObject", m_parentGameObject, typeof( GameObject ), true );

		m_numberOfGameObjects = EditorGUILayout.IntField( "Number of GameObjects", m_numberOfGameObjects );

		m_baseGameObjectName = EditorGUILayout.TextField( "Base GameObject Name", m_baseGameObjectName );

		EditorGUILayout.Space();

		if ( GUILayout.Button( "Mass Create GameObjects", GUILayout.MinHeight( 60 ) ) )
		{
			var numSourceGameObjects = m_sourceGameObject.transform.childCount;

			for ( var i = 0; i < m_numberOfGameObjects; i++ )
			{
				var newGameObject = Instantiate( m_sourceGameObject.transform.GetChild( i % numSourceGameObjects ), m_parentGameObject.transform );

				newGameObject.name = m_baseGameObjectName + " " + ( i + 1 );

				var colliderComponents = newGameObject.GetComponents<Collider>();

				foreach ( var component in colliderComponents )
				{
					component.enabled = false;
				}

				var meshRendererComponents = newGameObject.GetComponents<MeshRenderer>();

				foreach ( var component in meshRendererComponents )
				{
					component.enabled = false;
				}
			}
		}
	}
}

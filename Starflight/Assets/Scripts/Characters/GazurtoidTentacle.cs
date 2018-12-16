
using UnityEngine;

public class GazurtoidTentacle : MonoBehaviour
{
	public float m_speed = 1.0f;
	public float m_range = 45.0f;
	public float m_increment = -0.6f;
	public float m_rangeIncrement = 15.0f;
	public float m_offset = 0.0f;

	int m_numBones;
	Transform[] m_bones;
	Vector3[] m_originalRotation;
	float m_angle;

	void Start()
	{
		// count number of bones (expecting 1d hierarchy)
		m_numBones = 1;

		var bone = transform.GetChild( 0 );

		while ( bone )
		{
			m_numBones++;

			if ( bone.childCount > 0 )
			{
				bone = bone.GetChild( 0 );
			}
			else
			{
				bone = null;
			}
		}

		// allocate array of transform references
		m_bones = new Transform[ m_numBones ];
		m_originalRotation = new Vector3[ m_numBones ];

		// collect references to bones
		bone = transform;

		for ( var i = 0; i < m_numBones; i++ )
		{
			m_bones[ i ] = bone;
			m_originalRotation[ i ] = bone.localRotation.eulerAngles;

			if ( bone.childCount > 0 )
			{
				bone = bone.GetChild( 0 );
			}
			else
			{
				bone = null;
			}
		}
	}

	void Update()
	{
		m_angle += Time.deltaTime * m_speed;

		var angle = m_angle + m_offset;

		for ( var i = 0; i < m_numBones; i++ )
		{
			var boneAngle = Mathf.Sin( angle ) * ( m_range + ( m_rangeIncrement * i ) );

			m_bones[ i ].localRotation = Quaternion.Euler( m_originalRotation[ i ].x, m_originalRotation[ i ].y, m_originalRotation[ i ].z + boneAngle );

			angle += m_increment;
		}
	}
}

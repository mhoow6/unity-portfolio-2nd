using UnityEngine;
using System.Collections;

public class LoadingTitle_CameraMove : MonoBehaviour
{
	public float MoveSpeed;
	GameObject m_mainCamera;

	// Use this for initialization
	void Start()
	{
		m_mainCamera = Camera.main.gameObject;
	}

	void LateUpdate()
	{
		MoveObj();
	}


	void MoveObj()
	{
		float moveAmount = Time.smoothDeltaTime * MoveSpeed;
		transform.Translate(0f, 0f, moveAmount);
	}

	void ChangeView01()
	{
		transform.position = new Vector3(0, 2, 10);
		// x:0, y:1, z:52
		m_mainCamera.transform.localPosition = new Vector3(-8, 2, 0);
		m_mainCamera.transform.localRotation = Quaternion.Euler(14, 90, 0);
	}

	void ChangeView02()
	{
		transform.position = new Vector3(0, 2, 10);
		// x:0, y:1, z:52
		m_mainCamera.transform.localPosition = new Vector3(0, 0, 0);
		m_mainCamera.transform.localRotation = Quaternion.Euler(19, 180, 0);
		MoveSpeed = -20f;

	}
}
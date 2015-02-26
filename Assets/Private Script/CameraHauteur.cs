using UnityEngine;
using System.Collections;

public class CameraHauteur : MonoBehaviour {
	public float sensitivityY = 8F;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float deltaY = Input.GetAxis("Mouse Y") * sensitivityY;
		if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
			ChangeHeight(deltaY);
			
		
	}
	void ChangeHeight(float aVal)
    {
        transform.position += aVal * Vector3.up;
    }
}

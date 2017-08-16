using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public float _CameraMovementSpeed;
	// Use this for initialization
	void Start () {
        _CameraMovementSpeed = _CameraMovementSpeed * -1;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            var _mousePos = Input.mousePosition;
            _mousePos.x -= Screen.width / 2;
            _mousePos.y -= Screen.height / 2;
            _mousePos.Normalize();
            Vector3 _dragDirection = new Vector3(_mousePos.x, 0, _mousePos.y) * _CameraMovementSpeed;
            transform.position += _dragDirection;
        }
    }
}

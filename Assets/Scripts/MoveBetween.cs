using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetween : MonoBehaviour {

	public Transform _transform1;
	public Transform _transform2;
	public  float _duration;
	private float _eventTime;
	private float _fracTime;
	private bool _backwards;

	// Use this for initialization
	void Start () {
		_eventTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (_fracTime > 1) {
			_eventTime = Time.time;
			_backwards = !_backwards;
		}

		_fracTime = (Time.time - _eventTime) / _duration;
		
		if (!_backwards) {
			transform.position = Vector3.Lerp(_transform1.position,_transform2.position,_fracTime);
		} else {
			transform.position = Vector3.Lerp(_transform2.position,_transform1.position,_fracTime);
		}
	}
}

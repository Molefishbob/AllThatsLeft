using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour, IPauseable
{
    public LayerMask _ignoredLayer;
    public Transform _lookAt;
    public float _distance = 5.0f;
    public string _cameraXAxis = "Camera X";
    public string _cameraYAxis = "Camera Y";
    private float _yaw = 0.0f;
    private float _pitch = 0.0f;
    public float _horizontalSensitivity = 1.0f;
    public float _verticalSensitivity = 1.0f;
    private bool _paused;
    public float _smooth = 8.0f;
    private float _tempDistance, _oldDistance;
    private float _lerpDistance;
    private float _lerperHelper = 0;
    private bool _zooming = false;

    public void Pause()
    {
        _paused = true;
    }

    public void UnPause()
    {
        _paused = false;
    }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _tempDistance = _distance;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _paused = GameManager.Instance.GamePaused;
        GameManager.Instance.AddPauseable(this);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemovePauseable(this);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        _yaw += _horizontalSensitivity * Input.GetAxis(_cameraXAxis);
        _pitch -= _verticalSensitivity * Input.GetAxis(_cameraYAxis);

        if (_pitch > 85)
        {
            _pitch = 85;
        }
        else if (_pitch < -70)
        {
            _pitch = -70;
        }

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);

        
        if (!_zooming)
        {
            _oldDistance = _tempDistance;
            _tempDistance = CheckCollision(_tempDistance);
        }
        //Debug.Log("olddistance " + _oldDistance + "newDistance " + _tempDistance);
        _lerpDistance = Mathf.Lerp(_oldDistance, _tempDistance, _lerperHelper);

        if (_zooming)
        {
            _lerperHelper += 0.1f;
        }

        if (_lerperHelper > 1)
        {
            _zooming = false;
            _lerperHelper = 0;
        }

        Debug.Log(_lerperHelper);
        Vector3 dir = new Vector3(0, 0, -_lerpDistance);
        
        transform.position = _lookAt.position + rotation * dir;
        transform.LookAt(_lookAt.position);
    }

    private float CheckCollision(float tDistance)
    {
        tDistance = _distance;
        RaycastHit hit;

        if (Physics.Raycast(_lookAt.position, transform.TransformDirection(Vector3.back), out hit, _distance, ~_ignoredLayer))
        {
            Debug.DrawLine(_lookAt.position, hit.point, Color.red, 1.0f, false);
            float newDistance = Vector3.Distance(hit.point, _lookAt.position);
            tDistance = newDistance;
            _zooming = true;
        }

        return tDistance;
    }

    /*IEnumerator reeeeeeeee()
    {
        for (int i = 0; i <= 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            Mathf.Lerp(_distance, _tempDistance, i / 100);
        }
    }*/
}
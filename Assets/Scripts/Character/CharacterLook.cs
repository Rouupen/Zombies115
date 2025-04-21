using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterLook : MonoBehaviour
{
    [Header("Values")]
    public float m_lookSpeed = 5.0f;

    [Header("Camera Shake")]
    public float m_shakeDuration = 0.2f;
    public float m_shakeMagnitude = 0.1f;

    private float xRotation = 0;
    private Vector2 _lookVelocity;

    private float _shakeTimer = 0.0f;
    private Vector3 _initialLocalPos;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _initialLocalPos = transform.localPosition;
    }

    void Update()
    {
        //Temp - Rotation
        transform.parent.Rotate(Vector3.up, GetLookInput().x * m_lookSpeed * Time.deltaTime);
        xRotation -= GetLookInput().y * m_lookSpeed * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        _lookVelocity = new Vector2(GetLookInput().x * m_lookSpeed * Time.deltaTime, GetLookInput().y * m_lookSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        // Camera shake
        if (_shakeTimer > 0)
        {
            transform.localPosition = _initialLocalPos + Random.insideUnitSphere * m_shakeMagnitude;
            _shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = _initialLocalPos;
        }
    }

    Vector2 GetLookInput()
    {
        //Temp - Read "Look" input value
        //Screen size dependent - CHANGE!!
        Vector2 rawInput = GameManager.GetInstance().m_inputManager.m_look.ReadValue<Vector2>();
        //return new Vector2(rawInput.x / Screen.width,rawInput.y / Screen.height);
        return rawInput;
    }

    public Vector2 GetLookVelocity()
    {
        return _lookVelocity;
    }

    public void ShakeCamera()
    {
        _shakeTimer = m_shakeDuration;
    }
}
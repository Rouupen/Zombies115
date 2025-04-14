using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterLook : MonoBehaviour
{
    [Header("Values")]
    public float m_lookSpeed = 5.0f;


    private float xRotation = 0;
    private Vector2 _lookVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Temp - Lock currsor
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Temp - Rotation
        transform.parent.Rotate(Vector3.up, GetLookInput().x * m_lookSpeed * Time.deltaTime);
        xRotation -= GetLookInput().y * m_lookSpeed * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        _lookVelocity = new Vector2(GetLookInput().x * m_lookSpeed * Time.deltaTime, GetLookInput().y * m_lookSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        print(GetLookInput().magnitude);
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
}
 
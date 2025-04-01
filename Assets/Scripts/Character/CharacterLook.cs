using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterLook : MonoBehaviour
{
    [Header("Values")]
    public float m_lookSpeed = 5.0f;


    private float xRotation = 0;


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
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    Vector2 GetLookInput()
    {
        //Temp - Read "Look" input value
        //Screen size dependent - CHANGE!!
        return GameManager.GetInstance().m_inputManager.m_look.ReadValue<Vector2>();
    }
}
 
using UnityEngine;

//TESTING
public class Weapon : MonoBehaviour
{
    public float m_maxAngleInertia = 30.0f;
    public float m_inertiaSpeed = 5.0f;
    public float m_returnToOriginalPositionInertiaSpeed = 5.0f;
    private Transform _socket;
    private Quaternion _socketOriginalRotation;
    private Quaternion _lastRotation;

    private void Awake()
    {
        _socket = transform.parent;
        _socketOriginalRotation = _socket.localRotation;
    }
    private void Update()
    {
        Vector2 dir = GameManager.GetInstance().m_inputManager.m_look.ReadValue<Vector2>();
        dir.y += GameManager.GetInstance().m_playerController.m_characterController.velocity.normalized.y;
        dir = Vector2.ClampMagnitude(dir, 1f);
        if (dir == Vector2.zero)
        {
            float angleDiff = Quaternion.Angle(_socket.localRotation, _socketOriginalRotation);
            float returnSpeed = (angleDiff / m_maxAngleInertia) * m_returnToOriginalPositionInertiaSpeed;
            _socket.localRotation = Quaternion.Slerp(_socket.localRotation, _socketOriginalRotation, Time.deltaTime * returnSpeed);
        }
        else
        {
            Quaternion targetRot = Quaternion.Euler(dir.y * m_maxAngleInertia, -dir.x * m_maxAngleInertia, 0f);
            _socket.localRotation = Quaternion.Slerp(_socket.localRotation, _socketOriginalRotation * targetRot, Time.deltaTime * m_inertiaSpeed);
            _lastRotation = _socket.localRotation;
        }
    }
}

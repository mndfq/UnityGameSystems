using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity;
    public float viewclamp;
    public Transform player;
    public Transform viewport;
    private float hInput, vInput;
    public float maxTiltAngle = 5f;    
    public float tiltSensitivity = 2f;    
    public float returnSpeed = 4f;        
    private float currentTilt;            
    private float tiltVelocity;           



    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        float hMouseInput = Input.GetAxis("Mouse X") * sensitivity;
        float vMouseInput = Input.GetAxis("Mouse Y") * sensitivity;

        hInput += hMouseInput;
        vInput -= vMouseInput;
        vInput = Mathf.Clamp(vInput, -viewclamp, viewclamp);

        player.rotation = Quaternion.Euler(0f, hInput, 0f);
        viewport.localRotation = Quaternion.Euler(vInput, 0f, 0f);


        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float targetTilt = horizontalInput * -maxTiltAngle;
        float currentSpeed = (horizontalInput != 0) ? tiltSensitivity : returnSpeed;
        currentTilt = Mathf.SmoothDamp(
            currentTilt,
            targetTilt,
            ref tiltVelocity,
            1f / currentSpeed
        );
        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            currentTilt
        );
    }
}

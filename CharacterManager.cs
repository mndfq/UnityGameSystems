using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] KeyCode jumpAction = KeyCode.Space;
    // Camera Variables
    public Transform character;
    public Transform characterCamera;
    public float characterCameraSensitivity = 1.75f;
    private float mouseHorizontalInput, mouseVerticalInput;
    private float currentTilt;
    private float tiltVelocity;
    // Force's
    private float velocityY = 0.0f;
    [SerializeField] private float fWalkSpeed = 12.0f, fDuckSpeed = 5.5f, fJumpMultiplier = 10.0f, fGravity = 9f, fDuckTime = 8.5f;
    [SerializeField] AnimationCurve jumpCurve;
    private float smoothTime = 0.1f;
    private float currentSpeed = 0f;
    Vector2 currentDirection = Vector2.zero;
    Vector2 currentDirectionVelocity = Vector2.zero;
    private CharacterController cc = null;
    private bool isJumping = false;
    // ---
    private void CameraStart()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void CameraUpdate()
    {
        Vector2 mouseAxis = new Vector2(Input.GetAxis("Mouse X") * characterCameraSensitivity, Input.GetAxis("Mouse Y") * characterCameraSensitivity);
        mouseHorizontalInput += mouseAxis.x;
        mouseVerticalInput -= mouseAxis.y;
        mouseVerticalInput = Mathf.Clamp(mouseVerticalInput, -65.0f, 64.0f);
        character.rotation = Quaternion.Euler(0f, mouseHorizontalInput, 0f);
        characterCamera.localRotation = Quaternion.Euler(mouseVerticalInput, 0f, 0f);

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float targetTilt = horizontalInput * -1.00f;
        float currentTiltSpeed = (horizontalInput != 0) ? 7.00f : 6.5f;
        currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, 1f / currentTiltSpeed);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, currentTilt);
    }

    private void MovementStart()
    {
        cc = GetComponent<CharacterController>();
    }

    private void MovementUpdate()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = fDuckSpeed;
        }
        else
        {
            currentSpeed = fWalkSpeed;
        }
        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDirection.Normalize();
        currentDirection = Vector2.SmoothDamp(currentDirection, targetDirection, ref currentDirectionVelocity, smoothTime);

        if (cc.isGrounded)
            velocityY = 0.0f;

        velocityY += fGravity * Time.deltaTime;

        Vector3 velocity = (transform.forward * currentDirection.y + transform.right * currentDirection.x) * currentSpeed + Vector3.down * velocityY;

        cc.Move(velocity * Time.deltaTime);
        JumpHandler();
        DuckHandler();
    }
    private void JumpHandler()
    {
        if (Input.GetKeyDown(jumpAction) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }

    }
    private void DuckHandler()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            cc.height = Mathf.MoveTowards(cc.height, 1.6f, fDuckTime * Time.deltaTime);
            cc.center = new Vector3(0, 0.8f, 0);

        }
        else
        {
            cc.height = Mathf.MoveTowards(cc.height, 2.8f, fDuckTime * Time.deltaTime);
            cc.center = new Vector3(0, 1.38f, 0);
        }
        
    }
    private void UpdateCameraPosition()
    {
        Vector3 worldCenter = cc.transform.position + cc.center;
        float eyeHeight = (cc.height / 2f) * 0.9f;
        Vector3 camPos = worldCenter + Vector3.up * eyeHeight;
        characterCamera.position = Vector3.Lerp(
        characterCamera.position, 
        camPos, 
        Time.deltaTime * 10f
        );
    }

    private IEnumerator JumpEvent()
    {
        float timer = 0.0f;
        do
        {
            float jumpForce = jumpCurve.Evaluate(timer);
            cc.Move(Vector3.up * jumpForce * fJumpMultiplier * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        } while (!cc.isGrounded && cc.collisionFlags != CollisionFlags.Above);
        isJumping = false;
    }
    private void AnimationUpdate()
    {
    /*
    *    
    *
    * You gotta figure this out.
    *
    *
    */
    }
    private void Awake()
    {

    }
    private void Start()
    {
        CameraStart();
        MovementStart();
    }
    private void Update()
    {
        CameraUpdate();
        MovementUpdate();
        UpdateCameraPosition();
    }

}
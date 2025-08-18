using ControllerFrameWork;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CController o_Controller = null;

    private void Start()
    {
        o_Controller.Set_DefaultValues();
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = (transform.forward * moveY + transform.right * moveX).normalized;
        o_Controller.CC_Move(moveDirection);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            o_Controller.CC_TryJump();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            o_Controller.Set_fSpeed(16.66f);
        }
        else
        {
            o_Controller.Set_fSpeed(9.99f);
        }

        o_Controller.CC_Think(Time.deltaTime);
    }

    private void Awake()
    {
        o_Controller = GetComponent<CController>();
    }
}
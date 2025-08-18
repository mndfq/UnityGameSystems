using UnityEngine;
namespace ControllerFrameWork
{
    [RequireComponent(typeof(CharacterController))]

    public class CController : MonoBehaviour
    {
        //-------------------------------------------------------------------------
        private float fSpeed, fJump, fFriction, fGravity, fAcceleration, fStop;
        //-------------------------------------------------------------------------
        [Header("Collision")]
        [SerializeField]
        [Tooltip("A mask of all objects which the player can collide with")]
        private LayerMask CollisionMask;
        //-------------------------------------------------------------------------
        private CharacterController cc_CharacterController = null;
        private Vector3 cc_WishDirection = Vector3.zero;
        private Vector3 cc_Velocity = Vector3.zero;
        private bool cc_OnFloor = true;
        private bool cc_RememberJump = false;
        private static readonly Vector3 cc_XZPlane = new Vector3(1.0f, 0.0f, 1.0f);
        //-------------------------------------------------------------------------
        public float Get_fSpeed()
        {
            return fSpeed;
        }
        public float Get_fJump()
        {
            return fJump;
        }
        public float Get_fFriction()
        {
            return fFriction;
        }
        public float Get_fGravity()
        {
            return fGravity;
        }
        public float Get_fAcceleration()
        {
            return fAcceleration;
        }
        public float Get_fStop()
        {
            return fStop;
        }
        //-------------------------------------------------------------------------
        public void Set_fSpeed(float speed)
        {
            fSpeed = speed;
        }
        public void Set_fJump(float jump)
        {
            fJump = jump;
        }
        public void Set_fFriction(float friction)
        {
            fFriction = friction;
        }
        public void Set_fGravity(float gravity)
        {
            fGravity = gravity;
        }
        public void Set_fAcceleration(float acceleration)
        {
            fAcceleration = acceleration;
        }
        public void Set_fStop(float stop)
        {
            fStop = stop;
        }
        public void Set_DefaultValues()
        {
            fSpeed          = 9.6f;
            fJump           = 10.5f;
            fFriction       = 3.8f;
            fGravity        = 28.5f;
            fAcceleration   = 8.5f;
            fStop           = 3.5f;
        }
        //-------------------------------------------------------------------------
        public void CC_Move(Vector3 moveDirection)
        {
            cc_WishDirection = moveDirection * fSpeed;
        }
        public void CC_Jump()
        {
            cc_RememberJump = true;
        }
        public void CC_TryJump()
        {
            if (cc_OnFloor) CC_Jump();
        }
        public void CC_Think(float deltaTime)
        {
            deltaTime = Mathf.Min(deltaTime, 1.0f);

            CC_IsOnFloor();
            CCUser_Gravity(deltaTime);
            CCUser_Friction(deltaTime);

            if (cc_OnFloor)
                CC_GroundAccelerate(deltaTime);
            else
                CC_AirAccelerate(deltaTime);

            if (cc_CharacterController)
                cc_CharacterController.Move(cc_Velocity * deltaTime);
            else
                Debug.LogWarning("Missing reference to CharacterController!");
        }
        //-------------------------------------------------------------------------
        private void CC_IsOnFloor()
        {
            float bodyRadius = cc_CharacterController.radius;
            float bodyHeight = cc_CharacterController.height;

            Vector3 origin = transform.position;
            Vector3 sphereCheckPosition = origin - ((transform.up * (bodyHeight * 0.5f) - transform.up * bodyRadius * 0.5f));

            cc_OnFloor = Physics.CheckSphere(sphereCheckPosition, bodyRadius, CollisionMask.value);
        }
        private void CCUser_Gravity(float deltaTime)
        {
            if (cc_OnFloor)
            {
                if (cc_RememberJump)
                {
                    cc_Velocity.y = fJump;
                    cc_OnFloor = false;

                    //hack because the character controller won't jump after getting stuck on a slope
                    cc_CharacterController.Move(Vector3.up * fJump * deltaTime);
                    cc_Velocity.y -= fJump * deltaTime;
                }
                else
                {
                    cc_Velocity.y = 0.0f;
                }
            }
            else
            {
                cc_Velocity.y -= deltaTime * fGravity;
            }

            cc_RememberJump = false; //Reset jump input for next frame
        }
        private void CCUser_Friction(float deltaTime)
        {
            float speed = Vector3.Scale(cc_XZPlane, cc_Velocity).magnitude;
            if (speed < 0.01f)
            {
                cc_Velocity = Vector3.Scale(cc_Velocity, Vector3.up);
                return;
            }

            float drop = 0.0f;
            if (cc_OnFloor)
            {
                float control = speed < fStop ? fStop : speed;
                drop += control * fFriction * deltaTime;
            }

            float newSpeed = Mathf.Max(speed - drop, 0.0f) / speed;
            cc_Velocity *= newSpeed;
        }
        private void CC_GroundAccelerate(float deltaTime) 
        {
            float alignment = Vector3.Dot(cc_Velocity, cc_WishDirection.normalized);
            float addSpeed = fSpeed - alignment;
            if (addSpeed <= 0)
                return;

            float accelSpeed = Mathf.Min(fAcceleration * deltaTime * fSpeed, addSpeed);
            cc_Velocity += accelSpeed * Vector3.Scale(cc_WishDirection.normalized, cc_XZPlane);
        }
        private void CC_AirAccelerate(float deltaTime)
        {
            float wishSpeed = Mathf.Min(cc_WishDirection.magnitude, 1.07f); //1.07f is 30.0f in Quake Units
            float alignment = Vector3.Dot(cc_Velocity, cc_WishDirection.normalized);

            float addSpeed = wishSpeed - alignment;
            if (addSpeed <= 0.0f)
                return;

            float accelSpeed = Mathf.Min(fAcceleration * wishSpeed * deltaTime, addSpeed);
            cc_Velocity += cc_WishDirection * accelSpeed;
        }
        private void Awake()
        {
            cc_CharacterController = GetComponent<CharacterController>();
        }
    }
}

/*
 * Code taken from https://github.com/LiamRousselle/Unity-Quake-Movement
 */
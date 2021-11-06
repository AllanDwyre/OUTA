using UnityEngine;

namespace PlayerMovement.Walking
{
    public class GroundMovement : MovementManager
    {
        [Header("Ground Movement")]
        [SerializeField] private float jumpForce = 7f;
        [SerializeField, Range(0f, 20f)] private float maxGroundSpeed = 10f;
        //[SerializeField, Range(0f, 5f)] private float airMultiplier = .5f;
        bool desiredJump;

       protected override void Awake()
       {
          base.Awake();
       }

        // Update is called once per frame
       protected override void Update()
       {
            desiredJump |= Input.GetKeyDown(KeyCode.Space);
            velocity = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        }

        protected override void FixedUpdate()
        {
            if (CurrentMoveType == MovementTypes.OnGound) 
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                Movement();
                if (desiredJump)
                {
                    desiredJump = false;
                    Jump();
                }
            }
            base.FixedUpdate();
        }
        private void Movement()
        {

            rb.MovePosition(rb.position + velocity * maxGroundSpeed * Time.fixedDeltaTime);
            //Debug.LogWarning("Ground");
           
        }
        /// <summary>
        /// Verifie si on est connecté au sol
        /// </summary>
        void Jump()
        {
            if (!SnapToGround() && !OnGround)
                return;
            rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
            stepsSinceLastJump = 0;
        }
    }
}
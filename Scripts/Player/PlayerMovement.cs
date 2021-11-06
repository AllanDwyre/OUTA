using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(Animator))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Ground setting : ")]
        [SerializeField] float runSpeed;
        [SerializeField] float walkSpeed;
        [SerializeField] float jumpPower;
        bool isClimbing;
        bool desiredJump; // Si le bouton pour le saut à été appuyer 


        [Header("Climb setting : ")]
        [SerializeField, Range(0f, 180f)]  float maxJumpAngle = 50f;
        [SerializeField] float climbSpeed;
        [SerializeField] float wallJumpPower;
        [SerializeField] float jumpLerp;

        [Header("Collision Setting : ")]
        [SerializeField] Vector3 pBoundsOffset;
        [SerializeField] Vector3 nBoundsOffset;

        bool desiredWallJump; // Si le bouton pour le saut à été appuyer 
        bool isJumping; //il est entrain de sauter. ( empeche un mouvement silmutanée avec un saut)
        bool canMove = true; //permission de mouvement
        bool jumpFromClimb = false; // empeche un mouvement lors d'un saut
        bool onBounds; // surveille si il y a un mur

        Rigidbody rb;
        Collision collision;
        Vector3 velocity = Vector3.zero;
        Animator animator;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            collision = GetComponent<Collision>();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {   //On interdit qu'il se colle a une surface quand il saute (&& !isJumping)
            if ((collision.onWall || collision.onCorner ) && !collision.onGround && !isJumping) // priorise le sol
            {
                velocity.y = Input.GetAxis("Vertical");
                isClimbing = true;
                velocity.x = 0; // Pour eviter le rebont infinit (valeur non-nul si saut + wallmove)
            }
            else //Si detection de sol
            {
                isClimbing = false;
                velocity.x = Input.GetAxis("Horizontal");
                velocity.y = 0; // Pour eviter le rebont infinit (valeur non-nul si saut + wallmove)
            }

            if (collision.onGround)
            {
                jumpFromClimb = false;
            }

            animator.SetBool("inAir", collision.onGround || isClimbing);
            animator.SetFloat("xVelocity", Mathf.Abs(velocity.x));
            animator.SetFloat("yVelocity", velocity.y);
            animator.SetBool("onWall", isClimbing);

            desiredJump |= Input.GetKeyDown(KeyCode.Space); // |= , desiredJump = desiredJump || input
            desiredWallJump |= Input.GetKeyDown(KeyCode.Mouse0);

            Debug.DrawRay(rb.position, ClampDirection(DirectionMouse()), Color.red);

        }

        private void FixedUpdate()
        {
            //Set gravity to 0 for climbing 
            if (isClimbing) 
            {
                rb.useGravity = false;
                rb.velocity = new Vector3(rb.velocity.x, 0);
            }
            else
            {
                rb.useGravity = true;
            }

            if (!canMove)
                return;

            Corner();

            Movement();

            //jump : 
            if (!isClimbing && desiredJump) // Saut normal
                Jump();
            else if (isClimbing && !collision.onGround && desiredWallJump)// Saut de mur
                WallJump();
        }

        /// <summary>
        /// Detect Corner
        /// </summary>
        public void Corner()
        {
            Vector3 boundsOffset = velocity.y > 0 ? pBoundsOffset : nBoundsOffset;
            boundsOffset += rb.position;

            onBounds = Physics.Raycast(boundsOffset, Vector3.left * -collision.side, 1f);

            Debug.DrawRay(boundsOffset, Vector3.left * -collision.side, Color.magenta);
        }
        private void Movement()
        {
            if (!canMove)
                return;

            if (!isClimbing) // si sur le sol
            {
                if (jumpFromClimb) // N'autorise aucun mouvement dans les airs suite a un saut
                    return;

                if (!isJumping) // Vitesse normal
                    rb.MovePosition(rb.position + velocity * walkSpeed * Time.fixedDeltaTime);
                else // vitesse apres un saut
                    rb.MovePosition(Vector3.Lerp(velocity,
                        rb.position + velocity * walkSpeed * Time.fixedDeltaTime, jumpLerp * Time.fixedDeltaTime));
                //Add sprint code
            }
            else // si sur le mur
            {
                // Bloque les mouvements si il n'y a aucun mur.
                if (!onBounds)
                    return;

                float speed = velocity.y > 0 ? climbSpeed : walkSpeed * .7f; // permet le slice
                rb.MovePosition(rb.position + velocity * speed * Time.fixedDeltaTime);
            }
        }

        void Jump()
        {
            desiredJump = false;
            if (collision.onGround)
            {
                rb.AddForce(jumpPower * Vector3.up, ForceMode.Impulse);
            }
        }

        void WallJump()
        {
            isJumping = true;
            desiredWallJump = false;
            jumpFromClimb = true;
            StartCoroutine(DisableMovement(.1f));
            rb.AddForce(wallJumpPower * ClampDirection(DirectionMouse()), ForceMode.Impulse);
        }

        public Vector3 DirectionMouse()
        {
            return ( Input.mousePosition - Camera.main.WorldToScreenPoint(rb.position) ).normalized;
        }
        private Vector3 ClampDirection(Vector3 direction)
        {
            float normal =  -collision.side;
            float angle = Mathf.Atan2(direction.y, direction.x * normal) * Mathf.Rad2Deg;

            Vector3 maxDir = Quaternion.Euler(0, 0, maxJumpAngle) * Vector3.right;
            maxDir.x *= normal;
            Vector3 minDir = Quaternion.Euler(0, 0, -maxJumpAngle) * Vector3.right;
            minDir.x *= normal;

            float maxAngle = Mathf.Atan2(maxDir.y, maxDir.x * normal) * Mathf.Rad2Deg;
            float minAngle = Mathf.Atan2(minDir.y, minDir.x * normal) * Mathf.Rad2Deg;


            if (Mathf.Max(angle, maxAngle) == angle)
                return maxDir.normalized;
            else if (Mathf.Min(angle, minAngle) == angle)
                return minDir.normalized;
            else
                return direction.normalized;
        }

        /// <summary>
        /// Désactive le mouvement du joueur pendans t (s).
        /// </summary>
        IEnumerator DisableMovement(float time)
        {
            canMove = false;
            yield return new WaitForSeconds(time);
            canMove = true;
            isJumping = false;
        }
    }
};
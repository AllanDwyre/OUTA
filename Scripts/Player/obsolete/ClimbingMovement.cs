using UnityEngine;
using System.Collections;

namespace PlayerMovement.Climbing
{
	public class ClimbingMovement : MovementManager
	{
		[SerializeField] private float jumpForce = 6f;

		[SerializeField, Range(0f, 20f)]
		float maxClimbSpeed = 4f;
		[SerializeField, Range(0f, 180f)]
		float maxJumpAngle = 50f;

		bool desiredJump; 
		bool desiredClimb;

		protected override void Awake()
		{
			base.Awake();
		}
        protected override void Update()
        {
            base.Update();
			desiredJump |= Input.GetKeyDown(KeyCode.Mouse0);
			velocity = new Vector3(0, Input.GetAxis("Vertical"), 0);
		}
		protected override void FixedUpdate()
        {
			if(CurrentMoveType == MovementTypes.Climbing)
            {
				rb.isKinematic = true;
				Debug.LogWarning("Climbing");
				Debug.DrawRay(rb.position, ClampDirection(DirectionMouse()), Color.yellow); //Souris
				Debug.DrawRay(rb.position, ProjectDirectionOnPlane(direction: velocity, normal: LastClimbNormal), Color.green);//Velocité
				Debug.DrawRay(rb.position, climbNormal.normalized, Color.red); //Normal

				Movement();
				
				if (desiredJump && velocity == Vector3.zero)
                {
					desiredJump = false;
					Jump();
				}
            }
			base.FixedUpdate();
		}

		private void Movement()
        {
			//Active ou desactive la physique
			if(velocity != Vector3.zero)
            {
				rb.isKinematic = false;
				rb.useGravity = false;
			}
            else
            {
				rb.isKinematic = true;
				rb.useGravity = true;
			}

			// Bloque les mouvements si il n'y a aucun mur.
			Vector3 rayCastPos = rb.position + (Vector3.up * .1f)* Mathf.Round(velocity.y);
			bool haveWall = Physics.Raycast(rayCastPos, Vector3.left * Mathf.Round(LastClimbNormal.x), out RaycastHit hit, 2f);

			Debug.DrawRay(rayCastPos, Vector3.left * Mathf.Round(LastClimbNormal.x), Color.magenta);

			if (haveWall)
            {
				rb.MovePosition(
					rb.position + ProjectDirectionOnPlane(direction: velocity, normal: LastClimbNormal) * maxClimbSpeed * Time.fixedDeltaTime);
			}
		}
		private void Jump()
		{
			rb.isKinematic = false;
			ChangeMoveType();

			Vector3 dir = ClampDirection(direction : DirectionMouse() );
			rb.AddForce(jumpForce * dir, ForceMode.Impulse);
		}

		/// <summary>
		/// Limite la direction de Jump entre un angle
		/// </summary>
		private Vector3 ClampDirection(Vector3 direction)
        {
			float normal = Mathf.Round(LastClimbNormal.x);
			float angle = Mathf.Atan2(direction.y, direction.x * normal) * Mathf.Rad2Deg;

			Vector3 maxDir = Quaternion.Euler(0, 0, maxJumpAngle) *  Vector3.right;
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
				return direction;
		}

	}
}

using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Collision : MonoBehaviour
    {
        [Header("Detect Ground setting : ")]
        public bool onGround;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] float groundDistance;
        [SerializeField] Vector3 groundOffset;


        [Header("Detect Climbable setting : ")]
        public bool onWall;
        public bool onCorner;
        bool onRightWall;
        bool onLeftWall;
        [SerializeField] LayerMask climbableLayer;
        [SerializeField] float wallDistance;
        [SerializeField] Vector3 rClimbOffset;
        [SerializeField] Vector3 lClimbOffset;
        [SerializeField] Vector3 cornerOffset;

        public int side;


        private void Update()
        {
            onGround = Physics.Raycast(transform.position + groundOffset, Vector3.down, groundDistance, groundLayer);

            onRightWall = Physics.Raycast(transform.position + rClimbOffset, Vector3.right, wallDistance, climbableLayer);
            onLeftWall = Physics.Raycast(transform.position + lClimbOffset, Vector3.left, wallDistance, climbableLayer);

            Vector3 dir = onRightWall? Vector3.right : Vector3.left;

            onCorner = Physics.Raycast(transform.position + cornerOffset, dir, wallDistance);

            onWall = onRightWall || onLeftWall;
            side = onRightWall ? 1 : -1;
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position + rClimbOffset, transform.position + rClimbOffset + Vector3.right * wallDistance);
            Gizmos.DrawLine(transform.position + lClimbOffset, transform.position + lClimbOffset + Vector3.left * wallDistance);
            Vector3 dir = onRightWall ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(transform.position + cornerOffset, transform.position + cornerOffset + dir * wallDistance);

            Gizmos.DrawLine(transform.position + groundOffset, transform.position + groundOffset + Vector3.down * groundDistance);
        }
    }
}
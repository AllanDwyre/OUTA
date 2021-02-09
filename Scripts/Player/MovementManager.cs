using UnityEngine;

namespace PlayerMovement
{
    /// <summary>
    /// Cette classe permet de determiner le type de mouvement du joueur à appliquer
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class MovementManager : MonoBehaviour
    {

        //--------------Protected ---------------------------------
        protected Rigidbody rb, connectedBody, previousConnectedBody; // Notre body, celui ou on es attache et l'ancien
        protected Vector3 velocity;
        protected Vector3 LastClimbNormal { get; private set; }
        protected int stepsSinceLastGrounded, stepsSinceLastJump;
        protected bool OnGround => groundContactCount > 0;
        protected MovementTypes CurrentMoveType { get; private set; }
        //--------------Private  SerializeField-------------------
        [Header("Movement Controller")]
        //Angle max :
        [SerializeField, Range(0, 90)]
        float maxGroundAngle = 25f;
        [SerializeField, Range(90, 170)]
        float maxClimbAngle = 140f;

        //Permet de detecter le sol :
        [SerializeField, Min(0f)]
        float probeDistance = 1f;
        [SerializeField]
        LayerMask climbMask = -1, goundMask = -1;

        //--------------Private  ----------------------------------
        //Angle en Produit Scalaire
        float minGround, minClimb;
        //Normal:
        protected Vector3 groundNormal, climbNormal ;
        int groundContactCount,climbContactCount;
        bool climbing => climbContactCount > 0 && stepsSinceLastJump > 2;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
            OnValidate();
        }

        /// <summary>
        /// Transforme l'angle en produit Scalaire
        /// </summary>
        private void OnValidate()
        {
            minGround = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
            minClimb = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
        }

        protected virtual void Update()
        {
        }
        protected virtual void FixedUpdate()
        {
            
            if (SnapToGround() || OnGround)
            {
                CurrentMoveType = MovementTypes.OnGound;
            }   
            else if (climbing && previousConnectedBody == connectedBody)
            {
                CurrentMoveType = MovementTypes.Climbing;
            }
            UpdateState();
            ClearState();
        }

        /// <summary>
        /// Met à jour des variable qui son egal au nombre passer
        /// </summary>
        private void UpdateState() 
        {
            stepsSinceLastGrounded += 1;
            stepsSinceLastJump += 1;
            velocity = rb.velocity;
            if (CheckClimbing() || OnGround || SnapToGround())
            {
                stepsSinceLastGrounded = 0;
                if (groundContactCount > 1)
                {
                    groundNormal.Normalize();
                }
            }
            else
            {
                groundNormal = Vector3.up;
            }

        }
        
        /// <summary>
        /// ClearState permet de réinsilliser cetaines valeurs à chaques frames.
        /// </summary>
        protected void ClearState()
        {
            climbContactCount = groundContactCount = 0;
            climbNormal = groundNormal = Vector3.zero;
            previousConnectedBody = connectedBody;
            connectedBody = null;
        }
        public Vector3 DirectionMouse()
        {
            return ( Input.mousePosition - Camera.main.WorldToScreenPoint(rb.position) ).normalized;
        }
        protected void ChangeMoveType()
        {
            CurrentMoveType = MovementTypes.OnGound;
        }

        /// <summary>
        /// Vérifie si il est accroché au sol. 
        /// Et met à jour certaines information.
        /// </summary>
        protected bool SnapToGround()
        {
            if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2)
            {
                return false;
            }
            if (!Physics.Raycast(rb.position, -Vector3.up, out RaycastHit hit, probeDistance, goundMask))
            {
                return false;
            }
            connectedBody = hit.rigidbody;
            return true;
        }

        /// <summary>
        /// On verrifie si on est en escalade
        /// </summary>
        bool CheckClimbing()
        {
            if (climbing)
            {
                groundContactCount = climbContactCount;
                groundNormal = climbNormal;
                return true;
            }
            return false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            EvaluateCollision(collision);
        }
        private void OnCollisionStay(Collision collision)
        {
            EvaluateCollision(collision);
        }
        protected void EvaluateCollision(Collision collision)
        {
            int layer = collision.gameObject.layer;
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;
                float upDot = Vector3.Dot(Vector3.up, normal);
                
                if (upDot >= minGround) //Si il est sur le sol
                {
                    groundContactCount++;
                    groundNormal += normal;
                    connectedBody = collision.rigidbody;
                }
                else if (upDot >= minClimb && ( climbMask & ( 1 << layer ) ) != 0) //Si c'est une surfarce grimpable
                {
                    climbContactCount++;
                    climbNormal += normal;
                    LastClimbNormal = normal;
                    connectedBody = collision.rigidbody;
                }
            }
        }

        /// <summary>
        /// Donne la direction de l'obstacle. Pour un mouvement sur un objet
        /// </summary>
        protected Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return ( direction - normal * Vector3.Dot(direction, normal) ).normalized;
        }
    }
    public enum MovementTypes
    {
        OnGound,
        Climbing,
        InSpace,
        None
    }
}
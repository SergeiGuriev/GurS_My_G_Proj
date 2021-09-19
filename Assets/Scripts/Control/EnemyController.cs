using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using UnityEngine;
using RPG.Attributes;
using System;

namespace RPG.Control
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] float patrolDistance = 5f;        
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float patrolSpeed = 3f;
        [SerializeField] float chaseSpeed = 5f;
        [SerializeField] float waypointAvailability = 1f;
        [SerializeField] float waypointDwellTime = 4f;
        [SerializeField] float aggressiveBehCooldownTime = 2f;
        [SerializeField] float attentionDistance = 5f;
        private GameObject player;
        private Fighter fighter;
        private Health health;
        private Mover mover;
        private Collider colider;
        private Vector3 guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtPoint = Mathf.Infinity;
        private float suspicionTime = 5f;
        private int currentWaypointIndex = 0;
        private float aggressiveBehaviourTime = Mathf.Infinity;
        private bool isEnemyAggresive = false;
        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            colider = GetComponent<Collider>();
        }
        private void Start()
        {            
            player = GameObject.FindWithTag("Player");
            guardPosition = transform.position;
        }
        void Update()
        { 
            colider.enabled = !health.IsDead();

            if (health.IsDead()) return;
            if (IsAggressive() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();       // else не будет срабатывать потому что мы его постоянно отменяем в 
                isEnemyAggresive = false;
            }                               // <ActionScheduler>().CancelCurrentAction();
            else
            {
                PatrolBehaviour();
            }
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtPoint += Time.deltaTime;
            aggressiveBehaviourTime += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            mover.SetSpeed(patrolSpeed);
            Vector3 nextPosition = guardPosition;   // стартовая позиция
            if (patrolPath != null)
            {
                if (AtWayPoint())
                {
                    CurrentCycleWaypoint();
                    timeSinceArrivedAtPoint = 0;
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtPoint >= waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition);
            }            
        }

        private bool AtWayPoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointAvailability;
        }
        private void CurrentCycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }
        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }


        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
            mover.SetSpeed(chaseSpeed);

            if (isEnemyAggresive == false)
            {
                isEnemyAggresive = true;
                drawGuardGroupAttention();
            }
        }

        private void drawGuardGroupAttention()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, attentionDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                EnemyController controller = hit.collider.GetComponent<EnemyController>();
                if (controller == null)
                {
                    continue;
                }                
                controller.AggressiveReaction();                
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, patrolDistance);
        }

        private bool IsAggressive()
        {
            return Vector3.Distance(player.transform.position, transform.position) <= patrolDistance || aggressiveBehaviourTime < aggressiveBehCooldownTime;
        }


        public void AggressiveReaction()
        {
            aggressiveBehaviourTime = 0;
        }
    }
}
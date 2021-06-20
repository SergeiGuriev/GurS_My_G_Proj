using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Control
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] float patrolDistance = 5f;
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointAvailability = 1f;
        [SerializeField] float waypointDwellTime = 4f;
        private GameObject player;
        private Fighter fighter;
        private Health health;
        private Mover mover;
        private Vector3 guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtPoint = Mathf.Infinity;
        private float suspicionTime = 5f;
        private int currentWaypointIndex = 0;
        private void Start()
        {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            guardPosition = transform.position;
        }
        void Update()
        {
            if (health.IsDead()) return; 
            if (IsPlayerInRange() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();       // else не будет срабатывать потому что мы его постоянно отменяем в 
            }                               // <ActionScheduler>().CancelCurrentAction();
            else
            {
                PatrolBehaviour();
            }
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtPoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition;   // стартовая позиция игрока
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
        }

        private bool IsPlayerInRange()
        {
            return Vector3.Distance(player.transform.position, transform.position) <= patrolDistance;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, patrolDistance);
        }
    }
}

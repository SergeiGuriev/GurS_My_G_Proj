using RPG.Combat;
using RPG.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxNavMeshPathLength = 40f;
        NavMeshAgent navMeshAgent;
        Health health;
        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();
        }

        void UpdateAnimator()
        {                                                                                // velocity = скорость отдаления от нулевой z координаты
            Vector3 globalVelocity = navMeshAgent.velocity;                              // скорость движения относительно (0,0,0) мировых координат
            Vector3 localVelocity = transform.InverseTransformDirection(globalVelocity); // если персонаж будет бежать в сторону отдаления от мировой Z координаты
                                                                                         //Debug.Log(globalVelocity.z);                                               // то его velocity будет отрицательным
            float speed = localVelocity.z;                                               // поэтому нужно расчитывать скорость относительно (0,0,0) координат персонажа
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);  // мне нужно знать с какой скоростью(изменения Z координаты) персонаж отдаляется от своей позиции, 
        }                                                              // а не относительно нуля мира

        public void StartMoveAction(Vector3 destination)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            GetComponent<Fighter>().Cancel();
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.isStopped = false;
        }
        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }


        public void SetSpeed(float speed)
        {
            navMeshAgent.speed = speed;
        }

        public object CaptureState()
        {
            //return new SerializableVector3(transform.position);
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)state;
            navMeshAgent.enabled = false;
            transform.position = ((SerializableVector3)data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            navMeshAgent.enabled = true;
        }

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath navMeshPath = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, navMeshPath);
            if (!hasPath)
            {
                return false;
            }
            if (navMeshPath.status != NavMeshPathStatus.PathComplete)
            {
                return false;
            }
            if (GetNavMeshPathLength(navMeshPath) > maxNavMeshPathLength)
            {
                return false;
            }
            return true;
        }

        private float GetNavMeshPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2)
            {
                return total;
            }
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }
    }
}

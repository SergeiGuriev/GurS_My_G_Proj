using RPG.Combat;
using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        NavMeshAgent navMeshAgent;
        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        void Update()
        {
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
    }
}

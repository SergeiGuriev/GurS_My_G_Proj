﻿using RPG.Combat;
using RPG.Movement;
using UnityEngine;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {       

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] float maxNavMeshProjDistance = 1f;
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float raycastRadius = 0.5f;
        
        Health health;
        Collider colider;
        private void Awake()
        {
            health = GetComponent<Health>();
            colider = GetComponent<Collider>();
        }
        void Update()
        {
            colider.enabled = !health.IsDead();
            if (InteractWithComponent())
            {
                return;
            }
            if (InteractWithUI())
            {
                SetCursor(CursorType.UI);
                return;
            }
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
               IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            //RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        public Vector3 target;
        private bool InteractWithMovement()
        {            
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target))
                {
                    return false;
                }
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;            
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit)
            {
                return false;
            }
            NavMeshHit navMeshHit;
            bool isCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjDistance, NavMesh.AllAreas);
            if (!isCastToNavMesh)
            {
                return false;
            }
            target = navMeshHit.position;
            return true;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping cursorMapping = GetCursorMapping(type);
            Cursor.SetCursor(cursorMapping.texture, cursorMapping.hotspot, CursorMode.ForceSoftware);
            //Cursor.SetCursor(cursorMapping.texture, cursorMapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping item in cursorMappings)
            {
                if (item.type == type)
                {
                    return item;
                }
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
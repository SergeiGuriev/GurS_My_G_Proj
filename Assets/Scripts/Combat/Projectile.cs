using RPG.Attributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 10f;
        [SerializeField] float damage = 0;
        [SerializeField] bool isAimbotActive = false;
        [SerializeField] UnityEvent onHit;
        Health target = null;
        GameObject instigator = null;

        private void Start()
        {            
            transform.LookAt(GetAimPoint());
            StartCoroutine("DestroyLostProjectiles");
        }
        private void Update()
        {
            if (target == null) return;
            if (isAimbotActive && !target.IsDead())
            {
                transform.LookAt(GetAimPoint());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }        
        private Vector3 GetAimPoint()
        {
            CapsuleCollider targetCapsCol = target.GetComponent<CapsuleCollider>();
            if (targetCapsCol == null) return target.transform.position;
            return target.transform.position + Vector3.up * targetCapsCol.height / 2;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;
            onHit.Invoke();
            target.TakeDamage(instigator, damage);
            Destroy(gameObject);
        }
        IEnumerator DestroyLostProjectiles()
        {
            yield return new WaitForSeconds(2.5f);
            Destroy(gameObject);
        }
        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
        }
    }
}

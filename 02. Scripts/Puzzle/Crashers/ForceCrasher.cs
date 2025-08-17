using System.Collections.Generic;
using UnityEngine;

public class ForceCrasher : MonoBehaviour, ICrashable
{
    //[Header("폭발 설정")]
    //[SerializeField] private float explosionForce = 10f;
    //[SerializeField] private float explosionRadius = 3f;
    //[SerializeField] private float upwardModifier = 0.5f;

    //[Header("한 번만 폭발")]
    //private bool hasExploded = false;

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (hasExploded) return;

    //    if (collision.relativeVelocity.magnitude > 3f)
    //    {
    //        BreakApart();
    //        hasExploded = true;
    //    }
    //}

    //public void BreakApart()
    //{
    //    // 하위 모든 Rewinder 가져오기
    //    Rewinder[] pieces = GetComponentsInChildren<Rewinder>();

    //    BoxCollider box = GetComponent<BoxCollider>();
    //    box.enabled = false;

    //    foreach (Rewinder rewinder in pieces)
    //    {
    //        Rigidbody rb = rewinder.GetComponent<Rigidbody>();

    //        if (rb != null)
    //        {
    //            // 물리 활성화
    //            rb.isKinematic = false;

    //            // 폭발 방향 계산
    //            Vector3 explosionPos = transform.position;

    //            rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, upwardModifier, ForceMode.Impulse);
    //        }
    //    }
    //}

    public void Crash()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            // 시작 시 랜덤한 방향과 세기로 힘을 가함
            Vector3 randomDir = Random.onUnitSphere;
            float forceMagnitude = Random.Range(8f, 13f);
            rb.AddForce(randomDir * forceMagnitude, ForceMode.Impulse);
        }

    }
}

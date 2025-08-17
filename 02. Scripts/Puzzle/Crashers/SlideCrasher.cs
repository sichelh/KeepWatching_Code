using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideCrasher : MonoBehaviour, ICrashable
{
    public Transform targetPosition;
    public float duration = 1f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void Crash()
    {
        StartCoroutine(SlideTo());
    }

    private IEnumerator SlideTo()
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        float time = 0f;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition.position, time / duration);
            transform.rotation = Quaternion.Lerp(startRotation, targetPosition.rotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition.position;
        rb.isKinematic = false;
    }
}

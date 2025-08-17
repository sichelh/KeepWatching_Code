using UnityEngine;

public class GrabObjectController : MonoBehaviour
{
    [SerializeField] private Transform grabObjectPosition;
    [SerializeField] private GameObject grabObject;

    private Rigidbody grabRigidbody;

    public bool IsGrab { get; private set; }

    public void GrabObject(GameObject obj)
    {
        obj.transform.parent = grabObjectPosition;
        IsGrab = true;
        grabRigidbody = obj.GetComponent<Rigidbody>();
        grabRigidbody.isKinematic = true;
        grabObject = obj;
    }

    public void UnGrabObject()
    {
        IsGrab = false;
        grabRigidbody.isKinematic = false;
        grabObject.transform.parent = null;
        grabObject = null;
    }
}
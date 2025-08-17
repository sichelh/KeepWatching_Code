using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleConnector : MonoBehaviour
{
    [Header("촛대")]
    public Rigidbody candleBody;

    private FixedJoint joint;

    public void Joint()
    {
        if (candleBody == null)
        {
            Debug.Log("촛대 없음");
                return;
        }

        joint = gameObject.GetComponent<FixedJoint>();
        joint.connectedBody = candleBody;
        joint.breakForce = Mathf.Infinity;
        joint.breakTorque = Mathf.Infinity;
    }

    public void UnJoint()
    {

    }

}

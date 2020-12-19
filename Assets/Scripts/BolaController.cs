using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolaController : MonoBehaviour
{
    private Rigidbody2D rg;

    // Start is called before the first frame update
    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        FixedJoint2D joint = GetComponent<FixedJoint2D>();

        if (collision.gameObject.CompareTag("teto") || collision.gameObject.CompareTag("bubble"))
        {
            joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
            joint.autoConfigureConnectedAnchor = false;
            rg.bodyType = RigidbodyType2D.Static;
        }
        else if (joint == null)
        {
            ContactPoint2D point = collision.GetContact(0);
            Debug.Log(point.relativeVelocity);
            Vector3 vec = new Vector3(0, -point.relativeVelocity.y * 60);
            // rg.AddForce(vec, ForceMode2D.Impulse);
        }
    }
}
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

            if (collision.gameObject.CompareTag("bubble") && rg.bodyType != RigidbodyType2D.Static)
            {
                FixBobblePosition(collision);
            }

            rg.bodyType = RigidbodyType2D.Static;
        }
    }

    private void FixBobblePosition(Collision2D collision)
    {
        Vector3 colliderPosition = collision.gameObject.transform.position;
        Vector3 positionDif = colliderPosition - transform.position;

        Debug.Log("subtract" + positionDif);

        Vector3 fixedPos = new Vector3();
        if (positionDif.x <= 0 && positionDif.z >= 0.2f) //posicionar no lado direito 
        {
            fixedPos = new Vector3(0.8f, 0, 0);
        }
        else if (positionDif.x <= 0 && positionDif.z < 0.2f) //posicionar no canto inferior direito 
        {
            fixedPos = new Vector3(0.4f, -0.6f, 0);
        }
        else if (positionDif.x > 0 && positionDif.z < 0.2f) //posicionar no canto inferior esquerdo 
        {
            fixedPos = new Vector3(-0.4f, -0.6f, 0);
        }
        else if (positionDif.x > 0 && positionDif.z >= 0.2f) //posicionar no lado esquerdo
        {
            fixedPos = new Vector3(-0.8f, 0, 0);
        }

        transform.position = colliderPosition + fixedPos;
    }
}
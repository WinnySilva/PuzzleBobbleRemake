using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BolaController : MonoBehaviour
{
    public Grid posicaoBolinhas;
    public float offsetJuncao = 0.35f;
    private bool shooted = false;
    private Rigidbody2D rg;

    public bool Shooted { get => shooted; set => shooted = value; }

    // Start is called before the first frame update
    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        FixedJoint2D joint = GetComponent<FixedJoint2D>();


        if (collision.gameObject.CompareTag("teto")
            || (collision.gameObject.CompareTag("bubble") && rg.bodyType != RigidbodyType2D.Static && shooted))
        {
            StringBuilder log = new StringBuilder();
            log.AppendLine(collision.gameObject.tag);
            //  FixBobblePosition(collision);
            Vector3 point = FixBobblePosition(collision); //collision.contacts[0].point + new Vector2(0.0f,-0.50f);
            log.AppendLine("colisao corrigido "+point);
            Vector3Int cellPosition = posicaoBolinhas.WorldToCell(point);
            transform.position = posicaoBolinhas.CellToWorld(cellPosition);
            rg.bodyType = RigidbodyType2D.Static;

            Rigidbody2D rig = collision.gameObject.GetComponent<Rigidbody2D>();
            joint = gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = rig;
            joint.autoConfigureConnectedAnchor = false;

            log.AppendLine("POSICAO NO GRID " + cellPosition);
            log.AppendLine("position " + transform.position);
            log.AppendLine("positionDiff " + ( new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y) - transform.position));
            
            Debug.Log(log.ToString());
        }
    }

    private Vector3 FixBobblePosition(Collision2D collision)
    {
        Vector3 colliderPosition = collision.contacts[0].point; //collision.gameObject.transform.position;
        Vector3 positionDif = colliderPosition - transform.position;

        //     Debug.Log("diff y: " + positionDif.y+" diff x: " + positionDif.x);

        float offset = offsetJuncao;

        Vector3 fixedPos = new Vector3();
        if (positionDif.x <= 0 && positionDif.y <= 0.2f) //posicionar no lado direito 
        {
            fixedPos = new Vector3(offset, 0, 0);
        }
        else if (positionDif.x <= 0 && positionDif.y > 0.2f) //posicionar no canto inferior direito 
        {
            fixedPos = new Vector3(offset, -offset, 0);
        }
        else if (positionDif.x > 0 && positionDif.y > 0.2f) //posicionar no canto inferior esquerdo 
        {
            fixedPos = new Vector3(-offset, -offset, 0);
        }
        else if (positionDif.x > 0 && positionDif.y <= 0.2f) //posicionar no lado esquerdo
        {
            fixedPos = new Vector3(-offset, 0, 0);
        }

        return (colliderPosition + fixedPos);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BolaController : MonoBehaviour
{
    public Grid posicaoBolinhas;
    public Tilemap posicaoBolinhasTile;
    public float offsetJuncao = 0.35f;
    public CoresBolinhas cor;
    public int limiteLinhas = 12;

    public delegate void LimiteAlcancado();
    public static event LimiteAlcancado LimiteBolinhasAlcancado;

    public delegate void BolinhaFixadaAction();
    public static event BolinhaFixadaAction BolinhaFixada;


    private bool shooted = false;
    private Rigidbody2D rg;
    private FixedJoint2D joint;


    public bool Shooted
    {
        get => shooted;
        set => shooted = value;
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        joint = GetComponent<FixedJoint2D>();
        rg = GetComponent<Rigidbody2D>();
    }

    public void setColor(CoresBolinhas novaCor)
    {
        cor = novaCor;
        SpriteRenderer spRen = GetComponent<SpriteRenderer>();

        switch (novaCor)
        {
            case CoresBolinhas.AMARELO:
                spRen.color = Color.yellow;
                break;
            case CoresBolinhas.AZUL:
                spRen.color = Color.blue;
                break;
            case CoresBolinhas.VERMELHO:
                spRen.color = Color.red;
                break;
            default:
                spRen.color = Color.white;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rg.bodyType == RigidbodyType2D.Static || !shooted)
        {
            return;
        }

        if (collision.gameObject.CompareTag("teto")
            || collision.gameObject.CompareTag("bubble"))
        {

            Vector3Int cellPosition = FixBobblePosition(collision);

            rg.bodyType = RigidbodyType2D.Static;

            //Rigidbody2D rig = collision.gameObject.GetComponent<Rigidbody2D>();
            //joint = gameObject.AddComponent<FixedJoint2D>();
            //joint.connectedBody = rig;
            //joint.autoConfigureConnectedAnchor = false;

            StringBuilder log = new StringBuilder();
            log.AppendLine(collision.gameObject.tag);
            log.AppendLine("POSICAO NO GRID " + cellPosition);
            log.AppendLine("position " + transform.position);
            log.AppendLine("positionDiff " + (new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y) - transform.position));
            log.AppendLine("colisoes " + collision.contacts.Length);

            Debug.Log(log.ToString());

            BolaController bc;
            if (collision.gameObject.TryGetComponent<BolaController>(out bc))
            {
                bc.setColor((CoresBolinhas)9);
            }


            if (cellPosition.y <= -limiteLinhas)
            {
                LimiteBolinhasAlcancado();
            }
            BolinhaFixada();
        }
    }

    private Vector3Int FixBobblePosition(Collision2D collision)
    {

        Vector3 ponto = collision.contacts[0].point;

        Vector3Int celulaColidida = posicaoBolinhasTile.WorldToCell(ponto);
        //Vector3Int novaCelula = new Vector3Int(celulaColidida.x - 1, celulaColidida.y, celulaColidida.z);
        Vector3Int novaCelula = buscarPosicaoLivre(collision, celulaColidida);
        Tile water = ScriptableObject.CreateInstance<Tile>();

        water.sprite = Resources.Load<Sprite>("Tilesets/Hexagon") as Sprite;


        posicaoBolinhasTile.SetTile(novaCelula, water);
        transform.position = posicaoBolinhasTile.CellToWorld(novaCelula);

        return novaCelula;
    }

    private Vector3Int buscarPosicaoLivre(Collision2D collision, Vector3Int celulaColidida)
    {
        Vector3 colliderPosition = collision.contacts[0].point; //collision.gameObject.transform.position;
        Vector3 positionDif = colliderPosition - transform.position;
        Vector3Int celula = celulaColidida;
        bool posicaoOcupada = false;

        bool ahEsquerda = positionDif.x > 0;

        if (!posicaoBolinhasTile.HasTile(celula))
        {
            return celula;
        }

        if (ahEsquerda)
        {
            Vector3Int esqInf = new Vector3Int(celulaColidida.x - 1, celulaColidida.y - 1, celulaColidida.z);
            Vector3Int esq = new Vector3Int(celulaColidida.x - 1, celulaColidida.y, celulaColidida.z);

            if ((positionDif.y < 0.2f)) // esquerda inferior
            {
                if (!posicaoBolinhasTile.HasTile(esqInf))
                {
                    return esqInf;
                }
                else
                {
                    return esq;
                }
            }

            if ((positionDif.y >= 0.2f)) //esquerda
            {

                if (!posicaoBolinhasTile.HasTile(esq))
                {
                    return esq;
                }
                else
                {
                    return esqInf;
                }
            }

        }
        else
        {
            Vector3Int dir = new Vector3Int(celulaColidida.x + 1, celulaColidida.y, celulaColidida.z);
            Vector3Int dirInf = new Vector3Int(celulaColidida.x + 1, celulaColidida.y - 1, celulaColidida.z);

            if ((positionDif.y < 0.2f)) // direita inferior
            {
                if (!posicaoBolinhasTile.HasTile(dirInf))
                {
                    return dirInf;
                }
                else
                {
                    return dir;
                }

            }

            if ((positionDif.y >= 0.2f)) //direita
            {

                if (!posicaoBolinhasTile.HasTile(dir))
                {
                    return dir;
                }
                else
                {
                    return dirInf;
                }
            }

        }

        return celula;

    }
}
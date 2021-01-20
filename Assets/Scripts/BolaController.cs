using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BolaController : MonoBehaviour
{

    public Tilemap posicaoBolinhasTile;
    public float offsetJuncao = 0.35f;
    public CoresBolinhas cor;
    public int limiteLinhas = 12;
    public GameController controleJogo;

    public delegate void LimiteAlcancado();

    public static event LimiteAlcancado LimiteBolinhasAlcancado;

    public delegate void BolinhaFixadaAction();

    public static event BolinhaFixadaAction BolinhaFixada;
    public int x;
    public int y;

    private bool shooted;
    private bool fixado;
    private Rigidbody2D rg;
    private bool isMatched;


    public bool Shooted
    {
        get => shooted;
        set => shooted = value;
    }


    // Start is called before the first frame update
    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        fixado = false;
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
            case CoresBolinhas.MATCHED:
                spRen.color = Color.clear;
                break;
            default:
                spRen.color = Color.white;
                break;
        }
    }

    private void OnDestroy()
    {
        Vector3Int celulaGrid = new Vector3Int(x, y, 0);
        controleJogo.RemoverBolinha(this);
        posicaoBolinhasTile.SetTile(celulaGrid, null);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rg.bodyType == RigidbodyType2D.Static || !shooted || fixado)
        {
            return;
        }

        if (collision.gameObject.CompareTag("hexTeto") || collision.gameObject.CompareTag("teto")
            || collision.gameObject.CompareTag("bubble"))
        {
            fixado = true;
            Vector3Int cellPosition = FixBobblePosition(collision);
            // rg.bodyType = RigidbodyType2D.Static;

            StringBuilder log = new StringBuilder();
            log.AppendLine(collision.gameObject.tag);
            log.AppendLine("POSICAO NO GRID " + cellPosition);
            log.AppendLine("position " + transform.position);
            log.AppendLine("positionDiff " +
                           (new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y) -
                            transform.position));
            log.AppendLine("colisoes " + collision.contacts.Length);
            Debug.Log(log.ToString());

            if (collision.gameObject.CompareTag("hexTeto"))
            {

                FixedJoint2D fj = this.gameObject.AddComponent<FixedJoint2D>();
                fj.autoConfigureConnectedAnchor = true;
                fj.anchor = Vector2.zero;
      //          fj.connectedAnchor = Vector2.zero;
                fj.frequency = 0;
                fj.connectedBody = this.controleJogo.ObterPosicaoBolinhaTeto(cellPosition);
            }

            if (cellPosition.y <= -limiteLinhas)
            {
                LimiteBolinhasAlcancado();
            }

            BolinhaFixada();
            x = cellPosition.x;
            y = cellPosition.y;
            controleJogo.AdicionarBolinha(x, y, this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (rg.bodyType == RigidbodyType2D.Static || !shooted)
        {
            return;
        }

        if (collision.gameObject.CompareTag("chao"))
        {
            StartCoroutine(SelfDestruct());
        }
    }

    private Vector3Int FixBobblePosition(Collision2D collision)
    {
        Vector3Int celulaColidida = posicaoBolinhasTile.WorldToCell(transform.position);
        Vector3Int novaCelula = buscarPosicaoLivre(collision, celulaColidida);
        Tile hex = ScriptableObject.CreateInstance<Tile>();

        hex.sprite = Resources.Load<Sprite>("Tilesets/Hexagon");

        posicaoBolinhasTile.SetTile(novaCelula, hex);
        transform.position = posicaoBolinhasTile.CellToWorld(novaCelula);

        return novaCelula;
    }

    private Vector3Int buscarPosicaoLivre(Collision2D collision, Vector3Int celula)
    {
        Vector3 colliderPosition = collision.contacts[0].point;
        Vector3 positionDif = colliderPosition - transform.position;

        bool ahEsquerda = positionDif.x > 0;

        if (!posicaoBolinhasTile.HasTile(celula))
        {
            return celula;
        }

        Vector3Int tilePosition = posicaoBolinhasTile.WorldToCell(colliderPosition);

        Vector3Int esqInf = new Vector3Int(tilePosition.x - 1, tilePosition.y - 1, tilePosition.z);
        Vector3Int esq = new Vector3Int(tilePosition.x - 1, tilePosition.y, tilePosition.z);

        Vector3Int dirInf = new Vector3Int(tilePosition.x, tilePosition.y - 1, tilePosition.z);
        Vector3Int dir = new Vector3Int(tilePosition.x + 1, tilePosition.y, tilePosition.z);
        //isso diminui a taxa de erro porém ainda precisa ser melhorado
        if (ahEsquerda)
        {
            if (positionDif.y > 0.13f) // esquerda inferior
            {
                return !posicaoBolinhasTile.HasTile(esqInf) ? esqInf :
                    !posicaoBolinhasTile.HasTile(esq) ? esq :
                    !posicaoBolinhasTile.HasTile(dir) ? dir : dirInf;
            }

            //esquerda
            return !posicaoBolinhasTile.HasTile(esq) ? esq :
                !posicaoBolinhasTile.HasTile(esqInf) ? esqInf :
                !posicaoBolinhasTile.HasTile(dirInf) ? dirInf : dir;
        }


        if (positionDif.y > 0.13f) // direita inferior
        {
            return !posicaoBolinhasTile.HasTile(dirInf) ? dirInf :
                !posicaoBolinhasTile.HasTile(dir) ? dir :
                !posicaoBolinhasTile.HasTile(esq) ? esq : esqInf;
        }

        return !posicaoBolinhasTile.HasTile(dir) ? dir :
            !posicaoBolinhasTile.HasTile(dirInf) ? dirInf :
            !posicaoBolinhasTile.HasTile(esqInf) ? esqInf : esq;
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }


}
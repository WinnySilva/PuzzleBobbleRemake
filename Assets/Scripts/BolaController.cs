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
    public GameController controleJogo;

    public delegate void LimiteAlcancado();

    public static event LimiteAlcancado LimiteBolinhasAlcancado;

    public delegate void BolinhaFixadaAction();

    public static event BolinhaFixadaAction BolinhaFixada;
    public int x;
    public int y;

    private bool shooted = false;
    private Rigidbody2D rg;
    private FixedJoint2D joint;
    private bool isMatched = false;


    public bool Shooted
    {
        get => shooted;
        set => shooted = value;
    }

    public bool IsMatched
    {
        get => isMatched;
        set => isMatched = value;
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
        posicaoBolinhasTile.SetTile(celulaGrid, null);
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

            StringBuilder log = new StringBuilder();
            log.AppendLine(collision.gameObject.tag);
            log.AppendLine("POSICAO NO GRID " + cellPosition);
            log.AppendLine("position " + transform.position);
            log.AppendLine("positionDiff " +
                           (new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y) -
                            transform.position));
            log.AppendLine("colisoes " + collision.contacts.Length);
            Debug.Log(log.ToString());

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

    private Vector3Int FixBobblePosition(Collision2D collision)
    {
        Vector3 ponto = collision.contacts[0].point;

        Vector3Int celulaColidida = posicaoBolinhasTile.WorldToCell(ponto);
        //Vector3Int novaCelula = new Vector3Int(celulaColidida.x - 1, celulaColidida.y, celulaColidida.z);
        Vector3Int novaCelula = buscarPosicaoLivre(collision, celulaColidida);
        Tile hex = ScriptableObject.CreateInstance<Tile>();

        hex.sprite = Resources.Load<Sprite>("Tilesets/Hexagon");

        posicaoBolinhasTile.SetTile(novaCelula, hex);
        transform.position = posicaoBolinhasTile.CellToWorld(novaCelula);

        return novaCelula;
    }

    private Vector3Int buscarPosicaoLivre(Collision2D collision, Vector3Int celula)
    {
        Vector3 colliderPosition = collision.contacts[0].point; //collision.gameObject.transform.position;
        Vector3 positionDif = colliderPosition - transform.position;

        bool ahEsquerda = positionDif.x > 0;

        if (!posicaoBolinhasTile.HasTile(celula))
        {
            return celula;
        }

        Vector3Int tilePosition = posicaoBolinhasTile.WorldToCell(colliderPosition);
        Debug.Log("Tile da colisão: " + tilePosition + " Diferença: " + positionDif);

        if (ahEsquerda)
        {
            Vector3Int esqInf = new Vector3Int(tilePosition.x - 1, tilePosition.y - 1, tilePosition.z);
            Vector3Int esq = new Vector3Int(tilePosition.x - 1, tilePosition.y, tilePosition.z);

            if (positionDif.y > 0.13f) // esquerda inferior
            {
                return posicaoBolinhasTile.HasTile(esqInf) ? esq : esqInf;
            }

            //esquerda
            return posicaoBolinhasTile.HasTile(esq) ? esqInf : esq;
        }

        Vector3Int dirInf = new Vector3Int(tilePosition.x, tilePosition.y - 1, tilePosition.z);
        Vector3Int dir = new Vector3Int(tilePosition.x + 1, tilePosition.y, tilePosition.z);

        if (positionDif.y > 0.13f) // direita inferior
        {
            return posicaoBolinhasTile.HasTile(dirInf) ? dir : dirInf;
        }

        return posicaoBolinhasTile.HasTile(dir) ? dirInf : dir;
    }
}
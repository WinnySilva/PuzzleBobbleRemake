using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BolaController : MonoBehaviour
{
    public Tilemap posicaoBolinhasTile;
    public CoresBolinhas cor;
    public GameObject alturaDerrota;
    public GameController controleJogo;
    public int x;
    public int y;

    public delegate void LimiteAlcancado();
    public static event LimiteAlcancado LimiteBolinhasAlcancado;
    
    public delegate void BolinhaFixadaAction();
    public static event BolinhaFixadaAction BolinhaFixada;
    

    private bool _atirado;
    private bool _fixado;
    private Rigidbody2D _rg;
    private bool _coladoNoTeto;

    public bool ColadoNoTeto
    {
        get => _coladoNoTeto;
    }

    public Rigidbody2D Rg
    {
        get => _rg;
    }


    public bool Atirado
    {
        set => _atirado = value;
    }
    
    public bool Fixado
    {
        set => _fixado = value;
    }


    // Start is called before the first frame update
    private void Start()
    {
        _rg = GetComponent<Rigidbody2D>();
        _fixado = false;
        TetoController.AcaoTetoAbaixou += AbaixarBolinha;
    }

    private void AbaixarBolinha(float offsetBaixar)
    {
        try
        {
            Vector3 pos = transform.position;
            pos.y -= offsetBaixar;
            transform.position = pos;
            
            if (transform.position.y <= alturaDerrota.transform.position.y
            ) // se for mais baixo que o game object de game over
            {
                LimiteBolinhasAlcancado?.Invoke();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }

    public void SetCor(CoresBolinhas novaCor)
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
        controleJogo.SinalizaBolinhaDestruida();
        Vector3Int celulaGrid = new Vector3Int(x, y, 0);
        controleJogo.RemoverBolinha(this);
        posicaoBolinhasTile.SetTile(celulaGrid, null);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_rg.bodyType == RigidbodyType2D.Static || !_atirado || _fixado)
        {
            return;
        }

        if (collision.gameObject.CompareTag("hexTeto") || collision.gameObject.CompareTag("teto")
                                                       || collision.gameObject.CompareTag("bubble"))
        {
            _fixado = true;
            Vector3Int cellPosition = ArrumarPosicaoDaBolha(collision);
            _rg.bodyType = RigidbodyType2D.Static;

            if (collision.gameObject.CompareTag("hexTeto"))
            {
                _coladoNoTeto = true;
            }

            if (transform.position.y <= alturaDerrota.transform.position.y) // se for mais baixo que o game object de game over
            {
                LimiteBolinhasAlcancado?.Invoke();
            }

            BolinhaFixada?.Invoke();
            x = cellPosition.x;
            y = cellPosition.y;
            controleJogo.AdicionarBolinha(x, y, this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);
        if (_rg.bodyType == RigidbodyType2D.Static || !_atirado)
        {
            if (collision.gameObject.CompareTag("chaoGameOver"))
            {
                //termina o jogo 
                Debug.Log("Entrou no trigger " + _fixado);
            }
            return;
        }

        if (collision.gameObject.CompareTag("chao"))
        {
            StartCoroutine(AutoDestruir());
        } 
        
    }

    private Vector3Int ArrumarPosicaoDaBolha(Collision2D collision)
    {
        Vector3Int celulaColidida = posicaoBolinhasTile.WorldToCell(transform.position);
        Vector3Int novaCelula = BuscarPosicaoLivre(collision, celulaColidida);
        Tile hex = ScriptableObject.CreateInstance<Tile>();

        hex.sprite = Resources.Load<Sprite>("Tilesets/Hexagon");

        posicaoBolinhasTile.SetTile(novaCelula, hex);
        transform.position = posicaoBolinhasTile.CellToWorld(novaCelula);

        return novaCelula;
    }

    private Vector3Int BuscarPosicaoLivre(Collision2D collision, Vector3Int celula)
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

    IEnumerator AutoDestruir()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
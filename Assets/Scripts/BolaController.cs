using System;
using System.Collections;
using Enums;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BolaController : MonoBehaviour
{
    public Tilemap posicaoBolinhasTile;
    public Tilemap paredeTileMap;
    public CoresBolinhas cor;
    public GameObject alturaDerrota;
    public GameController controleJogo;
    public int x;
    public int y;

    public delegate void LimiteAlcancado();

    public static event LimiteAlcancado LimiteBolinhasAlcancado;

    public delegate void BolinhaFixadaAction();

    public static event BolinhaFixadaAction BolinhaFixada;

    [SerializeField] private bool _atirado;
    [SerializeField] private bool _fixado;
    private Rigidbody2D _rg;
    [SerializeField] private bool _coladoNoTeto;

    public bool ColadoNoTeto
    {
        get => _coladoNoTeto;
        set => _coladoNoTeto = value;
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
    private void Awake()
    {
        _rg = GetComponent<Rigidbody2D>();
        _fixado = false;
        TetoController.AcaoTetoAbaixou += AbaixarBolinha;
    }
    
    private void AbaixarBolinha(float offsetBaixar)
    {
        try
        {
            if (_atirado && transform.position.y <= alturaDerrota.transform.position.y
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
                spRen.color = new Color(0.9725491f, 0.9411765f, 0);
                break;
            case CoresBolinhas.AZUL:
                spRen.color = Color.blue;
                break;
            case CoresBolinhas.VERMELHO:
                spRen.color = new Color(0.8301887f, 0.1057316f, 0.1057316f);
                break;
            case CoresBolinhas.VERDE:
                spRen.color = new Color(0, 0.282353f, 0);
                break;
            case CoresBolinhas.BRANCO:
                spRen.color = Color.white;
                break;
            case CoresBolinhas.CINZA:
                spRen.color = new Color(0.3137255f, 0.3764706f, 0.3764706f);
                break;
            case CoresBolinhas.LARANJA:
                spRen.color = new Color(0.9411765f, 0.517f, 0.09411766f);
                break;
            case CoresBolinhas.ROXO:
                spRen.color = new Color(0.5333334f, 0.1882353f, 0.6901961f);
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
        AnimExplodir();
        controleJogo.SinalizaBolinhaDestruida();
        Vector3Int celulaGrid = new Vector3Int(x, y, 0);
        posicaoBolinhasTile.SetTile(celulaGrid, null);
        controleJogo.RemoverBolinha(this);
        
        TetoController.AcaoTetoAbaixou -= AbaixarBolinha;
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
            Debug.Log($"Adding at position {cellPosition}");

            _rg.bodyType = RigidbodyType2D.Static;

            if (collision.gameObject.CompareTag("hexTeto"))
            {
                _coladoNoTeto = true;
            }

            if (transform.position.y <= alturaDerrota.transform.position.y
            ) // se for mais baixo que o game object de game over
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
        Vector3 bolaQueFoiColidida = collision.gameObject.transform.position;
        Vector3 colliderPosition = collision.contacts[0].point;
        Vector3 positionDif = colliderPosition - transform.position;

        Vector3Int tileDaColisao = posicaoBolinhasTile.WorldToCell(colliderPosition);
        Vector3Int tileDaBolaQueFoiColidida = posicaoBolinhasTile.WorldToCell(bolaQueFoiColidida);
        Debug.Log($"Collider position {tileDaColisao}");

        bool ahEsquerda = positionDif.x > 0;

        if (!posicaoBolinhasTile.HasTile(tileDaColisao))
        {
            Debug.Log($"Retornando posicao de colisao, tem parede {paredeTileMap.HasTile(tileDaColisao)}");
            return tileDaColisao;
        }

        if (!posicaoBolinhasTile.HasTile(celula))
        {
            return AcharPosicaoBaseadoNaBola(celula, ahEsquerda, positionDif, tileDaBolaQueFoiColidida);
        }


        int xEsquerda, xDireita;
        if (tileDaColisao.y % 2 == 0)
        {
            xDireita = celula.x;
            xEsquerda = celula.x - 1;
        }
        else
        {
            xDireita = celula.x + 1;
            xEsquerda = celula.x;
        }


        Vector3Int esqInf = new Vector3Int(xEsquerda, tileDaColisao.y - 1, tileDaColisao.z);
        Vector3Int esq = new Vector3Int(xEsquerda, tileDaColisao.y, tileDaColisao.z);

        Vector3Int dirInf = new Vector3Int(xDireita, tileDaColisao.y - 1, tileDaColisao.z);
        Vector3Int dir = new Vector3Int(xDireita, tileDaColisao.y, tileDaColisao.z);

        //isso diminui a taxa de erro porém ainda precisa ser melhorado
        if (ahEsquerda)
        {
            if (positionDif.y > 0.20f) // esquerda inferior
            {
                return !posicaoBolinhasTile.HasTile(esqInf) ? esqInf :
                    !posicaoBolinhasTile.HasTile(esq) ? esq : dirInf;
            }

            //esquerda
            return !posicaoBolinhasTile.HasTile(esq) ? esq : esqInf;
        }


        if (positionDif.y > 0.20f) // direita inferior
        {
            return !posicaoBolinhasTile.HasTile(dirInf) ? dirInf :
                !posicaoBolinhasTile.HasTile(dir) ? dir : esqInf;
        }

        return !posicaoBolinhasTile.HasTile(dir) ? dir : dirInf;
    }

    private Vector3Int AcharPosicaoBaseadoNaBola(Vector3Int celula, bool ahEsquerda, Vector3 positionDif,
        Vector3Int tileDaBolaQueFoiColidida)
    {
        int xEsquerda, xDireita;

        Debug.Log($"Retornando posicao da bolinha {celula}, tile do colidido {tileDaBolaQueFoiColidida}");
        if (celula.y % 2 == 0)
        {
            xDireita = celula.x;
            xEsquerda = celula.x - 1;
        }
        else
        {
            xDireita = celula.x + 1;
            xEsquerda = celula.x;
        }

        Vector3Int cimaEsq = new Vector3Int(xEsquerda, celula.y + 1, celula.z);
        Vector3Int esquerda = new Vector3Int(celula.x - 1, celula.y, celula.z);

        Vector3Int cimaDireita = new Vector3Int(xDireita, celula.y + 1, celula.z);
        Vector3Int direita = new Vector3Int(celula.x + 1, celula.y, celula.z);

        if (tileDaBolaQueFoiColidida.y % 2 == 0)
        {
            xDireita = tileDaBolaQueFoiColidida.x;
            xEsquerda = tileDaBolaQueFoiColidida.x - 1;
        }
        else
        {
            xDireita = tileDaBolaQueFoiColidida.x + 1;
            xEsquerda = tileDaBolaQueFoiColidida.x;
        }

        Vector3Int esqInfDoColidido =
            new Vector3Int(xEsquerda, tileDaBolaQueFoiColidida.y - 1, tileDaBolaQueFoiColidida.z);
        Vector3Int dirInfDoColidido =
            new Vector3Int(xDireita, tileDaBolaQueFoiColidida.y - 1, tileDaBolaQueFoiColidida.z);
        Debug.Log($"Position dif y {positionDif.y}");
        if (!posicaoBolinhasTile.HasTile(cimaEsq) && !posicaoBolinhasTile.HasTile(cimaDireita) && positionDif.y > 0.20f
        ) // caso onde o bolinha esta sem tile, e deve adionar em baixo da colisao, e a posicao da bola nao tem vizinhos superiores
        {
            Debug.Log($"Adicionando para esquerda {ahEsquerda}");

            Debug.Log(paredeTileMap.HasTile(ahEsquerda ? cimaEsq : cimaDireita));


            if (ahEsquerda)
            {
                //adicionar no vizinho de baixo da esquerda do tileDaBolaQueFoiColidida, caso tenha parede, adiciona na direta
                return !paredeTileMap.HasTile(esqInfDoColidido) ? esqInfDoColidido : dirInfDoColidido;
            }

            //adicionar no vizinho de baixo da direta do tileDaBolaQueFoiColidida, caso tenha parede, adiciona na esquerda
            return !paredeTileMap.HasTile(dirInfDoColidido) ? dirInfDoColidido : esqInfDoColidido;
        }


        if (paredeTileMap.HasTile(celula)) //caso a posicao da bola seja parede
        {
            if (posicaoBolinhasTile.HasTile(cimaEsq)
            ) //adiciona na esquerda quando a sustencao vier da esquerda, ou seja, parede na direita
            {
                return esquerda;
            }

            if (posicaoBolinhasTile.HasTile(cimaDireita)
            ) //adiciona na direita quando a sustencao vier da direita, ou seja, parede na esquerda
            {
                return direita;
            }
        }

        Vector3Int esqDoColidido = new Vector3Int(tileDaBolaQueFoiColidida.x - 1, tileDaBolaQueFoiColidida.y,
            tileDaBolaQueFoiColidida.z);
        Vector3Int dirDoColidido = new Vector3Int(tileDaBolaQueFoiColidida.x + 1, tileDaBolaQueFoiColidida.y,
            tileDaBolaQueFoiColidida.z);


        if (ahEsquerda)
        {
            if (!posicaoBolinhasTile.HasTile(esqDoColidido) && !paredeTileMap.HasTile(esqDoColidido))
            {
                return esqDoColidido;
            }

            if (!paredeTileMap.HasTile(esqInfDoColidido))
            {
                return esqInfDoColidido;
            }

            return dirInfDoColidido;
        }
        
        if (!posicaoBolinhasTile.HasTile(dirDoColidido) && !paredeTileMap.HasTile(dirDoColidido))
        {
            return dirDoColidido;
        }

        if (!paredeTileMap.HasTile(dirInfDoColidido))
        {
            return dirInfDoColidido;
        }

        return esqInfDoColidido;
    }

    IEnumerator AutoDestruir()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public void AnimExplodir()
    {
        Animator anim;
        if (TryGetComponent<Animator>(out anim))
        {
            anim.SetBool("toDestroy", true);
        }
    }
}
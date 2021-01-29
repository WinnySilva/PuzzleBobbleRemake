using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using Enums;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public Tilemap posicaoBolinhasTile;
    public TetoController controleTeto;

    [SerializeField] private int contagemBolinhasDestruidas = 0;
    [SerializeField] private int contagemBolinhasDisparadas = 0;


    public delegate void FinalJogoAcao(bool vitoria);
    public static event FinalJogoAcao FinalJogo;
    public double PontuacaoBonus { get; set; }
    public double TempoJogo { get; set; }
    public DateTime HoraInicio { get; set; }

    private int _x, _y;
    BolaController _bola;
    private Dictionary<string, BolaController> _bolasNoJogo;
    private Dictionary<string, BolaController> _bolasNoTeto;
    private bool _finalDeJogo;
    private GerenciadorDeSom _gerenciadorDeSom;
    private AudioSource musicaFase;  


    private GameInfoAcrossRounds gameInfo;       

    public Dictionary<string, BolaController> BolasNoJogo
    {
        get => _bolasNoJogo;
    }

    // Start is called before the first frame update
    void Awake()
    {
        _bolasNoJogo = new Dictionary<string, BolaController>();
        _bolasNoTeto = new Dictionary<string, BolaController>();
        BolaController.LimiteBolinhasAlcancado += FinalJogoDerrota;
        _x = 0;
        _y = 0;
       
        _gerenciadorDeSom = FindObjectOfType<GerenciadorDeSom>();
        StartCoroutine(TocarSonsDeInicio());        
    }

    private void OnDestroy()
    {
        BolaController.LimiteBolinhasAlcancado -= FinalJogoDerrota;

    }

    private void Start()
    {
         gameInfo = GameObject.FindObjectOfType<GameInfoAcrossRounds>();
    }

    private void Update()
    {
        if (!_finalDeJogo && _bolasNoJogo.Count == 0)
        {
            _finalDeJogo = true;
            DateTime horaFinal = DateTime.Now;
            TimeSpan diff = horaFinal.Subtract(HoraInicio);
            this.CalculoBonus(diff.TotalSeconds);
            TempoJogo = Math.Truncate(diff.TotalSeconds);            
            this.gameInfo.Pontuacao = this.PontuacaoBonus;
            
            FinalJogo?.Invoke(true);
        }
    }

    public void RemoverBolinha(BolaController obj)
    {
        string key = hashPos(obj.x, obj.y);
        _bolasNoJogo.Remove(key);
    }

    public void AdicionarBolinha(int x, int y, BolaController obj)
    {
        string coord = hashPos(x, y);
        _x = x;
        _y = y;

        if (y == 6 || obj.ColadoNoTeto)
        {
            obj.ColadoNoTeto = true;
            _bolasNoTeto.Add(coord, obj);
        }

        _bolasNoJogo.Add(coord, obj);
        EncontrarMatches(obj);
        contagemBolinhasDisparadas++;
        if (contagemBolinhasDisparadas % 8 == 0)
        {
            controleTeto.BaixarNivelTeto();
        }
    }


    public void AdicionarBolinhaContrucaoFase(int x, int y, BolaController obj)
    {
        string coord = hashPos(x, y);
        _x = x;
        _y = y;

        if (y == 6 || obj.ColadoNoTeto)
        {
            obj.ColadoNoTeto = true;
            _bolasNoTeto.Add(coord, obj);
        }
        _bolasNoJogo.Add(coord, obj);
    }


    public void EncontrarMatches(BolaController val)
    {
        List<BolaController> avaliados = new List<BolaController>();
        List<BolaController> listaMatches = new List<BolaController>();

        Queue<BolaController> matches = new Queue<BolaController>();

        matches.Enqueue(val);
        listaMatches.Add(val);
        avaliados.Add(val);

        while (matches.Count > 0)
        {
            BolaController m = matches.Dequeue();
            List<BolaController> vizinhos = BuscaVizinhos(m);
            foreach (BolaController v in vizinhos)
            {
                if (!avaliados.Contains(v))
                {
                    avaliados.Add(v);
                    if (v.cor == val.cor)
                    {
                        matches.Enqueue(v);
                        listaMatches.Add(v);
                    }
                }
            }
        }

        if (listaMatches.Count > 2)
        {
            _gerenciadorDeSom.Play(ConstantesDeAudio.ESTOURO_BOLHA);
            StartCoroutine(DestruirBolinhasMatches(listaMatches));
        }
    }

    public void DerrubarBolinhasSemEncontrarTeto()
    {
        List<BolaController> avaliados = new List<BolaController>();
        Queue<BolaController> matches = new Queue<BolaController>();

        foreach (BolaController bola in _bolasNoTeto.Values)
        {
            matches.Enqueue(bola);
            avaliados.Add(bola);
        }

        while (matches.Count > 0)
        {
            BolaController m = matches.Dequeue();
            List<BolaController> vizinhos = BuscaVizinhos(m);
            foreach (BolaController v in vizinhos)
            {
                if (!avaliados.Contains(v))
                {
                    avaliados.Add(v);
                    matches.Enqueue(v);
                }
            }
        }

        int bolasADerrubar = 0;
        foreach (BolaController bola in _bolasNoJogo.Values)
        {
            if (!avaliados.Contains(bola))
            {
                bola.gameObject.GetComponent<CircleCollider2D>().isTrigger = true;
                bola.Rg.bodyType = RigidbodyType2D.Dynamic;
                bola.Rg.gravityScale = 1f;
                bola.Fixado = false;
                bolasADerrubar++;
            }
        }

        this.gameInfo.Pontuacao = bolasADerrubar == 0 ? 0 : Mathf.Pow(2, bolasADerrubar) * 10;

    }

    public void SinalizaBolinhaDestruida()
    {
        contagemBolinhasDestruidas++;
    }

    public int ObterQtdBolinhasDestruidas()
    {
        return contagemBolinhasDestruidas;
    }

    private void DestruirBolinhas(List<BolaController> listaMatches)
    {
        string coord;
        foreach (BolaController b in listaMatches)
        {
            coord = hashPos(b.x, b.y);
            _bolasNoJogo.Remove(coord);
            if (b.ColadoNoTeto)
            {
                _bolasNoTeto.Remove(coord);
            }
            this.gameInfo.Pontuacao = 10;
            Destroy(b.gameObject);

        }
    }

    private string hashPos(int x, int y)
    {
        return $"x:{x} - y:{y}";
    }

    void OnGUI()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 mouse = ray.origin;
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow);
        Vector3Int cord = posicaoBolinhasTile.WorldToCell(mouse);

        GUI.Label(new Rect(10, 10, 100, 20), $"{cord}");
        GUI.Label(new Rect(10, 40, 100, 20), $"{ray.origin}");
        GUI.Label(new Rect(10, 60, 100, 20), $"Destruidas:: {contagemBolinhasDestruidas}");
        GUI.Label(new Rect(10, 80, 100, 20), $"Disparadas:: {contagemBolinhasDisparadas}");
        GUI.Label(new Rect(10, 100, 100, 20), $"Em jogo:: {_bolasNoJogo.Count}");
        GUI.Label(new Rect(10, 120, 100, 20), $":: {_x},{_y}");
    }

    private List<BolaController> BuscaVizinhos(BolaController val)
    {
        bool par = val.y % 2 == 0;
        int xEsquerda, xDireita;
        string auxKey;
        List<BolaController> vizinhos = new List<BolaController>();
        BolaController auxVal;

        if (par)
        {
            xDireita = val.x;
            xEsquerda = val.x - 1;
        }
        else
        {
            xDireita = val.x + 1;
            xEsquerda = val.x;
        }

        //esquerda topo
        auxKey = hashPos(xEsquerda, val.y + 1);
        if (_bolasNoJogo.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }

        //esquerda inferior
        auxKey = hashPos(xEsquerda, val.y - 1);
        if (_bolasNoJogo.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }

        //direita topo
        auxKey = hashPos(xDireita, val.y + 1);
        if (_bolasNoJogo.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }

        //direita inferior
        auxKey = hashPos(xDireita, val.y - 1);
        if (_bolasNoJogo.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }

        //esquerda
        auxKey = hashPos((val.x - 1), val.y);
        if (_bolasNoJogo.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }

        //direita
        auxKey = hashPos(val.x + 1, val.y);
        if (_bolasNoJogo.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }

        return vizinhos;
    }

    IEnumerator DestruirBolinhasMatches(List<BolaController> bolinhasParaDestruir)
    {
        bolinhasParaDestruir.ForEach(b => b.AnimExplodir());
        yield return new WaitForSeconds(0.3f);

        DestruirBolinhas(bolinhasParaDestruir);
        DerrubarBolinhasSemEncontrarTeto();
    }

    private void FinalJogoDerrota()
    {
        FinalJogo?.Invoke(false);
    }

    private IEnumerator TocarSonsDeInicio()
    {
        _gerenciadorDeSom.Play(ConstantesDeAudio.INICIO_READY);
        yield return new WaitForSeconds(0.9f);

        _gerenciadorDeSom.Play(ConstantesDeAudio.INICIO_GO);

        _gerenciadorDeSom.Play(ConstantesDeAudio.MUSICA_FASE);

    }

    private void CalculoBonus(Double sec)
    {
        if (sec > 48)
        {
            this.PontuacaoBonus = 0;
        }
        else if (sec < 6)
        {
            this.PontuacaoBonus = 50000;
        }
        else
        {
            this.PontuacaoBonus = Math.Truncate((48 - sec) * 1162.79);
        }        

    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;
using Random = UnityEngine.Random;

public class RecargaController : MonoBehaviour
{
    public MiraController mira;
    public FixedJoint2D joint;
    public GameObject bolaClone;
    public GameObject atualProjetil;
    public GameObject teto;
    public GameController gameController;

    private Vector3 _posicaoInicialProjetil;
    private GameObject _proximoProjetil;
    private bool _pararRecarregamento;
    private bool _pararTiro;
    private static List<CoresBolinhas> _ultimasBolinhas = new List<CoresBolinhas>();


    private void Awake()
    {
        MiraController.Atirar += Atirado;
        BolaController.BolinhaFixada += RecarregarMira;
        GameController.FinalJogo += PararRegarga;
    }

    private void OnDestroy()
    {
        MiraController.Atirar -= Atirado;
        BolaController.BolinhaFixada -= RecarregarMira;
        GameController.FinalJogo -= PararRegarga;
    }

    private void Start()
    {
        StartCoroutine(CarregarPrimeiraBola());
    }

    private void RecarregarMira()
    {
        if (_pararRecarregamento || _proximoProjetil == null)
        {
            return;
        }

        atualProjetil = _proximoProjetil;
        _proximoProjetil = null;
        atualProjetil.transform.position = mira.transform.position;

        mira.AtualProjetil = atualProjetil;

        _pararTiro = false;
        mira.Atirando = false;
        StartCoroutine(CarregarProximaBolha());
    }

    private void PararRegarga(bool ehVitoria)
    {
        _pararRecarregamento = true;
    }

    private void Atirado()
    {
        if (_pararTiro)
        {
            return;
        }

        atualProjetil.GetComponent<CircleCollider2D>().enabled = true;
        atualProjetil.transform.parent = teto.transform;
        BolaController bc;
        bc = atualProjetil.GetComponent<BolaController>();
        bc.Atirado = true;
        _pararTiro = true;
    }

    private IEnumerator CarregarProximaBolha()
    {
        yield return new WaitForSeconds(0.2f);

        BolaController bc;

        _proximoProjetil = Instantiate(bolaClone, _posicaoInicialProjetil, Quaternion.identity);
        _proximoProjetil.GetComponent<CircleCollider2D>().enabled = false;
        bc = _proximoProjetil.GetComponent<BolaController>();
        bc.SetCor(ProximaCor());
        _proximoProjetil.SetActive(true);
    }

    private CoresBolinhas ProximaCor()
    {
        CoresBolinhas proxima;
        while (true)
        {
            Dictionary<string, BolaController> bolasDoJogo = gameController.BolasNoJogo; //bolinhas do jogo 
            BolaController[] bolas = bolasDoJogo.Values.ToArray();

            if (bolas.Length > 0)
            {
                int rnd = Random.Range(0, bolasDoJogo.Count);

                proxima = bolas[rnd].cor;

                break;
            }


            //caso seja necessario ter uma logica onde nao deixa repetir mais de x vezes a mesma cor 
            // int countRepeticao = 0;
            // if (_ultimasBolinhas.Count == 2)
            // {
            //     foreach (CoresBolinhas bola in _ultimasBolinhas)
            //     {
            //         if (bola == proxima)
            //         {
            //             countRepeticao++;
            //         }
            //     }
            //
            //     if (countRepeticao < 1)
            //     {
            //         _ultimasBolinhas.RemoveAt(0);
            //         break;
            //     }
            // }
            // else
            // {
            //     break;
            // }
        }


        _ultimasBolinhas.Add(proxima);

        return proxima;
    }

    private IEnumerator CarregarPrimeiraBola()
    {
        yield return new WaitForSeconds(1);

        _posicaoInicialProjetil = atualProjetil.transform.position;
        _posicaoInicialProjetil.x += 0.1f;

        BolaController bc;

        _proximoProjetil = Instantiate(bolaClone, _posicaoInicialProjetil, Quaternion.identity);
        _proximoProjetil.GetComponent<CircleCollider2D>().enabled = false;
        bc = _proximoProjetil.GetComponent<BolaController>();
        bc.SetCor(ProximaCor());
        _proximoProjetil.SetActive(true);

        RecarregarMira();
    }
}
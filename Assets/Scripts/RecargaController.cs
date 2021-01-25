using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecargaController : MonoBehaviour
{
    public MiraController mira;
    public FixedJoint2D joint;
    public GameObject bolaClone;
    public GameObject atualProjetil;
    public GameObject teto;

    private Vector3 _posicaoInicialProjetil;
    private GameObject _proximoProjetil;
    private bool _pararRecarregamento;
    private bool _pararTiro;
    private static List<CoresBolinhas> _ultimasBolinhas = new List<CoresBolinhas>();

    private void Awake()
    {
        MiraController.Atirar += Atirado;
        BolaController.BolinhaFixada += RecarregarMira;
        BolaController.LimiteBolinhasAlcancado += LimiteBolinhasAlcancado;

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

    private void RecarregarMira()
    {
        if (_pararRecarregamento)
        {
            return;
        }

        atualProjetil = _proximoProjetil;
        _proximoProjetil = null;
        atualProjetil.transform.position = mira.transform.position;

        mira.AtualProjetil = atualProjetil;
        
        _pararTiro = false;
        StartCoroutine(CarregarProximaBolha());

    }

    private void LimiteBolinhasAlcancado()
    {
        Debug.Log("Limite Alcançado");
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

    private static CoresBolinhas ProximaCor()
    {
        CoresBolinhas proxima;
        while (true)
        {
            int rnd = Random.Range(0, 4);

            proxima = (CoresBolinhas) rnd;

            if (_ultimasBolinhas.Count == 3)
            {
                int countRepeticao = 0;
                foreach (CoresBolinhas bola in _ultimasBolinhas)
                {
                    if (bola == proxima)
                    {
                        countRepeticao++;
                    }
                }

                if (countRepeticao < 2)
                {
                    _ultimasBolinhas.RemoveAt(0);
                    break;
                }
            }
            else
            {
                break;
            }
        }


        _ultimasBolinhas.Add(proxima);

        return proxima;
    }
}
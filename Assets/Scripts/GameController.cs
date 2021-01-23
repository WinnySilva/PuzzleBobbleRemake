﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{

    public Tilemap posicaoBolinhasTile;
    public Rigidbody2D[] posicoesTeto;
    public TetoController controleTeto;

    [SerializeField]
    private Dictionary<int, BolaController> conj;
    private Dictionary<int, BolaController> bolasNoTeto;
    [SerializeField]
    private int contagemBolinhasDestruidas = 0;
    [SerializeField]
    private int contagemBolinhasDisparadas = 0;


    private int x, y;
    BolaController b;
    // Start is called before the first frame update
    void Start()
    {
        conj = new Dictionary<int, BolaController>();
        bolasNoTeto = new Dictionary<int, BolaController>();
        x = 0;
        y = 0;
    }

    public void RemoverBolinha(BolaController obj)
    {
        int key = hashPos(obj.x, obj.y);
        conj.Remove(key);
    }
    public void AdicionarBolinha(int x, int y, BolaController obj)
    {
        int coord = hashPos(x, y);
        this.x = x;
        this.y = y;
        if (obj.ColadoNoTeto)
        {
            bolasNoTeto.Add(coord, obj);
        }
        conj.Add(coord, obj);
        EncontrarMatches(obj);
        contagemBolinhasDisparadas++;
        if (contagemBolinhasDisparadas%8==0)
        {
            controleTeto.BaixarNivelTeto();
        }
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
            Debug.Log("Encontrou vizinhos: " + vizinhos.Count);
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
        Debug.Log(" iguais " + listaMatches.Count + " avaliados: " + avaliados.Count + " iterador " + matches.Count);
        if (listaMatches.Count > 2)
        {
            StartCoroutine(DestruirBolinhasMatches(listaMatches));
        }

    }
    
    public void DerrubarBolinhasSemEncontrarTeto()
    {
        List<BolaController> avaliados = new List<BolaController>();
        Queue<BolaController> matches = new Queue<BolaController>();

        foreach (BolaController bola in bolasNoTeto.Values)
        {
            matches.Enqueue(bola);
            avaliados.Add(bola);
        }

        while (matches.Count > 0)
        {
            BolaController m = matches.Dequeue();
            List<BolaController> vizinhos = BuscaVizinhos(m);
            Debug.Log("Encontrou vizinhos: " + vizinhos.Count);
            foreach (BolaController v in vizinhos)
            {
                if (!avaliados.Contains(v))
                {
                    avaliados.Add(v);
                    matches.Enqueue(v);
                }
            }
        }


        foreach (BolaController bola in conj.Values)
        {
            if (!avaliados.Contains(bola))
            {
                bola.Rg.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }

    public void SinalizaBolinhaDestruida()
    {
        contagemBolinhasDestruidas++;
    }

    private void DestruirBolinhas(List<BolaController> listaMatches)
    {
        int coord;
        foreach (BolaController b in listaMatches)
        {
            coord = hashPos(b.x, b.y);
            conj.Remove(coord);
            if (b.ColadoNoTeto)
            {
                bolasNoTeto.Remove(coord);
            }
            Destroy(b.gameObject);
        }
    }
    private int hashPos(int x, int y)
    {
        return x * 5000 + y * 500;
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
        GUI.Label(new Rect(10, 100, 100, 20), $"Em jogo:: {conj.Count}");
        GUI.Label(new Rect(10, 120, 100, 20), $":: {x},{y}");

        /* if (b != null)
         {
             List<BolaController> vs = BuscaVizinhos(b);
             int ypos = 55;
             foreach (BolaController b in vs)
             {
                 ypos += 30;
                 GUI.Label(new Rect(10, ypos, 100, 20), $"{b.cor}");
             }

         }*/

    }

    private List<BolaController> BuscaVizinhos(BolaController val)
    {
        bool par = val.y % 2 == 0;
        int xEsquerda, xDireita;
        int auxKey;
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
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }

        //esquerda inferior
        auxKey = hashPos(xEsquerda, val.y - 1);
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }

        //direita topo
        auxKey = hashPos(xDireita, val.y + 1);
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }
        //direita inferior
        auxKey = hashPos(xDireita, val.y - 1);
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }

        //esquerda
        auxKey = hashPos((val.x - 1), val.y);
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }

        //direita
        auxKey = hashPos(val.x + 1, val.y);
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }

        return vizinhos;
    }

    IEnumerator DestruirBolinhasMatches(List<BolaController> bolinhasParaDestruir)
    {
        yield return new WaitForSeconds(0.15f);

        DestruirBolinhas(bolinhasParaDestruir);
        DerrubarBolinhasSemEncontrarTeto();
    }

}

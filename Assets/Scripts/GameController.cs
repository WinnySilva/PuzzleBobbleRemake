using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Dictionary<int, BolaController> conj;

    // Start is called before the first frame update
    void Start()
    {
        conj = new Dictionary<int, BolaController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AdicionarBolinha(int x, int y, BolaController obj)
    {
        int coord = hashPos(x, y);
        conj.Add(coord, obj);
        EncontrarMatches(obj);
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
            List<BolaController> vizinhos = this.BuscaVizinhos(m);
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
            DestruirBolinhas(listaMatches);
        }

    }

    private void DestruirBolinhas(List<BolaController> listaMatches)
    {
        int coord;
        foreach (BolaController b in listaMatches)
        {
            coord = hashPos(b.x, b.y);
            conj.Remove(coord);
            Destroy(b.gameObject);
        }
    }
    private int hashPos(int x, int y)
    {
        return x * 500 + y;
    }


    private List<BolaController> BuscaVizinhos(BolaController val)
    {
        int auxKey = 0;
        List<BolaController> vizinhos = new List<BolaController>();
        BolaController auxVal = null;

        //x-1
        auxKey = hashPos((val.x - 1), val.y);
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }
        auxKey = hashPos(val.x - 1, val.y - 1); // (val.x - 1) * 10 + (val.y - 1);
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }

        // x
        auxKey = hashPos(val.x, val.y - 1);//auxKey = (val.x) * 10 + (val.y - 1);
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }
        auxKey = hashPos(val.x, val.y + 1); // (val.x) + (val.y + 1);
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }
        //x+1
        auxKey = hashPos(val.x + 1, val.y - 1);
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }
        auxKey = hashPos(val.x + 1, val.y);
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }
        auxKey = hashPos(val.x + 1, val.y + 1);
        if (conj.TryGetValue(auxKey, out auxVal))
        {
            vizinhos.Add(auxVal);
        }
        return vizinhos;
    }

}

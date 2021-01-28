using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GerenciadorFase : MonoBehaviour
{
    public GameObject prefab;
    public Tilemap gridBolinhas;
    public GameObject alturaDerrota;
    public GameController controleJogo;
    public GameObject teto;

    private void Start()
    {
        ConfiguraFaseUm();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ConfiguraFaseUm()
    {
        Vector3Int[] posicoesVermelho = {new Vector3Int(-4,6,0),new Vector3Int(-3,6,0),
        new Vector3Int(-4,5,0),new Vector3Int(-3,5,0)   };

        posicionarBolinhas(posicoesVermelho, prefab, CoresBolinhas.VERMELHO);

        Vector3Int[] posicoesAmarelo = {new Vector3Int(-2,6,0),new Vector3Int(-1,6,0),
        new Vector3Int(-2,5,0),new Vector3Int(-1,5,0)   };

        posicionarBolinhas(posicoesAmarelo, prefab, CoresBolinhas.AMARELO);

        Vector3Int[] posicoesAzul = {new Vector3Int(0,6,0),new Vector3Int(1,6,0),
        new Vector3Int(0,5,0),new Vector3Int(1,5,0)   };

        posicionarBolinhas(posicoesAzul, prefab, CoresBolinhas.AZUL);

        Vector3Int[] verde = {new Vector3Int(2,6,0),new Vector3Int(3,6,0),
        new Vector3Int(2,5,0)};

       // posicionarBolinhas(verde, prefab, CoresBolinhas.VERDE);

    }

    private void posicionarBolinhas(Vector3Int[] posicoes, GameObject prefab, CoresBolinhas cor)
    {
        Vector3 vec3;
        Rigidbody2D rg;
        BolaController bc;

        foreach (Vector3Int v in posicoes)
        {
            vec3 = gridBolinhas.CellToWorld(v);


            GameObject novaBolinha = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            novaBolinha.transform.position = vec3;
            novaBolinha.transform.parent = teto.transform;

            bc = novaBolinha.GetComponent<BolaController>();
            bc.alturaDerrota = alturaDerrota;
            bc.controleJogo = controleJogo;
            bc.posicaoBolinhasTile = gridBolinhas;
            bc.SetCor(cor);           
            bc.x = v.x;
            bc.y = v.y;
            bc.Atirado = true;
            bc.Fixado = true;

            Tile hex = ScriptableObject.CreateInstance<Tile>();
            hex.sprite = Resources.Load<Sprite>("Tilesets/Hexagon");
            gridBolinhas.SetTile(v, hex);


            rg = novaBolinha.GetComponent<Rigidbody2D>();
            rg.bodyType = RigidbodyType2D.Static;

            controleJogo.AdicionarBolinhaContrucaoFase(v.x, v.y, bc);
            novaBolinha.SetActive(true);

            Debug.Log(vec3);
            Debug.Log(v);
        }
    }

}

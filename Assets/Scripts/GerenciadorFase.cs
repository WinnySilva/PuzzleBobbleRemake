using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GerenciadorFase : MonoBehaviour
{

    public Vector3Int[] posicoesVermelho = {
        new Vector3Int(-4,6,0),new Vector3Int(-3,6,0),
        new Vector3Int(-4,5,0),new Vector3Int(-3,5,0),
        new Vector3Int(0,4,0),new Vector3Int(1,4,0),
         new Vector3Int(-1,3,0),new Vector3Int(0,3,0),};

    public Vector3Int[] posicoesAmarelo = {
            new Vector3Int(-2,6,0),new Vector3Int(-1,6,0),
        new Vector3Int(-2,5,0),new Vector3Int(-1,5,0),
        new Vector3Int(2,4,0),new Vector3Int(3,4,0),
         new Vector3Int(1,3,0),new Vector3Int(2,3,0),
        };

    public Vector3Int[] posicoesAzul = {
        new Vector3Int(0,6,0),new Vector3Int(1,6,0),
        new Vector3Int(0,5,0),new Vector3Int(1,5,0),
        new Vector3Int(-4,4,0),new Vector3Int(-3,4,0),
        new Vector3Int(-4,3,0) };

    public Vector3Int[] verde = {new Vector3Int(2,6,0),new Vector3Int(3,6,0),
        new Vector3Int(2,5,0),
         new Vector3Int(-2,4,0),new Vector3Int(-1,4,0),
         new Vector3Int(-3,3,0),new Vector3Int(-2,3,0),
        };

    public Vector3Int[] posicoesCinza = { };
    public Vector3Int[] posicoesRoxa = { };
    public Vector3Int[] posicoesBranca = { };

    public GameObject prefab;
    public Tilemap gridBolinhas;
    public GameObject alturaDerrota;
    public GameController controleJogo;
    public GameObject teto;

    private void Start()
    {
        ConfiguraFase();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ConfiguraFase()
    {
        PosicionarBolinhas(posicoesVermelho, prefab, CoresBolinhas.VERMELHO);
        PosicionarBolinhas(posicoesAmarelo, prefab, CoresBolinhas.AMARELO);
        PosicionarBolinhas(posicoesAzul, prefab, CoresBolinhas.AZUL);
        PosicionarBolinhas(verde, prefab, CoresBolinhas.VERDE);

        PosicionarBolinhas(posicoesCinza, prefab, CoresBolinhas.CINZA) ;
        PosicionarBolinhas(posicoesRoxa, prefab, CoresBolinhas.ROXO);
        PosicionarBolinhas(posicoesBranca, prefab, CoresBolinhas.BRANCO);        

}

    private void PosicionarBolinhas(Vector3Int[] posicoes, GameObject prefab, CoresBolinhas cor)
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

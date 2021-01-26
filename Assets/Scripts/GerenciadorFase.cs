using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GerenciadorFase : MonoBehaviour
{
    public GameObject bolinhaVermelha;
    public Tilemap gridBolinhas;
    public GameObject alturaDerrota;
    public GameController controleJogo;

    private Vector3Int[] posicoesVermelho = 
        {new Vector3Int(-5,8,0),new Vector3Int(-4,8,0),
        new Vector3Int(-5,7,0),new Vector3Int(-4,7,0),
    };

    // Start is called before the first frame update
    void Awake()
    {
        posicionarBolinhas(posicoesVermelho, bolinhaVermelha);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void posicionarBolinhas(Vector3Int[] posicoes, GameObject prefab)
    {
        Vector3 vec3;
        Rigidbody2D rg;
        BolaController bc;

        foreach (Vector3Int v in posicoes)
        {
            vec3 = gridBolinhas.CellToWorld(v);
          //  vec3.y += 0.1; 
            
            GameObject novaBolinha = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            
            bc = novaBolinha.GetComponent<BolaController>();
            bc.alturaDerrota = alturaDerrota;
            bc.controleJogo = controleJogo;
            bc.posicaoBolinhasTile = gridBolinhas;
            bc.Atirado = true;

            rg = novaBolinha.GetComponent<Rigidbody2D>();
            rg.bodyType = RigidbodyType2D.Dynamic;
            novaBolinha.SetActive(true);
            rg.MovePosition(vec3);

        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetoController : MonoBehaviour
{

    public float offsetBaixar = 0.35f;
    public GameObject posicaoBolinhas;
    public GameObject posicoesTeto;
    public Rigidbody2D offLimits;
    [SerializeField]
    private int nivelTeto = 0;
    private Rigidbody2D rg;

    // Start is called before the first frame update
    void Start()
    {
        rg = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BaixarNivelTeto()
    {
        nivelTeto--;

        Vector3 pos = transform.position;
        pos.y -= offsetBaixar;
      //  transform.position = pos;

        rg.MovePosition(pos);

        pos = posicaoBolinhas.transform.position;
        pos.y -= offsetBaixar;
        posicaoBolinhas.transform.position = pos;

        pos = posicoesTeto.transform.position;
        pos.y -= offsetBaixar;
        posicoesTeto.transform.position = pos;


        pos = offLimits.position;
        pos.y -= offsetBaixar;
        offLimits.MovePosition(pos);

    }

}

using System.Collections.Generic;
using UnityEngine;

public class TetoController : MonoBehaviour
{

    public float offsetBaixar = 1;
    public GameObject posicaoBolinhas;
    public GameObject posicoesTeto;
    public Rigidbody2D offLimits;
    [SerializeField]
    private int nivelTeto = 0;
    private Rigidbody2D rg;
    
    public delegate void EventoTetoAbaixou(float offsetAbaixar);

    public static event EventoTetoAbaixou AcaoTetoAbaixou;

    // Start is called before the first frame update
    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
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

        pos = offLimits.position;
        pos.y -= offsetBaixar;
        offLimits.MovePosition(pos);

        AcaoTetoAbaixou?.Invoke(offsetBaixar);

    }

}

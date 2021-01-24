using UnityEngine;

public class TetoController : MonoBehaviour
{

    public float offsetBaixar = 1;
    public GameObject posicaoBolinhas;
    public Rigidbody2D foraDoLimite;
    [SerializeField]
    private Rigidbody2D rg;
    
    public delegate void EventoTetoAbaixou(float offsetAbaixar);

    public static event EventoTetoAbaixou AcaoTetoAbaixou;

    // Start is called before the first frame update
    private void Start()
    {
        rg = GetComponent<Rigidbody2D>();
    }


    public void BaixarNivelTeto()
    {

        Vector3 pos = transform.position;
        pos.y -= offsetBaixar;

        rg.MovePosition(pos);

        pos = posicaoBolinhas.transform.position;
        pos.y -= offsetBaixar;
        posicaoBolinhas.transform.position = pos;

        pos = foraDoLimite.position;
        pos.y -= offsetBaixar;
        foraDoLimite.MovePosition(pos);

        AcaoTetoAbaixou?.Invoke(offsetBaixar);

    }

}

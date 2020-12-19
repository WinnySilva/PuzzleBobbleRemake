using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RecargaController : MonoBehaviour
{
    public MiraController mira;
    public Rigidbody2D rg;
    public FixedJoint2D joint;
    public GameObject atualProjetil;
    public GameObject clonavel;
    public float breakForce = 10;

    private Vector3 posicaoInicialProjetil;

    private void Awake()
    {
        MiraController.Fired += RecarregarMira;
        posicaoInicialProjetil = atualProjetil.transform.position;
        RecarregarMira();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void RecarregarMira()
    {
        atualProjetil = Instantiate(clonavel, posicaoInicialProjetil, Quaternion.identity);

        joint = atualProjetil.AddComponent<FixedJoint2D>();
        joint.connectedBody = mira.gameObject.GetComponent<Rigidbody2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = new Vector2(0, 0);

        mira.AtualProjetil = atualProjetil;

        atualProjetil.transform.position = posicaoInicialProjetil;

        StartCoroutine(this.RecarregarCoroutine());
    }

    IEnumerator RecarregarCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        atualProjetil.SetActive(true);
    }
}

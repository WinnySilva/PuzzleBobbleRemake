using System.Collections;
using UnityEngine;

public class RecargaController : MonoBehaviour
{
    public MiraController mira;
    public FixedJoint2D joint;
    public GameObject bolaClone;
    public GameObject atualProjetil;
    public GameObject teto;

    private Vector3 _posicaoInicialProjetil;
    private bool stopRecarregamento = false;
    private bool stopTiro = false;
    private void Awake()
    {
        MiraController.Fired += Fired;
        BolaController.BolinhaFixada  += RecarregarMira;
        BolaController.LimiteBolinhasAlcancado += LimiteBolinhasAlcancado;

        _posicaoInicialProjetil = atualProjetil.transform.position;
        RecarregarMira();
    }

    void RecarregarMira()
    {
        if (stopRecarregamento)
        {
            return;
        }
        BolaController bc;
       
        atualProjetil = Instantiate(bolaClone, _posicaoInicialProjetil, Quaternion.identity);
        atualProjetil.GetComponent<CircleCollider2D>().enabled = false;
        bc = atualProjetil.GetComponent<BolaController>();       
        bc.setColor(proximaCor());

        joint = atualProjetil.AddComponent<FixedJoint2D>();
        joint.connectedBody = mira.gameObject.GetComponent<Rigidbody2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = new Vector2(0, 0);

        mira.AtualProjetil = atualProjetil;

        atualProjetil.transform.position = _posicaoInicialProjetil;

        StartCoroutine(RecarregarCoroutine());
    }

    public void LimiteBolinhasAlcancado()
    {
        Debug.Log("Limite Alcançado");
        stopRecarregamento = true;
    }

    public void Fired()
    {
        if (stopTiro)
        {
            return;
        }
        atualProjetil.GetComponent<CircleCollider2D>().enabled = true;
        atualProjetil.transform.parent = teto.transform;
        BolaController bc;
        bc = atualProjetil.GetComponent<BolaController>();
        bc.Shooted = true;
        stopTiro = true;
    }

    IEnumerator RecarregarCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        atualProjetil.SetActive(true);
        stopTiro = false;
    }

    private CoresBolinhas proximaCor()
    {
       
        int rnd = Random.Range(0, 3);

        return (CoresBolinhas)rnd;
    }
}

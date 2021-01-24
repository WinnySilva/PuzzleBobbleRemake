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
    private bool _pararRecarregamento = false;
    private bool _pararTiro = false;
    
    private void Awake()
    {
        MiraController.Atirar += Atirado;
        BolaController.BolinhaFixada  += RecarregarMira;
        BolaController.LimiteBolinhasAlcancado += LimiteBolinhasAlcancado;

        _posicaoInicialProjetil = atualProjetil.transform.position;
        RecarregarMira();
    }

    private void RecarregarMira()
    {
        if (_pararRecarregamento)
        {
            return;
        }
        BolaController bc;
       
        atualProjetil = Instantiate(bolaClone, _posicaoInicialProjetil, Quaternion.identity);
        atualProjetil.GetComponent<CircleCollider2D>().enabled = false;
        bc = atualProjetil.GetComponent<BolaController>();       
        bc.SetCor(ProximaCor());

        joint = atualProjetil.AddComponent<FixedJoint2D>();
        joint.connectedBody = mira.gameObject.GetComponent<Rigidbody2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = new Vector2(0, 0);

        mira.AtualProjetil = atualProjetil;

        atualProjetil.transform.position = _posicaoInicialProjetil;

        StartCoroutine(RecarregarCoroutine());
    }

    private void LimiteBolinhasAlcancado()
    {
        Debug.Log("Limite Alcançado");
        _pararRecarregamento = true;
    }

    private void Atirado()
    {
        if (_pararTiro)
        {
            return;
        }
        atualProjetil.GetComponent<CircleCollider2D>().enabled = true;
        atualProjetil.transform.parent = teto.transform;
        BolaController bc;
        bc = atualProjetil.GetComponent<BolaController>();
        bc.Atirado = true;
        _pararTiro = true;
    }

    private IEnumerator RecarregarCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        atualProjetil.SetActive(true);
        _pararTiro = false;
    }

    // TODO nao deixar mais de 3 vezes a mesma bolinha, e aumentar as cores
    private static CoresBolinhas ProximaCor()
    {
       
        int rnd = Random.Range(0, 3);

        return (CoresBolinhas)rnd;
    }
}

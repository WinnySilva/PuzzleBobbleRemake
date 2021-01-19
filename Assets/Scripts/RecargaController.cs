using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

public class RecargaController : MonoBehaviour
{
    public MiraController mira;
    public FixedJoint2D joint;
    public GameObject bolaClone;
    public GameObject atualProjetil;

    private Vector3 _posicaoInicialProjetil;
    private bool stopRecarregamento = false;
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
        atualProjetil.GetComponent<CircleCollider2D>().enabled = true;
        BolaController bc;
        bc = atualProjetil.GetComponent<BolaController>();
        bc.Shooted = true;
    }

    IEnumerator RecarregarCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        atualProjetil.SetActive(true);
    }

    private CoresBolinhas proximaCor()
    {
       
        int rnd = Random.Range(0, 3);

        return (CoresBolinhas)rnd;
    }
}

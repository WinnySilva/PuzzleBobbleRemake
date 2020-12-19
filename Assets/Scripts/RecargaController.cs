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

    private void Awake()
    {
        MiraController.Fired += RecarregarMira;
        _posicaoInicialProjetil = atualProjetil.transform.position;
        RecarregarMira();
    }

    void RecarregarMira()
    {
        atualProjetil = Instantiate(bolaClone, _posicaoInicialProjetil, Quaternion.identity);

        joint = atualProjetil.AddComponent<FixedJoint2D>();
        joint.connectedBody = mira.gameObject.GetComponent<Rigidbody2D>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = new Vector2(0, 0);

        mira.AtualProjetil = atualProjetil;

        atualProjetil.transform.position = _posicaoInicialProjetil;

        StartCoroutine(RecarregarCoroutine());
    }

    IEnumerator RecarregarCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        atualProjetil.SetActive(true);
    }
}

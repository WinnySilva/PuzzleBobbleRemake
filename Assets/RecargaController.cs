using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RecargaController : MonoBehaviour
{
    public MiraController mira;
    public GameObject atualProjetil;
    public GameObject clonavel;

    private Vector3 posicaoInicialProjetil;
    // Start is called before the first frame update
    void Start()
    {
        MiraController.Fired += RecarregarMira;
        posicaoInicialProjetil = atualProjetil.transform.position;
        RecarregarMira();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RecarregarMira()
    {
        StartCoroutine(this.RecarregarCoroutine());
    }

    IEnumerator RecarregarCoroutine()
    {
        yield return new WaitForSeconds(2);
        atualProjetil = Instantiate(clonavel, posicaoInicialProjetil, Quaternion.identity);
        mira.AddNovoProjetil(atualProjetil.GetComponent<Rigidbody2D>());
        atualProjetil.SetActive(true);
        atualProjetil.transform.position = posicaoInicialProjetil;
    }
}

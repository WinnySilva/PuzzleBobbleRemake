using System.Collections;
using Audio;
using Enums;
using UnityEngine;

public class MiraController : MonoBehaviour
{
    public int forcaImpulso = 20;
    public AtiradorController seta;

    public delegate void AcaoAtirar();
    public static event AcaoAtirar Atirar;

    protected internal GameObject AtualProjetil { get; set; }

    private bool _tiroParado;
    private GerenciadorDeSom _gerenciadorDeSom;
    private bool _atirando;

    public bool Atirando
    {
        set => _atirando = value;
    }


    private void Awake()
    {
        GameController.FinalJogo += PararTiros;
        _gerenciadorDeSom = FindObjectOfType<GerenciadorDeSom>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && !_tiroParado && !_atirando)
        {
            
            _atirando = true;
            
            Rigidbody2D rg = AtualProjetil.GetComponent<Rigidbody2D>();
            Joint2D joint = AtualProjetil.GetComponent<Joint2D>();
            
            
            Vector3 vec = seta.transform.up * forcaImpulso;

            Destroy(joint);
           
            rg.AddForce(vec, ForceMode2D.Impulse);
            _gerenciadorDeSom.Play(ConstantesDeAudio.APOS_TIRO);
            Atirar?.Invoke();
        }
    }

    private IEnumerator TocarSomDoTiro()
    {
        yield return new WaitForSeconds(0.005f);
        
    }

    void PararTiros(bool ehVitoria)
    {
        _tiroParado = true;
    }

}

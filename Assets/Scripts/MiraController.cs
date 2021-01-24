using UnityEngine;

public class MiraController : MonoBehaviour
{
    public int forcaImpulso = 20;
    public AtiradorController seta;

    public delegate void AcaoAtirar();
    public static event AcaoAtirar Atirar;

    protected internal GameObject AtualProjetil { get; set; }


    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Rigidbody2D rg = AtualProjetil.GetComponent<Rigidbody2D>();
            Joint2D joint = AtualProjetil.GetComponent<Joint2D>();
            
            
            Vector3 vec = seta.transform.up * forcaImpulso;

            Destroy(joint);
           
            rg.AddForce(vec, ForceMode2D.Impulse);
            Atirar?.Invoke();
        }
    }

}

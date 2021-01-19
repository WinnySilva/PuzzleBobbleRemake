using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiraController : MonoBehaviour
{
    public int forcaImpulso = 20;
    public AtiradorController seta;
    private FixedJoint2D joint;
    // private GameObject _atualProjetil;


    public delegate void FireAction();
    public static event FireAction Fired;

    protected internal GameObject AtualProjetil { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        joint = GetComponent<FixedJoint2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {

            //Debug.Log("fire");
            Rigidbody2D rg = AtualProjetil.GetComponent<Rigidbody2D>();
            Joint2D joint = AtualProjetil.GetComponent<Joint2D>();

            float graus = Mathf.Abs(rg.rotation);
            graus = graus / 85;

            Vector3 vec = seta.transform.up * forcaImpulso;

            Destroy(joint);
            //Debug.Log(rg.transform.up + " :: " + vec);
            rg.AddForce(vec, ForceMode2D.Impulse);
            if (Fired != null)
            {
                Fired();
            }

        }
    }

}

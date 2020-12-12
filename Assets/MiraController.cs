using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiraController : MonoBehaviour
{
    public int forcaImpulso = 20;

    private FixedJoint2D joint;
    public delegate void FireAction();
    public static event FireAction Fired;
    // Start is called before the first frame update
    void Start()
    {
        joint = this.GetComponent<FixedJoint2D>();
 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (TryGetComponent<FixedJoint2D>(out joint))
            {
                Debug.Log("fire");
                Rigidbody2D rg = joint.connectedBody;
                Destroy(joint);

                float graus = Mathf.Abs(rg.rotation);

                graus = graus / 85;

                rg.AddForce(new Vector3(graus * forcaImpulso, (1 - graus) * forcaImpulso), ForceMode2D.Impulse);

                Fired();

            }
        }
    }

    public void AddNovoProjetil(Rigidbody2D bolinha)
    {
        joint = this.gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = bolinha; 
    }

}

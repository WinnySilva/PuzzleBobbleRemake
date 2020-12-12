using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtiradorController : MonoBehaviour
{
    public float speed = 5;
    private float velocidade;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {



    }

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float graus = 85 * (horizontalInput );
               
        Quaternion rot = Quaternion.Euler(0, 0, -graus);
        
        this.transform.rotation = rot;

    }


}

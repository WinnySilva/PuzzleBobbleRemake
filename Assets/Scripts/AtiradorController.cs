using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtiradorController : MonoBehaviour
{
    public float speed = 5;

    private float velocidade;

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        float graus = 85 * (horizontalInput);

        Quaternion rot = Quaternion.Euler(0, 0, - graus);

        transform.rotation = rot;
    }
}
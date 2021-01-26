using UnityEngine;

public class AtiradorController : MonoBehaviour
{
    public float velocidade;

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        float graus = velocidade * (horizontalInput);

        Quaternion rot = Quaternion.Euler(0, 0, - graus);

        transform.rotation = rot;
    }
}
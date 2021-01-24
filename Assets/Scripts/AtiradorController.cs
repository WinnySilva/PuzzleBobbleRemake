using UnityEngine;

public class AtiradorController : MonoBehaviour
{
    private float _velocidade;

    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        float graus = 85 * (horizontalInput);

        Quaternion rot = Quaternion.Euler(0, 0, - graus);

        transform.rotation = rot;
    }
}
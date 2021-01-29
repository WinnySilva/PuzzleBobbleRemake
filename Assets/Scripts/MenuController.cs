using Audio;
using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    private GerenciadorDeSom _gerenciadorDeSom;
    private void Start()
    {
        _gerenciadorDeSom = FindObjectOfType<GerenciadorDeSom>();

        _gerenciadorDeSom.Play(ConstantesDeAudio.TITLE);
    }
    void Update()
    {
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("fase1", LoadSceneMode.Single); //trocar de cena
        }
    }
}

using System;
using UnityEngine;

namespace Audio
{
    public class GerenciadorDeSom : MonoBehaviour
    {
        public Som[] sons;

        private void Awake()
        {
            foreach (Som som in sons)
            {
                som.origem = gameObject.AddComponent<AudioSource>();
                som.origem.clip = som.clip;
                som.origem.loop = som.loop;
            }
        }
        
        public void Play(string nome)
        {
            Som somParaTocar = Array.Find(sons, som => som.nome == nome);
            if (somParaTocar == null)
            {
                Debug.LogWarning($"Som selecionado nao encontrado: {nome}");
                return;
            }
            
            somParaTocar.origem.Play();
        }
    }
}
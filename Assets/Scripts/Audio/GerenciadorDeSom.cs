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
        
        public AudioSource Play(string nome)
        {
            Som somParaTocar = Array.Find(sons, som => som.nome == nome);
            if (somParaTocar == null)
            {
                Debug.LogWarning($"Som selecionado nao encontrado: {nome}");
                return null;
            }
            
            somParaTocar.origem.Play();
            return somParaTocar.origem;
        }

        public AudioSource Stop(string nome)
        {
            Som somParaTocar = Array.Find(sons, som => som.nome == nome);
            if (somParaTocar == null)
            {
                Debug.LogWarning($"Som selecionado nao encontrado: {nome}");
                return null;
            }

            somParaTocar.origem.Stop();
            return somParaTocar.origem;
        }

    }
}
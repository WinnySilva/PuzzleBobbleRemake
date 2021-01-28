using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class Som
    {

        public string nome;
        public AudioClip clip;
        public bool loop;

        [HideInInspector]
        public AudioSource origem;

    }
}
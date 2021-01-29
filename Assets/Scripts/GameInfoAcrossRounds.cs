using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfoAcrossRounds : MonoBehaviour
{

    [SerializeField]
    private double _pontuacao;
    public double Pontuacao
    {
        get => _pontuacao;
        set => _pontuacao += value;
    }

    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("gameInfo").Length  == 0)
        {
            DontDestroyOnLoad(this.gameObject);
            _pontuacao = 0;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameController controleJogo;
    public Text pontuacaoUIText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AtualizaPontuacao();
    }

    void AtualizaPontuacao()
    {
        int qntBolinhas = controleJogo.ObterQtdBolinhasDestruidas();

        int pontuacao = qntBolinhas * 10;

        string pont = pontuacao.ToString().PadLeft(8,'0');

        pontuacaoUIText.text = pont;
    }

}

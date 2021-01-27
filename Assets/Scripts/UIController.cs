using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameController controleJogo;
    public Text pontuacaoUIText;


    private void Awake()
    {
        GameController.FinalJogo += FinalJogo;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!(pontuacaoUIText is null))
        {
            AtualizaPontuacao();
        }
    }

    void AtualizaPontuacao()
    {
        int qntBolinhas = controleJogo.ObterQtdBolinhasDestruidas();

        int pontuacao = qntBolinhas * 10;

        string pont = pontuacao.ToString().PadLeft(8, '0');

        pontuacaoUIText.text = pont;
    }

    private void FinalJogo(bool ehVitoria)
    {
        if (ehVitoria)
        {
            Debug.Log("VITOORIA");
        }
    }
}
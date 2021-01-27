using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameController controleJogo;
    public Text pontuacaoUIText;

    public GameObject roundClearMsg;
    public GameObject gameOverMsg;
    public GameObject inicioRoundMsg;


    private void Awake()
    {
        GameController.FinalJogo += FinalJogo;
        roundClearMsg.SetActive(false);
        gameOverMsg.SetActive(false);
        inicioRoundMsg.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(AparecerMensagemInicial());
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
            roundClearMsg.SetActive(true);
        }
        else
        {
            gameOverMsg.SetActive(true);
        }
    }

    IEnumerator AparecerMensagemInicial()
    {
        inicioRoundMsg.SetActive(true);
        yield return new WaitForSeconds(2);
        inicioRoundMsg.SetActive(false);
    }
}
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameController controleJogo;
    public TextMeshProUGUI pontuacaoUIText;

    public TextMeshProUGUI pontuacaoFinalUIText;
    public TextMeshProUGUI segundosTotaisUIText;
    public TextMeshProUGUI roundClearMsg;

    public GameObject gameOverMsg;
    public GameObject inicioRoundMsg;
    public String proximaCena;
    private GameInfoAcrossRounds gameInfo;

    private void Awake()
    {
        GameController.FinalJogo += FinalJogo;
        gameOverMsg.SetActive(false);
        inicioRoundMsg.SetActive(false);
        pontuacaoFinalUIText.gameObject.SetActive(false);
        segundosTotaisUIText.gameObject.SetActive(false);
        roundClearMsg.gameObject.SetActive(false);
        
    }

    private void Start()
    {
        StartCoroutine(AparecerMensagemInicial());
        gameInfo = GameObject.FindObjectOfType<GameInfoAcrossRounds>();
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

        double pontuacao = this.gameInfo.Pontuacao;

        string pont = Mathf.RoundToInt((float)pontuacao).ToString().PadLeft(8, '0');

        pontuacaoUIText.text = pont;
    }

    private void FinalJogo(bool ehVitoria)
    {
        if (ehVitoria)
        {
            StartCoroutine(Vitoria());
            
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
        this.controleJogo.HoraInicio = DateTime.Now;
    }

    IEnumerator Vitoria()
    {       
        pontuacaoFinalUIText.text = Mathf.RoundToInt((float)controleJogo.PontuacaoBonus).ToString()+" PTS" ;
        segundosTotaisUIText.text = Mathf.RoundToInt((float)controleJogo.TempoJogo).ToString().PadLeft(2, '0') + " SEC";

        pontuacaoFinalUIText.gameObject.SetActive(true);
        segundosTotaisUIText.gameObject.SetActive(true);
        roundClearMsg.gameObject.SetActive(false);
        yield return new WaitForSeconds(3);

        pontuacaoFinalUIText.gameObject.SetActive(false);
        segundosTotaisUIText.gameObject.SetActive(false);
        roundClearMsg.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);
        pontuacaoFinalUIText.gameObject.SetActive(false);
        segundosTotaisUIText.gameObject.SetActive(false);
        roundClearMsg.gameObject.SetActive(false);
        if (!String.IsNullOrEmpty(proximaCena))
        {
            SceneManager.LoadScene(this.proximaCena, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("MenuInicial", LoadSceneMode.Single);
        }

    }


}
using Audio;
using Enums;
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
    private GerenciadorDeSom _gerenciadorDeSom;
    private bool foiDerrotado;

    private void Awake()
    {
        GameController.FinalJogo += FinalJogo;
        gameOverMsg.SetActive(false);
        inicioRoundMsg.SetActive(false);
        pontuacaoFinalUIText.gameObject.SetActive(false);
        segundosTotaisUIText.gameObject.SetActive(false);
        roundClearMsg.gameObject.SetActive(false);
        foiDerrotado = false;

    }

    private void OnDestroy()
    {
        GameController.FinalJogo -= FinalJogo;
    }

    private void Start()
    {
        _gerenciadorDeSom = FindObjectOfType<GerenciadorDeSom>();
        gameInfo = GameObject.FindObjectOfType<GameInfoAcrossRounds>();
        StartCoroutine(AparecerMensagemInicial());
    }

    // Update is called once per frame
    private void Update()
    {
        if (!(pontuacaoUIText is null))
        {
            AtualizaPontuacao();
        }

        if (foiDerrotado && Input.anyKeyDown)
        {
            SceneManager.LoadScene("MenuInicial", LoadSceneMode.Single); //trocar de cena
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
        _gerenciadorDeSom.Stop(ConstantesDeAudio.MUSICA_FASE);
        if (ehVitoria)
        {
            _gerenciadorDeSom.Play(ConstantesDeAudio.STAGE_CLEAR);
            StartCoroutine(Vitoria());
        }
        else
        {
            foiDerrotado = true;
            _gerenciadorDeSom.Play(ConstantesDeAudio.DERROTA_SOM);
            _gerenciadorDeSom.Play(ConstantesDeAudio.GAME_OVER);
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
        String ptBonuts = "NO BONUS";
        if (controleJogo.PontuacaoBonus > 0)
        {
            ptBonuts = Mathf.RoundToInt((float)controleJogo.PontuacaoBonus).ToString() + " PTS";
        }

        pontuacaoFinalUIText.text = ptBonuts;
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
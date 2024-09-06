using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Config")]
    [SerializeField] private Image vidaPlayer;
    [SerializeField] private TextMeshProUGUI vidaTMP;


    [SerializeField] private GameObject panelStats;
    [SerializeField] private GameObject panelInventario;
    [SerializeField] private GameObject panelCodigoPergamino;

    private float vidaActual;
    private float vidaMax;

    private float ManaActual;
    private float manaMax;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
       
    }

    void Update()
    {
        ActualizarUIPersonaje();
    }

    private void ActualizarUIPersonaje()
    {
        vidaPlayer.fillAmount = Mathf.Lerp(vidaPlayer.fillAmount,
            vidaActual / vidaMax, 10f * Time.deltaTime);
        
        vidaTMP.text = $"{vidaActual}/{vidaMax}";
    }

    public void ActualizarVidaPersonaje (float pVidaActual, float pVidaMax)
    {
        vidaActual = pVidaActual;
        vidaMax = pVidaMax;
    }

    public void ActualizarManaPersonaje (float pManaActual, float pmanaMax)
    {
        ManaActual = pManaActual;
        manaMax = pmanaMax;
    }

    #region Paneles
    
    public void AbriCerrarPanelStats()
    {
        panelStats.SetActive(!panelStats.activeSelf );

    }

    public void AbrirCerrarPanelInventario()
    {
        panelInventario.SetActive(!panelInventario.activeSelf);
    }


    public void AbrirCerrarPanelCodigo()
    {
        panelCodigoPergamino.SetActive(!panelCodigoPergamino.activeSelf);
    }
    #endregion

}

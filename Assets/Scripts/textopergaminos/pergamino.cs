using System.Collections;
using UnityEngine;
using TMPro;

public class pergamino : MonoBehaviour
{
    public GameObject panelCodigoPergamino;
    public int codigo;
    public string texto;

    public bool estado;

    public TextMeshProUGUI descripcion;
    public bool activo;

    private void Awake()
    {
        activo = false;
    }

    public void iniciar(int v, string t)
    {
        codigo = v;
        texto = t;
    }

    public int getcodigo()
    {
        return codigo;
    }

    public void cargarventana()
    {
        if (!activo)
        {
            StartCoroutine(animar(texto));
        }
        panelCodigoPergamino.SetActive(true);
    }   

    private void Start()
    {
        ItemPorAgregar.OnPlayerEnter += OnPlayerEnterHandler; // Suscribirse al evento
    }

    private void OnDestroy()
    {
        ItemPorAgregar.OnPlayerEnter -= OnPlayerEnterHandler; // Desuscribirse del evento
    }

    private void OnPlayerEnterHandler()
    {
        // Acción a ejecutar cuando el jugador entra
        estado = true;
    }

    private IEnumerator animar(string texto)
    {
        activo = true;
        descripcion.text = "";
        char[] letras = texto.ToCharArray();
        for (int i = 0; i < letras.Length; i++)
        {
            descripcion.text += letras[i];
            yield return new WaitForSeconds(0.03f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Asegúrate de que el jugador tiene el tag "Player"
        {
            cargarventana();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Asegúrate de que el jugador tiene el tag "Player"
        {
            panelCodigoPergamino.SetActive(false);
            activo = false;
        }
    }
}
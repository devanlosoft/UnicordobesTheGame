using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Clase que representa un pergamino en el juego.
/// </summary>
public class pergamino : MonoBehaviour
{
    // Panel que muestra el código del pergamino.
    public GameObject panelCodigoPergamino;

    // Código único del pergamino.
    public int codigo;

    // Texto del pergamino.
    public string texto;

    // Estado del pergamino.
    public string estado;


    // Componente de texto para mostrar la descripción del pergamino.
    public TextMeshProUGUI descripcion;

    // Indica si el pergamino está activo.
    public bool activo;

    /// <summary>
    /// Método llamado al inicializar el objeto.
    /// </summary>
    private void Awake()
    {
        activo = false;
    }

    /// <summary>
    /// Inicializa el pergamino con un código y un texto.
    /// </summary>
    /// <param name="v">Código del pergamino.</param>
    /// <param name="t">Texto del pergamino.</param>
    public void iniciar(int v, string t, string e)
    {
        codigo = v;
        texto = t;
        estado = e;
    }

    /// <summary>
    /// Obtiene el código del pergamino.
    /// </summary>
    /// <returns>Código del pergamino.</returns>
    public int getcodigo()
    {
        return codigo;
    }

    /// <summary>
    /// Carga la ventana del pergamino y muestra el texto animado.
    /// </summary>
    public void cargarventana()
    {
        if (!activo)
        {
            StartCoroutine(animar(texto));
        }
        panelCodigoPergamino.SetActive(true);
    }

    /// <summary>
    /// Corrutina que anima el texto del pergamino letra por letra.
    /// </summary>
    /// <param name="texto">Texto a animar.</param>
    /// <returns>IEnumerator para la corrutina.</returns>
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

    /// <summary>
    /// Método llamado cuando otro objeto entra en el trigger del pergamino.
    /// </summary>
    /// <param name="collision">Información sobre la colisión.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Asegúrate de que el jugador tiene el tag "Player"
        {
            cargarventana();
        }
    }

    /// <summary>
    /// Método llamado cuando otro objeto sale del trigger del pergamino.
    /// </summary>
    /// <param name="collision">Información sobre la colisión.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Asegúrate de que el jugador tiene el tag "Player"
        {
            panelCodigoPergamino.SetActive(false);
            activo = false;
        }
    }
}
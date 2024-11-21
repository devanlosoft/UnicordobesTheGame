using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;

    private float typingTime = 0.05f;

    private PersonajeVida personajeVida;
    private conexion conexionScript;

    private bool isPlayerInRange;
    private bool didDialogueStart;
    private int lineIndex;

    private bool dialogoPanel = true;

    public Sprite ganador; // Arrastra tu Sprite aquí desde el Inspector
    private GameObject spriteObject;

    private string texto = "texto de prueba";
    private bool primeraInteraccion = false;

    private string lineaDialogoInteractiva = "no se ha definido";

    private int vidaJugadorAlIniciar = 50;
    private bool jugadorGano = false; // Variable para saber si ganó el jugador

    // Cambiar de escena por índice
    public void ChangeSceneByIndex(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Cambiando a la escena con índice {sceneIndex}...");
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"El índice de la escena {sceneIndex} está fuera de rango. Asegúrate de que la escena está en Build Settings.");
        }
    }

    void Start()
    {
        texto = DatosCompartidos.Enunciado;

        GameObject gameObjectConVidaBase = GameObject.Find("Player");
        if (gameObjectConVidaBase != null)
        {
            personajeVida = gameObjectConVidaBase.GetComponent<PersonajeVida>();
        }
        else
        {
            Debug.LogError("No se encontró el GameObject con el componente VidaBase.");
        }

        GameObject gameObjectConConexion = GameObject.Find("managerpergamino");
        if (gameObjectConConexion != null)
        {
            conexionScript = gameObjectConConexion.GetComponent<conexion>();
        }
        else
        {
            Debug.LogError("No se encontró el GameObject con el componente conexion.");
        }

        spriteObject = new GameObject("WinningSprite");
        SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = ganador;
        spriteObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetButtonDown("Fire1"))
        {
            if (!didDialogueStart)
            {
                StartDialogue();
            }
            else if (dialogueText.text == dialogueLines[lineIndex])
            {
                NextDialogueLine();
            }
            else
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[lineIndex];
            }
        }
    }

    private void StartDialogue()
    {
        didDialogueStart = true;
        dialoguePanel.SetActive(true);
        dialogueMark.SetActive(false);
        lineIndex = 0;
        Time.timeScale = 0f;
        StartCoroutine(ShowLine());

        if (!primeraInteraccion)
        {
            string[] newDialogueLines = {
                "Bienvenido al juego.",
                "Deberas caminar por el mapa para buscar los pergaminos que resuelvan el siguiente problema.",
                DatosCompartidos.Enunciado,
                "¡Buena suerte recuerda volver cuando creas haber encontrado la solucion!",
                "¡Si vuelves y no tienes los pergaminos correctos perderas vida!",
            };

            SetDialogueLines(newDialogueLines);

            if (conexionScript != null)
            {
                conexionScript.GenerarPergaminos();
            }
            else
            {
                Debug.LogError("conexionScript no está inicializado.");
            }

        }
        else
        {
            comprobarPergaminos();

            string[] newDialogueLines = {
                "Veo que volviste, dejame revisar tu solucion.",
                "Revisando...",
                lineaDialogoInteractiva
            };

            SetDialogueLines(newDialogueLines);
        }
    }

    private void NextDialogueLine()
    {
        lineIndex++;
        if (lineIndex < dialogueLines.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            didDialogueStart = false;
            dialoguePanel.SetActive(false);
            dialogueMark.SetActive(true);
            Time.timeScale = 1f;

            if (primeraInteraccion && !jugadorGano)
            {
                // Descontar vida si el jugador no ganó
                StartCoroutine(DescontarVidaDespuesDeDialogo());
            }

            if (!primeraInteraccion)
            {
                Debug.Log("Se ha terminado el diálogo");
                primeraInteraccion = true;
            }

            // Cambiar de escena si el jugador ganó
            if (jugadorGano)
            {
                Debug.Log("Final del diálogo. El jugador ganó. Cambiando de escena...");
                StartCoroutine(ChangeSceneAfterDelay(1f, 3)); // Cambia "3" por el índice correcto
            }

        }
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;
        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            dialogueMark.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialogueMark.SetActive(false);
        }
    }

    public void SetDialogueLines(string[] newLines)
    {
        dialogueLines = newLines;
    }

    public void pergaminosRecogidos()
    {
        Inventario inventario = Inventario.Instance;
        int puestosOcupados = 0;

        for (int i = 0; i < inventario.ItemsInventario.Length; i++)
        {
            if (inventario.ItemsInventario[i] != null)
            {
                puestosOcupados++;
            }
        }

        if (puestosOcupados == DatosCompartidos.NFragmentos)
        {
            int correctosCount = 0;
            int totalItems = 0;

            for (int i = 0; i < inventario.ItemsInventario.Length; i++)
            {
                if (inventario.ItemsInventario[i] != null)
                {
                    totalItems++;
                    var estado = inventario.ItemsInventario[i].Estado;

                    if (estado == "Correcto")
                    {
                        correctosCount++;
                    }
                }
            }

            if (correctosCount == totalItems && totalItems > 0)
            {
                lineaDialogoInteractiva = "¡Genial! Tu solución es la correcta.";
                jugadorGano = true; // Marca que el jugador ganó
                spriteObject.SetActive(true);
                Debug.Log("El jugador ha ganado. Esperando el fin del diálogo...");
            }
            else
            {
                Debug.Log("No todos los pergaminos son correctos.");
                lineaDialogoInteractiva = "No todos los pergaminos son correctos, perderás vida...";
            }
        }
        else
        {
            lineaDialogoInteractiva = "La solución que propones es incorrecta.";
        }
    }

    private IEnumerator DescontarVidaDespuesDeDialogo()
    {
        yield return new WaitForSeconds(0.5f); // Espera un poco para asegurarse de que el diálogo haya terminado
        descontarVida();
    }

    public void descontarVida()
    {
        vidaJugadorAlIniciar -= 25;
        if (personajeVida != null)
        {
            personajeVida.RecibirDaño(25);
        }
        else
        {
            Debug.LogError("personajeVida no está inicializado.");
        }
    }

    public void comprobarPergaminos()
    {
        Inventario inventario = Inventario.Instance;

        if (inventario == null || inventario.ItemsInventario == null || inventario.ItemsInventario.Length == 0)
        {
            lineaDialogoInteractiva = "Tu solución no es correcta, perderás vida...";
            StartCoroutine(DescontarVidaDespuesDeDialogo());
            return;
        }

        pergaminosRecogidos();
    }

    private IEnumerator ChangeSceneAfterDelay(float delay, int sceneIndex)
    {
        Debug.Log($"Esperando {delay} segundos antes de cambiar a la escena {sceneIndex}...");
        yield return new WaitForSecondsRealtime(delay);
        ChangeSceneByIndex(sceneIndex);
    }
}
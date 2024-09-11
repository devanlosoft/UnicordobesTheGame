using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private GameObject dialogueMark;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;

    private float typingTime = 0.05f;

    private bool isPlayerInRange;
    private bool didDialogueStart;
    private int lineIndex;
    
    

    public string texto = "texto de prueba";
    public bool primeraInteraccion;
  


    // Ejemplo de cómo cambiar las líneas de diálogo desde el propio script
    void Start()
    {
        texto = DatosCompartidos.Enunciado;
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

        if (primeraInteraccion == false)
        {
            // Define nuevas líneas de diálogo aquí
            string[] newDialogueLines = {
            "Bienvenido al juego.",
            "Deberas caminar por el mapa para buscar los pergaminos que resuelvan el siguiente problema.",
            DatosCompartidos.Enunciado,
            "¡Buena suerte recuerda volver cuando creas haber encontrado la solucion!",
            "¡Si vuelves y no tienes los pergaminos correctos perderas vida!",

        };

            SetDialogueLines(newDialogueLines);
        } else if (primeraInteraccion == true)
        {
            // Define nuevas líneas de diálogo aquí
            string[] newDialogueLines = {
            "Veo que volviste, dejame revisar tu solucion.",
            "Revisando...",
            "Tus pergaminos no resuelven el problema",
            "Perdiste vida, vuelve a intentarlo."
        };


            comprobarPergaminos();
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

            
            if(primeraInteraccion == false)
            {
                Debug.Log("Se ha terminado el diálogo");
                primeraInteraccion = true;
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

    // Método agregado para establecer las líneas de diálogo desde otro código
    public void SetDialogueLines(string[] newLines)
    {
        dialogueLines = newLines;
    }

    public void pergaminosRecogidos() {

        // Obtener la instancia del inventario
        Inventario inventario = Inventario.Instance;

        int puestosOcupados = 0;
        for (int i = 0; i < inventario.ItemsInventario.Length; i++)
        {
            if (inventario.ItemsInventario[i] != null)
            {
                puestosOcupados++;
            }
        }
        Debug.Log(puestosOcupados);
        Debug.Log(DatosCompartidos.NFragmentos); 

        // comprobar si la cantidad de pergaminos es correcta para resolver el problema
        if (puestosOcupados == DatosCompartidos.NFragmentos)
        {
            Debug.Log("La cantidad de pergaminos es correcta.");

            int correctosCount = 0;
            int totalItems = 0;


            // Iterar sobre los items en el inventario p
            for (int i = 0; i < inventario.ItemsInventario.Length; i++)
            {
                if (inventario.ItemsInventario[i] != null)
                {
                    totalItems++;
                    // Acceder al estado del item
                    var estado = inventario.ItemsInventario[i].Estado;

                    Debug.Log($"Pergamino {i}: {estado}");

                    if (estado == "Correcto")
                    {
                        correctosCount++;
                    }
                }
                else
                {
                    Debug.Log($"El item en el índice {i} es null.");
                }
            }


            if (correctosCount == totalItems && totalItems > 0)
            {
                Debug.Log("Todos los pergaminos son correctos.");
                Debug.Log(totalItems);
                Debug.Log(correctosCount);
            }
            else
            {
                
                Debug.Log("No todos los pergaminos son correctos.");
                Debug.Log(totalItems);
                Debug.Log(correctosCount);
            }

           
        }
        else
        {
            Debug.Log("La cantidad de pergaminos no es correcta.");
            //descontarle vida al jugador
            descontarVida();
        }
    }

    public void descontarVida()
    {
        Debug.Log("Perdiste");
        //ejecutar logica de perder vida
    }

 


    public void comprobarPergaminos()
    {
        Debug.Log("Método comprobar pergaminos en Dialogue ejecutado.");

        // Obtener la instancia del inventario
        Inventario inventario = Inventario.Instance;

        if (inventario == null)
        {
            Debug.LogError("No se pudo obtener la instancia del inventario.");
            return;
        }

        // Asegurarse de que el inventario no esté vacío
        if (inventario.ItemsInventario == null || inventario.ItemsInventario.Length == 0)
        {
            Debug.LogWarning("El inventario está vacío o no inicializado.");
            return;
        } else { pergaminosRecogidos(); }

    }


}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;


[System.Serializable]
public class Problema
{
    public string enunciado;
    public int nfragmentos;
    public Fragmento[] fragmentos;
}

[System.Serializable]
public class Tema
{
    public string descripcion;
    public Problema[] problemas;
}

[System.Serializable]
public class Area
{
    public string _id;
    public string nombre;
    public Tema[] temas;
}

[System.Serializable]
public class AreaList
{
    public Area[] areas;
}

public class conexion : MonoBehaviour
{
    private string apiUrl = "http://localhost:8000/area";
    public GameObject pergamino;
    private GameObject[] referencias;
    public AreaList areaList;

    private void Awake()
    {
        //  StartCoroutine(LoadTextFromAPI());
    }

    void Start()
    {
        StartCoroutine(LoadTextFromAPI());
        referencias = GameObject.FindGameObjectsWithTag("punto");
    }

    IEnumerator LoadTextFromAPI()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al cargar datos desde la API: " + webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Respuesta de la API: " + jsonResponse);
                areaList = JsonUtility.FromJson<AreaList>("{\"areas\":" + jsonResponse + "}");
                Debug.Log("Datos deserializados correctamente.");
                generarpergaminos();
            }
        }
    }

    private void generarpergaminos()
    {
        // Suponiendo que solo hay un área, un tema y un problema
        //var fragmentos = areaList.areas[0].temas[0].problemas[0].fragmentos;

        // Asegúrate de que no intentas acceder a más fragmentos de los que existen
        int numFragmentos = DatosCompartidos.Fragmentos.Count;
        Debug.Log("Número de fragmentos a generar: " + numFragmentos);

        // Crear una lista de índices de referencia y mezclarlos aleatoriamente
        List<int> indices = new List<int>();
        for (int i = 0; i < referencias.Length; i++)
        {
            indices.Add(i);
        }
        indices.Shuffle(); // Método de extensión para mezclar la lista

        for (int i = 0; i < numFragmentos; i++)
        {
            int indiceReferencia = indices[i];
            GameObject instancia = Instantiate(pergamino, referencias[indiceReferencia].transform.position, Quaternion.identity);
            instancia.tag = "per" + i;
            instancia.transform.localScale = new Vector3(12, 12, 2); // Ajustar la escala a 2,2,2
            pergamino scriptInstancia = instancia.GetComponent<pergamino>();

            if (scriptInstancia != null)
            {
                Debug.Log("Inicializando pergamino " + i + " con enunciado: " + DatosCompartidos.Fragmentos[i].enunciado);
                //scriptInstancia.iniciar(i, fragmentos[i].enunciado);
                scriptInstancia.iniciar(i, DatosCompartidos.Fragmentos[i].enunciado, DatosCompartidos.Fragmentos[i].estado);

                //
            }
            else
            {
                Debug.LogError("El componente 'pergamino' no se encontró en la instancia.");
            }
        }
    }

    public string GetEnunciado(int index)
    {
        // Suponiendo que solo hay un área, un tema y un problema
        var fragmento = areaList.areas[0].temas[0].problemas[0].fragmentos[index];
        return fragmento.enunciado;
    }
}

public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
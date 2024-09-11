using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using SimpleJSON;

public static class DatosCompartidos
{
    public static string Enunciado { get; set; } = "Enunciado inicial";
    public static List<Fragmento> Fragmentos { get; set; } = new List<Fragmento>();
    public static int NFragmentos { get; set; } = 0;

    public static string selectedArea;
    public static string selectedTheme;
}

/// <summary>
/// Represents a script that handles the functionality of a dropdown menu.
/// </summary>
public class DropdownCode : MonoBehaviour
{
    /// <summary>
    /// The first dropdown component.
    /// </summary>
    public TMP_Dropdown tmpDropdown;

    /// <summary>
    /// The second dropdown component.
    /// </summary>
    public TMP_Dropdown tmpDropdown1;

    /// <summary>
    /// The list of areas.
    /// </summary>
    private List<string> areas = new List<string>();

    /// <summary>
    /// The list of area IDs.
    /// </summary>
    private List<string> areaIds = new List<string>();

    /// <summary>
    /// The dictionary that maps areas to their corresponding list of topics.
    /// </summary>
    private Dictionary<string, List<string>> temasPorArea = new Dictionary<string, List<string>>();

    /// <summary>
    /// The base URL for API requests.
    /// </summary>
    private string baseUrl = "http://localhost:8000/area";

    /// <summary>
    /// The URL for retrieving a specific problem.
    /// </summary>
    private string traerProblema;

    /// <summary>
    /// The currently selected area.
    /// </summary>
    public string areaSeleccionada;

    /// <summary>
    /// The currently selected topic.
    /// </summary>
    public string temaSeleccionado;

    /// <summary>
    /// Initializes the dropdown menu and starts the coroutine to retrieve the areas.
    /// </summary>
    void Start()
    {
        StartCoroutine(GetAreas());
        tmpDropdown.onValueChanged.AddListener(delegate { DropdownAreaChanged(tmpDropdown); });
        tmpDropdown1.onValueChanged.AddListener(delegate { DropdownTemaChanged(tmpDropdown1); });
    }

    /// <summary>
    /// Sets the URL for retrieving a specific problem based on the selected area and topic.
    /// </summary>
    /// <param name="areaSeleccionada">The selected area.</param>
    ///     /// <param name="temaSeleccionado">The selected topic.</param>
    void selectEnpoint(string areaSeleccionada, string temaSeleccionado)
    {
        traerProblema = "http://localhost:8000/area/problema/" + areaSeleccionada + "/" + temaSeleccionado;
    }

    /// <summary>
    /// Coroutine that retrieves the areas from the API and updates the dropdown menu options.
    /// </summary>
    IEnumerator GetAreas()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(baseUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                JSONNode stats = JSON.Parse(json);
                foreach (JSONNode area in stats)
                {
                    string areaId = area["_id"];
                    string areaNombre = area["nombre"];
                    areas.Add(areaNombre);
                    areaIds.Add(areaId);

                    List<string> temas = new List<string>();
                    foreach (JSONNode tema in area["temas"].AsArray)
                    {
                        temas.Add(tema["descripcion"]);
                    }
                    temasPorArea.Add(areaId, temas);
                }

                UpdateDropdownOptions();
            }
        }

        StartCoroutine(GetEnunciado());
    }

    /// <summary>
    /// Coroutine that retrieves the problem details from the API and updates the shared data.
    /// </summary>
    IEnumerator GetEnunciado()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(traerProblema))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                JSONNode datos = JSON.Parse(json);
                DatosCompartidos.Enunciado = datos["enunciado"];
                Debug.Log("Enunciado obtenido: " + DatosCompartidos.Enunciado);

                DatosCompartidos.Fragmentos.Clear();
                foreach (JSONNode fragmento in datos["fragmentos"].AsArray)
                {
                    Fragmento nuevoFragmento = new Fragmento
                    {
                        enunciado = fragmento["enunciado"],
                        estado = fragmento["estado"]
                    };
                    DatosCompartidos.Fragmentos.Add(nuevoFragmento);
                }

                // Actualiza el número de fragmentos con el valor recibido del backend
                DatosCompartidos.NFragmentos = datos["nfragmentos"].AsInt;
            }
        }
    }

    /// <summary>
    /// Event handler for when the area dropdown value changes.
    /// </summary>
    /// <param name="change">The dropdown component that triggered the event.</param>
    void DropdownAreaChanged(TMP_Dropdown change)
    {
        if (tmpDropdown1 != null && areaIds.Count > change.value)
        {
            tmpDropdown1.ClearOptions();
            string selectedAreaId = areaIds[change.value];
            areaSeleccionada = selectedAreaId;
            if (temasPorArea.ContainsKey(selectedAreaId))
            {
                tmpDropdown1.AddOptions(temasPorArea[selectedAreaId]);
            }
            tmpDropdown1.value = 0;
            if (temasPorArea[selectedAreaId].Count > 0)
            {
                temaSeleccionado = temasPorArea[selectedAreaId][0];
                selectEnpoint(areaSeleccionada, temaSeleccionado);
                StartCoroutine(GetEnunciado());

                DatosCompartidos.selectedArea = areaSeleccionada;
                DatosCompartidos.selectedTheme = temaSeleccionado;
            }
        }
    }

    /// <summary>
    /// Event handler for when the topic dropdown value changes.
    /// </summary>
    /// <param name="change">The dropdown component that triggered the event.</param>
    void DropdownTemaChanged(TMP_Dropdown change)
    {
        if (temasPorArea[areaSeleccionada].Count > change.value)
        {
            temaSeleccionado = temasPorArea[areaSeleccionada][change.value];
            selectEnpoint(areaSeleccionada, temaSeleccionado);
            StartCoroutine(GetEnunciado());

            DatosCompartidos.selectedArea = areaSeleccionada;
            DatosCompartidos.selectedTheme = temaSeleccionado;
        }
    }

    /// <summary>
    /// Updates the options of the area dropdown menu.
    /// </summary>
    void UpdateDropdownOptions()
    {
        if (tmpDropdown != null)
        {
            tmpDropdown.ClearOptions();
            tmpDropdown.AddOptions(areas);
            tmpDropdown.value = 0;
            DropdownAreaChanged(tmpDropdown);
        }
    }
}
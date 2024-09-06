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
}

/// <summary>
public class DropdownCode : MonoBehaviour
{
    public TMP_Dropdown tmpDropdown; public TMP_Dropdown tmpDropdown1;

    private List<string> areas = new List<string>();
    private List<string> areaIds = new List<string>();
    private Dictionary<string, List<string>> temasPorArea = new Dictionary<string, List<string>>();

    private string baseUrl = "http://localhost:8000/area";
    private string traerProblema;
    public string areaSeleccionada;
    public string temaSeleccionado;

    void Start()
    {
        StartCoroutine(GetAreas());
        tmpDropdown.onValueChanged.AddListener(delegate { DropdownAreaChanged(tmpDropdown); });
        tmpDropdown1.onValueChanged.AddListener(delegate { DropdownTemaChanged(tmpDropdown1); });
    }

    void selectEnpoint(string areaSeleccionada, string temaSeleccionado)
    {
        traerProblema = "http://localhost:8000/area/problema/" + areaSeleccionada + "/" + temaSeleccionado;
    }

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

        // Obtener el enunciado desde otro endpoint o proceso
        StartCoroutine(GetEnunciado());
    }

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

                // Procesar y almacenar los fragmentos
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
            }
        }
    }

    void DropdownAreaChanged(TMP_Dropdown change)
    {
        if (tmpDropdown1 != null && areaIds.Count > change.value)
        {
            tmpDropdown1.ClearOptions();
            string selectedAreaId = areaIds[change.value];
            areaSeleccionada = selectedAreaId; // Actualizar la variable areaSeleccionada
            if (temasPorArea.ContainsKey(selectedAreaId))
            {
                tmpDropdown1.AddOptions(temasPorArea[selectedAreaId]);
            }
            tmpDropdown1.value = 0;
            // Actualizar temaSeleccionado con el primer tema de la nueva área seleccionada
            if (temasPorArea[selectedAreaId].Count > 0)
            {
                temaSeleccionado = temasPorArea[selectedAreaId][0];
                selectEnpoint(areaSeleccionada, temaSeleccionado); // Actualizar el endpoint
                StartCoroutine(GetEnunciado()); // Obtener el enunciado para la selección inicial
            }
        }
    }

    void DropdownTemaChanged(TMP_Dropdown change)
    {
        if (temasPorArea[areaSeleccionada].Count > change.value)
        {
            temaSeleccionado = temasPorArea[areaSeleccionada][change.value]; // Actualizar la variable temaSeleccionado
            selectEnpoint(areaSeleccionada, temaSeleccionado); // Actualizar el endpoint
            StartCoroutine(GetEnunciado()); // Obtener el enunciado para el tema seleccionado
        }
    }

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
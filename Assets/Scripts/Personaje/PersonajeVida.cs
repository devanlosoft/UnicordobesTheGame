using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersonajeVida : VidaBase
{
    public static Action EventoPersonajeDerrotado;

    public bool Derrotado { get; private set; }
    public bool PuedeSerCurado => Salud < saludMax;

    private BoxCollider2D _boxCollider2D;

    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    protected override void Start()
    {
        base.Start();
        ActualizarBarraVida(Salud, saludMax);
    }

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            RecibirDaño(10);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            RestaurarSalud(10);
        }
    }

    public void RestaurarSalud(float cantidad)
    {
        if (Derrotado)
        {
            return;
        }

        if (PuedeSerCurado)
        {
            Salud += cantidad;
            if (Salud > saludMax)
            {
                Salud = saludMax;
            }
            ActualizarBarraVida(Salud, saludMax);
        }
    }

    protected override void PersonajeDerrotado()
    {
        _boxCollider2D.enabled = false;
        Derrotado = true;
        EventoPersonajeDerrotado?.Invoke();
        StartCoroutine(ChangeSceneAfterDelay(1f, 4)); // Cambia "3" por el índice correcto
    }

    public void RestaurarPersonaje()
    {
        _boxCollider2D.enabled = true;
        Derrotado = false;
        Salud = saludInicial;
        ActualizarBarraVida(Salud, saludInicial);
    }

    protected override void ActualizarBarraVida(float vidaActual, float vidaMax)
    {
        UIManager.Instance.ActualizarVidaPersonaje(vidaActual, vidaMax);
    }

    private IEnumerator ChangeSceneAfterDelay(float delay, int sceneIndex)
    {
        Debug.Log($"Esperando {delay} segundos antes de cambiar a la escena {sceneIndex}...");
        yield return new WaitForSecondsRealtime(delay);
        ChangeSceneByIndex(sceneIndex);
    }

}

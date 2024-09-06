using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personaje : MonoBehaviour
{
    public PersonajeMana PersonajeMana { get; private set; }
    public PersonajeVida PersonajeVida { get; private set; }
    public PersonajeAnimacion PersonajeAnimacion { get; private set; }

    private void Awake()
    {
        PersonajeMana = GetComponent<PersonajeMana>();
        PersonajeVida = GetComponent<PersonajeVida>();
        PersonajeAnimacion = GetComponent<PersonajeAnimacion>();
    }

    public void RestaurarPersonaje()
    {
        PersonajeMana.RestablecerMana();
        PersonajeVida.RestaurarPersonaje();
        PersonajeAnimacion.RevivirPersonaje();
    }
}

using System;
using UnityEngine;



public class ItemPorAgregar : MonoBehaviour
{
    [Header("Configurar")]
    [SerializeField] private InventarioItem InventarioItemReferencia;
    [SerializeField] private int cantidadPorAgregar;

    public AudioSource audioSource;
    [SerializeField] private AudioClip clipAtaque;

    public static event Action OnPlayerEnter;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            audioSource.PlayOneShot(clipAtaque);
            pergamino ventana = GetComponent<pergamino>();


            if (ventana != null)
            {
                GameObject objeto = GameObject.Find("escenapergaminos");
                pergaminoEliminar script = objeto.GetComponent<pergaminoEliminar>();
                script.codigo = ventana.codigo;

                // Llamar al método MetodoB en ScriptB
                ventana.cargarventana();



            }
        }

    }

    public void recoger()
    {

        Inventario.Instance.AñadirItem(InventarioItemReferencia, cantidadPorAgregar);
        GameObject objeto = GameObject.Find("escenapergaminos");
        pergaminoEliminar script = objeto.GetComponent<pergaminoEliminar>();
        script.Eliminar();


    }

}
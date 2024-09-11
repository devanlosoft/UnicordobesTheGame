using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventario : MonoBehaviour
{
    private static Inventario instance;
    public int code;

    public static Inventario Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Inventario>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<Inventario>();
                    singletonObject.name = "Inventario (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }

    [Header("Items")]
    [SerializeField] private InventarioItem[] itemsInventario;
    [SerializeField] private Personaje personaje;
    [SerializeField] private int numeroDeSlots;

    public Personaje Personaje => personaje;
    public int NumeroDeSlots => numeroDeSlots;
    public InventarioItem[] ItemsInventario => itemsInventario;

    private void Start()
    {
        if (itemsInventario == null || itemsInventario.Length == 0)
        {
            itemsInventario = new InventarioItem[numeroDeSlots];
        }
    }

    private void Awake()
    {
        if (itemsInventario == null || itemsInventario.Length == 0)
        {
            itemsInventario = new InventarioItem[numeroDeSlots];
        }
    }

    public void AñadirItem(InventarioItem itemPorAñadir, int Cantidad)
    {
        if (itemPorAñadir == null)
        {
            Debug.Log("El item por añadir es nulo.");
            return;
        }

        Debug.Log($"Añadiendo item: {itemPorAñadir.name}, Enunciado: {itemPorAñadir.Enunciado}, Estado: {itemPorAñadir.Estado}");

        // Verificación de items en inventario
        List<int> indexes = VerificarExistencias(itemPorAñadir.ID);

        if (itemPorAñadir.EsAcumulable)
        {
            if (indexes.Count > 0)
            {
                for (int i = 0; i < indexes.Count; i++)
                {
                    if (itemsInventario[indexes[i]].Cantidad < itemPorAñadir.AcumulacionMax)
                    {
                        itemsInventario[indexes[i]].Cantidad += Cantidad;
                        if (itemsInventario[indexes[i]].Cantidad > itemPorAñadir.AcumulacionMax)
                        {
                            int diferencia = itemsInventario[indexes[i]].Cantidad - itemPorAñadir.AcumulacionMax;
                            itemsInventario[indexes[i]].Cantidad = itemPorAñadir.AcumulacionMax;
                            AñadirItem(itemPorAñadir, diferencia);
                        }

                        InventarioUI inventarioUI = FindObjectOfType<InventarioUI>();
                        if (inventarioUI != null)
                        {
                            inventarioUI.DibujarItemEnInventario(itemPorAñadir, itemsInventario[indexes[i]].Cantidad, itemIndex: indexes[i]);
                        }

                        // Imprimir en consola
                        Debug.Log($"Añadido {Cantidad} de {itemPorAñadir.name} al inventario en el índice {indexes[i]}.");

                        return;
                    }
                }
            }
        }

        if (Cantidad <= 0)
        {
            Debug.Log("La cantidad a añadir es menor o igual a cero.");
            return;
        }

        if (Cantidad > itemPorAñadir.AcumulacionMax)
        {
            AñadirItemEnSlotDisponible(itemPorAñadir, itemPorAñadir.AcumulacionMax);
            Cantidad -= itemPorAñadir.AcumulacionMax;
            AñadirItem(itemPorAñadir, Cantidad);
        }
        else
        {
            AñadirItemEnSlotDisponible(itemPorAñadir, Cantidad);
        }
    }

    private List<int> VerificarExistencias(string itemID)
    {
        List<int> indexesDelItem = new List<int>();
        for (int i = 0; i < itemsInventario.Length; i++)
        {
            if (itemsInventario[i] != null)
            {
                if (itemsInventario[i].ID == itemID)
                {
                    indexesDelItem.Add(i);
                }
            }
        }
        return indexesDelItem;
    }

    private void AñadirItemEnSlotDisponible(InventarioItem item, int Cantidad)
    {
        for (int i = 0; i < itemsInventario.Length; i++)
        {
            if (itemsInventario[i] == null)
            {
                itemsInventario[i] = item.CopiarItem();
                itemsInventario[i].Cantidad = Cantidad;

                // Asignar enunciado y estado directamente del itemPorAñadir
                itemsInventario[i].Enunciado = DatosCompartidos.Fragmentos[code].enunciado;
                itemsInventario[i].Estado = DatosCompartidos.Fragmentos[code].estado;

                InventarioUI inventarioUI = FindObjectOfType<InventarioUI>();

                if (inventarioUI != null)
                {
                    inventarioUI.DibujarItemEnInventario(item, Cantidad, itemIndex: i);
                    Debug.Log(itemsInventario[0]);
                }

                // Imprimir en consola
                Debug.Log($"Añadido {Cantidad} de {item.name} al inventario en el índice {i}.");

                return;
            }
        }
    }

    private void EliminarItem(int index)
    {
        ItemsInventario[index].Cantidad--;
        if (itemsInventario[index].Cantidad <= 0)
        {
            itemsInventario[index].Cantidad = 0;
            itemsInventario[index] = null;
            InventarioUI.Instance.DibujarItemEnInventario(null, 0, index);
        }
        else
        {
            InventarioUI.Instance.DibujarItemEnInventario(ItemsInventario[index],
            itemsInventario[index].Cantidad, index);
        }
    }

    private void UsarItem(int index)
    {
        if (itemsInventario[index] == null)
        {
            return;
        }
        if (itemsInventario[index].UsarItem())
        {
            EliminarItem(index);
        }
    }

    #region Eventos

    private void SlotInteraccionRespuesta(TipoDeInteraccion tipo, int index)
    {
        switch (tipo)
        {
            case TipoDeInteraccion.Usar:
                UsarItem(index);
                break;
            case TipoDeInteraccion.Equipar:
                break;
            case TipoDeInteraccion.Remover:
                break;
        }
    }

    private void OnEnable()
    {
        InventarioSlot.EventoSlotInteraccion += SlotInteraccionRespuesta;
    }

    private void OnDisable()
    {
        InventarioSlot.EventoSlotInteraccion -= SlotInteraccionRespuesta;
    }

    #endregion

}

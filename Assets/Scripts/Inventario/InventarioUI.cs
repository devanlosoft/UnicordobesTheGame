using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventarioUI : MonoBehaviour
{
    private static InventarioUI instance;

    public static InventarioUI Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InventarioUI>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<InventarioUI>();
                    singletonObject.name = "InventarioUI (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }

    [Header("Panel Inventario Descripcion")]
    [SerializeField] private GameObject panelInventarioDescripcion;
    [SerializeField] private Image itemIcono;
    [SerializeField] private TextMeshProUGUI itemNombre;
    [SerializeField] private TextMeshProUGUI itemDescripcion;

    [SerializeField] private InventarioSlot slotPrefab;
    [SerializeField] private Transform contenedor;

    public InventarioSlot SlotSeleccionado { get; private set;}
    private List<InventarioSlot> slotsDisponibles = new List<InventarioSlot>();

    private void Start()
    {
        InicializarInventario();
    }

    private void Update()
    {
        ActualizarSlotSeleccionado();
    }

    private void InicializarInventario()
    {
        for (int i = 0; i < Inventario.Instance.NumeroDeSlots; i++)
        {
            InventarioSlot nuevoSlot = Instantiate(slotPrefab, contenedor);    
            nuevoSlot.Index = i;
            slotsDisponibles.Add(nuevoSlot);
        }
    }

    private void ActualizarSlotSeleccionado()
    {
        GameObject goSeleccionado = EventSystem.current.currentSelectedGameObject;
        if (goSeleccionado == null)
        {
            return;
        }
        InventarioSlot slot = goSeleccionado.GetComponent<InventarioSlot>();
        if (slot !=null)
        {
            SlotSeleccionado = slot;
        }
    }

    public void DibujarItemEnInventario(InventarioItem itemPorAñadir, int Cantidad, int itemIndex)
    {
       
        InventarioSlot slot = slotsDisponibles[itemIndex];
       
        if (itemPorAñadir != null)
        {
            Debug.Log("antes");
            slot.ActivarSlotUI(true);
            Debug.Log("aqui");
            slot.ActualizarSlot(itemPorAñadir, Cantidad);
        }
        else
        {
            slot.ActivarSlotUI(false);
        }
    }

    private void ActualizarInventarioDescripcion(int index)
    {
        if (Inventario.Instance.ItemsInventario[index] != null)
        {
            itemIcono.sprite = Inventario.Instance.ItemsInventario[index].Icono;
            itemNombre.text = Inventario.Instance.ItemsInventario[index].Nombre;
            itemDescripcion.text = Inventario.Instance.ItemsInventario[index].Descripcion;
            panelInventarioDescripcion.SetActive(true);
        }
        else
        {
            panelInventarioDescripcion.SetActive(false);
        }
    }

    public void UsarItem()
    {
        if (SlotSeleccionado != null)
        {
            SlotSeleccionado.SlotUsarItem();
            SlotSeleccionado.SeleccionarSlot();
        }
    }

    #region Eventos

    private void SlotInteraccionRespuesta(TipoDeInteraccion tipo, int index)
    {
        if (tipo == TipoDeInteraccion.Click)
        {
            ActualizarInventarioDescripcion(index);
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


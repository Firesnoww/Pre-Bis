using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoDatosAlmacen : MonoBehaviour
{
    public string datoRecibido = "0|0|0|0"; // Por defecto inicial
    public List<int> ListaEstados = new List<int>();

    public GameObject[] botones;
    public GameObject[] botonesApagados;
    public GameObject[] Info;
    

    void Start()
    {
        ResetearDatos();
        CargarDatos(); // Lo primero que hace
        //AplicarEstadosAUI(ListaEstados); // Mostrar en pantalla según el estado
    }

    // Convierte string a lista
    public List<int> ProcesarDatos(string datos)
    {
        List<int> resultado = new List<int>();
        string[] partes = datos.Split('|');

        foreach (string parte in partes)
        {
            if (int.TryParse(parte, out int valor) && valor >= 0 && valor <= 2)
                resultado.Add(valor);
            else
                resultado.Add(0);
        }

        return resultado;
    }

    // Convierte lista a string para guardar
    public string ObtenerStringDesdeEstados()
    {
        return string.Join("|", ListaEstados);
    }

    public void GuardarDatos()
    {
        string datosAGuardar = ObtenerStringDesdeEstados();
        datoRecibido = ObtenerStringDesdeEstados();
        PlayerPrefs.SetString("DatosLibelulas", datosAGuardar);
        PlayerPrefs.Save();
    }

    public void CargarDatos()
    {
        if (PlayerPrefs.HasKey("DatosLibelulas"))
            ListaEstados = ProcesarDatos(PlayerPrefs.GetString("DatosLibelulas"));
        else
            ListaEstados = ProcesarDatos(datoRecibido);
    }

    public void ActualizarEstado(int indice, int nuevoEstado)
    {
        if (indice >= 0 && indice < ListaEstados.Count)
        {
            if (nuevoEstado > ListaEstados[indice]) // Solo se actualiza si hay avance
            {
                ListaEstados[indice] = nuevoEstado;
                GuardarDatos(); // guarda inmediatamente después de cambiar
                AplicarEstadosAUI(ListaEstados);
            }
        }

    }

    public void MaterialInvestigado(int id)
    {
        // Si el jugador descubre algo nuevo:
        ActualizarEstado(id, 1); // desbloquea info básica
    }


    public void AplicarEstadosAUI(List<int> estados)
    {
        int total = Mathf.Min(estados.Count, botones.Length, botonesApagados.Length);

        for (int i = 0; i < total; i++)
        {
            int estado = estados[i];
            switch (estado)
            {
                case 0:
                    Debug.Log($"Botón {i}: Falta investigar");
                    botones[i].SetActive(false);
                    botonesApagados[i].SetActive(true);
                    if (Info[i] != null)
                    {
                        Info[i].SetActive(false);
                    }
                    break;

                case 1:
                    Debug.Log($"Botón {i}: Info básica activada");
                    botones[i].SetActive(true);
                    botonesApagados[i].SetActive(false);

                    if (Info[i] != null)
                    {
                        Info[i].SetActive(false);
                    }
                    break;


                case 2:
                    Debug.Log($"Botón {i}: Info extendida activada");
                    botones[i].SetActive(true); // o algo más si necesitas
                    botonesApagados[i].SetActive(false);
                    if (Info[i] != null)
                    {
                        Info[i].SetActive(true);
                    }
                    break;

            }
        }
    }

    public void ResetearDatos()
    {
        PlayerPrefs.DeleteKey("DatosLibelulas");
        ListaEstados = ProcesarDatos(datoRecibido);
        AplicarEstadosAUI(ListaEstados);
    }
    int a = 0;
    public void temporalCrecimiento() 
    {
        
        MaterialInvestigado(a);
        a++;
    }
}

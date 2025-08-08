using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMisiones : MonoBehaviour
{
    [Header("Lista de misiones en orden")]
    public Misiones[] misiones;

    [Header("Misión actual (por índice)")]
    public int misionActual = 0;



    void Start()
    {
        EjecutarMisionActual();
    }

    [ContextMenu("Ejecutar Mision Actual")]
    void EjecutarMisionActual()
    {
        if (misiones == null || misiones.Length == 0)
        {
            Debug.LogWarning("No hay misiones asignadas.");
            return;
        }

        if (misionActual < 0 || misionActual >= misiones.Length)
        {
            Debug.LogError("Índice de misión actual fuera de rango.");
            return;
        }

        Misiones m = misiones[misionActual];
        if (m.InicioMision)
        {
            Debug.Log($"--- Misión: {m.NombreMision} ---");
            Debug.Log($"Dificultad: {m.Dificultad}");
            Debug.Log($"Descripción general:\n{m.InfoMision}");
           
        }
        
        if (m.EsCaptura)
        {
            Debug.Log("[FASE CAPTURA ACTIVADA]");
            Debug.Log($"Info captura: {m.InfoCaptura}");
            Debug.Log($"Libélulas Low: {m.LibelulaLow.Length} | High: {m.LibelulaHigt.Length}");
           
        }

        if (m.EsRecoleccion)
        {
            Debug.Log("[FASE RECOLECCIÓN ACTIVADA]");
            Debug.Log($"Info recolección: {m.InfoRecoleccion}");
            Debug.Log($"Muestras: {m.Muestra.Length}");
            
        }

        if (m.EsExploracion)
        {
            Debug.Log("[FASE EXPLORACIÓN ACTIVADA]");
            Debug.Log($"Info exploración: {m.InfoExploracion}");
            Debug.Log($"Zonas a visitar: {m.Lugar.Length}");
         
        }

        if (m.EsMicroscopio)
        {
            Debug.Log("[FASE MICROSCOPIO ACTIVADA]");
            Debug.Log($"Info microscopio: {m.InfoMicroscopio}");
          
        }

        if (m.EsResultado)
        {
            Debug.Log("[FASE RESULTADO FINAL ACTIVADA]");
            Debug.Log($"Info final: {m.InfoFinal}");
            Debug.Log($"Datos desbloqueados: {m.InfoDesbloqueada}");

        }

        Debug.Log("Estado ACTUAL MISION = " + misionActual);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// Misiones
// ScriptableObject que describe UNA fase de misión.
// Usa flags para activar el tipo de fase (Inicio/Captura/Recolección/Exploración/Microscopio/Resultado).
// Se puede usar como "intro" (InicioMision) o una fase jugable concreta.
//
[CreateAssetMenu(fileName = "Mision #", menuName = "Scriptable Objs/Mision", order = 1)]
public class Misiones : ScriptableObject
{
    [Header("------------------Datos Mision------------------")]
    public bool InicioMision;
    public AudioClip AuMision;
    public string NombreMision;
    [Multiline(4)]
    public string InfoMision;
    public int Dificultad;

    [Header("------------CAPTURA_LIBELULAS---------------")]
    public bool EsCaptura;
    public AudioClip AuLibelulas;
    [Multiline(4)]
    public string InfoCaptura;
    public GameObject[] LibelulaLow;
    public GameObject[] LibelulaHigt;

    [Header("---------------RECOLECCION---------------")]
    public bool EsRecoleccion;
    public AudioClip AuRecoleccion;
    [Multiline(4)]
    public string InfoRecoleccion;
    public GameObject[] Muestra;
    public GameObject[] Cantidad;

    [Header("---------------EXPLORACION---------------")]
    public bool EsExploracion;
    public AudioClip AuExploracion;
    [Multiline(4)]
    public string InfoExploracion;
    public GameObject[] Lugar;

    [Header("---------------MICROSCOPIO---------------")]
    public bool EsMicroscopio;
    public AudioClip AuMicroscopio;
    [Multiline(4)]
    public string InfoMicroscopio;

    [Header("-------------RESULTADO_FINAL---------------")]
    public bool EsResultado;
    public AudioClip AuFinal;
    [Multiline(4)]
    public string InfoFinal;
    public int InfoDesbloqueada;

    // --------------------- NUEVO: Contadores sencillos por fase ---------------------
    // Si no rellenas estos campos, se asume 1 como objetivo por defecto.
    [Header("Objetivos simples (opcionales)")]
    public int CapturasObjetivo = 1;        // Para EsCaptura
    public int RecoleccionesObjetivo = 1;   // Para EsRecoleccion
    public int ZonasObjetivo = 1;           // Para EsExploracion
    public int AnalisisObjetivo = 1;        // Para EsMicroscopio

    // --------------------- NUEVO: Objetivos multi‑ítem (opcionales) ---------------------
    // Úsalos si necesitas "1 Flor + 1 Hoja + 1 Ala" (IDs y cantidades).
    // Si están vacíos o desparejos, se ignoran y se usa el contador simple.
    [Header("Recolección avanzada (por ID, opcional)")]
    public string[] RecoleccionIDs;   // IDs requeridos (p.ej. "Flor","Hoja","Ala")
    public int[] RecoleccionCant;  // Cantidades por ID (mismo largo que RecoleccionIDs)

    [Header("Captura avanzada (por ID, opcional)")]
    public string[] CapturaIDs;       // IDs de especies/variantes a capturar
    public int[] CapturaCant;      // Cantidades por ID (mismo largo que CapturaIDs)
}
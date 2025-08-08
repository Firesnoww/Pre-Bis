using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

}

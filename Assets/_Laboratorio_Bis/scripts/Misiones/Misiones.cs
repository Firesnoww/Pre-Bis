using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NuevaMision", menuName = "Misiones/Mision", order = 1)]
public class Misiones : PlayerPrefs
{
    //[Header "Mision"]
    public string NombreMision;
    public string descripcion;
    public int dificultad;

    // Materiales misiones
    public GameObject LibelulaLow;
    public string zonaObjetivo;
    public GameObject LibelulaHigt;

}

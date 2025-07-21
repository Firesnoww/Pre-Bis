using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Mision #", menuName = "Scriptable Objs/Mision", order = 1)]
public class Misiones : ScriptableObject
{
    [Header ("------------------Datos Mision------------------")]
    public string NombreMision;
    [Multiline(4)]
    public string descripcion;
    public string zonaObjetivo;
    public int dificultad;
    public bool Investigado;

    [Header("---------------Modelos Libelula---------------")]
    public GameObject LibelulaLow;
    public GameObject LibelulaHigt;

}

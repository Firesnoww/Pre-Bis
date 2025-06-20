using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevaLibelula", menuName = "Datos/Libélula")]
public class LibelulaData : ScriptableObject
{
    public string nombre;
    public Color color;
    public Material icono;
    public bool esFija;
    public string zonaFija;
}
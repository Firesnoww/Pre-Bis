using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Libelula: ", menuName = "Scriptable Objs/Lib�lula")]
public class LibelulaData : ScriptableObject
{
    public string nombre;
    public Material icono;
    public bool esFija;
    public string zonaFija;
}
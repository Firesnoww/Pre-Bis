using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Lista Mision", menuName = "Scriptable Objs/Lista Mision", order = 1)]
public class MisionesAnidada : ScriptableObject
{
    [Header("Secuencia lineal de misiones/fases")]
    public Misiones[] misiones;  
}

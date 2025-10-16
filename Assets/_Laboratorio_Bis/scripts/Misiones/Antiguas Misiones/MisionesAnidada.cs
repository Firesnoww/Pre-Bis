using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// MisionesAnidada
// ScriptableObject que agrupa una SECUENCIA lineal de ScriptableObjects "Misiones".
// La línea se ejecuta en orden (indices 0..N-1) bajo el control de MainMisiones.
//
[CreateAssetMenu(fileName = "Lista Mision", menuName = "Scriptable Objs/Lista Mision", order = 1)]
public class MisionesAnidada : ScriptableObject
{
    [Header("Secuencia lineal de misiones/fases")]
    public Misiones[] misiones;  // Cada elemento describe una fase (Inicio, Captura, Recolección, etc.)
}

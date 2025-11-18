using UnityEngine;

namespace Quests
{
    /// <summary>
    /// Línea de misión: una secuencia lineal de fases (0..N-1).
    /// - lineId es CLAVE para guardado/carga.
    /// - La fase 0 suele ser Intro (preview/brief).
    /// </summary>
    [CreateAssetMenu(fileName = "Line_", menuName = "Quests/Mission Line")]
    public class MissionLine : ScriptableObject
    {
        [Header("Identidad de la línea")]
        [Tooltip("ID estable de la línea. Ej: line/npc_profesor_intro")]
        public string lineId;

        [Tooltip("Nombre visible de la misión para UI/listados.")]
        public string displayName;

        [Header("Fases en orden")]
        public MissionPhase[] phases;
    }
}

using UnityEngine;

namespace Quests
{
    /// <summary>
    /// Tipo de fase de misión. Mantén estos valores estables (se guardan como texto).
    /// </summary>
    public enum PhaseType { Intro, Capture, Collect, Explore, Microscope, Result }

    /// <summary>
    /// Describe UNA fase de una línea de misión.
    /// - Usa PhaseType para saber cómo se valida el avance.
    /// - Para Capture/Collect puedes usar modo simple (cantidad total) o por-ID (listas paralelas).
    /// - Para Explore usa IDs de zonas (zone/*). El conteo simple representa "cuántas zonas".
    /// - Para Result puedes indicar IDs de info a desbloquear.
    /// - Para Capture puedes incluir configuración de spawn (opcional).
    /// </summary>
    [CreateAssetMenu(fileName = "Phase_", menuName = "Quests/Mission Phase")]
    public class MissionPhase : ScriptableObject
    {
        [Header("Identidad")]
        [Tooltip("ID estable de esta fase (opcional para analítica). Úsalo solo si lo necesitas).")]
        public string phaseId = "";

        [Tooltip("Título que verá el jugador en esta fase.")]
        public string title;

        [TextArea(3, 6)]
        [Tooltip("Descripción/brief de la fase.")]
        public string description;

        [Header("Tipo de fase")]
        public PhaseType phaseType = PhaseType.Intro;

        [Header("Objetivo simple (alternativa al objetivo por ID)")]
        [Min(1)] public int targetAmount = 1;  // Capturas, recolectas, zonas, análisis...

        [Header("Objetivo por-ID (opcional)")]
        [Tooltip("IDs con namespace. Ej: species/red_low, item/leaf, zone/wetlands_A")]
        public string[] requiredIds;
        [Tooltip("Cantidades por cada ID. Debe igualar el largo de requiredIds.")]
        public int[] requiredAmounts;

        [Header("Desbloqueos al terminar (opcional)")]
        [Tooltip("IDs de 'información' o recompensas lógicas. Ej: info/species/red_low")]
        public string[] unlockInfoIds;

        [Header("SPAWN (solo usado si phaseType = Capture)")]
        [Tooltip("Prefab de libélula a instanciar para esta fase (opcional).")]
        public GameObject dragonflyPrefab;

        [Tooltip("Puntos exactos de spawn (si se asignan, tienen prioridad).")]
        public Transform[] spawnPoints;

        [Tooltip("Si no hay puntos, spawnea alrededor de 'spawnOrigin' usando 'spawnRadius'.")]
        public Transform spawnOrigin;
        public float spawnRadius = 8f;

        [Min(0)] public int spawnCount = 0;
    }
}

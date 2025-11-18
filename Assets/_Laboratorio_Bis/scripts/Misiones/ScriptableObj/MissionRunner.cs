using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Quests
{
    /// <summary>
    /// Snapshot liviano para la UI.
    /// </summary>
    [Serializable]
    public class MissionSnapshot
    {
        public bool active;
        public string lineId;
        public string lineName;
        public int phaseIndex1;   // 1-based
        public int phasesTotal;
        public string phaseTitle;
        public string phaseDesc;
        public string phaseType;  // texto del enum
        public int remaining;     // modo simple
        public int target;        // modo simple
    }

    /// <summary>
    /// Par id-cantidad serializable (evita Dictionary para JsonUtility).
    /// </summary>
    [Serializable]
    public class IdAmount
    {
        public string id;
        public int amount;
    }

    /// <summary>
    /// Estructura de guardado JSON (local).
    /// </summary>
    [Serializable]
    public class MissionSave
    {
        public string activeLineId = "";
        public int phaseIndex = 0;

        // Progreso simple
        public int remaining = 0;
        public int target = 0;

        // Progreso por-ID
        public List<IdAmount> pendingById = new List<IdAmount>();

        // Info desbloqueada acumulada (IDs lógicos)
        public List<string> unlockedInfoIds = new List<string>();
    }

    /// <summary>
    /// MissionRunner
    /// - Administra UNA línea activa (pero está escrito para escalar).
    /// - Expone eventos para UI y para los minijuegos.
    /// - Guarda/carga el estado en JSON (Application.persistentDataPath).
    ///
    /// Flujo básico:
    /// 1) StartLine(line) -> prepara y entra a fase 0.
    /// 2) Los sistemas llaman: ReportCapture/Collect/Zone/Analysis según corresponda.
    /// 3) Cuando se cumplen objetivos: avanza a la siguiente fase. En Result: desbloquea y cierra.
    /// </summary>
    public class MissionRunner : MonoBehaviour
    {
        [Header("Config")]
        [Tooltip("Si la primera fase es Intro, avanzar automáticamente a la siguiente al comenzar.")]
        public bool autoSkipIntroOnStart = true;

        [Header("Estado (solo lectura en inspector)")]
        [SerializeField] private MissionLine activeLine;
        [SerializeField] private int phaseIndex = 0;
        [SerializeField] private bool isActive = false;

        // Runtime objetivo (simple o por-ID)
        int _remaining = 0, _target = 0;
        List<IdAmount> _pendingById = null;

        // —— Eventos ——
        public event Action<MissionSnapshot> OnSnapshot;
        public event Action<List<string>> OnInfoUnlocked; // al cerrar Result
        public event Action<MissionPhase> OnPhaseEnter;   // útil para spawners
        public event Action<MissionPhase> OnPhaseExit;

        // —— Guardado ——
        const string SaveFileName = "mission_save.json";
        MissionSave _save = new MissionSave();

        #region API pública — Línea
        /// <summary>
        /// Comienza una línea. Si ya había una activa, la reemplaza.
        /// </summary>
        public void StartLine(MissionLine line)
        {
            if (line == null || line.phases == null || line.phases.Length == 0)
            {
                Debug.LogWarning("[MissionRunner] Línea inválida.");
                return;
            }

            // Cerrar la anterior si estaba activa
            if (isActive) ExitPhase();

            activeLine = line;
            isActive = true;
            phaseIndex = 0;

            PreparePhase();

            // Si es Intro y se debe auto-saltar, avanzar 1
            if (autoSkipIntroOnStart && CurrentPhaseType == PhaseType.Intro)
            {
                CompleteCurrentPhase(); // esto llama a PreparePhase nuevamente
            }

            PushSnapshot();
            SaveToDisk();
        }

        /// <summary>
        /// Aborta la línea actual (pierde progreso).
        /// </summary>
        public void AbortLine()
        {
            if (!isActive) return;
            ExitPhase();

            isActive = false;
            activeLine = null;
            phaseIndex = 0;
            _remaining = _target = 0;
            _pendingById = null;

            // Limpiar parte del save (mantenemos la info desbloqueada histórica)
            _save.activeLineId = "";
            _save.phaseIndex = 0;
            _save.remaining = 0;
            _save.target = 0;
            _save.pendingById = new List<IdAmount>();

            PushSnapshot();
            SaveToDisk();
        }

        public bool HasActiveLine => isActive;
        public MissionLine ActiveLine => activeLine;
        public MissionPhase CurrentPhase => (isActive && activeLine && phaseIndex >= 0 && phaseIndex < activeLine.phases.Length)
            ? activeLine.phases[phaseIndex] : null;
        public PhaseType CurrentPhaseType => CurrentPhase ? CurrentPhase.phaseType : PhaseType.Intro;
        #endregion

        #region API pública — Reportes de gameplay
        /// <summary>Reporta captura de especie (IDs tipo "species/*").</summary>
        public void ReportCapture(string speciesId, int amount = 1)
        {
            if (!ValidatePhase(PhaseType.Capture)) return;
            ApplyProgress(speciesId, amount);
        }

        /// <summary>Reporta recolección de ítem (IDs tipo "item/*").</summary>
        public void ReportCollect(string itemId, int amount = 1)
        {
            if (!ValidatePhase(PhaseType.Collect)) return;
            ApplyProgress(itemId, amount);
        }

        /// <summary>Reporta visita de zona (IDs tipo "zone/*").</summary>
        public void ReportZone(string zoneId)
        {
            if (!ValidatePhase(PhaseType.Explore)) return;
            ApplyProgress(zoneId, 1);
        }

        /// <summary>Reporta análisis (Microscopio). Usa objetivo simple.</summary>
        public void ReportAnalysis(int amount = 1)
        {
            if (!ValidatePhase(PhaseType.Microscope)) return;
            ApplyProgress(null, amount);
        }
        #endregion

        #region Guardado / Carga
        /// <summary>
        /// Carga el save local y, si hay línea activa, intenta resolverla del array disponible.
        /// Llama esto al iniciar escena (pásale todas las líneas registradas del juego).
        /// </summary>
        public void LoadFromDisk(MissionLine[] allLines)
        {
            var path = Path.Combine(Application.persistentDataPath, SaveFileName);
            if (!File.Exists(path))
            {
                _save = new MissionSave();
                PushSnapshot();
                return;
            }

            try
            {
                var json = File.ReadAllText(path);
                _save = JsonUtility.FromJson<MissionSave>(json) ?? new MissionSave();
            }
            catch (Exception e)
            {
                Debug.LogWarning("[MissionRunner] Error leyendo save: " + e.Message);
                _save = new MissionSave();
            }

            // Reconstruir estado
            if (!string.IsNullOrEmpty(_save.activeLineId) && allLines != null)
            {
                var line = Array.Find(allLines, l => l && l.lineId == _save.activeLineId);
                if (line != null && line.phases != null && line.phases.Length > 0)
                {
                    activeLine = line;
                    isActive = true;
                    phaseIndex = Mathf.Clamp(_save.phaseIndex, 0, line.phases.Length - 1);

                    // Rehidratar progreso
                    _remaining = _save.remaining;
                    _target = _save.target;
                    _pendingById = _save.pendingById != null ? new List<IdAmount>(_save.pendingById) : null;

                    // **No** llamamos PreparePhase aquí porque ya rehidratamos el runtime.
                    PushSnapshot();
                    return;
                }
            }

            // Si no se pudo rehidratar una línea válida
            activeLine = null;
            isActive = false;
            phaseIndex = 0;
            _remaining = _target = 0;
            _pendingById = null;
            PushSnapshot();
        }

        void SaveToDisk()
        {
            try
            {
                _save.activeLineId = (isActive && activeLine) ? activeLine.lineId : "";
                _save.phaseIndex = phaseIndex;
                _save.remaining = _remaining;
                _save.target = _target;
                _save.pendingById = _pendingById != null ? new List<IdAmount>(_pendingById) : new List<IdAmount>();

                var path = Path.Combine(Application.persistentDataPath, SaveFileName);
                var json = JsonUtility.ToJson(_save, true);
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Debug.LogWarning("[MissionRunner] Error guardando save: " + e.Message);
            }
        }
        #endregion

        #region Internos — preparar, avanzar, validar, snapshot
        void PreparePhase()
        {
            var phase = CurrentPhase;
            if (phase == null) { FinishLine(); return; }

            // Exit previo (por si cambiamos manualmente de fase)
            ExitPhase();

            // Entrar a la nueva fase
            _pendingById = null;
            _remaining = _target = 0;

            if (phase.requiredIds != null && phase.requiredAmounts != null &&
                phase.requiredIds.Length > 0 &&
                phase.requiredIds.Length == phase.requiredAmounts.Length)
            {
                _pendingById = new List<IdAmount>(phase.requiredIds.Length);
                for (int i = 0; i < phase.requiredIds.Length; i++)
                {
                    int a = Mathf.Max(1, phase.requiredAmounts[i]);
                    if (string.IsNullOrWhiteSpace(phase.requiredIds[i])) continue;
                    _pendingById.Add(new IdAmount { id = phase.requiredIds[i], amount = a });
                    _target += a;
                }
                _remaining = _target;
            }
            else
            {
                _target = Mathf.Max(1, phase.targetAmount);
                _remaining = _target;
            }

            // Notificar entrada (spawners se enganchan aquí)
            OnPhaseEnter?.Invoke(phase);

            PushSnapshot();
            SaveToDisk();
        }

        void ExitPhase()
        {
            var phase = CurrentPhase;
            if (phase != null)
            {
                OnPhaseExit?.Invoke(phase); // para limpiar spawns, etc.
            }
        }

        void CompleteCurrentPhase()
        {
            ExitPhase();

            phaseIndex++;
            if (activeLine == null || phaseIndex >= activeLine.phases.Length)
            {
                FinishLine();
                return;
            }

            PreparePhase();
        }

        void FinishLine()
        {
            // Desbloqueos de la última fase si aplica
            var last = (activeLine && activeLine.phases != null && activeLine.phases.Length > 0)
                       ? activeLine.phases[Mathf.Clamp(phaseIndex - 1, 0, activeLine.phases.Length - 1)]
                       : null;

            if (last && last.unlockInfoIds != null && last.unlockInfoIds.Length > 0)
            {
                // Agregar al save (evitando duplicados simples)
                foreach (var id in last.unlockInfoIds)
                {
                    if (string.IsNullOrWhiteSpace(id)) continue;
                    if (!_save.unlockedInfoIds.Contains(id))
                        _save.unlockedInfoIds.Add(id);
                }
                OnInfoUnlocked?.Invoke(new List<string>(_save.unlockedInfoIds));
            }

            isActive = false;
            activeLine = null;
            phaseIndex = 0;
            _remaining = _target = 0;
            _pendingById = null;

            PushSnapshot();
            SaveToDisk();
        }

        bool ValidatePhase(PhaseType expected)
        {
            if (!isActive || activeLine == null) return false;
            if (CurrentPhase == null) return false;
            if (CurrentPhase.phaseType != expected) return false;
            return true;
        }

        void ApplyProgress(string idOrNull, int delta)
        {
            if (delta <= 0) delta = 1;

            // Modo por ID
            if (_pendingById != null)
            {
                if (string.IsNullOrEmpty(idOrNull)) return; // esta fase requiere IDs
                var entry = _pendingById.Find(x => x.id == idOrNull);
                if (entry == null) return; // ID no requerido

                entry.amount = Mathf.Max(0, entry.amount - delta);
                _remaining = 0;
                foreach (var x in _pendingById) _remaining += Mathf.Max(0, x.amount);

                PushSnapshot();
                SaveToDisk();

                if (_remaining <= 0) CompleteCurrentPhase();
                return;
            }

            // Modo simple
            _remaining = Mathf.Max(0, _remaining - delta);
            PushSnapshot();
            SaveToDisk();

            if (_remaining <= 0) CompleteCurrentPhase();
        }

        void PushSnapshot()
        {
            var s = new MissionSnapshot();

            s.active = isActive && activeLine != null;
            if (!s.active)
            {
                OnSnapshot?.Invoke(s);
                return;
            }

            s.lineId = activeLine.lineId;
            s.lineName = string.IsNullOrEmpty(activeLine.displayName) ? activeLine.name : activeLine.displayName;

            s.phasesTotal = activeLine.phases != null ? activeLine.phases.Length : 0;
            s.phaseIndex1 = Mathf.Clamp(phaseIndex, 0, s.phasesTotal - 1) + 1;

            var p = CurrentPhase;
            if (p != null)
            {
                s.phaseTitle = p.title;
                s.phaseDesc = p.description;
                s.phaseType = p.phaseType.ToString();
            }

            s.remaining = _remaining;
            s.target = _target;

            OnSnapshot?.Invoke(s);
        }
        #endregion
    }
}

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace ValuableSoundTrigger
{
    [BepInPlugin("LingeringHopes.ValuableSoundTrigger", "ValuableSoundTrigger", "1.0")]
    [BepInDependency("REPOLib", BepInDependency.DependencyFlags.HardDependency)]
    public class ValuableSoundTrigger : BaseUnityPlugin
    {
        internal static ValuableSoundTrigger Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger => Instance._logger;
        private ManualLogSource _logger => base.Logger;
        internal Harmony? Harmony { get; set; }

        static ValuableSoundNetwork valuableSoundNetwork = null;

        void Awake()
        {
            Instance = this;

            // Prevent the plugin from being deleted
            this.gameObject.transform.parent = null;
            this.gameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        void Start()
        {
            InputTriggerManager triggerManager = InputTriggerManager.Instance;
            triggerManager.LoadValuableSoundTriggerActionFromConfig(Config);

            valuableSoundNetwork = new ValuableSoundNetwork();
            valuableSoundNetwork.Initialize(triggerManager);
        }

        internal void Patch()
        {
            Harmony ??= new Harmony(Info.Metadata.GUID);
            Harmony.PatchAll();
        }

        internal void Unpatch()
        {
            Harmony?.UnpatchSelf();
        }

        void OnDestroy()
        {
            Config.Save();
        }
    }
}
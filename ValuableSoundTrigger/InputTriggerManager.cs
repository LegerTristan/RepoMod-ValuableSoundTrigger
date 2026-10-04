using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace ValuableSoundTrigger
{
    public class InputTriggerManager
    {
        readonly Dictionary<InputTriggerCode, string> DEFAULT_TRIGGER_INPUT_KEYS = new Dictionary<InputTriggerCode, string>
        {
            { InputTriggerCode.ListenToValuableSound, "<Keyboard>/g"},
            { InputTriggerCode.PlayBreakLightAudio, "<Keyboard>/numpad1"},
            { InputTriggerCode.PlayBreakMediumAudio, "<Keyboard>/numpad2"},
            { InputTriggerCode.PlayBreakHeavyAudio, "<Keyboard>/numpad3"},
            { InputTriggerCode.PlayImpactLightAudio, "<Keyboard>/numpad4"},
            { InputTriggerCode.PlayImpactMediumAudio, "<Keyboard>/numpad5"},
            { InputTriggerCode.PlayImpactHeavyAudio, "<Keyboard>/numpad6"},
            { InputTriggerCode.PlayPhysGrabDetectAudio, "<Keyboard>/numpad7"},
            { InputTriggerCode.PlayDestroyAudio, "<Keyboard>/numpad8"},
        };

        Dictionary<InputTriggerCode, ConfigEntry<string>> triggerConfigs = null;

        Dictionary<InputTriggerCode, InputAction> triggerActions = null;

        static InputTriggerManager instance = null;

        InputActionMap valuableSoundInputActionMap = null;

        internal static InputTriggerManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new InputTriggerManager();
                }

                return instance;
            }
        }

        public InputTriggerManager()
        {
            valuableSoundInputActionMap = new InputActionMap("ValuableSoundInputActionMap");
            valuableSoundInputActionMap.devices = InputSystem.devices;
            valuableSoundInputActionMap.Enable();

            triggerConfigs = new Dictionary<InputTriggerCode, ConfigEntry<string>>();
            triggerActions = new Dictionary<InputTriggerCode, InputAction>();
        }

        internal void LoadValuableSoundTriggerActionFromConfig(ConfigFile config)
        {
            foreach (InputTriggerCode code in Enum.GetValues(typeof(InputTriggerCode)))
            {
                string configName = code.ToString();
                string defaultKey = DEFAULT_TRIGGER_INPUT_KEYS[code].Split('/')[1];

                ConfigEntry<string> entry = config.Bind("Controls", configName, DEFAULT_TRIGGER_INPUT_KEYS[code].Split('/')[1], $"Key used to {code}");
                
                if (!triggerConfigs.ContainsKey(code))
                {
                    triggerConfigs.Add(code, entry);
                }

                entry.SettingChanged += (sender, args) =>
                {
                    string keyPath = $"<Keyboard>/{entry.Value}";
                    string finalValue = ModifyValuableSoundTriggerActionKey(code, keyPath);
                };
            }
        }
        internal void AddValuableSoundTriggerAction(InputTriggerCode triggerCode, Action<InputAction.CallbackContext> startedCallback = null,
            Action<InputAction.CallbackContext> performedCallback = null, Action<InputAction.CallbackContext> canceledCallback = null)
        {
            if (triggerActions.ContainsKey(triggerCode))
            {
                ValuableSoundTrigger.Logger.LogWarning("Input already exist for => " + triggerCode);
                return;
            }

            valuableSoundInputActionMap.Disable();

            // Get key from configs if possible, else take the default value.
            string keyPath = null;
            if(triggerConfigs.TryGetValue(triggerCode, out ConfigEntry<string> configEntry))
            {
                keyPath = configEntry.Value.Contains("<") ? configEntry.Value : $"<Keyboard>/{configEntry.Value}";
            }
            else
            {
                keyPath = DEFAULT_TRIGGER_INPUT_KEYS[triggerCode];
            }

            InputAction newAction = valuableSoundInputActionMap.AddAction(triggerCode.ToString(), InputActionType.Button, binding: keyPath); 
            if (startedCallback != null)
            {
                newAction.started += startedCallback;
            }
            if (performedCallback != null)
            {
                newAction.performed += performedCallback;
            }
            if (canceledCallback != null)
            {
                newAction.canceled += canceledCallback;
            }

            newAction.Enable();

            triggerActions.Add(triggerCode, newAction);
            valuableSoundInputActionMap.Enable();
        }

        internal string ModifyValuableSoundTriggerActionKey(InputTriggerCode triggerCode, string keyPath)
        {
            if (!triggerActions.ContainsKey(triggerCode) || string.IsNullOrEmpty(keyPath))
            {
                return "";
            }

            InputAction action = triggerActions[triggerCode];
            string previousKeyPath = action.bindings[0].effectivePath;

            action.Disable();
            action.ApplyBindingOverride(0, keyPath);
            action.Enable();

            if (action.controls.Count <= 0)
            {
                ValuableSoundTrigger.Logger.LogWarning($"Invalid key '{keyPath}', reverting to '{previousKeyPath}'");
                action.Disable();
                action.ApplyBindingOverride(0, previousKeyPath);
                action.Enable();

                return previousKeyPath.Contains("/") ? previousKeyPath.Split('/')[1] : previousKeyPath;
            }

            return keyPath.Contains("/") ? keyPath.Split('/')[1] : keyPath;
        }
    }
}




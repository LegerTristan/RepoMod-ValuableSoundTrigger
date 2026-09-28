using System;
using UnityEngine.InputSystem;

namespace ValuableSoundTrigger
{
    public class InputTriggerManager
    {
        enum InputTriggerCode
        {
            Listen = 0,
            BreakLight,
            BreakMedium,
            BreakHeavy,
            ImpactLight,
            ImpactMedium,
            ImpactHeavy,
            PhysGrabDetect,
            Destroy,
        }

        Dictionary<InputTriggerCode, InputAction> triggerActions = null:

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

                return instance
            }
        }

        public InputTriggerManager()
        {
            valuableSoundInputActionMap = new InputActionMap("ValuableSoundInputActionMap");
            triggerActions = new Dictionary<InputTriggerCode, InputAction>();

            listenAction = valuableSoundInputActionMap.AddAction("PlayValuableSound", InputActionType.Button, "<Keyboard>/g");
            InputAction breakLightAudioAction = valuableSoundInputActionMap.AddAction("PlayBreakLightAudio", InputActionType.Button, "<Keyboard>/digit1");
            InputAction breakMediumAudioAction = valuableSoundInputActionMap.AddAction("PlayBreakMediumAudio", InputActionType.Button, "<Keyboard>/digit2");
            InputAction breakHeavyAudioAction = valuableSoundInputActionMap.AddAction("PlayBreakHeavyAudio", InputActionType.Button, "<Keyboard>/digit3");
            InputAction impactLightAudioAction = valuableSoundInputActionMap.AddAction("PlayImpactLightAudio", InputActionType.Button, "<Keyboard>/digit4");
            InputAction impactMediumAudioAction = valuableSoundInputActionMap.AddAction("PlayImpactMediumAudio", InputActionType.Button, "<Keyboard>/digit5");
            InputAction impactHeavyAudioAction = valuableSoundInputActionMap.AddAction("PlayImpactHeavyAudio", InputActionType.Button, "<Keyboard>/digit6");
            InputAction PhysGrabDetectAudioAction = valuableSoundInputActionMap.AddAction("PlayPhysGrabDetecEvent", InputActionType.Button, "<Keyboard>/digit7");
            InputAction DestroyAudioAction = valuableSoundInputActionMap.AddAction("PlayDestroyAudio", InputActionType.Button, "<Keyboard>/digit8");
            valuableSoundInputActionMap.devices = InputSystem.devices;

            listenAction.started += ListenToValuableSoundTriggers;
            listenAction.canceled += UnlistenToValuableSoundTriggers;

            breakLightAudioAction.performed += PlayBreakLightAudio;
            breakMediumAudioAction.performed += PlayBreakMediumAudio;
            breakHeavyAudioAction.performed += PlayBreakHeavyAudio;
            impactLightAudioAction.performed += PlayImpactLightAudio;
            impactMediumAudioAction.performed += PlayImpactMediumAudio;
            impactHeavyAudioAction.performed += PlayImpactHeavyAudio;
            PhysGrabDetectAudioAction.performed += PlayPhysGrabDetecEvent;
            DestroyAudioAction.performed += PlayDestroyAudio;

            valuableSoundInputActionMap.Enable();
            listenAction.Enable();
        }
    }
}
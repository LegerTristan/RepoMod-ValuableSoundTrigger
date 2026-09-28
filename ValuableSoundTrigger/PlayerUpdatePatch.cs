using HarmonyLib;
using UnityEngine.InputSystem;
using REPOLib.Modules;
using ExitGames.Client.Photon;
using UnityEngine;

namespace ValuableSoundTrigger
{    
    internal static class PlayerUpdatePatch
    {
        static NetworkedEvent playValuableSoundEvent = null;

        static bool listening = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerAvatar), "Start")]
        private static void InitializeActions(PlayerAvatar __instance)
        {
            InputTriggerManager triggerManager = InputTriggerManager.Instance;

            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.ListenToValuableSound, startedCallback: ListenToValuableSoundTriggers, canceledCallback: UnlistenToValuableSoundTriggers);
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayBreakLightAudio, startedCallback: PlayBreakLightAudio);
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayBreakMediumAudio, startedCallback: PlayBreakMediumAudio);
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayBreakHeavyAudio, startedCallback: PlayBreakHeavyAudio);
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayImpactLightAudio, startedCallback: PlayImpactLightAudio);
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayImpactMediumAudio, startedCallback: PlayImpactMediumAudio);
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayImpactHeavyAudio, startedCallback: PlayImpactHeavyAudio);
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayPhysGrabDetectAudio, startedCallback: PlayPhysGrabDetecEvent);
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayDestroyAudio, startedCallback: PlayDestroyAudio);

            playValuableSoundEvent = new NetworkedEvent("PlayValuableSound", OnPlayValuableSoundReceived);
        }

        static void OnPlayValuableSoundReceived(EventData data)
        {
            AudioSource valuableAudioSource = (AudioSource)data.CustomData;
            if(valuableAudioSource)
            {
                valuableAudioSource.PlayOneShot();
            }
        }

        static void ListenToValuableSoundTriggers(InputAction.CallbackContext context)
        {
            listening = true;
            ValuableSoundTrigger.Logger.LogInfo("Start listening !");
        }

        static void UnlistenToValuableSoundTriggers(InputAction.CallbackContext context)
        {
            listening = false;
            ValuableSoundTrigger.Logger.LogInfo("Stop listening !");
        }

        static void PlayBreakLightAudio(InputAction.CallbackContext context)
        {
            if (!listening)
            {
                return;
            }

            PhysGrabObject grabbedObject = PhysGrabber.instance?.grabbedPhysGrabObject;
            if (!grabbedObject)
            {
                return;
            }

            if (grabbedObject.TryGetComponent(out ValuableObject valuable) && valuable.audioPreset)
            {
                AudioSource source = valuable.audioPreset.breakLight.Play(grabbedObject.transform.position);
                playValuableSoundEvent.RaiseEvent(source, NetworkingEvents.RaiseAll, SendOptions.SendReliable);
            }
        }

        static void PlaySoundFromValuableObject()

        static void PlayBreakMediumAudio(InputAction.CallbackContext context)
        {
            PhysGrabObject grabbedObject = PhysGrabber.instance?.grabbedPhysGrabObject;
            if (!grabbedObject)
                return;

            if (grabbedObject.TryGetComponent(out ValuableObject valuable) && valuable.audioPreset)
            {
                valuable.audioPreset.breakMedium?.Play(grabbedObject.transform.position);
            }
        }

        static void PlayBreakHeavyAudio(InputAction.CallbackContext context)
        {
            PhysGrabObject grabbedObject = PhysGrabber.instance?.grabbedPhysGrabObject;
            if (!grabbedObject)
                return;

            if (grabbedObject.TryGetComponent(out ValuableObject valuable) && valuable.audioPreset)
            {
                valuable.audioPreset.breakHeavy?.Play(grabbedObject.transform.position);
            }
        }

        static void PlayImpactLightAudio(InputAction.CallbackContext context)
        {
            PhysGrabObject grabbedObject = PhysGrabber.instance?.grabbedPhysGrabObject;
            if (!grabbedObject)
                return;

            if (grabbedObject.TryGetComponent(out ValuableObject valuable) && valuable.audioPreset)
            {
                valuable.audioPreset.impactLight?.Play(grabbedObject.transform.position);
            }
        }

        static void PlayImpactMediumAudio(InputAction.CallbackContext context)
        {
            PhysGrabObject grabbedObject = PhysGrabber.instance?.grabbedPhysGrabObject;
            if (!grabbedObject)
                return;

            if (grabbedObject.TryGetComponent(out ValuableObject valuable) && valuable.audioPreset)
            {
                valuable.audioPreset.impactMedium?.Play(grabbedObject.transform.position);
            }
        }

        static void PlayImpactHeavyAudio(InputAction.CallbackContext context)
        {
            PhysGrabObject grabbedObject = PhysGrabber.instance?.grabbedPhysGrabObject;
            if (!grabbedObject)
                return;

            if (grabbedObject.TryGetComponent(out ValuableObject valuable) && valuable.audioPreset)
            {
                valuable.audioPreset.impactHeavy?.Play(grabbedObject.transform.position);
            }
        }

        static void PlayDestroyAudio(InputAction.CallbackContext context)
        {
            PhysGrabObject grabbedObject = PhysGrabber.instance?.grabbedPhysGrabObject;
            if (!grabbedObject)
                return;

            if (grabbedObject.TryGetComponent(out ValuableObject valuable) && valuable.audioPreset)
            {
                valuable.audioPreset.destroy?.Play(grabbedObject.transform.position);
            }
        }

        static void PlayPhysGrabDetecEvent(InputAction.CallbackContext context)
        {
            PhysGrabObject grabbedObject = PhysGrabber.instance?.grabbedPhysGrabObject;
            if (!grabbedObject)
                return;

            if (grabbedObject.TryGetComponent(out PhysGrabObjectImpactDetector detector))
            {
                detector.onAllBreaks.Invoke();
            }
        }
    }
}

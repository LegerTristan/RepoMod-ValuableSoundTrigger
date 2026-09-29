using ExitGames.Client.Photon;
using HarmonyLib;
using Photon.Pun;
using REPOLib.Modules;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace ValuableSoundTrigger
{   
    class NetworkValuableSoundPlayData
    {
        public Sound SourceSound;

        public Vector3 SourcePosition;

        public NetworkValuableSoundPlayData(Sound valuableSound, Vector3 valuablePosition)
        {
            SourceSound = valuableSound;
            SourcePosition = valuablePosition;
        }
    }

    internal static class PlayerUpdatePatch
    {
        static NetworkedEvent playValuableSoundEvent = null;

        static NetworkedEvent playGrabImpactDetectorAllBreaksEvent = null;

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
            playGrabImpactDetectorAllBreaksEvent = new NetworkedEvent("PlayValuableSoundFromGrabDetector", OnPlayValuableSoundFromGrabDetectorReceived);
        }

        static void OnPlayValuableSoundReceived(EventData data)
        {
            try
            {
                // payload contains [photon view ID, audio to play as an enum "InputTriggerCode" value]
                object[] datas = (object[])data.CustomData;

                int photonViewID = (int)datas[0];
                PhotonView instigatorPhotonView = PhotonView.Find(photonViewID);

                InputTriggerCode audioToPlay = (InputTriggerCode)datas[1];

                ValuableSoundTrigger.Logger.LogInfo($"instigatorPhotonView {instigatorPhotonView} AND valuable {instigatorPhotonView?.GetComponent<ValuableObject>()}");
                if (null != instigatorPhotonView && instigatorPhotonView.TryGetComponent(out ValuableObject valuable) && valuable.audioPreset)
                {
                    ValuableSoundTrigger.Logger.LogInfo($"valuable {valuable} AND valuable.audioPreset {valuable.audioPreset}");

                    switch (audioToPlay)
                    {
                        case InputTriggerCode.PlayBreakLightAudio:
                            valuable.audioPreset.breakLight.Play(valuable.transform.position);
                            break;
                        case InputTriggerCode.PlayBreakMediumAudio:
                            valuable.audioPreset.breakMedium.Play(valuable.transform.position);
                            break;
                        case InputTriggerCode.PlayBreakHeavyAudio:
                            valuable.audioPreset.breakHeavy.Play(valuable.transform.position);
                            break;
                        case InputTriggerCode.PlayImpactLightAudio:
                            valuable.audioPreset.impactLight.Play(valuable.transform.position);
                            break;
                        case InputTriggerCode.PlayImpactMediumAudio:
                            valuable.audioPreset.impactMedium.Play(valuable.transform.position);
                            break;
                        case InputTriggerCode.PlayImpactHeavyAudio:
                            valuable.audioPreset.impactHeavy.Play(valuable.transform.position);
                            break;
                        case InputTriggerCode.PlayDestroyAudio:
                            valuable.audioPreset.destroy.Play(valuable.transform.position);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                ValuableSoundTrigger.Logger.LogError($"Error when playing sound from ValuableObject : {ex}");
            }
        }

        static void OnPlayValuableSoundFromGrabDetectorReceived(EventData data)
        {
            try
            {
                int photonViewID = (int)data.CustomData;
                PhotonView instigatorPhotonView = PhotonView.Find(photonViewID);
                if (null != instigatorPhotonView)
                {
                    instigatorPhotonView.GetComponent<PhysGrabObjectImpactDetector>().onAllBreaks.Invoke();
                }
            }
            catch (System.Exception ex)
            {
                ValuableSoundTrigger.Logger.LogError($"Error when playing sound from PhysGrabObjectImpactDetector : {ex}");
            }

        }

        #region Listening
        static void ListenToValuableSoundTriggers(InputAction.CallbackContext context) => listening = true;

        static void UnlistenToValuableSoundTriggers(InputAction.CallbackContext context) => listening = false;
        #endregion

        static bool RetrieveValuablePhotonViewID(out int photonViewID)
        {
            photonViewID = -1;

            try
            {
                if (!listening)
                {
                    return false;
                }

                PhysGrabObject grabbedObject = PhysGrabber.instance?.grabbedPhysGrabObject;
                if (!grabbedObject)
                {
                    return false;
                }

                if (grabbedObject != null && grabbedObject.TryGetComponent(out ValuableObject valuable))
                {
                    photonViewID = valuable.GetComponent<PhotonView>().ViewID;
                    return true;
                }

                return false;
            }
            catch (System.Exception ex)
            {
                ValuableSoundTrigger.Logger.LogError($"Error when retrieving valuable's photon view ID : {ex}");
                return false;
            }
        }

        #region PlayAudios
        static void PlayBreakLightAudio(InputAction.CallbackContext context)
        {
            if (RetrieveValuablePhotonViewID(out int valuablePhotonViewID))
            {
                playValuableSoundEvent.RaiseEvent(new object[] {valuablePhotonViewID, InputTriggerCode.PlayBreakLightAudio}, NetworkingEvents.RaiseAll, SendOptions.SendReliable);
            }
        }

        static void PlayBreakMediumAudio(InputAction.CallbackContext context)
        {
            if (RetrieveValuablePhotonViewID(out int valuablePhotonViewID))
            {
                playValuableSoundEvent.RaiseEvent(new object[] {valuablePhotonViewID, InputTriggerCode.PlayBreakMediumAudio}, NetworkingEvents.RaiseAll, SendOptions.SendReliable);
            }
        }

        static void PlayBreakHeavyAudio(InputAction.CallbackContext context)
        {
            if (RetrieveValuablePhotonViewID(out int valuablePhotonViewID))
            {
                playValuableSoundEvent.RaiseEvent(new object[] { valuablePhotonViewID, InputTriggerCode.PlayBreakHeavyAudio }, NetworkingEvents.RaiseAll, SendOptions.SendReliable);
            }
        }

        static void PlayImpactLightAudio(InputAction.CallbackContext context)
        {
            if (RetrieveValuablePhotonViewID(out int valuablePhotonViewID))
            {
                playValuableSoundEvent.RaiseEvent(new object[] { valuablePhotonViewID, InputTriggerCode.PlayImpactLightAudio }, NetworkingEvents.RaiseAll, SendOptions.SendReliable);
            }
        }

        static void PlayImpactMediumAudio(InputAction.CallbackContext context)
        {
            if (RetrieveValuablePhotonViewID(out int valuablePhotonViewID))
            {
                playValuableSoundEvent.RaiseEvent(new object[] { valuablePhotonViewID, InputTriggerCode.PlayImpactMediumAudio }, NetworkingEvents.RaiseAll, SendOptions.SendReliable);
            }
        }

        static void PlayImpactHeavyAudio(InputAction.CallbackContext context)
        {
            if (RetrieveValuablePhotonViewID(out int valuablePhotonViewID))
            {
                playValuableSoundEvent.RaiseEvent(new object[] { valuablePhotonViewID, InputTriggerCode.PlayImpactHeavyAudio }, NetworkingEvents.RaiseAll, SendOptions.SendReliable);
            }
        }

        static void PlayDestroyAudio(InputAction.CallbackContext context)
        {
            if (RetrieveValuablePhotonViewID(out int valuablePhotonViewID))
            {
                playValuableSoundEvent.RaiseEvent(new object[] { valuablePhotonViewID, InputTriggerCode.PlayDestroyAudio }, NetworkingEvents.RaiseAll, SendOptions.SendReliable);
            }
        }

        /// <summary>
        /// Specific function for compatibility with SillyValuables mod.
        /// </summary>
        /// <param name="context">Callback context of the action. Not used, you can ignore this.</param>
        static void PlayPhysGrabDetecEvent(InputAction.CallbackContext context)
        {
            try
            {
                // Special case, we cannot use the same base function for this one.
                if (!listening)
                    return;

                PhysGrabObject grabbedObject = PhysGrabber.instance?.grabbedPhysGrabObject;
                if (!grabbedObject)
                    return;

                if (grabbedObject.TryGetComponent(out PhysGrabObjectImpactDetector detector))
                {
                    playGrabImpactDetectorAllBreaksEvent.RaiseEvent(detector.GetComponent<PhotonView>().ViewID, NetworkingEvents.RaiseAll, SendOptions.SendReliable);
                }
            }
            catch (System.Exception ex)
            {
                ValuableSoundTrigger.Logger.LogError($"Error when raising event from PhysGrabObjectImpactDetector : {ex}");
            }
        }
        #endregion
    }
}

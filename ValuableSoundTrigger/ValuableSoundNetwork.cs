using ExitGames.Client.Photon;
using Photon.Pun;
using REPOLib.Modules;
using UnityEngine.InputSystem;

namespace ValuableSoundTrigger
{
    internal class ValuableSoundNetwork
    {
        NetworkedEvent playValuableSoundEvent = null;

        NetworkedEvent playGrabImpactDetectorAllBreaksEvent = null;

        bool listening = false;

        public void Initialize(InputTriggerManager triggerManager)
        {
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.ListenToValuableSound, startedCallback: ListenToValuableSoundTriggers, canceledCallback: UnlistenToValuableSoundTriggers);
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayBreakLightAudio, startedCallback: (context) => PlayAudio(InputTriggerCode.PlayBreakLightAudio));
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayBreakMediumAudio, startedCallback: (context) => PlayAudio(InputTriggerCode.PlayBreakMediumAudio));
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayBreakHeavyAudio, startedCallback: (context) => PlayAudio(InputTriggerCode.PlayBreakHeavyAudio));
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayImpactLightAudio, startedCallback: (context) => PlayAudio(InputTriggerCode.PlayImpactLightAudio));
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayImpactMediumAudio, startedCallback: (context) => PlayAudio(InputTriggerCode.PlayImpactMediumAudio));
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayImpactHeavyAudio, startedCallback: (context) => PlayAudio(InputTriggerCode.PlayImpactHeavyAudio));
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayPhysGrabDetectAudio, startedCallback: PlayPhysGrabDetecEvent);
            triggerManager.AddValuableSoundTriggerAction(InputTriggerCode.PlayDestroyAudio, startedCallback: (context) => PlayAudio(InputTriggerCode.PlayDestroyAudio));

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

                if (null != instigatorPhotonView && instigatorPhotonView.TryGetComponent(out ValuableObject valuable) && valuable.audioPreset)
                {
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
        void ListenToValuableSoundTriggers(InputAction.CallbackContext context) => listening = true;

        void UnlistenToValuableSoundTriggers(InputAction.CallbackContext context) => listening = false;
        #endregion

        bool RetrieveValuablePhotonViewID(out int photonViewID)
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

        void PlayAudio(InputTriggerCode code)
        {
            if (RetrieveValuablePhotonViewID(out int valuablePhotonViewID))
            {
                playValuableSoundEvent.RaiseEvent(new object[] { valuablePhotonViewID, code }, NetworkingEvents.RaiseAll, SendOptions.SendReliable);
            }
        }

        /// <summary>
        /// Specific function for compatibility with SillyValuables mod.
        /// </summary>
        /// <param name="context">Callback context of the action. Not used, you can ignore this.</param>
        void PlayPhysGrabDetecEvent(InputAction.CallbackContext context)
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
    }
}

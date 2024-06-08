using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

namespace NetworkController
{
    public class PhotonPlayerController : MonoBehaviourPun
    {
        public Transform Head;
        public Transform Body;
        public Transform LeftHand;
        public Transform RightHand;
        public Text NameText;
        public bool HideLocalPlayer = true;

        private void Awake()
        {
            if (photonView.IsMine)
            {
                PhotonNetworkController.NetworkController.LocalPlayer = this;
                if (HideLocalPlayer)
                {
                    Head.gameObject.SetActive(false);
                    Body.gameObject.SetActive(false);
                    RightHand.gameObject.SetActive(false);
                    LeftHand.gameObject.SetActive(false);
                    NameText.gameObject.SetActive(false);
                }
            }

            DontDestroyOnLoad(gameObject);

            _RefreshPlayerValues();
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                Head.transform.position = PhotonNetworkController.NetworkController.Head.transform.position;
                Head.transform.rotation = PhotonNetworkController.NetworkController.Head.transform.rotation;

                RightHand.transform.position = PhotonNetworkController.NetworkController.RightHand.transform.position;
                RightHand.transform.rotation = PhotonNetworkController.NetworkController.RightHand.transform.rotation;

                LeftHand.transform.position = PhotonNetworkController.NetworkController.LeftHand.transform.position;
                LeftHand.transform.rotation = PhotonNetworkController.NetworkController.LeftHand.transform.rotation;
            }
        }

        public void RefreshPlayerValues() => photonView.RPC("RPCRefreshPlayerValues", RpcTarget.All);

        [PunRPC]
        private void RPCRefreshPlayerValues()
        {
            _RefreshPlayerValues();
        }

        private void _RefreshPlayerValues()
        {
            if (NameText != null)
                NameText.text = photonView.Owner.NickName;
        }
    }
}
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using NetworkController;
using Unity.Collections.LowLevel.Unsafe;
using System;

public class PhotonNetworkController : MonoBehaviourPunCallbacks
{
    public static PhotonNetworkController NetworkController { get; private set; }


// Photon stuff
    [Header("PhotonServerSettings")]
    [SerializeField] public string punAppId;
    [SerializeField] public string voiceAppId;
    [SerializeField] public string roomName = "Public0001";
    [SerializeField] public int maxRoomSize = 10;

// Player stuff
    [Header("Player")]
    public GameObject[] OfflinePlayer;
    public Text OfflineName;

// Network Player
 [Header("NetPlayer")]
    public Transform Head;
    public Transform LeftHand;
    public Transform RightHand;
    private GameObject playerTemp;
    [Header("The location of the NetworkPlayer")]
    public string PrefabLocation = "GorillaPrefabs/NetworkPlayer";
    [NonSerialized] public PhotonPlayerController LocalPlayer;
    private RoomOptions options;

// Debug stuff
    [Header("Debug Stuff (Leave it alone if you dont want debugging)")]
    public Text DebugText;
    public bool DebugMode = false;

    private void Start()
    {
        if (NetworkController == null)
            NetworkController = this;
        else
        {
            Debug.LogError("There can't be multiple PhotonVRManagers in a scene");
            Application.Quit();
        }
        if (PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime != punAppId)
        {
            Application.Quit();
            InvokeRepeating(nameof(DisconnectSystem), 5f, 5f);
            DebugText.text = "why you trying to steal my game\n <color=red>Bitch</color> you aint a nice person";
            Debug.Log("Frick you stealing my game.");
        }
        else
        {
            ConnectToPhoton();
        }
    }

    private void Awake() => DontDestroyOnLoad(gameObject);



    private void DisconnectSystem()
    {
        PhotonNetwork.Disconnect();
        Debug.Log("Disconnect from <color=blue>photon</color>.");
    }


    private void ConnectToPhoton()
    {
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = punAppId;
        PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice = voiceAppId;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxRoomSize;

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        playerTemp = PhotonNetwork.Instantiate(PrefabLocation, Vector3.zero, Quaternion.identity);
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        
        if (DebugMode == true)
        {
            DebugText.text = "RoomCode: " + PhotonNetwork.CurrentRoom.Name;
        }
//        foreach (GameObject Rig in OfflinePlayer)
//        {
//            Rig.SetActive(false);
//        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Destroy(playerTemp);
        DebugText.text = "NOT CONNECTED TO A ROOM!";
        Debug.Log("Left the room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Shit fuck piss Peepee whineer: " + message);

        if (DebugMode == true)
        {
            DebugText.text = "Join room failed: " + message;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Disconnected from Photon: " + cause.ToString());

        if (DebugMode == true)
        {
            DebugText.text = "Photon Disconnected: " + cause.ToString();
        }
    }

#if UNITY_EDITOR
        public void CheckDefaultValues()
        {
            bool b = CheckForRig(this);
            if (b)
            {
                if (string.IsNullOrEmpty(punAppId))
                    punAppId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdFusion;

                if (string.IsNullOrEmpty(voiceAppId))
                    voiceAppId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice;

                Debug.Log("Attempted to set default values");
            }
        }

    private bool CheckForRig(PhotonNetworkController NetworkController)
    {
        GameObject[] objects = FindObjectsOfType<GameObject>();

        bool b = false;

        if (NetworkController.Head == null)
        {
            b = true;
            foreach (GameObject obj in objects)
            {
                if (obj.name.Contains("Camera") || obj.name.Contains("Head"))
                {
                    NetworkController.Head = obj.transform;
                    break;
                }
            }
        }

        if (NetworkController.LeftHand == null)
        {
            b = true;
            foreach (GameObject obj in objects)
            {
                if (obj.name.Contains("Left") && (obj.name.Contains("Hand") || obj.name.Contains("Controller")))
                {
                    NetworkController.LeftHand = obj.transform;
                    break;
                }
            }
        }

        if (NetworkController.RightHand == null)
        {
            b = true;
            foreach (GameObject obj in objects)
            {
                if (obj.name.Contains("Right") && (obj.name.Contains("Hand") || obj.name.Contains("Controller")))
                {
                    NetworkController.RightHand = obj.transform;
                    break;
                }
            }
        }
    return b;
    }
    #endif


}



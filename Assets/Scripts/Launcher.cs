using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    public GameObject loadingScreen;
    public TMP_Text loadingText;

    public GameObject createRoomScreen;
    public TMP_InputField roomNameInput;

    public GameObject createdRoom;
    public TMP_Text createdRoomText;



    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        loadingScreen.SetActive(true);
        loadingText.text = "Connecting....";

        PhotonNetwork.ConnectUsingSettings();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        loadingText.text = "joining lobby";
    }

    public override void OnJoinedLobby()
    {
        loadingScreen.SetActive(false);
        OpenCreateRoomScreen();
    }

    public void OpenCreateRoomScreen()
    {
        createRoomScreen.SetActive(true);
    }

    public void CreateRoom()
    {
        if(!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 10;

            PhotonNetwork.CreateRoom(roomNameInput.text);

            loadingScreen.SetActive(true);
            loadingText.text = "Creating Room....";
        }
    }

    public override void OnCreatedRoom()
    {
        loadingScreen.SetActive(false);
        createdRoom.SetActive(true);
        createdRoomText.text = PhotonNetwork.CurrentRoom.Name;
    }

    public void LeaveRoom()
    {
        createdRoom.SetActive(false);
        loadingScreen.SetActive(true);
        loadingText.text = "Leaving Room";
        PhotonNetwork.LeaveRoom();
    }

}

using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [Header("Info Status")]
    [SerializeField] TextMeshProUGUI _connectionStatusText;
    [SerializeField] TextMeshProUGUI _pingRateText;

    [Header("Login Panel")]
    [SerializeField] TMP_InputField _playerNameInput;
    [SerializeField] GameObject _loginPanel;

    [Header("Connection Panel")]
    [SerializeField] GameObject _connectionStatusPanel;

    [Header("Connection Options Panel")]
    [SerializeField] GameObject _connectionOptionsPanel;

    [Header("Create Room Panel")]
    [SerializeField] GameObject _createRoomPanel;
    [SerializeField] TMP_InputField _roomNameInputField;
    [SerializeField] TMP_InputField _maxPlayerInputField;

    [Header("Inside Room Panel")]
    [SerializeField] GameObject _insideRoomPanel;
    [SerializeField] TextMeshProUGUI _roomInfoText;
    [SerializeField] GameObject _playerListPrefab;
    [SerializeField] GameObject _playerListContent;
    [SerializeField] GameObject _startGameButton;


    [Header("Room List Panel")]
    [SerializeField] GameObject _roomListPanel;
    [SerializeField] GameObject _roomListEntryPrefab;
    [SerializeField] GameObject _roomListParentGameObject;



    [Header("Join Random Room Panel")]
    [SerializeField] GameObject _joinRandomRoomPanel;

    private Dictionary<string, RoomInfo> _cacheRoomList;
    private Dictionary<string, GameObject> _roomListGameObjects;
    private Dictionary<int, GameObject> _playerListGameObjects;


    #region Unity Methods

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    // Start is called before the first frame update
    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
        ActivatePanel(_loginPanel.name);

        _cacheRoomList = new Dictionary<string, RoomInfo>();
        _roomListGameObjects = new Dictionary<string, GameObject>();

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        _connectionStatusText.text = "Conection Status: " + PhotonNetwork.NetworkClientState;
        _pingRateText.text = "Ping Rate: " + PhotonNetwork.GetPing();

    }

    #endregion Unity Methods







    #region UI Callbacks

    public void OnLoginButtonClicked()
    {
        string playerName = _playerNameInput.text;

        if (string.IsNullOrEmpty(playerName))
        {
            playerName = "Guest" + Random.Range(1000, 10000).ToString();
        }


        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
            ActivatePanel(_connectionStatusPanel.name);
        }
        else
        {
            Debug.Log("Playername is invalid");
        }

    }




    public void OnCreateRoomButtonClicked()
    {
        string roomName = _roomNameInputField.text;

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(100, 10000);
        }

        byte maxPlayer = 20;
        if (!string.IsNullOrEmpty(_maxPlayerInputField.text))
            maxPlayer = (byte)int.Parse(_maxPlayerInputField.text);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayer;

        PhotonNetwork.CreateRoom(roomName, roomOptions);

    }


    public void OnCancelButtonClicked()
    {
        ActivatePanel(_connectionOptionsPanel.name);
    }


    public void OnShowRoomListButtonClicked()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        ActivatePanel(_roomListPanel.name);

    }


    public void OnBackButtonClicked()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        ActivatePanel(_connectionOptionsPanel.name);
    }


    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }


    public void OnJoinRandomRoomButtonClicked()
    {
        ActivatePanel(_joinRandomRoomPanel.name);
        PhotonNetwork.JoinRandomRoom();
    }


    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Game");
    }


    #endregion UI Callbacks





    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.NickName + " conectado al servidor de Photon");
        ActivatePanel(_connectionOptionsPanel.name);
    }


    public override void OnConnected()
    {
        Debug.Log("Conectado a internet");
    }



    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("Error: " + message);
        //CreateAndJoinRoom();


        string roomName = "Room " + Random.Range(100, 10000);

        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = 20
        };


        PhotonNetwork.CreateRoom(roomName, roomOptions);


    }

    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        //PhotonNetwork.LoadLevel("Game");
        ActivatePanel(_insideRoomPanel.name);


        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            _startGameButton.SetActive(true);
        else
            _startGameButton.SetActive(false);




        _roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + "Players/Max Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        if (_playerListGameObjects == null)
            _playerListGameObjects = new Dictionary<int, GameObject>();


        //instanciando player list gameobjects

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListGameObject = Instantiate(_playerListPrefab);
            playerListGameObject.transform.SetParent(_playerListContent.transform);
            playerListGameObject.transform.localScale = Vector3.one;

            playerListGameObject.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text = player.NickName;

            if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
            else
                playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(false);

            _playerListGameObjects.Add(player.ActorNumber, playerListGameObject);

        }


    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + "Players/Max Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;




        Debug.Log(newPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name + " and Count " + PhotonNetwork.CurrentRoom.PlayerCount);

        GameObject playerListGameObject = Instantiate(_playerListPrefab);
        playerListGameObject.transform.SetParent(_playerListContent.transform);
        playerListGameObject.transform.localScale = Vector3.one;

        playerListGameObject.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text = newPlayer.NickName;

        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
        else
            playerListGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(false);

        _playerListGameObjects.Add(newPlayer.ActorNumber, playerListGameObject);


    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " + "Players/Max Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;



        Destroy(_playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        _playerListGameObjects.Remove(otherPlayer.ActorNumber);


        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            _startGameButton.SetActive(true);
        


    }

    public override void OnLeftRoom()
    {
        ActivatePanel(_connectionOptionsPanel.name);

        foreach (GameObject playerListGameObject in _playerListGameObjects.Values)
        {
            Destroy(playerListGameObject);
        }


        _playerListGameObjects.Clear();
        _playerListGameObjects = null;
    }


    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created");

    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();

        foreach (RoomInfo room in roomList)
        {
            Debug.Log(room.Name);

            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (_cacheRoomList.ContainsKey(room.Name))
                {
                    _cacheRoomList.Remove(room.Name); ;
                }
            }
            else
            {
                //update cacheRoomList
                if (_cacheRoomList.ContainsKey(room.Name))
                {
                    _cacheRoomList[room.Name] = room;
                }
                //add new room to the cache
                else
                {
                    _cacheRoomList.Add(room.Name, room);
                }

            }
        }


        foreach (RoomInfo room in _cacheRoomList.Values)
        {
            GameObject roomListEntryGameObject = Instantiate(_roomListEntryPrefab);
            roomListEntryGameObject.transform.SetParent(_roomListParentGameObject.transform);
            roomListEntryGameObject.transform.localScale = Vector3.one;

            roomListEntryGameObject.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = room.Name;
            roomListEntryGameObject.transform.Find("RoomPlayersText").GetComponent<TextMeshProUGUI>().text = room.PlayerCount + " / " + room.MaxPlayers;
            roomListEntryGameObject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomButtonClicked(room.Name) );

            _roomListGameObjects.Add(room.Name, roomListEntryGameObject);
        }
    }


    public override void OnLeftLobby()
    {
        ClearRoomListView();
        _cacheRoomList.Clear();
    }

    



    #endregion Photon Callbacks



    #region Private methods
    private void CreateAndJoinRoom()
    {
        string randomRoomName = "Room " + Random.Range(100, 10000);

        RoomOptions lRoomOpcions = new RoomOptions();
        lRoomOpcions.MaxPlayers = 20;


        PhotonNetwork.CreateRoom(randomRoomName, lRoomOpcions);
    }


    void OnJoinRoomButtonClicked(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        PhotonNetwork.JoinRoom(roomName);


    }

    void ClearRoomListView ()
    {

        foreach (var roomListGameObject in _roomListGameObjects.Values)
        {
            Destroy(roomListGameObject);
        }

        _roomListGameObjects.Clear();
    }

    #endregion Private methods




    #region Public Methods

    public void ActivatePanel(string panelToBeActivated)
    {
        _loginPanel.SetActive(panelToBeActivated.Equals(_loginPanel.name));
        _connectionStatusPanel.SetActive(panelToBeActivated.Equals(_connectionStatusPanel.name));
        _connectionOptionsPanel.SetActive(panelToBeActivated.Equals(_connectionOptionsPanel.name));
        _createRoomPanel.SetActive(panelToBeActivated.Equals(_createRoomPanel.name));
        _insideRoomPanel.SetActive(panelToBeActivated.Equals(_insideRoomPanel.name));
        _roomListPanel.SetActive(panelToBeActivated.Equals(_roomListPanel.name));
        _joinRandomRoomPanel.SetActive(panelToBeActivated.Equals(_joinRandomRoomPanel.name));

    }


    #endregion  Public Methods
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SWNetwork;

public class My_Lobby : MonoBehaviour
{

    public enum LobbyState
    {
        Default,
        JoinedRoom
    }

    public LobbyState State = LobbyState.Default;
    public bool Debugging = false;
    public GameObject PopoverBackground;
    public GameObject EnterNamePopover;
    public GameObject WaitOpponentPopover;
    public GameObject StartGameBtn;
    public GameObject LocalPlayerPic;
    public GameObject remote;
    public GameObject LocalPlayerName;
    public InputField PlayerName;

    string player_name;
    [SerializeField]
    protected Player localPlayer, remotePlayer;
    [SerializeField]
    protected Text localPlayer_name, remotePlayer_name;


    // Start is called before the first frame update
    void Start()
    {
        HideAllPopover();
        NetworkClient.Lobby.OnLobbyConnectedEvent += OnLobbyConnected;
        NetworkClient.Lobby.OnNewPlayerJoinRoomEvent += OnNewPlayerJoinRoomEvent;
        NetworkClient.Lobby.OnRoomReadyEvent += OnRoomReadyEvent;
    }

    void OnDestroy()
    {
        if (NetworkClient.Lobby != null)
        {
            NetworkClient.Lobby.OnLobbyConnectedEvent -= OnLobbyConnected;
            NetworkClient.Lobby.OnNewPlayerJoinRoomEvent -= OnNewPlayerJoinRoomEvent;
        }
    }

    void ShowEnterNamePopover()
    {
        PopoverBackground.SetActive(true);
        EnterNamePopover.SetActive(true);
    }

    void ShowJoinedRoomPopover()
    {
        EnterNamePopover.SetActive(false);
        WaitOpponentPopover.SetActive(true);
        StartGameBtn.SetActive(false);
    }

    void ShowReadyToStartUI()
    {
        LocalPlayerPic.SetActive(true);
        LocalPlayerName.SetActive(true);
        remote.SetActive(true);
        StartGameBtn.SetActive(true);
    }

    public void HideAllPopover()
    {
        PopoverBackground.SetActive(false);
        EnterNamePopover.SetActive(false);
        WaitOpponentPopover.SetActive(false);
        StartGameBtn.SetActive(false);
    }

    void Checkin()
    {
        NetworkClient.Instance.CheckIn(player_name, (bool successful, string error) =>
        {
            if (!successful)
            {
                Debug.LogError(error);
            }
        });
    }

    void RegisterToTheLobbyServer()
    {
        NetworkClient.Lobby.Register(player_name, (successful, reply, error) => {
            if (successful)
            {
                Debug.Log("Lobby registered " + reply);
                if (string.IsNullOrEmpty(reply.roomId))
                {
                    JoinOrCreateRoom();
                }
                else if (reply.started)
                {
                    State = LobbyState.JoinedRoom;
                    ConnectToRoom();
                }
                else
                {
                    State = LobbyState.JoinedRoom;
                    ShowJoinedRoomPopover();
                }
                GetPlayersInTheRoom();
            }
            else
            {
                Debug.Log("Lobby failed to register " + reply);
            }
        });
    }

    void JoinOrCreateRoom()
    {
        NetworkClient.Lobby.JoinOrCreateRoom(false, 2, 60, (successful, reply, error) => {
            if (successful)
            {
                Debug.Log("Joined or created room " + reply);
                State = LobbyState.JoinedRoom;
                ShowJoinedRoomPopover();
                GetPlayersInTheRoom();
            }
            else
            {
                Debug.Log("Failed to join or create room " + error);
            }
        });
    }

    private void setPlayerName()
    {
        NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) =>
        {
            foreach (SWPlayer swPlayer in reply.players)
            {
                string playerName = swPlayer.GetCustomDataString();
                string playerId = swPlayer.id;

                if (playerId.Equals(NetworkClient.Instance.PlayerId))
                {
                    localPlayer.id = playerId;
                    localPlayer.name = playerName;
                    localPlayer_name.text = playerName;
                }
                else
                {
                    remotePlayer.id = playerId;
                    remotePlayer.name = playerName;
                    remotePlayer_name.text = playerName;
                }
            }
        });
    }

    void GetPlayersInTheRoom()
    {
        Netcode netCode = FindObjectOfType<Netcode>();
        NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) =>
        {
            if (successful)
            {
                Debug.Log("Got players " + reply);
                if (reply.players.Count == 1)
                {
                    setPlayerName();
                    LocalPlayerPic.SetActive(true);
                    LocalPlayerName.SetActive(true);
                }
                else
                {
                    setPlayerName();
                    LocalPlayerPic.SetActive(true);
                    LocalPlayerName.SetActive(true);
                    remote.SetActive(true);
                    if (NetworkClient.Lobby.IsOwner)
                    {
                        ShowReadyToStartUI();
                    }
                }
            }
            else
            {
                Debug.Log("Failed to get players " + error);
            }
        });
    }

    void LeaveRoom()
    {
        NetworkClient.Lobby.LeaveRoom((successful, error) => {
            if (successful)
            {
                Debug.Log("Left room");
                State = LobbyState.Default;
            }
            else
            {
                Debug.Log("Failed to leave room " + error);
            }
        });
    }

    void StartRoom()
    {
        NetworkClient.Lobby.StartRoom((successful, error) => {
            if (successful)
            {
                Debug.Log("Started room.");
            }
            else
            {
                Debug.Log("Failed to start room " + error);
            }
        });
    }

    void ConnectToRoom()
    {
        // connect to the game server of the room.
        NetworkClient.Instance.ConnectToRoom((connected) =>
        {
            if (connected)
            {
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                Debug.Log("Failed to connect to the game server.");
            }
        });
    }

    void OnLobbyConnected()
    {
        RegisterToTheLobbyServer();
    }

    void OnNewPlayerJoinRoomEvent(SWJoinRoomEventData eventData)
    {
        if (NetworkClient.Lobby.IsOwner)
        {
            Debug.Log(eventData);
            setPlayerName();
            ShowReadyToStartUI();
        }
    }

    void OnRoomReadyEvent(SWRoomReadyEventData eventData)
    {
        ConnectToRoom();
    }

    public void OnStartGameClicked()
    {
        Debug.Log("Start Game btn Clicked");
        ShowEnterNamePopover();
    }

    public void OnCancelClicked()
    {
        Debug.Log("OnCancelClicked");

        if (State == LobbyState.JoinedRoom)
        {
            // TODO: leave room.
            LeaveRoom();
        }

        HideAllPopover();
    }

    public void OnStartRoomClicked()
    {
        Debug.Log("OnStartRoomClicked");
        // players are ready to player now.
        if (Debugging)
        {
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            // Start room
            StartRoom();
        }
    }

    public void OnConfirmNicknameClicked()
    {
        player_name = PlayerName.text;
        Debug.Log($"OnConfirmNicknameClicked: {player_name}");

        if (Debugging)
        {
            ShowJoinedRoomPopover();
            ShowReadyToStartUI();
        }
        else
        {
            //Use nickname as player custom id to check into SocketWeaver.
            Checkin();
        }
    }

}

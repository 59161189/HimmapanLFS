using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SWNetwork;
using System;

public class GamePlay : MonoBehaviour
{
    /*HUD*/
    [SerializeField]
    protected Text localPlayer_name;
    [SerializeField]
    protected Text remotePlayer_name;
    public HPBar localHealthbar;
    public HPBar remoteHealthbar;
    public Slider localManaBar;
    public Slider remoteManaBar;
    public GameObject battphaseBtn;
    public GameObject endphaseBtn;
    public Text PlayerTurnText;
    public GameObject win, lose, exitMenu;

    Netcode netCode;
    RemoteEventAgent remoteEventAgent;
    public GamedataManager gm;
    
    public enum GameState
    {
        Idle,
        TurnStarted,
        SummonPhase,
        AttackPhase,
        WaitingOpponent,
        OpponentAttack,
        EndPhase,
        GameFinished
    };

    [SerializeField]
    protected GameState gameState = GameState.Idle;

    public void Awake()
    {
        win.SetActive(false);
        lose.SetActive(false);
        Debug.Log("Awaked");
        netCode = FindObjectOfType<Netcode>();
        remoteEventAgent = gameObject.GetComponent<RemoteEventAgent>();
        //ตั้งค่าชื่อผู้เล่น
        NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) =>
        {
            if (successful)
            {
                foreach (SWPlayer swPlayer in reply.players)
                {
                    string playerName = swPlayer.GetCustomDataString();
                    string playerId = swPlayer.id;

                    if (playerId.Equals(NetworkClient.Instance.PlayerId))
                    {
                        Debug.Log("local player name : " + playerName);
                        localPlayer_name.text = playerName;
                        gm.setHostPlayer(playerId, playerName);
                        Debug.Log("set " + gm.hostPlayer.getName() + " to host player.");
                    }
                    else
                    {
                        Debug.Log("remote player name : " + playerName);
                        remotePlayer_name.text = playerName;
                        gm.setRemotePlayer(playerId, playerName);
                        Debug.Log("set " + gm.remotePlayer.getName() + " to remote player.");
                    }
                }
            }
            else
            {
                Debug.Log("Failed to get players in room.");
            }
            if (NetworkClient.Lobby.IsOwner)
            {
                gm.setCurrentPlayer(gm.hostPlayer.getId());
                gm.setCurrentTargetPlayer(gm.remotePlayer.getId());
                Debug.Log("set " + gm.hostPlayer.getName() + " to current player.");
                Debug.Log("Current player is set. His name is " + gm.currentTurn);
                PlayerTurnText.text = "Your turn";
                OnTurnStarted();
            }
            else
            {
                gm.setCurrentPlayer(gm.remotePlayer.getId());
                gm.setCurrentTargetPlayer(gm.hostPlayer.getId());
                Debug.Log("Current Target player is set. His name is " + gm.currentTurn);
                PlayerTurnText.text = "Opponent's turn";
                OnWaitingOpponent();
            }
        });
    }

    // Start is called before the first frame update
    public void Start()
    {
        
    }

    // เริ่มเทิร์นผู้เล่น
    public void OnTurnStarted()
    {
        gm.onSummonPhase = true;
        if (gm.turnCount<5)
            gm.turnCount += 1;
        netCode.UpdatePlayerMana(gm.currentTurn, gm.turnCount, "add");
        Debug.Log("mana updated");
        OnSummonPhase();
    }

    public void OnSummonPhase()
    {
        gameState = GameState.AttackPhase;
        gm.onSummonPhase = true;
        showBtn("battle");
    }
    
    public void OnBattlePhase()
    {
        gm.onSummonPhase = false;
        gm.canAttack = true;
        gm.onBattlePhase = true;
        showBtn("end");
    }

    public void OnWaitingOpponent()
    {
        hideAllButton();
    }

    public void hideAllButton()
    {
        showBtn("none");
    }

    public void OnEndPhase()
    {
        showBtn("none");
        gameState = GameState.WaitingOpponent;
        gm.clearCardHasAttack();
        netCode.NotifyPlayersTurnSwitched();
        gm.canAttack = false;
        gm.onBattlePhase = false;
        gm.onWaitingTarget= false;
        gm.clearCardHasAttack();
        gm.resetAttackAndHealth();
    }

    public void dealDamageToPlayer(string playerId, int damage)
    {
        if (playerId.Equals(gm.hostPlayer.getId()))
            localHealthbar.hp_decrease(damage);
        else if (playerId.Equals(gm.remotePlayer.getId()))
            remoteHealthbar.hp_decrease(damage);
    }

    public void SwitchTurn()
    {
        if (gm.currentTurn.Equals(null) || gm.currentTurn.Equals(gm.remotePlayer.getId()))
        {
            PlayerTurnText.text = "Your turn";
            gm.setCurrentPlayer(gm.hostPlayer.id);
            gm.setCurrentTargetPlayer(gm.remotePlayer.id);
            OnTurnStarted();
        }
        else if (gm.currentTurn.Equals(gm.hostPlayer.getId()))
        {
            Debug.Log(gm.currentTurn);
            gm.setCurrentPlayer(gm.remotePlayer.id);
            gm.setCurrentTargetPlayer(gm.hostPlayer.id);
            PlayerTurnText.text = "Opponent's turn";
            OnWaitingOpponent();
        }
    }

    public void showBtn(string cases)
    {
        switch (cases)
        {
            case "battle":
                {
                    battphaseBtn.SetActive(true);
                    endphaseBtn.SetActive(true);
                    break;
                }
            case "end":
                {
                    battphaseBtn.SetActive(false);
                    endphaseBtn.SetActive(true);
                    break;
                }
            case "none":
                {
                    battphaseBtn.SetActive(false);
                    endphaseBtn.SetActive(false);
                    break;
                }
        }
    }

    //Netcode
    public void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void showMenu()
    {
        exitMenu.SetActive(true);
    }

    public void hideMenu()
    {
        exitMenu.SetActive(false);
    }

    public void UpdateMana(SWNetworkMessage msg)
    {
        string player = msg.PopUTF8ShortString();
        int manaRemain = Int16.Parse(msg.PopUTF8ShortString());
        string solution = msg.PopUTF8ShortString();
        
        if (solution.Equals("remove"))
        {
            gm.reduceMana(player, manaRemain);
        }
        else if (solution.Equals("add"))
        {
            gm.refreshMana(player, manaRemain);
        }
        netCode.UpdateHUD();
    }

    public void OnAttack(SWNetworkMessage msg)
    {
        int damage = msg.PopInt32();
        int health = msg.PopInt32();
        string hasMon = msg.PopUTF8ShortString();
        Debug.Log("recieved damage " + damage + " units");
        if (hasMon.Equals("DoesntHasMon"))
        {
            Debug.Log(hasMon);
            //checkText.text = hasMon;
            if (gm.currentTurn.Equals(gm.hostPlayer.getId()))
            {
                gm.remotePlayer.setHp(gm.remotePlayer.getHp() - damage);
                remoteHealthbar.hp_decrease(damage);
            }
            else if (gm.currentTurn.Equals(gm.remotePlayer.getId()))
            {
                gm.hostPlayer.setHp(gm.hostPlayer.getHp() - damage);
                localHealthbar.hp_decrease(damage);
            }
        }
        else if (hasMon.Equals("HasMon"))
        {
            //TODO ให้มอนต่อสู้กัน
        }

        if (!gm.checkWinner().Equals(""))
        {
            if (gm.checkWinner().Equals(gm.hostPlayer.getId()))
            {
                Debug.Log(gm.hostPlayer.getName() + " wins..");
                win.SetActive(true);
            }
            else if (gm.checkWinner().Equals(gm.remotePlayer.getId()))
            {
                Debug.Log(gm.remotePlayer.getName() + " wins..");
                lose.SetActive(true);
            }
        }
    }

    public void updateHUD()
    {
            localHealthbar.setHealth(gm.hostPlayer.getHp());
            remoteHealthbar.setHealth(gm.remotePlayer.getHp());
            localManaBar.value = gm.hostPlayer.getMana();
            remoteManaBar.value = gm.remotePlayer.getMana();
    }

    public void addCardToOwner(SWNetworkMessage msg)
    {
        string owner = msg.PopUTF8ShortString();
        string cardId = msg.PopUTF8ShortString();
        gm.addCardToOwner(owner, cardId);
    }

    public void setOwnerTurnCount()
    {
        if (gm.turnCount < 5)
            gm.turnCount += 1;
    }

    public void loadLobbyScene()
    {
        NetworkClient.Instance.DisconnectFromRoom();
        NetworkClient.Lobby.LeaveRoom((successful, error) => {
            if (successful)
            {
                Debug.Log("Left room");
                SceneManager.LoadScene("LobbyScene");
            }
            else
            {
                Debug.Log("Failed to leave room " + error);
            }
        });
    }
}
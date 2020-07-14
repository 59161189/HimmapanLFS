using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWNetwork;
using UnityEngine.Events;
using System;

[Serializable]
public class UpdateManaEvent : UnityEvent<SWNetworkMessage>{ }

[Serializable]
public class AddCardToOwner : UnityEvent<SWNetworkMessage>{ }

[Serializable]
public class AttackEvent : UnityEvent<SWNetworkMessage>{ }

[Serializable]
public class UpdateOwnerTurnCount : UnityEvent { }

[Serializable]
public class AddAttackCardRemoteEvent : UnityEvent<SWNetworkMessage> { }

[Serializable]
public class AddTargetCardRemoteEvent : UnityEvent<SWNetworkMessage> { }

[Serializable]
public class SendMessageRemoteEvent : UnityEvent<SWNetworkMessage> { }



public class Netcode : MonoBehaviour
{
    public UnityEvent OnTurnSwitched = new UnityEvent();
    public UnityEvent OnLeftRoom = new UnityEvent();
    public AttackEvent OnAttack = new AttackEvent();
    public UpdateManaEvent OnUpdateManaEvent = new UpdateManaEvent();
    public AddCardToOwner OnAddCardToOwner = new AddCardToOwner();
    public UpdateOwnerTurnCount OnUpdateOwnerTurnCount = new UpdateOwnerTurnCount();
    public AddAttackCardRemoteEvent OnAddAttackCard = new AddAttackCardRemoteEvent();
    public AddTargetCardRemoteEvent OnAddTargetCard = new AddTargetCardRemoteEvent();
    public SendMessageRemoteEvent OnSendMessageRemoteEvent = new SendMessageRemoteEvent();

    RoomRemoteEventAgent roomRemoteEventAgent;

    const string GAME_STATE_CHANGED = "GameStateChanged";
    const string OPPONENT_CONFIRMED = "OpponentConfirmed";
    const string LEAVE_ROOM_WORKED = "LeaveRoom";
    const string UPDATE_MANA = "UpdateMana";
    const string UPDATE_TURNCOUNT = "UpdateOwnerTurnCount";
    const string UPDATE_HUD = "UpdateHUD";
    const string TURN_SWITCHED = "TurnSwitched";
    const string CARD_HAS_OWNER = "NotifyCardHasOwner";

    private void Awake()
    {
        roomRemoteEventAgent = FindObjectOfType<RoomRemoteEventAgent>();
    }

    //call room event
    public void NotifyHostPlayerOpponentConfirmed()
    {
        roomRemoteEventAgent.Invoke(OPPONENT_CONFIRMED);
    }

    public void NotifyOtherPlayerGameStateChanged()
    {
        roomRemoteEventAgent.Invoke(GAME_STATE_CHANGED);
    }

    public void UpdateHUD()
    {
        roomRemoteEventAgent.Invoke(UPDATE_HUD);
    }

    public void updateOwnerTurnCount()
    {
        roomRemoteEventAgent.Invoke(UPDATE_TURNCOUNT);
    }

    public void UpdatePlayerMana(string player, int manaRemain, string solution)
    {
        SWNetworkMessage msg = new SWNetworkMessage();
        msg.PushUTF8ShortString(player);
        msg.PushUTF8ShortString(manaRemain.ToString());
        msg.PushUTF8ShortString(solution);
        roomRemoteEventAgent.Invoke(UPDATE_MANA, msg);
    }

    public void AddCardToPlayer(string owner, string cardName)
    {
        SWNetworkMessage msg = new SWNetworkMessage();
        msg.PushUTF8ShortString(owner);
        msg.PushUTF8ShortString(cardName);
        roomRemoteEventAgent.Invoke(CARD_HAS_OWNER, msg);
    }

    public void NotifyPlayersTurnSwitched()
    {
        Debug.Log("Turn Switched.");
        roomRemoteEventAgent.Invoke(TURN_SWITCHED);
    }

    public void NotifyOpponentAttack(SWNetworkMessage msg)
    {
        roomRemoteEventAgent.Invoke("Attack", msg);
    }

    public void SetAttackCardEvent()
    {
        roomRemoteEventAgent.Invoke("Selecting");
    }

    public void AddAttackCardRemoteEvent(SWNetworkMessage msg)
    {
        roomRemoteEventAgent.Invoke("AddAttackCard", msg);
    }
    public void AddTargetCardRemoteEvent(SWNetworkMessage msg)
    {
        roomRemoteEventAgent.Invoke("AddAttackCard", msg);
    }

    public void LeaveRoom()
    {
        NetworkClient.Instance.DisconnectFromRoom();
        NetworkClient.Lobby.LeaveRoom((successful, error) => {

            if (successful)
            {
                Debug.Log("Left room");
            }
            else
            {
                Debug.Log($"Failed to leave room {error}");
            }

            OnLeftRoom.Invoke();
        });
    }

    //call game handler functions

    public void OnTurnSwitchedRemoteEvent()
    {
        OnTurnSwitched.Invoke();
    }

    public void OnAttackRemoteEvent(SWNetworkMessage msg)
    {
        OnAttack.Invoke(msg);
    }

    public void OnUpdateManaRemoteEvent(SWNetworkMessage msg)
    {
        OnUpdateManaEvent.Invoke(msg);
    }

    public void OnSetCardToOwnerRemoteEvent(SWNetworkMessage msg)
    {
        OnAddCardToOwner.Invoke(msg);
    }

    public void updateOwnerTurnCountRemoteEvent()
    {
        OnUpdateOwnerTurnCount.Invoke();
    }

    public void AddAttackCard(SWNetworkMessage cardStat)
    {
        OnAddAttackCard.Invoke(cardStat);
    }

    public void AddTargetCard(SWNetworkMessage cardStat)
    {
        OnAddTargetCard.Invoke(cardStat);
    }

}

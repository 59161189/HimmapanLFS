using SWNetwork;
using UnityEngine;

public class GamedataManager : MonoBehaviour
{
    public Player hostPlayer, remotePlayer;
    public string currentTurn, currentTargetPlayer, winner;
    public int attackCard_attack, targetCard_attack, attackCard_health, targetCard_health;
    public Card[] hasAttackCard = new Card[2];
    public int turnCount = 0;
    public int targetTurnCount = 0;
    public bool canAttack = false;
    public bool onSummonPhase = false;
    public bool onBattlePhase = false;
    public bool onWaitingTarget = false;

    public bool currentTargetHasMon()
    {
        bool hasMon;
        if (currentTargetPlayer.Equals(hostPlayer.getId()))
            hasMon = hostPlayer.getHasMon();
        else
            hasMon = remotePlayer.getHasMon();
        return hasMon;
    }

    public void setTurnCount()
    {
        turnCount += 1;
    }
    
    public void setHostPlayer(string id, string name)
    {
        hostPlayer.setId(id);
        hostPlayer.setName(name);
    }

    public void setRemotePlayer(string id, string name)
    {
        remotePlayer.setId(id);
        remotePlayer.setName(name);
    }

    public bool getPlayerHasMon()
    {
        bool checkHasMon;
        if (currentTurn.Equals(hostPlayer.getId()))
        {
            checkHasMon = remotePlayer.hasMon;
        }
        else
            checkHasMon = hostPlayer.hasMon;
        return checkHasMon;
    }

    public void setCardHasAttack(Card card)
    {
        for (int i = 0; i < hasAttackCard.Length; i++)
        {
            if (hasAttackCard[i] = null)
            {
                hasAttackCard[i] = card;
                break;
            }
        }
        // กำหนดให้ผู้เล่นโจมตีได้เทิร์นละครั้งเท่านั้น
        canAttack = false;
    }

    public void clearCardHasAttack()
    {
        for (int i = 0; i < hasAttackCard.Length; i++)
        {
            hasAttackCard[i] = null;
        }
    }

    public bool checkCardHasAttack(Card card)
    {
        bool result = false;
        for (int i = 0; i < hasAttackCard.Length; i++)
        {
            if (hasAttackCard[i] == card)
            {
                result = true;
            }
        }
        return result;
    }

    public void addCardToOwner(string owner, string cardId)
    {
        if (owner.Equals(hostPlayer.getId()))
        {
            hostPlayer.addCard(cardId);
            hostPlayer.setHasMon();
        }
        else if (owner.Equals(remotePlayer.getId()))
        {
            remotePlayer.addCard(cardId);
            remotePlayer.setHasMon();
        }
        Debug.Log("card added");
    }

    public void removeCardFromOwner(Player owner, string cardId)
    {
        owner.removeCard(cardId);
    }

    public bool checkRemainingMana(string owner, int toUseMana)
    {
        bool remain;
        if (owner.Equals(hostPlayer.getId()))
        {
            if (hostPlayer.getMana() >= toUseMana)
            {
                remain = true;
            }
            else
                remain = false;
        }
        else if (owner.Equals(remotePlayer.getId()))
        {
            if (remotePlayer.getMana() >= toUseMana)
            {
                remain = true;
            }
            else
                remain = true;
        }
        else
            remain = false;
        return remain;
    }

    public void refreshMana(string owner, int manaRemain)
    {
        if (owner.Equals(hostPlayer.getId()))
            hostPlayer.setMana(manaRemain);
        else
            remotePlayer.setMana(manaRemain);
    }

    public void setPlayerHealth()
    {
        hostPlayer.setHealth();
        remotePlayer.setHealth();
    }

    public void reduceMana(string owner, int manaUsed)
    {
        if (owner.Equals(hostPlayer.getId()))
            hostPlayer.setMana(hostPlayer.getMana() - manaUsed);
        else
            remotePlayer.setMana(remotePlayer.getMana() - manaUsed);
    }

    public void setCurrentPlayer(string playerId)
    {
        currentTurn = playerId;
    }

    public void setCurrentTargetPlayer(string playerId)
    {
        currentTargetPlayer = playerId;
    }

    //ต่อสู้
    public void directAttack()
    {
        if (currentTurn.Equals(hostPlayer.getId()))
        {
            hostPlayer.setHp(hostPlayer.getHp() - attackCard_attack);
        } else
        {
            remotePlayer.setHp(remotePlayer.getHp() - attackCard_attack);
        }
    }

    public void damageToOpponent(int damage)
    {
        remotePlayer.setHp(remotePlayer.getHp() - damage);
    }

    public void damageToHost(int damage)
    {
        hostPlayer.setHp(hostPlayer.getHp() - damage);
    }

    public void makeFight()
    {
        SWNetworkMessage msg = new SWNetworkMessage();
        //TODO quick attack ใน runeterra?
        //TODO ประกาศว่าใครตายหลังจบการต่อสู้
        Debug.Log("make fight");
        if (attackCard_attack >= targetCard_health)
        {
            //การ์ดที่โจมตีชนะ
            msg.PushUTF8ShortString("win fight");
            if (currentTurn.Equals(hostPlayer.getId()))
            {
                damageToOpponent(attackCard_attack - targetCard_health);
            }else 
                damageToHost(attackCard_attack - targetCard_health);
            Debug.Log("win fight");
        }
        else if (attackCard_attack < targetCard_health && targetCard_attack >= attackCard_health)
        {
            msg.PushUTF8ShortString("lose fight");
            if (currentTurn.Equals(hostPlayer.getId()))
            {
                damageToHost(attackCard_attack - targetCard_health);
            }
            else
                damageToOpponent(attackCard_attack - targetCard_health);
            Debug.Log("lose fight");
        }
        else if (attackCard_attack < targetCard_health && targetCard_attack < attackCard_health)
        {
            msg.PushUTF8ShortString("draw");
            Debug.Log("draw");
        }
    }

    public void resetAttackAndHealth()
    {
        attackCard_attack = 0;
        attackCard_health = 0;
        targetCard_attack = 0;
        targetCard_health = 0;
    }

    public string checkWinner()
    {
        if (hostPlayer.getHp() <= 0)
        {
            return remotePlayer.getId();
        }
        else if(remotePlayer.getHp() <= 0)
            return hostPlayer.getId();
        return "";
    }

    public void setAttackCard(SWNetworkMessage msg)
    {
        attackCard_attack = msg.PopInt16();
        attackCard_health = msg.PopInt16();
    }

    public void clearAttackCard()
    {
        attackCard_attack = 0;
        attackCard_health = 0;
    }

    public void setTargetCard(SWNetworkMessage msg)
    {
        targetCard_attack = msg.PopInt16();
        targetCard_health = msg.PopInt16();
        makeFight();
        clearAttackCard();
        clearTargetCard();
    }
    public void clearTargetCard()
    {
        targetCard_attack = 0;
        targetCard_health = 0;
    }
    
}
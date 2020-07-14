using UnityEngine;
using Vuforia;
using SWNetwork;

[System.Obsolete]
public class Naga_card : Card, IVirtualButtonEventHandler
{
    public Naga_card()
    {
        attack = 350;
        health = 300;
        manaCost = 4;
        cardName = "naga";
    }

    public GamedataManager gm;
    public Netcode netcode;
    VirtualButtonBehaviour[] virtualButtonBehaviours;
    string buttonEvent;

    // Start is called before the first frame update
    void Start()
    {
        virtualButtonBehaviours = GetComponentsInChildren<VirtualButtonBehaviour>();
        for (int i = 0; i < virtualButtonBehaviours.Length; i++)
        {
            virtualButtonBehaviours[i].RegisterEventHandler(this);
        }
    }

    public void OnTracked()
    {
        if (gm.onSummonPhase)
        {
            if (!gm.hostPlayer.checkDeployedCard(cardName) && gm.hostPlayer.amountOfCardOnField() < 2)
            {
                if (gm.currentTurn.Equals(gm.hostPlayer.getId()))
                {
                    owner = gm.currentTurn;
                    if (gm.currentTurn.Equals(gm.hostPlayer.getId()))
                    {
                        if (gm.checkRemainingMana(owner, manaCost))
                        {
                            netcode.UpdatePlayerMana(owner, manaCost, "remove");
                            netcode.AddCardToPlayer(owner, cardName);
                        }
                    }
                }
            }
        }
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        buttonEvent = vb.VirtualButtonName;
        if (gm.currentTurn.Equals(gm.hostPlayer.getId()))
        {
            if (buttonEvent.Equals("attackBtn") && gm.canAttack)
            {
                if (gm.canAttack)
                {
                    showAnimation("attack");
                    SWNetworkMessage msg = new SWNetworkMessage();
                    msg.Push(attack);
                    msg.Push(health);
                    msg.PushUTF8ShortString("DoesntHasMon");
                    netcode.NotifyOpponentAttack(msg);
                    gm.canAttack = false;
                }
            }
            else if (buttonEvent.Equals("targetBtn") && !gm.onWaitingTarget)
            {
                Debug.Log("target selected!!!!");
                SWNetworkMessage msg = new SWNetworkMessage();
                msg.Push(attack);
                msg.Push(health);
                netcode.AddTargetCard(msg);
                showAnimation("hit");
            }
        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        buttonEvent = vb.VirtualButtonName;
        if (buttonEvent.Equals("attackBtn"))
        {
            anim.SetBool("isAttack", false);
        }
        else if (buttonEvent.Equals("targetBtn"))
        {
            //gm.makeFight();
            anim.SetBool("isHit", false);
        }
    }

    public void resetAttackPermission()
    {
        canAttack = true;
    }

    public void OnDead()
    {
        anim.SetBool("isDead", true);
        model.SetActive(false);
        attackBtn.SetActive(false);
        targetBtn.SetActive(false);
    }

    public void updateAttackStatus()
    {

    }
}
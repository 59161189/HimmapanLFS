using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player : IEquatable<Player>
{
    public string id;
    public string name;
    private int hp = 1000;
    private int mana = 0;
    //public string[] cards = new string[2];
    public string[]  cards = {"",""};
    public bool hasMon = false;

    public bool Equals(Player other)
    {
        if (id.Equals(other.id))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void setId(string id)
    {
        this.id = id;
    }

    public string getId()
    {
        return id;
    }

    public void setName(string name)
    {
        this.name = name;
    }

    public string getName()
    {
        return name;
    }

    public void setHp(int new_hp)
    {
        hp = new_hp;
    }

    public int getHp()
    {
        return hp;
    }

    public void setMana(int manaRemain)
    {
        mana = manaRemain;
    }

    public int getMana()
    {
        return mana;
    }

    public void addCard(string newCard)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].Equals(""))
            {
                cards[i] = newCard;
                break;
            }
        }
    }

    public void removeCard(string cardToRemoveId)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].Equals(cardToRemoveId))
            {
                cards[i] = "";
            }
        }
    }

    public void setHasMon()
    {
        if (cards.Length == 0)
            hasMon = false;
        else
            hasMon = true;
    }

    public bool getHasMon()
    {
        if (cards.Length == 0)
            hasMon = false;
        else
            hasMon = true;
        return hasMon;
    }

    internal void setHealth()
    {
        throw new NotImplementedException();
    }

    //เช็คว่าดาร์ดใบนี้อยู่บนสนามอยู่หรือไม่
    public bool checkDeployedCard(string cardID)
    {
        bool result = false;
        if (cards.Length != 0)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].Equals(cardID))
                {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }

    //นับการ์ดบนสนามตัวเอง
    public int amountOfCardOnField()
    {
        int amount = 0;
        for (int i = 0; i < cards.Length; i++)
        {
            if (!cards[i].Equals(""))
            {
                amount++;
            }
        }
        return amount;
    }
}

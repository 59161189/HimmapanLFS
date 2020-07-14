using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    public Slider bar;

    public void mana_decrease(int cost)
    {
        bar.value -= cost;
    }

    public void reset_mana()
    {
        bar.value = 5;
    }
}

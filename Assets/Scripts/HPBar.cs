using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Slider bar;
    public Gradient gradient;
    public Image fill;

    public void start()
    {
        fill.color = gradient.Evaluate(1f);
    }

    public void hp_decrease(int damage)
    {
        bar.value -= damage;
        fill.color = gradient.Evaluate(bar.normalizedValue);
    }

    public void hp_increase(int hp)
    {
        bar.value += hp;
        fill.color = gradient.Evaluate(bar.normalizedValue);
    }

    public void setHealth(int health)
    {
        bar.value = health;
    }
}

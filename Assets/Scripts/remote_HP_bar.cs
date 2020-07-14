using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class remote_HP_bar : MonoBehaviour
{

    public Slider bar;
    
    // Start is called before the first frame update
    void start()
    {
        bar.value = 1000;
    }

    public void decrease(int damage)
    {
        bar.value -= damage;
    }
}

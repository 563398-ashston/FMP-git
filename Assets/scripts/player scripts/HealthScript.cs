using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    public int health;
    public int maxHealth = 100;
    public Slider slider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

    }

    private void Start()
    {
        health = maxHealth;
    }

    public void Update()
    {
        if (slider != null)
        {
            slider.value = health;
            slider.maxValue = maxHealth;

        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        slider.value = health;
        
        if (health <= 0)
        {
            Destroy(gameObject, 4f);
        }
        
    }

    public void ResetHealth()
    {
        health = maxHealth;
    }
}

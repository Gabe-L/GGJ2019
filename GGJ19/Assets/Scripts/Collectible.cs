using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{

    public enum Type
    {
        Health,
        Fuel
    }

    public Type type;
    public float amount;

    private void Awake()
    {
        
    }

    private void Collect(Spaceship spaceship)
    {
        switch (type)
        {
            case Type.Health:
                spaceship.health += amount;
                break;

            case Type.Fuel:
                spaceship.fuel += amount;
                break;

            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var spaceship = other.gameObject.GetComponent<Spaceship>();

        if (other.gameObject == GameObject.Find("Spaceship"))
        {
            Collect(spaceship);
            Destroy(gameObject);
        }
    }

}

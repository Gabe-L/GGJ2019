using UnityEngine;

public class Enemies : MonoBehaviour
{

    public GameObject Enemy;
    public float repeatTime = 10f;





    private void Start()
    {
        InvokeRepeating("Enemy", 10f, repeatTime);
    }

    void Spawn()
    {
        Instantiate(Enemy, transform.position, Quaternion.identity);
        Debug.Log("Enemy has spawned");
    }





}

    



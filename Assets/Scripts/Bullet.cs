using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public Game game;
    public Rigidbody rb;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponents<Enemy>().Length > 0)
        {
            foreach (Enemy enemy in collision.gameObject.GetComponents<Enemy>())
            {
                Destroy(enemy.gameObject);
            }

            game.setShootPossible(true);

            Destroy(gameObject);
        }
        if (collision.gameObject.GetComponents<Ground>().Length > 0)
        {
            game.setShootPossible(true);

            Destroy(gameObject);
        }
    }
}

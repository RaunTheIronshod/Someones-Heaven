using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : MonoBehaviour
{
	public Player player;

    public int damage;

    public Enemy enemy;

	public void OnTriggerEnter(Collider other) {

		if (other.tag == "Player") {
			player.TakeDamage(1);
			Debug.Log("takedamage");
		}

        if (other.tag == "Enemy")
        {
            enemy = other.gameObject.GetComponent<Enemy>();
            enemy.currentTrap = this.gameObject.GetComponent<Traps>();
            enemy.TakeDamage(1);
            Debug.Log("enemytakedamage");
        }
    }
}

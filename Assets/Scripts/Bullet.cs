using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public Vector2 Direction { get; set; }
    [SerializeField] private float _speed = 10f;
    public int Damage { get; set; }
    public string Owner { get; set; }

    private void Awake() {
        Damage = 1;
    }
    // Update is called once per frame
    void Update() {
        transform.Translate(Direction * Time.deltaTime * _speed);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(Owner)) {
            // can't shoot my owner!
            return;
        }

        if (other.CompareTag("Player")) {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(Damage);
            Destroy(this.gameObject);
        } else if (other.CompareTag("Enemy")) {
            SentryBot bot = other.gameObject.GetComponent<SentryBot>();
            bot.TakeDamage(Damage);
            Destroy(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
    }
}

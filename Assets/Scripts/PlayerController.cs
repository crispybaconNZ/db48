using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour {
    private DoorLink door = null;

    public enum Direction {
        Left = -1,
        Stationary = 0,
        Right = 1
    }

    private Direction _direction;   // direction player is going
    private Transform _weapon;
    [SerializeField] private Camera _camera;

    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _tiltAngle = 10f;    // angle player tilts in the direction they're going

    [SerializeField] private float _aimSpeed = 2.0f;
    [SerializeField] private float _rateOfFire = 2.0f;  // seconds between bullets
    private float _lastShotFired;
    [SerializeField] private Transform _bulletPrefab;
    [SerializeField] private Transform _launchPoint, _explosion;

    [SerializeField] private int _startingHealth = 10;
    private int _health;
    public bool IsDead { get { return _health <= 0; } }

    private AudioSource _gunshot_sound, _explosion_sound;

    void Awake() {
        _direction = Direction.Left;
        _weapon = transform.Find("Weapon");
        _health = _startingHealth;
        _lastShotFired = 0f;
    
        if (_camera == null) {
            _camera = GameObject.FindObjectOfType<Camera>();
        }

        AudioSource[] sounds = transform.GetComponentsInChildren<AudioSource>();
        _gunshot_sound = sounds[0];
        _explosion_sound = sounds[1];
    }

    // Update is called once per frame
    void Update() {
        MovePlayer();
        AimWeapon();
        FireWeapon();
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Escape)) { UnityEditor.EditorApplication.isPlaying = false; }
#endif

        if (Input.GetKey(KeyCode.Return)) { 
            Debug.Log("---- Explosion debugging code goes here ----");
        }
    }

    private void FireWeapon() {
        if (FirePressed) {
            if (Time.time - _lastShotFired < _rateOfFire) {
                return;
            }

            Transform bullet = Instantiate(_bulletPrefab, _launchPoint.position, _launchPoint.rotation);
            float fRotation = _launchPoint.rotation.z * Mathf.Deg2Rad;
            float fX = Mathf.Sin(fRotation);
            float fY = Mathf.Cos(fRotation);
            bullet.GetComponent<Bullet>().Direction = new Vector2(fY, fX).normalized;
            bullet.GetComponent<Bullet>().Owner = gameObject.tag;
            _gunshot_sound.Play();
            _lastShotFired = Time.time;
        }
    }

    private void AimWeapon() {
        // Based on CodeMonkey's "Aim at Mouse in Unity 2D video
        // https://www.youtube.com/watch?v=fuGQFdhSPg4
        Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector3 aimDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        if (angle < 0 ) {
            if (angle >= -90f) {
                angle = 0f;
            } else {
                // -90 < angle < 180
                angle = 180f;
            }
        }

        angle = Mathf.Lerp(_weapon.eulerAngles.z, angle, Time.deltaTime * _aimSpeed);

        _weapon.eulerAngles = new Vector3(0, 0, angle);
    }

    private void MovePlayer() {
        if (LeftPressed || RightPressed) {
            _direction = LeftPressed ? Direction.Left : Direction.Right;
            float new_x = transform.position.x + _speed * (float)_direction * Time.deltaTime;
            transform.SetPositionAndRotation(
                    new Vector3(new_x, transform.position.y, 0),
                    Quaternion.Euler(0, 0, -_tiltAngle * (float)_direction)
                );
        } else if (UpPressed || DownPressed) {
            if (door != null) {
                if ((UpPressed && door.IsBottom) || (DownPressed && !door.IsBottom)) {
                    if (door.Destination != Vector3.positiveInfinity) {
                        transform.position = door.Destination;
                    }
                }
            }
        } else {
            transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, 0, 0));
        }
    }

    private bool LeftPressed { get { return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Keypad4); } }
    private bool RightPressed { get { return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.Keypad6); } }
    private bool UpPressed { get { return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Keypad8); } }
    private bool DownPressed { get { return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Keypad2); } }
    private bool FirePressed { 
        get { 
            return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Keypad5) || Input.GetMouseButton(0); 
        } 
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Door")) {
            door = other.gameObject.GetComponent<DoorLink>();
        } else if (other.gameObject.CompareTag("EndOfLevel")) {
            SceneManager.LoadScene("GameWon");
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Door")) {
            door = null;
        }
    }

    IEnumerator Explode() {
        Transform exp = Instantiate<Transform>(_explosion, transform.position, Quaternion.identity);
        Explosion newExplosion = exp.GetComponent<Explosion>();
        StartCoroutine(newExplosion.Explode());
        yield return new WaitForSeconds(2);
        StopCoroutine(newExplosion.Explode());
        newExplosion.enabled = false;
    }

    public void TakeDamage(int damage) {
        _health -= damage;

        if (_health <= 0) {
            Debug.Log("You're dead!");
            _explosion_sound.Play();
            StartCoroutine(Explode());
        }
    }
}

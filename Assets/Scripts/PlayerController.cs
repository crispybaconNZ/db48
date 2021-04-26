using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Awake() {
        _direction = Direction.Left;
        _weapon = transform.Find("Weapon");
        if (_camera == null) {
            _camera = GameObject.FindObjectOfType<Camera>();
        }
    }

    // Update is called once per frame
    void Update() {
        MovePlayer();
        //AimWeapon();

        if (Input.GetKey(KeyCode.Escape)) { Debug.Log("Bail!"); }
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
                transform.position = door.GetDestination();
            }
        } else {
            transform.SetPositionAndRotation(
                    transform.position,
                    Quaternion.Euler(0, 0, 0)
                );
        }
    }

    private bool LeftPressed { get { return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Keypad4); } }
    private bool RightPressed { get { return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.Keypad6); } }
    private bool UpPressed { get { return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Keypad8); } }
    private bool DownPressed { get { return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.Keypad2); } }
    private bool FirePressed { get { return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Keypad5); } }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Door")) {
            Debug.Log("Player has entered door " + other);
            door = other.gameObject.GetComponent<DoorLink>();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Door")) {
            Debug.Log("Player has exited door " + other);
            door = null;
        }
    }
}

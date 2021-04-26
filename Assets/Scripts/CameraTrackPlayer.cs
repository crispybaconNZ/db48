using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrackPlayer : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _horizTrackSpeed;
    [SerializeField] private float _vertTrackSpeed;

    private void Start() {
        if (_player == null) {
            _player = GameObject.Find("Player").transform;
        }
        // transform.position = new Vector3(_player.position.x, _player.position.y, transform.position.z);
    }

    private void Update() {
        float x_delta = Time.deltaTime * _horizTrackSpeed;
        float y_delta = Time.deltaTime * _vertTrackSpeed;

        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, _player.position.x, x_delta),
            Mathf.Lerp(transform.position.y, _player.position.y, y_delta),
            transform.position.z);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("OnCollisionEnter()");
        if (other.gameObject.CompareTag("Wall")) {
            Debug.Log("Hit the wall!");
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Triggered!");
    }
}

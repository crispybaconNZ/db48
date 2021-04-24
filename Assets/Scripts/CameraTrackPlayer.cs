using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrackPlayer : MonoBehaviour {
    [SerializeField] private Transform _player;

    private void Start() {
        if (_player == null) {
            _player = GameObject.Find("Player").transform;
        }
        transform.position = new Vector3(_player.position.x, _player.position.y, transform.position.z);
    }

    private void Update() {
        float delta = Time.deltaTime;
        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, _player.position.x, delta),
            Mathf.Lerp(transform.position.y, _player.position.y, delta),
            transform.position.z);
    }
}

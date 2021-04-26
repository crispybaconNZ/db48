using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrackPlayer : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _horizTrackSpeed;
    [SerializeField] private float _vertTrackSpeed;
    [SerializeField] private float _horizOffset = 0f;
    [SerializeField] private float _vertOffset = 0f;

    private void Start() {
        if (_player == null) {
            _player = GameObject.Find("Player").transform;
        }
        transform.position = new Vector3(_player.position.x + _horizOffset, _player.position.y + _vertOffset, transform.position.z);
    }

    private void Update() {
        float x_delta = Time.deltaTime * _horizTrackSpeed;
        float y_delta = Time.deltaTime * _vertTrackSpeed;

        transform.position = new Vector3(
            Mathf.Lerp(transform.position.x, _player.position.x + _horizOffset, x_delta),
            Mathf.Lerp(transform.position.y, _player.position.y + _vertOffset, y_delta),
            transform.position.z);
    }
}

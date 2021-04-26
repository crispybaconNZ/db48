using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryBot : MonoBehaviour {
    [SerializeField] private PlayerController _player;
    [SerializeField] private float _detectionRange = 3;
    [SerializeField] private Sprite frontFacing;
    [SerializeField] private Sprite leftFacing;
    [SerializeField] private Sprite rightFacing;

    private readonly float _detectionThreshold = 1f;

    // Variables to handle the Idling bobbing motion
    private float start_y;
    private readonly float _floatStrength = 0.05f;
    private readonly float _floatFrequency = 4f;

    private enum SentryBotStatus {
        Idling, 
        AttackingPlayer,
        Hunting
    }

    private SentryBotStatus _currentStatus = SentryBotStatus.Idling;

    private void Awake() {
        start_y = transform.position.y;    
    }

    private void DoBob() {
        // bobbing motion
        Vector2 floatY = transform.position;
        floatY.y = start_y + (Mathf.Sin(Time.time * _floatFrequency) * _floatStrength);
        transform.position = floatY;
    }

    void Update() {
        switch (_currentStatus) {
            case SentryBotStatus.Idling: {
                    if (CanSeePlayer()) {
                        Debug.Log("Player spotted!");
                        _currentStatus = SentryBotStatus.AttackingPlayer;
                    } else {
                        DoBob();
                    }
                    break;
                }

            case SentryBotStatus.AttackingPlayer: {
                    Debug.Log("AttackingPlayer");

                    // face player
                    if (_player.transform.position.x < transform.position.x) {
                        // player is left of bot
                        gameObject.GetComponent<SpriteRenderer>().sprite = leftFacing;
                    } else {
                        // consider player is right of bot
                        gameObject.GetComponent<SpriteRenderer>().sprite = rightFacing;
                    }
                    break;
                }

            case SentryBotStatus.Hunting: {
                    Debug.Log("Hunting");
                    if (CanSeePlayer()) {
                        Debug.Log("Player spotted!");
                        _currentStatus = SentryBotStatus.AttackingPlayer;
                    }
                    break;
                }
        }
    }

    private bool CanSeePlayer() {
        if (_player == null) { return false; }
        Vector3 playerPos = _player.transform.position;

        if (Mathf.Abs(playerPos.y - transform.position.y) < _detectionThreshold) {
            // player's y is +/- _detectionThreshold of sentry bot's y
            if (Mathf.Abs(playerPos.x - transform.position.x) <= _detectionRange) {
                return true;
            }
        }

        return false;
    }
}

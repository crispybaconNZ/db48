using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SentryBot : MonoBehaviour {
    public PlayerController _player;
    public float _detectionRange = 3;
    public Sprite frontFacing;
    public Sprite leftFacing;
    public Sprite rightFacing;
    private SentryBotStatus initialStatus;
    private PlayerController.Direction initialDirection;

    // weaponry
    private Vector2 _bulletLaunchPoint;
    [SerializeField] private Transform _leftLaunchPoint, _rightLaunchPoint;
    [SerializeField] private Transform _bulletPrefab;

    private readonly float _verticalDetectionThreshold = 1f;

    // Variables to handle the Idling bobbing motion
    private Vector2 startPosition;
    private readonly float _bobStrength = 0.05f;
    private readonly float _bobFrequency = 4f;

    // Positions for patrol path
    public float _patrolSpeed = 1f;
    public Vector2 _startWaypoint, _endWaypoint;
    private Vector2 _currentDestination;
    private readonly float _distanceThreshold = 0.1f;    // how close the bot needs to be before changing direction

    [System.Serializable]
    public enum SentryBotStatus {
        Idling, 
        AttackingPlayer,
        Hunting
    }

    public SentryBotStatus _currentStatus = SentryBotStatus.Idling;
    public PlayerController.Direction direction = PlayerController.Direction.Stationary;

    private void SetLaunchpointOnDirection() {
        if (direction == PlayerController.Direction.Left) {
            _bulletLaunchPoint = _leftLaunchPoint.position;
        } else if (direction == PlayerController.Direction.Right) {
            _bulletLaunchPoint = _rightLaunchPoint.position;
        }
    }

    private void FireBullet() {
        Quaternion rotation;

        if (direction == PlayerController.Direction.Left) {
            rotation = Quaternion.Euler(0, 0, 180);
        } else {
            rotation = Quaternion.identity;
        }
        Transform bullet = Instantiate(_bulletPrefab, _bulletLaunchPoint, rotation);
        
    }

    private void Awake() {
        startPosition = transform.position;
        initialStatus = _currentStatus;
        initialDirection = direction;
        _currentDestination = _startWaypoint;

        if (_player == null) {
            _player = GameObject.Find("Player").GetComponent<PlayerController>();
        }        
    }

    private void DoIdleBob() {
        // idle bobbing motion
        Vector2 bobbing = transform.position;
        bobbing.y = startPosition.y + (Mathf.Sin(Time.time * _bobFrequency) * _bobStrength);
        transform.position = bobbing;
    }

    void Update() {
        switch (_currentStatus) {
            case SentryBotStatus.Idling:
                gameObject.GetComponent<SpriteRenderer>().sprite = frontFacing;
                direction = PlayerController.Direction.Stationary;

                if (CanSeePlayer()) {
                    _currentStatus = SentryBotStatus.AttackingPlayer;
                } else {
                    DoIdleBob();
                }
                break;

            case SentryBotStatus.AttackingPlayer:
                if (CanSeePlayer()) {
                    // face player
                    if (_player.transform.position.x < transform.position.x) {
                        // player is left of bot
                        gameObject.GetComponent<SpriteRenderer>().sprite = leftFacing;
                        direction = PlayerController.Direction.Left;
                    } else {
                        // consider player is right of bot
                        gameObject.GetComponent<SpriteRenderer>().sprite = rightFacing;
                        direction = PlayerController.Direction.Right;
                    }

                    // do shooting!
                } else {
                    gameObject.GetComponent<SpriteRenderer>().sprite = frontFacing;
                    direction = initialDirection;
                    _currentStatus = initialStatus;
                }
                break;

            case SentryBotStatus.Hunting:
                DoPatrol();
                if (CanSeePlayer()) {
                    _currentStatus = SentryBotStatus.AttackingPlayer;
                }
                break;
        }
    }

    private bool PlayerOnSameFloor {  get { return Mathf.Abs(_player.transform.position.y - transform.position.y) < _verticalDetectionThreshold; } }

    private bool CanSeePlayer() {
        if (_player == null) { return false; }  // player? what player?
        if (!PlayerOnSameFloor) { return false; }
        Vector3 playerPos = _player.transform.position;
        float absoluteDistanceToPlayer = Mathf.Abs(playerPos.x - transform.position.x);
        bool withinDetectionRange = absoluteDistanceToPlayer <= _detectionRange;

        bool result = false;
        switch (_currentStatus) {
            case SentryBotStatus.Idling:
                // when idling, doesn't matter which side player is coming from
                // player's y is +/- _detectionThreshold of sentry bot's y
                result = withinDetectionRange;
                break;

            case SentryBotStatus.Hunting:
                // When hunting, sentry bot can only see player if they're in front of the bot
                // so player X < bot X when left-facing or player X > bot X when right facing
                result = withinDetectionRange && 
                    ((direction == PlayerController.Direction.Left && transform.position.x > playerPos.x) ||
                    (direction == PlayerController.Direction.Right && transform.position.x < playerPos.x));
                break;

            case SentryBotStatus.AttackingPlayer:
                // When attacking, bot will lose sight of the player if they're more than double the detection range away
                result = absoluteDistanceToPlayer <= (_detectionRange * 2);
                break;
        }

        return result;
    }

    private void DoPatrol() {
        // move towards the current waypoint
        float patrolDirection = _currentDestination.x < transform.position.x ? -1 : 1;
        float newX = transform.position.x + (_patrolSpeed * patrolDirection * Time.deltaTime);
        transform.position = new Vector2(newX, transform.position.y);

        // if within _distanceThreshold of the waypoint, select the other waypoint as current
        if (Mathf.Abs(transform.position.x - _currentDestination.x) < _distanceThreshold) {
            if (_currentDestination == _startWaypoint) {
                _currentDestination = _endWaypoint;
                patrolDirection *= -1;  // change direction
            } else {
                _currentDestination = _startWaypoint;
                patrolDirection *= -1;  // change direction
            }
        }

        // make sure sprite matches direction
        if (patrolDirection == -1) {
            gameObject.GetComponent<SpriteRenderer>().sprite = leftFacing;
            direction = PlayerController.Direction.Left;
        } else {
            gameObject.GetComponent<SpriteRenderer>().sprite = rightFacing;
            direction = PlayerController.Direction.Right;
        }
    }

    private void OnDrawGizmosSelected() {
        if (_currentStatus == SentryBotStatus.Hunting) {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_startWaypoint, 0.1f);
            Gizmos.DrawLine(_startWaypoint, transform.position);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_endWaypoint, 0.1f);
            Gizmos.DrawLine(_endWaypoint, transform.position);
        }
    }
}

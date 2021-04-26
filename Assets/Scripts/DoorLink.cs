using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLink : MonoBehaviour {
    // Links this door to another door on the map

    [SerializeField] private DoorLink _linkedDoor;

    public Vector3 GetDestination() { return _linkedDoor.transform.position; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLink : MonoBehaviour {
    // Links this door to another door on the map

    [SerializeField] private DoorLink _linkedDoor;
    [SerializeField] private bool _bottomDoor;

    public Vector3 Destination { get {
            if (_linkedDoor != null) {
                return _linkedDoor.transform.position;
            } else {
                return Vector3.positiveInfinity;
            }
        } }
    public bool IsBottom { get { return _bottomDoor; } }
}

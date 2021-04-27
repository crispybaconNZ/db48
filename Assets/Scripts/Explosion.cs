using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    private Vector2 _centre;
    
    public Vector2 ExplosionCentre {
        get { return _centre; }
        set {
            _centre = value;
            transform.position = _centre;
        }
    }

    public IEnumerator Explode() {
        transform.localScale = new Vector2(0.25f, 0.25f);
        transform.Rotate(Vector3.forward, 0f);
        this.enabled = true;

        int i = 0;
        while (i < 16) {
            transform.localScale = new Vector2(0.25f * i, 0.25f * i);
            transform.Rotate(Vector3.forward, i * 15);
            yield return new WaitForSeconds(0.1f);
            i++;
        }
    }
}

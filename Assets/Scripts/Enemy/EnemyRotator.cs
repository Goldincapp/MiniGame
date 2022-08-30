using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRotator : MonoBehaviour
{
    [SerializeField] private bool _rotateOneAxis;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;

    private void OnEnable()
    {
        StartCoroutine(Rotate(!_rotateOneAxis ? Random.Range(_minSpeed, _maxSpeed) : 0, !_rotateOneAxis ? Random.Range(_minSpeed, _maxSpeed) : 0, Random.Range(_minSpeed, _maxSpeed)));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Rotate(float x, float y, float z)
    {
        while(true)
        {
            transform.Rotate(new Vector3(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime), Space.Self);
            yield return null;
        }
    }

    public void StopRotation() => StopAllCoroutines();
}

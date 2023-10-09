using OPS.AntiCheat.Field;
using OPS.AntiCheat.Speed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [Header("Spaceship characteristic")]
    [SerializeField] private ProtectedFloat _maxMoveSpeed;
    [SerializeField] private ProtectedFloat _moveAcceleration;
    [SerializeField] private ProtectedFloat _brakesForce;
    [SerializeField] private ProtectedFloat _stopBrakesValue;
    [SerializeField] private ProtectedFloat _rotateSpeed;

    [Header("Weapon & Projectile")]
    [SerializeField] private Weapon _weapon;
    [SerializeField] private GameObject _projectileParent;

    [Header("Physics")]
    [SerializeField] private Rigidbody _rigidBody;

    [Header("Effects")]
    [SerializeField] private ParticleSystem _engineEffect;
    [SerializeField] private ParticleSystem _destroyEffect;

    public Vector3 Direction { get => new Vector3(Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad) * -1, Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad), 0).normalized; }

    public void ActivateSpaceship()
    {
        gameObject.SetActive(true);

        _weapon.ActivateWeapon(_projectileParent, this);

        StartCoroutine(ControlSpaceship());
        StartCoroutine(EngineEffectControl());
    }

    public void DeactivateSpaceship()
    {
        StopAllCoroutines();

        if (_projectileParent)
            foreach(var i in _projectileParent.GetComponentsInChildren<ProjectileController>())
                Destroy(i?.gameObject);

        transform.SetPositionAndRotation(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
         
        _rigidBody.velocity = Vector2.zero;

        gameObject.SetActive(false);
    }

    private void OnDisable() => DeactivateSpaceship();

    public void PlayDestroyEffect()
    {
        StopAllCoroutines();

        SoundManager.Instance.StopEngineSound();

        SoundManager.Instance.PlayShipDestroySound();

        _engineEffect.Stop();

        _destroyEffect.Play();
    } 

    private IEnumerator ControlSpaceship()
    {
        var camera = Camera.main;
        var isPressedKey = false;

        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            // if (Mathf.Abs(_rigidBody.velocity.y) + Mathf.Abs(_rigidBody.velocity.x) < _maxMoveSpeed)
            // {
            //     var direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * _moveAcceleration;
            //     _rigidBody.velocity += new Vector3(direction.x, direction.y, 0);
            // }          

            // var mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -camera.transform.position.z)) - transform.position;
            // transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg - 90));

            //yield return null;

            if (!isPressedKey)
                if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Space))
                {
                    GUI.Instance.HideHint();
                    isPressedKey = true;
                }

            if (Input.GetKey(KeyCode.UpArrow) && Mathf.Abs(_rigidBody.velocity.y) + Mathf.Abs(_rigidBody.velocity.x) < _maxMoveSpeed)
                _rigidBody.velocity += _moveAcceleration * Direction;

            if (Input.GetKey(KeyCode.DownArrow) && Mathf.Abs(_rigidBody.velocity.y) + Mathf.Abs(_rigidBody.velocity.x) > _stopBrakesValue)
                _rigidBody.velocity -= _brakesForce * _rigidBody.velocity.normalized;

            if (Input.GetKey(KeyCode.RightArrow))
                transform.Rotate(new Vector3(0, 0, -_rotateSpeed * ProtectedTime.deltaTime), Space.Self);

            if (Input.GetKey(KeyCode.LeftArrow))
                transform.Rotate(new Vector3(0, 0, _rotateSpeed * ProtectedTime.deltaTime), Space.Self);

            yield return null;
        }
    }


    private IEnumerator EngineEffectControl()
    {
        while(true)
        {
            // if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))  
            //     if (!_engineEffect.isPlaying)   
            //         _engineEffect.Play();  
            
            // if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
            //     _engineEffect.Stop();

            // yield return null;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                 _engineEffect.Play();
                 SoundManager.Instance.PlayEngineSound();
            }    
            
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                _engineEffect.Stop();
                SoundManager.Instance.StopEngineSound();
            }

            yield return null;
        }
    }

    private void OnBecameInvisible()
    {
        var camera = Camera.main;

        if (camera)
        {
            var cameraPosition = Camera.main.WorldToScreenPoint(transform.position);
            var newPosition = transform.position;

            newPosition.x *= (cameraPosition.x >= camera.pixelWidth || cameraPosition.x <= 0) ?  -1 : 1;
            newPosition.y *= (cameraPosition.y >= camera.pixelHeight || cameraPosition.y <= 0) ?  -1 : 1;

            transform.position = newPosition;
        }
    }
}

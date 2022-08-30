using System.Collections;
using UnityEngine.Events;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private float _damage;
    private UnityAction<GameObject> _onDestroy;

    public void ActivateProjectile(float damage, float speed, Vector3 direction, Vector3 startPosition, Quaternion rotation, UnityAction<GameObject> onDestroy)
    {
        _damage = damage;
        _onDestroy = onDestroy;

        transform.SetPositionAndRotation(startPosition, rotation);
        
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().AddForce(direction * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Enemy>().Destroy();
            _onDestroy.Invoke(gameObject);
        }

        if (other.CompareTag("Friend"))
        {
            other.gameObject.GetComponent<Enemy>().Destroy();
            _onDestroy.Invoke(gameObject);
            
            GUI.Instance?.EndGame(); 
        } 
    }

    private void OnBecameInvisible()
    {   
        if (gameObject.activeInHierarchy)
            _onDestroy.Invoke(gameObject);
    }
}

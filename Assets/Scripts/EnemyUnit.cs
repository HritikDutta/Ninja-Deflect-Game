using System.Collections;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public Animator Animator { get; private set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void SetToDestroy(float delay)
    {
        transform.SetParent(null, true);
        StartCoroutine(DestroyAfterDelay(delay));
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

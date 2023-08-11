using System.Collections;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField]
    private Transform coinVisualTransform;

    private bool collected = false;

    public void Spawn()
    {
        collected = false;
        //StartCoroutine(ReduceSizeOverTime());
    }

    private void Update()
    {
        transform.Rotate(0f, 600f * Time.deltaTime, 0f);
    }

    private IEnumerator ReduceSizeOverTime()
    {
        float startTime = Time.time;
        Vector3 originalScale = coinVisualTransform.localScale;

        while (Time.time - startTime < GameSettings.instance.pickUpDuration)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / GameSettings.instance.pickUpDuration);
            coinVisualTransform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        // Can't collect it anymore :P
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        // No need to check layer since it only interacts with the player
        coinVisualTransform.gameObject.SetActive(false);
        PowerUpController.instance.AddCoins(1);
        AudioController.PlayAudioClipOneShot(AudioController.instance.coinCollectClip);

        collected = true;
        Destroy(gameObject);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public LayerMask layersToDestroy;
    public LayerMask targetLayers;
    [Header("Effects")] 
    public ParticleSystem hitEffect;
    public float fadeDuration;


    private BulletProperties bulletProperties; 
    private Rigidbody2D _rb;
    private SpriteRenderer[] _spriteRenderers;
    private Light2D[] _light2Ds;

    // Start is called before the first frame update
    // ReSharper disable Unity.PerformanceAnalysis
    public void Init(Vector2 direction, BulletProperties properties)
    {
        bulletProperties = properties;
        _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = direction.normalized * bulletProperties.speed;
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        _light2Ds = GetComponentsInChildren<Light2D>();
        Destroy(gameObject, bulletProperties.lifeTime);
        StartCoroutine(Fade());

        BulletDivision bulletDivision = GetComponent<BulletDivision>();
        if (bulletDivision && bulletDivision.enabled)
        {
            bulletDivision.StartCoroutine(bulletDivision.Init());
        }
    }

    private IEnumerator Fade()
    {
        yield return new WaitForSeconds(bulletProperties.lifeTime - fadeDuration);
        float startTime = Time.time;
        while (true)
        {
            float alpha = Mathf.Lerp(1, 0, (Time.time - startTime) / fadeDuration);
            SetBulletAlpha(alpha);
            yield return new WaitForEndOfFrame();
        }
    }

    private void SetBulletAlpha(float alpha)
    {
        foreach (var spriteRenderer in _spriteRenderers)
        {
            // XLogger.Log($"setting {spriteRenderer.gameObject.name}'s alpha to {alpha}");
            Color tempColor = spriteRenderer.color;
            tempColor.a = alpha;
            spriteRenderer.color = tempColor;
        }

        foreach (var light in _light2Ds)
        {
            light.intensity *= alpha;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (LayerMaskHelper.IsLayerInLayerMask(col.gameObject.layer, targetLayers))
        {
            OnTargetHit(col.gameObject);
        }
        if (LayerMaskHelper.IsLayerInLayerMask(col.gameObject.layer, layersToDestroy))
        {
            if (hitEffect)
            {
                var effect = Instantiate(hitEffect);
                effect.transform.position = transform.position;
            }
            Destroy(gameObject);
        }
    }

    private void OnTargetHit(GameObject target)
    {
        XLogger.Log(Category.Weapon,$"Bullet hit {target.name}");
        var health = target.GetComponentInParent<Health>();
        if (health)
        {
            health.ChangeHealth(-bulletProperties.damage, gameObject);
        }
    }

    public BulletProperties GetBulletProperty()
    {
        return bulletProperties;
    }
}
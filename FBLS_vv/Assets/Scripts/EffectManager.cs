using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;
    public GameObject effect;
    public GameObject Light;
    public MultiManager multi;

    private void Awake()
    {
        instance = this;
        multi = GetComponent<MultiManager>();
    }

    public void Effect(GameObject tile) //이펙트
    {
        GameObject priticle = Instantiate(effect);
        ParticleSystem ParticleSys = priticle.GetComponent<ParticleSystem>();

        ParticleSys.transform.position = tile.transform.position; //이펙트 생성 위치
        ParticleSys.Play();
        Destroy(priticle, 1f);
    }

    public IEnumerator WeatherEffect(GameObject effect) // 날씨 오브젝트 활성화
    {
        effect.SetActive(true);
        yield return new WaitForSeconds(5f);
        effect.SetActive(false);
    }
  

    public IEnumerator Lightning(Transform gameObject) // 번개 이펙트
    {
        yield return new WaitForSeconds(1f); // 번개 딜레이
        GameObject lightning = Instantiate(Light) as GameObject;
        lightning.transform.position = gameObject.position;
        lightning.transform.parent = gameObject.transform;
        Destroy(lightning, 1.3f);
    }
}

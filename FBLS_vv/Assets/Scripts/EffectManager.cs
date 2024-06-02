using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;
    public GameObject effect;
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
        Debug.Log("이펙트 실행");
    }

    public IEnumerator WeatherEffect(GameObject effect) // 날씨 오브젝트 활성화
    {
        if (multi.green1p || multi.green2p == false)
        {
            effect.SetActive(true);
            yield return new WaitForSeconds(5f);
            effect.SetActive(false);
        }
        else
        {
            effect.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void Effect(GameObject tile, GameObject effect) //����Ʈ
    {
        GameObject priticle = Instantiate(effect);
        ParticleSystem ParticleSys = priticle.GetComponent<ParticleSystem>();

        ParticleSys.transform.position = tile.transform.position; //����Ʈ ���� ��ġ
        ParticleSys.Play();
        Destroy(priticle, 2f);
    }

    public IEnumerator WeatherEffect(GameObject effect) // ���� ������Ʈ Ȱ��ȭ
    {
        effect.SetActive(true);
        ParticleSystem ParticleSys = effect.GetComponentsInChildren<ParticleSystem>()[0];
        ParticleSys.Play();
        yield return new WaitForSeconds(5f);
        effect.SetActive(false);
    }

}

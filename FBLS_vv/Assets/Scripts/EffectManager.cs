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

    public void Effect(GameObject tile) //����Ʈ
    {
        GameObject priticle = Instantiate(effect);
        ParticleSystem ParticleSys = priticle.GetComponent<ParticleSystem>();

        ParticleSys.transform.position = tile.transform.position; //����Ʈ ���� ��ġ
        ParticleSys.Play();
        Destroy(priticle, 1f);
        Debug.Log("����Ʈ ����");
    }

    public IEnumerator WeatherEffect(GameObject effect) // ���� ������Ʈ Ȱ��ȭ
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

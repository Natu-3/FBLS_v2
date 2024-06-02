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

    public void Effect(GameObject tile) //����Ʈ
    {
        GameObject priticle = Instantiate(effect);
        ParticleSystem ParticleSys = priticle.GetComponent<ParticleSystem>();

        ParticleSys.transform.position = tile.transform.position; //����Ʈ ���� ��ġ
        ParticleSys.Play();
        Destroy(priticle, 1f);
    }

    public IEnumerator WeatherEffect(GameObject effect) // ���� ������Ʈ Ȱ��ȭ
    {
        effect.SetActive(true);
        yield return new WaitForSeconds(5f);
        effect.SetActive(false);
    }
  

    public IEnumerator Lightning(Transform gameObject) // ���� ����Ʈ
    {
        yield return new WaitForSeconds(1f); // ���� ������
        GameObject lightning = Instantiate(Light) as GameObject;
        lightning.transform.position = gameObject.position;
        lightning.transform.parent = gameObject.transform;
        Destroy(lightning, 1.3f);
    }
}

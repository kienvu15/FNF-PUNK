using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance;

    [System.Serializable]
    public class PoolItem
    {
        public string id;
        public GameObject prefab;
        public int size = 5;
    }

    public List<PoolItem> poolItems;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var item in poolItems)
        {
            var queue = new Queue<GameObject>();
            for (int i = 0; i < item.size; i++)
            {
                GameObject obj = Instantiate(item.prefab, transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            poolDictionary[item.id] = queue;
        }
    }

    public GameObject SpawnFromPool(string id, Transform position)
    {
        if (!poolDictionary.ContainsKey(id))
        {
            Debug.LogWarning($"No pool found for {id}");
            return null;
        }

        var obj = poolDictionary[id].Dequeue();

        obj.transform.position = position.position;
        obj.transform.localScale = Vector3.one;
        obj.SetActive(true);

        // Scale hiệu ứng to lên rồi biến mất
        obj.transform.DOScale(1.3f, 0.15f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        {
            obj.SetActive(false);
            poolDictionary[id].Enqueue(obj);
        });

        return obj;
    }
}

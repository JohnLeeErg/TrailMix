using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Killbox : MonoBehaviour
{
    public enum KillType
    {
        InstantKill,
        DamageOverTime,
    }

    public KillType typeOfKillbox;

    [Header("DamageOverTime")]
    public List<IHealth> entitiesInside = new List<IHealth>();
    public Dictionary<IHealth, float> entityHealthTimer = new Dictionary<IHealth, float>();
    public float damageTickTime;
    public float damageTickValue;

    #region Triggers
    private void OnTriggerEnter2D(Collider2D collided)
    {
        if (collided.gameObject.tag == "Player")
        {
            IHealth healthC = collided.gameObject.transform.root.GetComponent<IHealth>();
            switch (typeOfKillbox)
            {
                case KillType.InstantKill:
                    healthC.Die();
                    break;
                case KillType.DamageOverTime:
                    if (!entityHealthTimer.ContainsKey(healthC))
                    {
                        healthC.TakeDamage(damageTickValue);
                        entityHealthTimer.Add(healthC, damageTickTime);
                    }
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collided)
    {
        if (collided.gameObject.tag == "Player")
        {
            IHealth healthC = collided.gameObject.transform.root.GetComponent<IHealth>();
            switch (typeOfKillbox)
            {
                case KillType.DamageOverTime:
                    if (entityHealthTimer.ContainsKey(healthC))
                    {
                        entityHealthTimer.Remove(healthC);
                    }
                    break;
            }
        }
    }
    #endregion

    void Update()
    {
        switch (typeOfKillbox)
        {
            case KillType.DamageOverTime:
                ApplyDamage();
                break;
        }
    }

    void ApplyDamage()
    {
        entitiesInside = entityHealthTimer.Keys.ToList();
        if (entitiesInside.Count > 0)
        {
            for (int i = 0; i < entitiesInside.Count; i++)
            {
                entityHealthTimer[entitiesInside[i]] -= Time.deltaTime;
                if (entityHealthTimer[entitiesInside[i]] <= 0f)
                {
                    entitiesInside[i].TakeDamage(damageTickValue);
                    entityHealthTimer[entitiesInside[i]] = damageTickTime;
                }
            }
        }
    }
}

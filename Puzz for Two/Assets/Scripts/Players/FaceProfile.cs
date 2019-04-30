using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FaceProfile : ScriptableObject
{
    public List<Sprite> runTimeSprites;
    public Sprite[] sprites;
    Sprite randomSprite;
    public Sprite GetRandomFace()
    {
        if (runTimeSprites == null || runTimeSprites.Count <= 1)
        {
            runTimeSprites = new List<Sprite>();
            runTimeSprites.AddRange(sprites);
        }
        else
        {

            runTimeSprites.Remove(randomSprite);
        }
        int randomInt = Random.Range(0, runTimeSprites.Count);
        randomSprite = runTimeSprites[randomInt];
        return randomSprite;

    }

}

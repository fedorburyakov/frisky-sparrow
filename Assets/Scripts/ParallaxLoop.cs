using UnityEngine;

public class ParallaxLoop : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] backgroundPartsLayer1;
    [SerializeField] private SpriteRenderer[] backgroundPartsLayer2;

    [SerializeField] private float speed = 2f;

    private float spriteWidth;

    private Transform[] layer1Transforms;
    private Transform[] layer2Transforms;
    private float overlapOffset = 0.01f;

    void Start()
    {
        layer1Transforms = new Transform[backgroundPartsLayer1.Length];
        for (int i = 0; i < backgroundPartsLayer1.Length; i++)
            layer1Transforms[i] = backgroundPartsLayer1[i].transform;

        layer2Transforms = new Transform[backgroundPartsLayer2.Length];
        for (int i = 0; i < backgroundPartsLayer2.Length; i++)
            layer2Transforms[i] = backgroundPartsLayer2[i].transform;

        spriteWidth = backgroundPartsLayer1[0].bounds.size.x;
    }

    void Update()
    {
        MoveLayer(layer1Transforms);
        MoveLayer(layer2Transforms);
    }

    private void MoveLayer(Transform[] layer)
    {
        foreach (var part in layer)
        {
            part.position += Vector3.left * speed * Time.deltaTime;

            if (part.position.x <= -spriteWidth + overlapOffset)
            {
                part.position += Vector3.right * (spriteWidth * layer.Length - overlapOffset);
            }
        }
    }


    public void SetLayerAlpha(SpriteRenderer[] layer, float alpha)
    {
        foreach (var part in layer)
        {
            var c = part.color;
            c.a = alpha;
            part.color = c;
        }
    }

    public void SetLayerSprite(SpriteRenderer[] layer, Sprite sprite)
    {
        foreach (var part in layer)
            part.sprite = sprite;
    }

    public void SwapLayers()
    {
        var temp = backgroundPartsLayer1;
        backgroundPartsLayer1 = backgroundPartsLayer2;
        backgroundPartsLayer2 = temp;
    }

    public SpriteRenderer[] GetLayer1() => backgroundPartsLayer1;
    public SpriteRenderer[] GetLayer2() => backgroundPartsLayer2;
}

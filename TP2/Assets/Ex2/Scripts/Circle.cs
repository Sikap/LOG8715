using UnityEngine;
using UnityEngine.Serialization;

public class Circle : MonoBehaviour
{
    [FormerlySerializedAs("I")] [HideInInspector]
    public int i;

    [FormerlySerializedAs("J")] [HideInInspector]
    public int j;

    public float Health { get; private set; }

    private const float BaseHealth = 1000;

    private const float HealingPerSecond = 1;
    private const float HealingRange = 3;
    private Grid grid;
    private float timer;

    // Start is called before the first frame update
    private void Start()
    {
        Health = BaseHealth;
        grid = FindObjectOfType<Grid>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateColor();
        HealNearbyShapes();
    }

    private void UpdateColor()
    {
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = grid.Colors[i, j] * Health / BaseHealth;
    }

    private void HealNearbyShapes()
    {
        timer += Time.deltaTime;
        while(timer >= 1 / HealingPerSecond)
        {
            var nearbyColliders = Physics2D.OverlapCircleAll(transform.position, HealingRange);
            foreach (var nearbyCollider in nearbyColliders)
            {
                if (nearbyCollider != null && nearbyCollider.TryGetComponent<Circle>(out var circle))
                {
                    circle.ReceiveHp(1);
                }
            }                
            timer -= 1 / HealingPerSecond;
        }
    }

    public void ReceiveHp(float hpReceived)
    {
        Health += hpReceived;
        Health = Mathf.Clamp(Health, 0, BaseHealth);
    }
}

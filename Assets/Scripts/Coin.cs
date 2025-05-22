using UnityEngine;

public class Coin : MonoBehaviour
{
    private Transform playerTransform;
    private TileManager tileManager;
    public float magnetSpeed = 10f;
    public float collectDistance = 100f;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        tileManager = FindObjectOfType<TileManager>();
    }
    
    void Update()
    {
        transform.Rotate(50 * Time.deltaTime, 0, 0);

        if (tileManager != null && tileManager.autoAssistEnabled)
        {
            if (playerTransform != null)
            {
                float distance = Vector3.Distance(transform.position, playerTransform.position);
                if (distance <= collectDistance)
                {
                    Vector3 direction = (playerTransform.position - transform.position).normalized;
                    transform.position += direction * magnetSpeed * Time.deltaTime;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindObjectOfType<AudioManager>().PlaySound("PickUpCoin");
            PlayerManager.numberOfCoins += 1;
            Destroy(gameObject);
        }
    }
}

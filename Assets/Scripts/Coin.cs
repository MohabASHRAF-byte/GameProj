
using UnityEngine;

public class Coin : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        transform.Rotate(50 * Time.deltaTime, 0, 0);    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FindObjectOfType<AudioManager>().PlaySound("PickUpCoin");
            PlayerManager.numberOfCoins += 1;
            Destroy(gameObject);
        }
    }
}

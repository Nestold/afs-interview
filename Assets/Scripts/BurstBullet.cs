namespace AFSInterview
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class BurstBullet : MonoBehaviour
    {
        private Rigidbody rb => GetComponent<Rigidbody>();
        
        public void Initialize(float distance, Vector3 direction)
        {
            var dir = direction * distance;
            dir.y *= 2;
            rb.AddForce(dir, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.tag.Equals("Enemy") && collision.transform.TryGetComponent(out Enemy enemy))
            {
                enemy.Kill();
            }
            Destroy(gameObject);
        }
    }
}
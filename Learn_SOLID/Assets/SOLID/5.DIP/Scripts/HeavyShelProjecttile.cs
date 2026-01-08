using UnityEngine;

public class HeavyShelProjecttile : MonoBehaviour,IProjectile
{
    public GameObject shellPrefab;
    public void Fire(Transform firePoint)
    {
        GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);
        shell.GetComponent<Rigidbody2D>().AddForce(shell.transform.up * 10, ForceMode2D.Impulse);
    }
}

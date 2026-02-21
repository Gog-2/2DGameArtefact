using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField]private int _health;
    [SerializeField]private Rigidbody2D _chips;
    [SerializeField]private int _howMuchChips;

    public void TakeDamage(int damage)
    {
        if (_health - damage > 0) _health -= damage;
        else DestroyBox();
    }

    private void DestroyBox()
    {
        float degress = 360f / _howMuchChips;
        for (int i = 0; i < _howMuchChips; i++)
        {
            Rigidbody2D chipik = Instantiate(_chips);
            chipik.AddForce(new Vector2(Mathf.Cos(degress * i), Mathf.Sin(degress * i)));
        }
        Destroy(this.gameObject);
    }
}

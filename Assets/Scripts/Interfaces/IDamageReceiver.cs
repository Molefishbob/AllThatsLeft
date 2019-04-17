public interface IDamageReceiver
{
    bool Dead { get; }
    void TakeDamage(int damage);
    void Die();
}
namespace DefaultNamespace
{
    /// <summary>
    /// Interface representing a damaging object
    /// </summary>
    public interface Damagable
    {
        /// <summary>
        /// Method called when damage is to be given to the object
        /// </summary>
        /// <param name="damage">The damage</param>
        public void takeDamage(float damage);
        
        /// <summary>
        /// If the object is still alive
        /// </summary>
        /// <returns></returns>
        public bool isAlive();
    }
}
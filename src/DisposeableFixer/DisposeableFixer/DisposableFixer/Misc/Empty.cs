namespace DisposableFixer.Misc
{
    public static class Empty
    {
        public static T[] Array<T>()
        {
            return new T[0];
        }
    }
}
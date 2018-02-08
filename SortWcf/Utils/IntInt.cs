namespace WcfSortTest.Utils
{
    /// <summary>
    /// Helper storage class to keep adress of 2d array/IEnumerable, 2 integers.
    /// </summary>
    public struct IntInt
    {
        public int ContainerIndex;
        public int ArrayIndex;

        public IntInt(int containerIndex, int arrayIndex)
        {
            ContainerIndex = containerIndex;
            ArrayIndex = arrayIndex;
        }
    }
}
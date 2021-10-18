namespace Robot
{
    public interface IUpdatableElement
    {

        public bool Stop();
        public bool Save();
        public bool AddToList();
        public IUpdatableElement GetLastInstance();
    }

    public enum UpdatableType
    {
        Arduino,
        Element,
        Sheet
    }
}

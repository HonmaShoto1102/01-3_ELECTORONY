namespace Client.FrameWork.State
{
    public class State<T>
    {
        public virtual void Enter(T obj) { }
        public virtual void Exit(T obj) { }
        public virtual State<T> Update(T obj) { return this; }
    }
}

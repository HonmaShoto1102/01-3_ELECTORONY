using Client.FrameWork.State;

namespace Client.FrameWork.State
{
    //ステート管理のクラス
    public class StateMachine<T>
    {
        private State<T> _currentState;
        private State<T> _nextState;
        private bool _isInitalized;

        //初期化の処理
        public void Initalize(State<T> firstState, T obj)
        {
            _currentState = firstState;
            _currentState.Enter(obj);

            _isInitalized = true;
        }

        //更新の処理
        public void Update(T obj)
        {
            if (!_isInitalized) return;

            State<T> _NextState = _currentState.Update(obj);

            // ステートの切り替え処理
            if (_NextState != _currentState) {
                _currentState.Exit(obj);
                _NextState.Enter(obj);
                _currentState = _NextState;
            }
        }
    }
}

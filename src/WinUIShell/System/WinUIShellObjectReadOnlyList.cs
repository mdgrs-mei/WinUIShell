using System.Collections;
using WinUIShell.Common;

namespace WinUIShell;

public class WinUIShellObjectReadOnlyList<T> : WinUIShellObject, IReadOnlyList<T> where T : class
{
    public int Count
    {
        get => PropertyAccessor.Get<int>(Id, nameof(Count))!;
    }

    public T this[int index]
    {
        get => PropertyAccessor.GetIndexer<T>(Id, index)!;
    }

    internal WinUIShellObjectReadOnlyList(ObjectId id)
        : base(id)
    {
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator<T>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private struct Enumerator<U> : IEnumerator<U> where U : class
    {
        private readonly IReadOnlyList<U> _list;
        private int _index = -1;
        public readonly U Current
        {
            get
            {
                if (_index < 0 || _index >= _list.Count)
                {
                    throw new InvalidOperationException();
                }
                return _list[_index];
            }
        }
        readonly object IEnumerator.Current
        {
            get => Current;
        }

        public Enumerator(IReadOnlyList<U> list)
        {
            _list = list;
        }

        public readonly void Dispose() { }

        public bool MoveNext()
        {
            if (_index < _list.Count - 1)
            {
                _index++;
                return true;
            }
            else
            {
                _index = _list.Count;
                return false;
            }
        }

        void IEnumerator.Reset()
        {
            _index = -1;
        }
    }
}

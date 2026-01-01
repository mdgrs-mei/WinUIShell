using System.Collections;
using WinUIShell.Common;

namespace WinUIShell;

public class WinUIShellObjectList<T> : WinUIShellObject, IList<T> where T : class
{
    public int Count
    {
        get => PropertyAccessor.Get<int>(Id, nameof(Count))!;
    }

    public bool IsReadOnly
    {
        get => PropertyAccessor.Get<bool>(Id, nameof(IsReadOnly))!;
    }

    public T this[int index]
    {
        get => PropertyAccessor.GetIndexer<T>(Id, index)!;
        set
        {
            if (value is WinUIShellObject obj)
            {
                PropertyAccessor.SetIndexer(Id, obj.Id, index);
            }
            else
            {
                PropertyAccessor.SetIndexer(Id, value, index);
            }
        }
    }

    internal WinUIShellObjectList(ObjectId id)
        : base(id)
    {
    }

    public int IndexOf(T item)
    {
        if (item is WinUIShellObject obj)
        {
            return CommandClient.Get().InvokeMethodAndGetResult<int>(Id, nameof(IndexOf), obj.Id);
        }
        else
        {
            return CommandClient.Get().InvokeMethodAndGetResult<int>(Id, nameof(IndexOf), item);
        }
    }

    public void Insert(int index, T item)
    {
        if (item is WinUIShellObject obj)
        {
            CommandClient.Get().InvokeMethod(Id, nameof(Insert), index, obj.Id);
        }
        else
        {
            CommandClient.Get().InvokeMethod(Id, nameof(Insert), index, item);
        }
    }

    public void RemoveAt(int index)
    {
        CommandClient.Get().InvokeMethod(Id, nameof(RemoveAt), index);
    }

    public void Add(T item)
    {
        if (item is WinUIShellObject obj)
        {
            CommandClient.Get().InvokeMethod(Id, nameof(Add), obj.Id);
        }
        else
        {
            CommandClient.Get().InvokeMethod(Id, nameof(Add), item);
        }
    }

    public void Clear()
    {
        CommandClient.Get().InvokeMethod(Id, nameof(Clear));
    }

    public bool Contains(T item)
    {
        if (item is WinUIShellObject obj)
        {
            return CommandClient.Get().InvokeMethodAndGetResult<bool>(Id, nameof(Contains), obj.Id);
        }
        else
        {
            return CommandClient.Get().InvokeMethodAndGetResult<bool>(Id, nameof(Contains), item);
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
        if (array.Length <= arrayIndex && Count > 0)
        {
            throw new ArgumentOutOfRangeException($"{nameof(arrayIndex)}");
        }

        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("Insufficient space to copy.");
        }

        int num = Count;
        for (int i = 0; i < num; i++)
        {
            array[i + arrayIndex] = this[i];
        }
    }

    public bool Remove(T item)
    {
        if (item is WinUIShellObject obj)
        {
            return CommandClient.Get().InvokeMethodAndGetResult<bool>(Id, nameof(Remove), obj.Id);
        }
        else
        {
            return CommandClient.Get().InvokeMethodAndGetResult<bool>(Id, nameof(Remove), item);
        }
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
        private readonly IList<U> _list;
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

        public Enumerator(IList<U> list)
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

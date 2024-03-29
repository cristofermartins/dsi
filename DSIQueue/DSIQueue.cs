using System;
using System.Collections;

namespace DSI
{
    public class DSIQueue<T> : IReadOnlyCollection<T>, ICollection
    {
        public DSIQueue() {}

        public void Enqueue(T value)
        {
            DSIQueueData<T> novo = new DSIQueueData<T>(value);
            if (_last != null)
            {
                _last.Next = novo;
            }
            else 
            {
                _first = novo;
            }
            _last = novo;

            _count++;
        }

        public T Dequeue()
        {
            if (_first == null)
            {
                throw new Exception("Não é possivel usar o metodo Dequeue em uma Queue vazia vazia.");
            }

            T value = _first.Value;
            _first = _first.Next;

            if (_first == null)
            {
                _last = null;
            }

            _count--;

            return value;
        }

        public bool Empty()
        {
            return (_count == 0);
        }

        public void Clear()
        {
            _first = null;
            _last = null;
            _count = 0;
        }

        // IReadOnlyCollection, ICollection
        public int Count
        {
            get 
            {
                return _count;
            }
        }

        // ICollection
        public bool IsSynchronized { get => false; }

        public object SyncRoot { get => this; }

        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException("Array é multidimencional.");
            }

            if ((index + array.Length) < this.Count)
            {
                throw new ArgumentException("Não há espaço no array a partir desse index para os items dentro da stack.");
            }

            // generics
            if (array is T[] typedArray)
            {
                DSIQueueData<T>? aux = this._first;
                while (aux != null)
                {
                    typedArray[index] = aux.Value;
                    aux = aux.Next;
                    index++;
                }
                return;
            }

            // Object
            if (array is object[] objectArray)
            {
                DSIQueueData<T>? aux = this._first;
                while (aux != null)
                {
                    objectArray[index] = aux.Value!;
                    aux = aux.Next;
                    index++;
                }
                return;
            }

            throw new ArgumentException("O Tipo do array não bate com o Tipo de dado da queue.");
        }

        // IEnumerable
        public IEnumerator<T> GetEnumerator()
        {
            return new DSIQueueEnumerator<T>(this, _first);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private DSIQueueData<T>? _first = null;
        private DSIQueueData<T>? _last = null;
        private int _count = 0;
    }

    internal class DSIQueueData<T>
    {
        public DSIQueueData(T value)
        {
            Value = value;
        }

        public readonly T Value;
        public DSIQueueData<T>? Next = null;
    }

    internal class DSIQueueEnumerator<T> : IEnumerator<T> 
    {
        public DSIQueueEnumerator(DSIQueue<T> queue, DSIQueueData<T>? first)
        {
            _queue = queue;
            _queueInitCount = _queue.Count;
            _first = first;
        }

        // IEnumerator
        public T Current
        {
            get
            {
                if (_currentStackValue == null)
                {
                    throw new InvalidOperationException();
                }
                return _currentStackValue.Value;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.Current!;
            }
        }

        public bool MoveNext()
        {
            if (_currentStackValue == null)
            {
                if (_first != null)
                {
                    _currentStackValue = _first;
                    return true;
                }

                return false; 
            }

            if (_currentStackValue.Next != null)
            {
                _currentStackValue = _currentStackValue.Next;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _currentStackValue = null;
            if (_queueInitCount != _queue.Count)
            {
                throw new InvalidOperationException();
            }
        }

        // IDisposable
        public void Dispose() {}

        private readonly DSIQueue<T> _queue;
        private readonly int _queueInitCount;
        private DSIQueueData<T>? _currentStackValue = null;
        private DSIQueueData<T>? _first = null;
    }
}
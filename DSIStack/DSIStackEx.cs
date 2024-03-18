using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace DSI
{
    public class DSIStackEx<T> : IReadOnlyCollection<T>, ICollection
    {
        private const double RESIZE_DELTA = 1.2d;
        public void Add(T value)
        {
            _count++;
            if (_internalArray.Length < _count)
            {
                Array.Resize(ref _internalArray, (int)Math.Ceiling(_count * RESIZE_DELTA));
            }
            _internalArray[_count -1] = value;
        }

        public T Peek()
        {
            if (Empty())
            {
                throw new InvalidOperationException("Stack está vazia.");
            }
            return _internalArray[_count -1]!;           
        }

        public void Remove()
        {
            if (Empty())
            {
                throw new InvalidOperationException("Stack está vazia.");
            }
            _count--;
            _internalArray[_count] = default;
        }

        public T Pop()
        {
            if (Empty())
            {
                throw new InvalidOperationException("Stack está vazia.");
            }
            _count--;
            T aux = _internalArray[_count]!;
            _internalArray[_count] = default;
            return aux;
        }

        public bool Empty()
        {
            return (_count == 0);
        }

        public void Clear()
        {
            _count = 0;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_internalArray[i]!.Equals(item))
                {
                    return true;
                }
            }

            return false;
        }

        public T this[int index]
        {
            get 
            {
                if (index < _count)
                {
                    return _internalArray[index]!;
                }
  
                throw new IndexOutOfRangeException(nameof(index));
            }
        }

        public void ShrinkToFit()
        {
            Array.Resize(ref _internalArray, _count);
        }

        public void Reserve(int amount)
        {
            if (amount > Capacity)
            {
                Array.Resize(ref _internalArray, amount);
            }
        }
        public int Capacity {get => _internalArray.Length; }

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
                for (int i = 0; i < _count; i++)
                {
                    typedArray[index] = _internalArray[i]!;
                    index++;
                }
                return;
            }

            // Object
            if (array is object[] objectArray)
            {
                for (int i = 0; i < _count; i++)
                {
                    objectArray[index] = _internalArray[i]!;
                    index++;
                }
            }

            throw new ArgumentException("O Tipo do array não bate com o Tipo de dados da stack.");
        }

        // IEnumerable
        public IEnumerator<T> GetEnumerator()
        {
            return new StackEnumeratorEx<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private int _count = 0;
        private T?[] _internalArray = Array.Empty<T?>();
    }
 
    internal class StackEnumeratorEx<T> : IEnumerator<T> 
    {
        public StackEnumeratorEx(DSIStackEx<T> stack)
        {
            _stack = stack;
            _stackInitialCount = _stack.Count;
        }

        // IEnumerator
        public T Current
        {
            get
            {
                if (_currentIndex == -1)
                {
                    throw new InvalidOperationException();
                }
                return _stack[_currentIndex];
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
            _currentIndex++;
            return (_currentIndex < _stackInitialCount);
        }

        public void Reset()
        {
            _currentIndex = -1;
            if (_stackInitialCount != _stack.Count)
            {
                throw new InvalidOperationException();
            }
        }

        // IDisposable
        public void Dispose() {}

        private DSIStackEx<T> _stack {get; init;}
        private int _stackInitialCount {get; init;}
        private int _currentIndex {get; set;} = -1;
    }
}

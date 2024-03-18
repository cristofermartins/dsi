using System;
using System.Collections;

namespace DSI
{
    internal class DSIStackValue<N>
    {
        public DSIStackValue(N value, DSIStackValue<N>? next)
        {
            Value = value;
            Next = next;
        }

        public N Value;
        public DSIStackValue<N>? Next;
    }

    public class DSIStack<T> : IReadOnlyCollection<T>, ICollection
    {
        public void Add(T value)
        {
            _count++;
            if (_top == null)
            {
                _top = new DSIStackValue<T>(value, null);
                return;
            }
            DSIStackValue<T> next = new DSIStackValue<T>(value, _top);
            _top = next;
        }

        public T Peek()
        {
            if (Empty())
            {
                throw new InvalidOperationException("Stack está vazia.");
            }
            return _top!.Value;           
        }

        public bool Remove()
        {
            if (Empty())
            {
                throw new InvalidOperationException("Stack está vazia.");
            }
            _top = _top!.Next;
            _count--;
            return true;
        }

        public T Pop()
        {
            if (Empty())
            {
                throw new InvalidOperationException("Stack está vazia.");
            }
            _count--;
            DSIStackValue<T> aux = _top!;
            _top = aux.Next;
            return aux.Value;
        }

        public bool Empty()
        {
            return (_count == 0);
        }

        public void Clear()
        {
            while (_top != null)
            {
                DSIStackValue<T> aux = _top; 
                _top = _top.Next; 
                aux.Next = null;
            }
            _count = 0;
        }

        public bool Contains(T item)
        {
            DSIStackValue<T>? aux = this._top;
            while (aux != null)
            {
                if (aux.Value!.Equals(item))
                {
                    return true;
                }

                aux = aux.Next;
            }

            return false;
        }

        public T this[int index]
        {
            get 
            {
                int i = 0;
                DSIStackValue<T>? aux = this._top;
                while (aux != null)
                {
                    if (i == index)
                    {
                        return aux.Value;
                    }
                    aux = aux.Next;
                    i++;
                }
  
                throw new IndexOutOfRangeException(nameof(index));
            }
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
                DSIStackValue<T>? aux = this._top;
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
                DSIStackValue<T>? aux = this._top;
                while (aux != null)
                {
                    objectArray[index] = aux.Value!;
                    aux = aux.Next;
                    index++;
                }
            }

            throw new ArgumentException("O Tipo do array não bate com o Tipo de dados da stack.");
        }

        // IEnumerable
        public IEnumerator<T> GetEnumerator()
        {
            return new StackEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal DSIStackValue<T>? _top = null;
        private int _count = 0;
    }
 
    internal class StackEnumerator<T> : IEnumerator<T> 
    {
        public StackEnumerator(DSIStack<T> stack)
        {
             _currentStackValue = null;
            _stack = stack;
            _stackInitialCount = _stack.Count;
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
                if (_stack.Empty())
                {
                    return false;
                }

                _currentStackValue = _stack._top!;
                return true;
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
            if (_stackInitialCount != _stack.Count)
            {
                throw new InvalidOperationException();
            }
        }

        // IDisposable
        public void Dispose() {}

        private DSIStack<T> _stack {get; init;}
        private int _stackInitialCount {get; init;}
        private DSIStackValue<T>? _currentStackValue {get; set;}
    }
}

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Dynamic;

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
            _Count++;
            if (Top == null)
            {
                Top = new DSIStackValue<T>(value, null);
                return;
            }
            DSIStackValue<T> next = new DSIStackValue<T>(value, Top);
            Top = next;
        }

        public T Peek()
        {
            if (Empty())
            {
                throw new InvalidOperationException("Stack está vazia.");
            }
            return Top!.Value;           
        }

        public bool Remove()
        {
            if (Empty())
            {
                throw new InvalidOperationException("Stack está vazia.");
            }
            Top = Top!.Next;
            _Count--;
            return true;
        }

        public T Pop()
        {
            if (Empty())
            {
                throw new InvalidOperationException("Stack está vazia.");
            }
            _Count--;
            DSIStackValue<T> aux = Top!;
            Top = aux.Next;
            return aux.Value;
        }

        public bool Empty()
        {
            return (_Count == 0);
        }

        public void Clear()
        {
            while (Top != null)
            {
                DSIStackValue<T> aux = Top; 
                Top = Top.Next; 
                aux.Next = null;
            }
            _Count = 0;
        }

        public bool Contains(T item)
        {
            DSIStackValue<T>? aux = this.Top;
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
                DSIStackValue<T>? aux = this.Top;
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

        public int Count
        {
            get 
            {
                return _Count;
            }
        }

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
                throw new ArgumentException();
            }

            if ((index + array.Length) < this.Count)
            {
                throw new ArgumentException();
            }

            // generics
            T[]? typedArray = array as T[];
            if (typedArray != null)
            {
                DSIStackValue<T>? aux = this.Top;
                while (aux != null)
                {
                    typedArray[index] = aux.Value;
                    aux = aux.Next;
                    index++;
                }
                return;
            }

            // Object
            object[]? objectArray = array as object[];
            if (objectArray != null)
            {
              DSIStackValue<T>? aux = this.Top;
                while (aux != null)
                {
                    objectArray[index] = aux.Value!;
                    aux = aux.Next;
                    index++;
                }
            }

            throw new ArgumentException("Target array type is not compatible with the type of items in the collection.");
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new StackEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal DSIStackValue<T>? Top = null;
        private int _Count = 0;
    }
 
    internal class StackEnumerator<T> : IEnumerator<T> 
    {
        private DSIStack<T> _stack;
        private DSIStackValue<T>? _currentStackValue;
        private  int _stackInitialCount = 0;

        public StackEnumerator(DSIStack<T> stack)
        {
            _stack = stack;
            _currentStackValue = null;
            _stackInitialCount = _stack.Count;
        }

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

        // Implement MoveNext and Reset, which are required by IEnumerator.
        public bool MoveNext()
        {
            if (_currentStackValue == null)
            {
                if (_stack.Empty())
                {
                    return false;
                }

                _currentStackValue = _stack.Top!;
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

        // Implement IDisposable, which is also implemented by IEnumerator(T).
        private bool disposedValue = false;
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // Dispose of managed resources.
                }
            }

            this.disposedValue = true;
        }

        ~StackEnumerator()
        {
            Dispose(disposing: false);
        }
    }
}
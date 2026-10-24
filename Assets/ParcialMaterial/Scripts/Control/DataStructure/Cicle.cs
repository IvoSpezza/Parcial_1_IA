using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Cicle<T> 
{
    public T Current => _values.Peek();

    private Queue<T> _values = new ();

    public bool IsEmpty => _values.Count == 0;
    
    public Cicle(List<T> path)
    {
        foreach (var item in path)
        {
            _values.Enqueue(item);
        }
    }

    public T Next()
    {
        T val = _values.Dequeue();
        _values.Enqueue(val);
        return val;
    }
}

#region GPLv2

/*
KeiSystem
Copyright (C) 2015 MIC Studio
Developer homepage: https://github.com/GridSciense

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

#endregion

#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kei
{

    // 行为：
    // TryGetValue 是正常行为；
    // this[](set) 是正常行为；
    // this[](get) 若指定了一个不存在的 key，则会根据该 key 新建一个 TValue 类型的值，加入字典，并返回这个加入的值
    // ContainsKey 是正常行为；
    // Contains(KeyValuePair<TKey, TValue>) 要求比较 key，若 key 存在还要求对应的值 Equals(value)
    // Remove(KeyValuePair<TKey, TValue>) 忽略了 value

    public class AutoAugmentDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TValue : new()
    {

        private Dictionary<TKey, TValue> _dictionary;

        public AutoAugmentDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _dictionary.ContainsValue(value);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return _dictionary.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get
            {
                return _dictionary.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_dictionary.ContainsKey(key))
                {
                    return _dictionary[key];
                }
                else
                {
                    var t = new TValue();
                    _dictionary.Add(key, t);
                    return t;
                }
            }
            set
            {
                _dictionary[key] = value;
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _dictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <summary>
        /// Calls Equals.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item) && _dictionary[item.Key].Equals(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.ToArray().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return _dictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
    }
}
#endif

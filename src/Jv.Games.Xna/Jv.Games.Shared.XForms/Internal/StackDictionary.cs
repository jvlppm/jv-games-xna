using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jv.Games.Xna.XForms
{
    class StackCollection<TValue> : Stack<TValue>, ICollection<TValue>
    {
        public void Add(TValue item)
        {
            base.Push(item);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TValue item)
        {
            bool found = false;
            Stack<TValue> temp = new Stack<TValue>();
            while(Count > 0)
            {
                var nextItem = Pop();
                if (Object.Equals(nextItem, item))
                {
                    found = true;
                    break;
                }
                temp.Push(item);
            }

            while (temp.Count > 0)
                Push(temp.Pop());

            return found;
        }
    }

    static class StackDictionary<TKey, TValue>
    {
        public static MultiValueDictionary<TKey, TValue> Create()
        {
            return MultiValueDictionary<TKey, TValue>.Create(() => new StackCollection<TValue>());
        }
    }
}

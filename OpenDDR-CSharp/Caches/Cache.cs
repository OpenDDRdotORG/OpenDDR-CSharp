/**
 * Copyright 2011 OpenDDR LLC
 * This software is distributed under the terms of the GNU Lesser General Public License.
 *
 *
 * This file is part of OpenDDR Simple APIs.
 * OpenDDR Simple APIs is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, version 3 of the License.
 *
 * OpenDDR Simple APIs is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with Simple APIs.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace Oddr.Caches
{
    public class Cache : ICache
    {
        private Dictionary<string, object> dic;
        private Queue<String> queue;
        private int cacheSize;

        public Cache(int cacheSize)
        {
            Init(cacheSize);
        }

        public Cache()
        {
            Init(100);
        }

        private void Init(int cacheSize)
        {
            this.cacheSize = cacheSize;
            this.dic = new Dictionary<string, object>(cacheSize);
            this.queue = new Queue<string>(cacheSize);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public object GetCachedElement(string id)
        {
            object toRet = null;
            dic.TryGetValue(id, out toRet);
            return toRet; 
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetCachedElement(string id, object value)
        {
            dic.Add(id, value);
            queue.Enqueue(id);
            if (dic.Count > cacheSize)
            {
                String toRemove = queue.Dequeue();
                dic.Remove(toRemove);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Clear()
        {
            dic.Clear();
            queue.Clear();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int UsedEntries()
        {
            return dic.Count;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<KeyValuePair<string, object>> GetAll()
        {
            List<KeyValuePair<string, object>> list = new List<KeyValuePair<string,object>>();

            foreach (KeyValuePair<string, object> kvp in dic)
            {
                list.Add(kvp);
            }

            return list;
        }
    }
}

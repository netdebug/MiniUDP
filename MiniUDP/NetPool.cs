﻿/*
 *  MiniUDP - A Simple UDP Layer for Shipping and Receiving Byte Arrays
 *  Copyright (c) 2015-2018 - Alexander Shoulson - http://ashoulson.com
 *
 *  This software is provided 'as-is', without any express or implied
 *  warranty. In no event will the authors be held liable for any damages
 *  arising from the use of this software.
 *  Permission is granted to anyone to use this software for any purpose,
 *  including commercial applications, and to alter it and redistribute it
 *  freely, subject to the following restrictions:
 *  
 *  1. The origin of this software must not be misrepresented; you must not
 *     claim that you wrote the original software. If you use this software
 *     in a product, an acknowledgment in the product documentation would be
 *     appreciated but is not required.
 *  2. Altered source versions must be plainly marked as such, and must not be
 *     misrepresented as being the original software.
 *  3. This notice may not be removed or altered from any source distribution.
*/

using System.Collections.Generic;

namespace MiniUDP
{
  internal interface INetPoolable<T>
    where T : INetPoolable<T>
  {
    void Reset();
  }

  internal interface INetPool<T>
  {
    T Allocate();
    void Deallocate(T obj);
  }

  internal class NetPool<T> : INetPool<T>
    where T : INetPoolable<T>, new()
  {
    private readonly Stack<T> freeList;

    public NetPool()
    {
      this.freeList = new Stack<T>();
    }

    public T Allocate()
    {
      if (this.freeList.Count > 0)
        return this.freeList.Pop();

      T obj = new T();
      obj.Reset();
      return obj;
    }

    public void Deallocate(T obj)
    {
      obj.Reset();
      this.freeList.Push(obj);
    }
  }
}

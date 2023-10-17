using System;
using System.Collections.Generic;

namespace EventManager.Model
{
    public interface INewsProvider
    {
        event Action OnNewsUpdate;
        IEnumerable<News> GetNews(int? count = null);
    }
}
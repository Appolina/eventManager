using EventManager.Model;
using System;

namespace EventManager.ViewModel
{
    public class NewsViewModel
    {
        private News news;

        public NewsViewModel(News news)
        {
            this.news = news;
        }

        public string Text { get { return this.news.Text; } }
        public Uri Photo { get { if (news.PhotoUri == null) return null; else return new Uri(news.PhotoUri); } }

        public string Date
        {
            get
            {
                var daysSpan = DateTime.Now.Day - this.news.NewsDate.Day;
                if (daysSpan > 1)
                    return this.news.NewsDate.ToString("dd MMMM");
                else
                {
                    if (daysSpan == 1)
                    {
                        return $"вчера в {this.news.NewsDate.ToString("H:mm")}";
                    }
                    else
                    {
                        var timeSpan = DateTime.Now - this.news.NewsDate;
                        if (timeSpan.Hours > 0)
                        {
                            return $"сегодня в {this.news.NewsDate.ToString("H:mm")}";
                        }
                        else
                        {
                            return $"{timeSpan.Minutes} минут назад";
                        }
                    }
                }
            }
        }
    }
}
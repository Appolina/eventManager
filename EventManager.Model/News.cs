using SQLite;
using System;

namespace EventManager.Model
{
    public class News
    {
        public enum enNewsType { EventDeclare = 0, MemerClaimed = 1 }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public DateTime NewsDate { get; set; }

        public string Content { get; set; }

        public string PhotoUri { get; set; }

        public enNewsType Type { get; set; }

        [Ignore]
        public string Text
        {
            get
            {
                if (string.IsNullOrEmpty(this.Content))
                {
                    switch (this.Type)
                    {
                        case enNewsType.EventDeclare:
                            return "Опубликовано объявление о событии";
                        case enNewsType.MemerClaimed:
                            return "На событие записался";
                        default:
                            return "Не определен тип новости";
                    }
                }
                else
                {
                    return this.Content;
                }
            }
        }

        public News()
        {

        }

        public News(DateTime date, string photoUri, enNewsType type, string content = null): this()
        {
            this.NewsDate = date;
            this.PhotoUri = photoUri;
            this.Type = type;
            this.Content = content;
        }

        public override string ToString()
        {
            return $"text = {this.Text}, date = {NewsDate.ToString("d MMMM")}";
        }
    }
}
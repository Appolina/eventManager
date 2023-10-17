using System;

namespace EventManager.Model
{
    public class Comment
    {
        public int FromId { get; private set; }

        public string Text { get; private set; }

        public DateTime Date { get; private set; }


        public Comment(string text, int fromId, DateTime date)
        {
            this.FromId = fromId;
            this.Text = text;
            this.Date = date;
        }

        public override string ToString()
        {
            return $"{Text} {Date.ToString()}";
        }
    }
}

using System;

namespace EventManager.Model
{
    public class WallPost
    {
        public int Id { get; private set; }
        public int IdOwner { get; private set; }
        public string Text { get; private set; }
        public DateTime Date { get; private set; }

        public WallPost(int id, int idOwner, string text, DateTime date)
        {
            this.Id = id;
            this.IdOwner = idOwner;
            this.Text = text;
            this.Date = date;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace EventManager.Model
{
    internal class WallForEventParser
    {
        private const string CLAIMERS_TITLE = "Идут:";
        private string standartSuffix;//= " в 21:00. 1.5 часа. 2 минуты от м. площадь Мужества. Стоимость 150р. Непокорённых 10. Кто желает, записывайтесь!";
        private string tempSuffix = Environment.NewLine + "Для записавшихся втечение дня с момента объявления - БОНУС: размещение в тексте объявления!";

        private string patternForDayMatch;
        private string patternForMonthMatch;
        private Dictionary<string, int> mounthDict;

        public int TITLE { get; private set; }

        public WallForEventParser(string standartSuffix)
        {
            patternForDayMatch = $"(\\d+) \\w+{standartSuffix}";
            patternForMonthMatch = $"\\d+ (\\w+){standartSuffix}";

            mounthDict = new Dictionary<string, int>();
            int i = 1;
            mounthDict.Add("января", i++);
            mounthDict.Add("февраля", i++);
            mounthDict.Add("марта", i++);
            mounthDict.Add("апреля", i++);
            mounthDict.Add("мая", i++);
            mounthDict.Add("июня", i++);
            mounthDict.Add("июля", i++);
            mounthDict.Add("августа", i++);
            mounthDict.Add("сентября", i++);
            mounthDict.Add("октября", i++);
            mounthDict.Add("ноября", i++);
            mounthDict.Add("декабря", i++);

            this.standartSuffix = standartSuffix;
        }

        internal bool IsEventPost(WallPost post)
        {
            return post.Text.Replace(" ", "").Contains(standartSuffix.Replace(" ", ""));
        }

        internal Event GetEventFromPost(WallPost post)
        {
            var dayMatch = Regex.Match(post.Text, patternForDayMatch);

            var day = Int32.Parse(dayMatch.Groups[1].Value);

            var monthMatch = Regex.Match(post.Text, patternForMonthMatch);


            var mounth = monthMatch.Groups[1].Value;

            var @event = new Event(new DateTime(DateTime.Now.Year, mounthDict[mounth], day, 21, 0, 0), post.Id);
            return @event;
        }

        private string getStringForMonth(int month)
        {
            return this.mounthDict.Single(kvp => kvp.Value == month).Key;
        }


        internal string UpdateTextWithClaimers(WallPost eventPost, IEnumerable<Person> claimedMembers)
        {
            //var startOfSign = eventPost.Text.IndexOf(this.standartSuffix);
            //var theEndIndexOfInitialSign = startOfSign + this.standartSuffix.Length;

            var theEndIndexOfInitialSign = eventPost.Text.IndexOf(CLAIMERS_TITLE);

            if (theEndIndexOfInitialSign < 0)
                theEndIndexOfInitialSign = eventPost.Text.Length;
            else
                theEndIndexOfInitialSign -= 3;

            var result = eventPost.Text.Substring(0, theEndIndexOfInitialSign);

            int nb = 1;

            foreach (var member in claimedMembers)
            {
                if (nb == 1)
                    result += Environment.NewLine + Environment.NewLine + CLAIMERS_TITLE + Environment.NewLine;

                var caressingNamesDict = new Dictionary<string, string>();

                caressingNamesDict.Add("Stepan", "Степа");
                caressingNamesDict.Add("Мария", "Маша");
                caressingNamesDict.Add("Константин", "Костя");
                caressingNamesDict.Add("Petr", "Петя");
                caressingNamesDict.Add("Катерина", "Катя");

                string resultName;

                if (!caressingNamesDict.TryGetValue(member.FirstName, out resultName))
                    resultName = member.FirstName;

                result += Environment.NewLine + $"{nb}. @id{member.Id} ({resultName})";
                nb++;
            }

            return result;
        }

        internal string TextForNewEventSign()
        {
            var cal = CultureInfo.InvariantCulture.Calendar;
            var nextEventDate = DateTime.Now;

            while (cal.GetDayOfWeek(nextEventDate) != DayOfWeek.Tuesday)
            {
                nextEventDate = nextEventDate.AddDays(1);
            }

            return $"{ nextEventDate.Day } { this.getStringForMonth(nextEventDate.Month) }{ this.standartSuffix }{ this.tempSuffix }";
        }
    }
}
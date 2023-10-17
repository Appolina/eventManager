using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EventManager.Model
{
    internal class CommentForEventParser
    {
        private IEnumerable<Person> personsCanBeClaimed;

        public CommentForEventParser(IEnumerable<Person> personsCanBeClaimed)
        {
            this.personsCanBeClaimed = personsCanBeClaimed;
        }

        internal ParseForClaimResult ParseForClaim(Comment comment)
        {
            var satisfyPattern = Regex.Match(comment.Text, "([\\d+\\.]|^)[ ]?Я", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase).Success;

            var containsI = Regex.Match(comment.Text, "(^| )?Я($| )", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase).Success;
            var containsNo = Regex.Match(comment.Text, "(^| )?не($| )", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase).Success;

            if (satisfyPattern || (containsI && !containsNo))
            {
                return new ParseForClaimResult() { Success = true, PersonId = comment.FromId };
            }

            if (containsNo)
                return new ParseForClaimResult() { Success = false };

            var suitableMembers = personsCanBeClaimed.Where(p => comment.Text.IndexOf(p.FirstName) >= 0 || comment.Text.IndexOf(p.LastName) >= 0);

            if (suitableMembers.Count() == 1)
                return new ParseForClaimResult() { Success = true, PersonId = suitableMembers.First().Id };
            else
                return new ParseForClaimResult() { Success = false };
        }

        internal struct ParseForClaimResult
        {
            public int PersonId;
            public bool Success;
        }
    }
}
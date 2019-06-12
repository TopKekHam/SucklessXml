
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Suckless.Xml
{

    public static class ListExtantions
    {

        //Takens string and Func<T,bool>, start index and end index, return the index of the first match, returns -1 if no item found. 
        public static int FirstIndex<T>(this IEnumerable<T> self, Func<T, bool> checkFunc, int start, int end)
        {
            int idx = start;
            var take = self.Count() - start - (self.Count() - end - 1);
            var list = self.Skip(start).Take(take);

            foreach (var item in list)
            {
                if (checkFunc(item))
                {
                    return idx;
                }
                else
                {
                    idx++;
                }
            }

            return -1;
        }

        public static int FirstIndex(this string self, Func<char, bool> checkFunc, int start, int end)
        {

            var take = self.Length - start - (self.Length - end - 1);

            for (int idx = start; idx < end; idx++)
            {
                 if (checkFunc(self[idx]))
                {
                    return idx;
                }
            }

            return -1;
        }

        public static int FirstIndex<T>(this IEnumerable<T> self, Func<T, bool> checkFunc)
        {
            return FirstIndex(self, checkFunc, 0, self.Count() - 1);
        }

        public static int FirstIndex(this string self, Func<char, bool> checkFunc, int start)
        {
            return self.FirstIndex(checkFunc, start, self.Length);
        }

        public static int FirstMatch(this string self, string match, int start, int end)
        {
            var matches = Regex.Matches(self, match);
            var matchIdx = 0;

            while (matchIdx < matches.Count && matches[0].Index < start)
            {
                matchIdx++;
            }

            if (matchIdx > end - match.Length)
            {
                return -1;
            }

            return matchIdx;

        }

        public static T Pop<T>(this List<T> self)
        {
            return self.PopAt(0);
        }

        public static T PopBottom<T>(this List<T> self)
        {
            return self.PopAt(self.Count - 1);
        }

        public static T PopAt<T>(this List<T> self, int index)
        {
            var temp = self[index];
            self.RemoveAt(index);
            return temp;
        }

    }
}
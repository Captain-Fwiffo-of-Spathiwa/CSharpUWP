using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Media.AppBroadcasting;



/*-------------------------------------------------------------------------
   [] Create a function that takes a natural language version of a date
      (eg “Next Tuesday”, “This Wednesday”, “Tomorrow”, “Friday”.) and
      turns it into a specific date. You should carefully consider where
      you draw the line between “this” and “next”. When does “next
      Wednesday” become “this Wednesday”? Is there a difference between
      “Friday”, “this Friday” and “next Friday”? Get it wrong and you
      risk users getting confused.

   [] Create a function that takes a time in natural language (eg “6”,
      “six in the morning”. “Half past six”, “3 PM”, “three in the
      afternoon”) and turns it into a specific time. Note that you will
      have to make a call on whether the user most likely meant morning
      or afternoon, so “Five o’clock” would most likely mean in the
      afternoon, for example. In the final app, we can let the user change
      it, but we should have a good default. 
  ------------------------------------------------------------------------*/
namespace TaskManagement.Helpers
{
    public static class StringParse
    {
        static public DateTime ParseNaturalDate(string naturalDate)
        {
            // Turn the user's input into a list of clean words
            List<string> dayWords = GetCleanWordsAndNumbers(naturalDate);       // ♪ Fighter of the night words ♪

            // Respond based on word count
            switch (dayWords.Count)
            {
                case 1: return ParseOneWordNaturalDate(dayWords[0]);
                case 2: return ParseTwoWordNaturalDate(dayWords[0], dayWords[1]);
                case 3: return ParseThreeWordNaturalDate(dayWords[0], dayWords[1], dayWords[2]);
                default: throw new ArgumentException("Invalid natural date format.");
            }
        }

        static public TimeSpan ParseNaturalTime(string naturalTime)
        {
            TimeSpan militaryTime = TimeSpan.Zero;
            int wordIndex = 0;

            List<string> timeWords = GetCleanWordsAndNumbers(naturalTime);

            if (TimePrepositions.Contains(timeWords[0]))
            {
                wordIndex = 1;
            }

            // Immutable strings, passing reference by reference, non-ref returning properties... Ok C#, calm down.
            string nextWord = timeWords[wordIndex];
            CorrectSpellingFromDictionary(ref nextWord, ClockSegmentMisspellings);

            return militaryTime;


            /*
                Make notes that it doesn't support:
                    - 24 hour time
                    - specific clock times - You can only use quarters or halves to/past
                    - specific minutes to or past
                    - "night"

                1. Clean. Replace ":,." with space, "'" with nothing, Turn into list of words and numbers.
                2. From now we're tracking "current word index".
                3. Loop ignore PREPOSITION or PREAMBLE: at/for/on/by/before or a.
                4. Look for PREFIX: quarter/half - misspellings ok
                    a. If yes, look for "past/to" in next word. Throw or set minutes offset then continue to step 5.
                    b. If no, continue to step 5.
                5. Look for HOUR: number or misspelled hour. Only 12 hour time.
                    a. If word is a mispelled hour, good.
                    b. Else if 1 or 2 characters, parse number.
                    c. Else if 3 or 4 characters, parse number + check am or pm and done. Or throw and whatever.


                6. Look for MINUTES: ":27".
                    a. If word begins ":"
                        i. If offset already exists, throw.
                       ii. If not 3 chars, throw.
                      iii. Parse chars [1] and [2]. If not 0-59, throw.
                       iv. Set offset.
                7. Look for MERIDIAN: Loop through remaining words, discarding "on, in, at, during, the, oclock". 
                                      When you find a AM, morning, PM, afternoon, evening, note which and break.
                   


             */
        }


        /*---------------------
           ~ Private helpers ~
          ---------------------*/

        static List<string> GetCleanWordsAndNumbers(string toClean)
        {
            // Replacing some things with space helps with inputs like "7:30" or "on Monday,at 6" 
            string paddedToClean = toClean.Replace(':', ' ').Replace('.', ' ').Replace(',', ' ');
            string cleanSentence = new string(toClean.Trim().ToLower().Where(c => char.IsLetterOrDigit(c)|| c == ' ').ToArray());
            return cleanSentence.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        static void CorrectSpellingFromDictionary(ref string candidate, Dictionary<string, List<string>> misspellings)
        {
            foreach (var targetWord in misspellings.Keys)
            {
                if (misspellings[targetWord].Contains(candidate))
                {
                    candidate = targetWord;
                }
            }
        }

        // Prepositions are acceptable to encounter but are then discarded
        static List<string> DatePrepositions = ["at", "by", "for", "on", "in"];
        
        // Article "a" could appear in "a quarter", so junk that too
        static List<string> TimePrepositions = ["at", "by", "for", "on", "before", "a"];    

        /// <summary>
        /// A very basic spelling correction lookup for weekdays, today, and tomorrow.
        /// </summary>
        static Dictionary<string, List<string>> DayMisspellings = new()
        {
            {"today"    ,  ["todya", "tody", "todat", "toady", "todau", "todey", "todday", "todsy", "otday", "ttoday", "todae", "toda", "roday"] }, 
            {"tomorrow" ,  ["tommorow", "tommorrow", "tomorow", "tomoro", "tomorrw", "tomorroe", "tomorraw", "tomorw", "tmoorrow", "otmorrow", "romorrow", "tormorrow"] }, 
            {"monday"   ,  ["mondya", "monady", "mondy", "mnday", "moneday", "mondai", "mnoday", "monda", "mondae", "nomday", "moonday", "mionday"] }, 
            {"tuesday"  ,  ["teusday", "tuseday", "tusday", "tuesdy", "tuesdya", "tuesady", "teusdy", "tueday", "twesday", "tuesdae", "tuesda", "tyuesday"] }, 
            {"wednesday",  ["wensday", "wendsay", "wednesdy", "wednesdya", "wendsday", "wedendsay", "wedensday", "wednsday", "wennesday", "wednesdae", "wednesda", "wendesday"] }, 
            {"thursday" ,  ["thurdsay", "thrusday", "thursdy", "thursdya", "thursady", "thusday", "thurday", "thirsday", "thursdae", "trhursday", "thursda", "thuresday"] }, 
            {"friday"   ,  ["firday", "firady", "fridya", "fridy", "fridai", "fridae", "frida", "froday", "fiday", "fridasy", "firdy", "fridau"] }, 
            {"saturday" ,  ["saterday", "saturdy", "saturdya", "saturady", "satuday", "sadurday", "satturday", "saterdy", "sataday", "saturdae", "saturda", "satrday"] }, 
            {"sunday"   ,  ["sundya", "sundy", "sunady", "sundai", "sundae", "snday", "suday", "suuday", "sunnday", "sundau", "sumday", "sunndy"] }
        };

        static Dictionary<string, List<string>> TimeOfDayMisspellings = new()
        {
            {"morning"   , ["mornig", "moring", "mornin", "mrning", "mournin", "mouring", "moarning", "moning", "mroning", "moening"] },
            {"afternoon" , ["afternon", "afternoom", "aternoon", "affternoon", "aftrnoon", "afternoonn", "fternoon", "afternoo", "aftanoon", "afteroon"] },
            {"evening"   , ["eveing", "evenin", "evning", "evaning", "eavning", "eavening", "evneing", "evenning", "eveining", "eving"] }
        };

        static Dictionary<string, List<string>> ClockSegmentMisspellings = new()
        {
            {"half"    , ["hlf", "halff", "hafl", "hal", "haof", "hlaf", "hallf", "haf", "hald", "haff"] },
            {"quarter" , ["qurter", "quater", "qaurter", "quartr", "quarterr", "quator", "qwarter", "quarfter", "qarter", "quorter"] }
        };

        static DateTime ParseOneWordNaturalDate(string namedDay)
        {
            namedDay = GetCorrectDaySpelling(namedDay);
            return DateTime.Today + TimeSpan.FromDays(GetDaysUntilNamedDay(false, namedDay));
        }

        static DateTime ParseTwoWordNaturalDate(string firstWord, string namedDay)
        {
            if (!IsValidPreposition(firstWord) && !IsValidWeekSpecifier(firstWord))
            {
                throw new ArgumentException("Invalid natural date format.");
            }

            namedDay = GetCorrectDaySpelling(namedDay);

            // If the first word was a preposition, it's dead weight to discard
            if (IsValidPreposition(firstWord))
            {
                return DateTime.Today + TimeSpan.FromDays(GetDaysUntilNamedDay(false, namedDay));
            }
            else
            {
                bool isNextWeekSpecified = firstWord == "next";
                return DateTime.Today + TimeSpan.FromDays(GetDaysUntilNamedDay(isNextWeekSpecified, namedDay));
            }
        }

        static DateTime ParseThreeWordNaturalDate(string preposition, string weekSpecifier, string namedDay)
        {
            if (!IsValidPreposition(preposition) || !IsValidWeekSpecifier(weekSpecifier))
            {
                throw new ArgumentException("Invalid natural date format.");
            }

            bool isNextWeekSpecified = weekSpecifier == "next";
            namedDay = GetCorrectDaySpelling(namedDay);
            return DateTime.Today + TimeSpan.FromDays(GetDaysUntilNamedDay(isNextWeekSpecified, namedDay));
        }

        static int GetDaysUntilNamedDay(bool isNextWeekSpecified, string dayName)
        {
            switch (dayName)      // Ooh you can switch on a string? Nice one, C#.
            {
                // Maybe silly, but we support things like "Next today" for those especially lazy users
                case "today":     return isNextWeekSpecified ? 7 : 0;
                case "tomorrow":  return isNextWeekSpecified ? 8 : 1;
                case "monday":    return GetDaysUntilIndexedDay(isNextWeekSpecified, 0);
                case "tuesday":   return GetDaysUntilIndexedDay(isNextWeekSpecified, 1);
                case "wednesday": return GetDaysUntilIndexedDay(isNextWeekSpecified, 2);
                case "thursday":  return GetDaysUntilIndexedDay(isNextWeekSpecified, 3);
                case "friday":    return GetDaysUntilIndexedDay(isNextWeekSpecified, 4);
                case "saturday":  return GetDaysUntilIndexedDay(isNextWeekSpecified, 5);
                case "sunday":    return GetDaysUntilIndexedDay(isNextWeekSpecified, 6);
                default:          throw new ArgumentException("Invalid day.");
            }
        }

        static int GetDaysUntilIndexedDay(bool isNextWeekSpecified, int targetDayIndex)
        {
            if (targetDayIndex < 0 || targetDayIndex >= 7)
            { 
                throw new ArgumentException("Invalid day index."); 
            }

            // DateTime's DayOfWeek says Sunday = 0.
            // We use Monday = 0 so indexes are bumped down for clarity.
            int todayIndex = (int)DateTime.Today.DayOfWeek - 1;  
            todayIndex = todayIndex < 0 ? 6 : todayIndex;

            /*-------------------------------------------------------------
               We don't need to ask which week we're targeting if the
               received day index is already before or on today.
               Eg., If today is Thursday and the day entered is Monday, it
                    will definitely be next week's Monday and we don't
                    care if the user wrote "this Monday" or "next Monday".
               Things like "Monday week" aren't supported (yet).
              ------------------------------------------------------------*/

            // If the user specifies "Saturday" when today is Saturday, they get a week from now
            if (targetDayIndex <= todayIndex)
            {
                return targetDayIndex - todayIndex + 7;
            }
            else
            {
                return targetDayIndex - todayIndex + (isNextWeekSpecified ? 7 : 0);
            }
        }

        static string GetCorrectDaySpelling(string candidate)
        {
            foreach (var targetWord in DayMisspellings.Keys)
            {
                if (candidate == targetWord || DayMisspellings[targetWord].Contains(candidate))
                {
                    return targetWord;
                }
            }

            throw new ArgumentException($"Invalid natural date spelling: {candidate}");
        }

        static bool IsValidPreposition(string candidate)
        {
            return DatePrepositions.Contains(candidate);
        }

        static bool IsValidWeekSpecifier(string candidate)
        {
            return candidate == "this" || candidate == "next";
        }
    }
}

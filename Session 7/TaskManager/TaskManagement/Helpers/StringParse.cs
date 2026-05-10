using System;
using System.Collections.Generic;
using System.Linq;
using TaskManagement.Models;



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
    enum TimeState
    {
        SeekingPreposition,
        SeekingFractionBeforeHour,
        SeekingFractionBeforeHourPostposition,
        SeekingHourNotMinutes,
        SeekingHourAndOptionalMinutes,
        SeekingAMPM,
        TimeComplete
    }

    public static class StringParse
    {
        #region Public functions

        static public DateTime ParseNaturalDate(string naturalDate)
        {
            // Clean up the user input
            List<string> dayWords = GetCleanWordsAndNumbers(naturalDate);       // ♪ Day Words... Fighter of the night cleanLoweredWords ♪

            // Parse based on word count
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
            // Clean up the user input
            List<string> timeWords = GetCleanWordsAndNumbers(naturalTime);

            /*-------------------------------------------------------------
              "at 3 15 o clock in the afternoon"

              A natural time could be too convoluted to handle by word
              count, so instead we're going to step through the string,
              handling a state. I'm sure there's a better way to do this.

              Not supported: "night", specific minutes "to" or "past",
              "twenty one", "twenty-one", minute words, "4pm" (no space).
             -------------------------------------------------------------*/

            TimeState timeState = TimeState.SeekingPreposition;
            int wordIndex = 0;
            int minutesToAdd = 0;
            int hour = 0;

            while (wordIndex < timeWords.Count)
            {
                string currentWord = timeWords[wordIndex];

                switch (timeState)
                {
                    case TimeState.TimeComplete:
                        // We already have a complete time, but there was more text? Abort.
                        throw new ArgumentException($"Invalid natural time format: Surplus from {currentWord}");

                    case TimeState.SeekingPreposition:
                        // at, by, etc.
                        if (TimePrepositions.Contains(currentWord))
                        {
                            wordIndex = 1;
                        }
                        timeState = TimeState.SeekingFractionBeforeHour;
                        break;

                    case TimeState.SeekingFractionBeforeHour:
                        // a quarter past
                        // quarter to
                        // half past
                        // a half past

                        // Drop the "a" and come back
                        if (currentWord == "a")
                        {
                            wordIndex++;
                            break;
                        }

                        if (CorrectSpellingFromDictionary(ref currentWord, ClockSegmentMisspellings))
                        {
                            if (timeWords.Count <= wordIndex + 1)  // OOB check
                            {
                                throw new ArgumentException($"Invalid natural time format: Unfinished input after { currentWord} ");
                            }

                            if (currentWord == "half")
                            {
                                if (timeWords[wordIndex + 1] == "to")
                                {
                                    minutesToAdd = -30;
                                }
                                else if (timeWords[wordIndex + 1] == "past")
                                {
                                    minutesToAdd = 30;
                                }
                                else
                                {
                                    throw new ArgumentException($"Invalid natural time format: {currentWord} {timeWords[wordIndex + 1]}");
                                }
                            }
                            else // currentWord == "quarter"
                            {
                                if (timeWords[wordIndex + 1] == "to")
                                {
                                    minutesToAdd = -15;
                                }
                                else if (timeWords[wordIndex + 1] == "past")
                                {
                                    minutesToAdd = 15;
                                }
                                else
                                {
                                    throw new ArgumentException($"Invalid natural time format: {currentWord} {timeWords[wordIndex + 1]}");
                                }
                            }

                            wordIndex += 2;
                            timeState = TimeState.SeekingHourNotMinutes;
                        }
                        else
                        {
                            timeState = TimeState.SeekingHourAndOptionalMinutes;
                        }
                        break;

                    case TimeState.SeekingHourNotMinutes:
                        if (CorrectSpellingFromDictionary(ref currentWord, MorningHourMisspellings)
                            || CorrectSpellingFromDictionary(ref currentWord, AfternoonHourMisspellings))
                        {
                            timeState = TimeState.TimeComplete;
                        }
                        else if (!CorrectSpellingFromDictionary(ref currentWord, HourMisspellings))
                        {
                            throw new ArgumentException($"Invalid natural time format: {currentWord}");
                        }
                        else
                        {
                            timeState = TimeState.SeekingAMPM;
                        }

                        hour = int.Parse(currentWord);
                        wordIndex++;
                        break;

                    case TimeState.SeekingHourAndOptionalMinutes:
                        if (CorrectSpellingFromDictionary(ref currentWord, MorningHourMisspellings)
                            || CorrectSpellingFromDictionary(ref currentWord, AfternoonHourMisspellings))
                        {
                            timeState = TimeState.TimeComplete;
                        }
                        else if (!CorrectSpellingFromDictionary(ref currentWord, HourMisspellings))
                        {
                            throw new ArgumentException($"Invalid natural time format: {currentWord}");
                        }
                        else
                        {
                            timeState = TimeState.SeekingAMPM;
                        }

                        hour = int.Parse(currentWord);

                        // We check minutes in the same pass due to the explicit "NotMinutes" state
                        if (timeWords.Count > wordIndex + 1)    // OOB check
                        {
                            int minutes = 0;
                            if (GetMinutesFromString(timeWords[wordIndex + 1], out minutes))
                            {
                                wordIndex++;
                            }

                            minutesToAdd = minutes;
                        }

                        wordIndex++;
                        break;

                    case TimeState.SeekingAMPM:

                        // Just skip useless cleanLoweredWords
                        if (currentWord == "in"
                            || currentWord == "at"
                            || currentWord == "the"
                            || currentWord == "oclock"
                            || currentWord == "o"
                            || currentWord == "clock"
                            )
                        {
                            wordIndex++;
                            break;
                        }

                        if (!CorrectSpellingFromDictionary(ref currentWord, SideOfNoonMisspellings))
                        {
                            throw new ArgumentException($"Invalid natural time format: {currentWord}");
                        }

                        if (currentWord == "pm" && hour < 12)
                        {
                            hour += 12;
                        }
                        else if (currentWord == "am" && hour == 12) // A natural 12 defaults to PM, so check it
                        {
                            hour = 0;
                        }

                        wordIndex++;
                        timeState = TimeState.TimeComplete;
                        break;
                }
            }

            // We've exhausted the input string. We have either a valid time,
            // a time needed an AM/PM, or an invalid time.
            if (timeState == TimeState.SeekingAMPM)
            {
                if (hour <= 4)
                {
                    hour += 12;
                }

                timeState = TimeState.TimeComplete;
            }
            else if (timeState != TimeState.TimeComplete)
            {
                throw new ArgumentException($"Invalid natural time format: Unfinished input");
            }

            // Deal with edge cases around 0:00
            if (hour == 0 && minutesToAdd < 0)
            {
                hour = 23;
                minutesToAdd += 60;
            }

            return TimeSpan.FromHours(hour) + TimeSpan.FromMinutes(minutesToAdd);
        }

        static public Task ParseNaturalTaskCreation(string naturalTaskCreation)
        {
            string trimmed = naturalTaskCreation.Trim();
            string cleaned = new string(trimmed.Select(c => char.IsPunctuation(c) ? ' ' : c).ToArray());
            string lowered = cleaned.ToLower();
            List<string> cleanLoweredWords = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
            
            // We keep a non-lowered version of the string to preserve the user's case
            List<string> cleanWords = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            bool isTimeFound = false;
            bool isDateFound = false;
            TimeSpan timeDue = TimeSpan.Zero;
            DateTime dateDue = DateTime.MinValue;

            if (GetNaturalTimeWithinString(cleanLoweredWords, out timeDue, out int timeWordsStart, out int timeWordsConsumed))
            {
                isTimeFound = true;
                cleanLoweredWords.RemoveRange(timeWordsStart, timeWordsConsumed);
                cleanWords.RemoveRange(timeWordsStart, timeWordsConsumed);
            }

            if (GetNaturalDateWithinString(cleanLoweredWords, out dateDue, out int dateWordsStart, out int dateWordsConsumed))
            {
                isDateFound = true;
                cleanLoweredWords.RemoveRange(dateWordsStart, dateWordsConsumed);
                cleanWords.RemoveRange(dateWordsStart, dateWordsConsumed);
            }

            string description = new(string.Join(" ", cleanWords));

            if (description.Length == 0)
            {
                throw new ArgumentException("No task description found.");
            }

            DateTime dueDateToSet = DateTime.Today;

            if (isDateFound)
            {
                dueDateToSet = dateDue;
            }

            if (isTimeFound)
            {
                dueDateToSet = dueDateToSet + timeDue;
            }

            Task newTask = new(description);
            if (isDateFound || isTimeFound)
            {
                newTask.DueDate = dueDateToSet;
            }

            return newTask;
        }

        #endregion


        #region Lists and Dictionaries

        // Prepositions are acceptable to encounter but are then discarded
        static List<string> DatePrepositions = ["at", "by", "for", "on", "in"];
        static List<string> TimePrepositions = ["at", "by", "for", "on", "before"];    

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

        static Dictionary<string, List<string>> SideOfNoonMisspellings = new()
        {
            {"am", ["morning", "mornig", "moring", "mornin", "mrning", "mournin", "mouring", "moarning", "moning", "mroning", "moening"] },
            {"pm", ["afternoon", "afternon", "afternoom", "aternoon", "affternoon", "aftrnoon", "afternoonn", "fternoon", "afternoo", "aftanoon", "afteroon",
                    "evening", "eveing", "evenin", "evning", "evaning", "eavning", "eavening", "evneing", "evenning", "eveining", "eving"] }
        };

        /// <summary>
        /// Misspellings that are clearly trying so specify an AM hour.
        /// </summary>
        static Dictionary<string, List<string>> MorningHourMisspellings = new()
        {
            {"00", ["00", "0", "zero"] },
            {"01", [] },  // We can't be sure a written morning hour is for the morning
            {"02", [] },
            {"03", [] },
            {"04", [] },
            {"05", [] },
            {"06", [] },
            {"07", [] },
            {"08", [] },
            {"09", [] }
        };

        /// <summary>
        /// Misspellings that are clearly trying to specify a PM hour.
        /// </summary>
        static Dictionary<string, List<string>> AfternoonHourMisspellings = new()
        {
            {"13", ["thirteen", "thiteen", "thriteen", "thirten", "thirten", "threeteen"] },
            {"14", ["fourteen", "forteen", "fourten", "fourtin", "fortin", "fourtin"] },
            {"15", ["fifteen", "fiveteen", "fiftheen", "fiftin", "fifteenn", "fifteem"] },
            {"16", ["sixteen", "sixten", "sixtin", "siksteen", "sikteen", "sixteem"] },
            {"17", ["seventeen", "seventen", "seventin", "seveteen", "seveenteen", "sevnteen"] },
            {"18", ["eighteen", "eighten", "eigtteen", "eighteen", "eightein", "eighteenn"] },
            {"19", ["nineteen", "nineten", "ninetin", "ninteen", "nintein", "ninteenn"] },
            {"20", ["twenty", "tweny", "twentie", "twente", "twentee", "twentey"] },
            {"21", [] },    // We don't look after users trying to write "twenty one" or above
            {"22", [] },
            {"23", [] }
        };

        /// <summary>
        /// Misspellings for hours that aren't clearly an AM or PM hour.
        /// </summary>
        static Dictionary<string, List<string>> HourMisspellings = new()
        {
            {"1" , ["one", "onr"] },
            {"2" , ["two", "twp"] },
            {"3" , ["three", "tree", "there", "thre", "thee", "theer"] },
            {"4" , ["four", "fuor", "fur", "foor", "fourr", "fore", "fure"] },
            {"5" , ["five", "fiv", "fife", "fuve", "fyve"] },
            {"6" , ["six", "sic"] },
            {"7" , ["seven", "sevn", "sven", "sevan", "seve"] },
            {"8" , ["eight", "eigt", "eit", "eght", "aight", "ate", "eite"] },
            {"9" , ["nine", "nien", "nin", "nein", "nighe"] },
            {"10", ["ten", "teb"] },
            {"11", ["eleven", "elevin", "elven", "elevn", "leven", "elevan"] },
            {"12", ["twelve", "twleve", "twelv", "twelf", "tweve", "twelfe"] }
        };

        /// <summary>
        /// Misspellings for half and quarter.
        /// </summary>
        static Dictionary<string, List<string>> ClockSegmentMisspellings = new()
        {
            {"half"    , ["hlf", "halff", "hafl", "hal", "haof", "hlaf", "hallf", "haf", "hald", "haff"] },
            {"quarter" , ["qurter", "quater", "qaurter", "quartr", "quarterr", "quator", "qwarter", "quarfter", "qarter", "quorter"] }
        };

        /// <summary>
        /// Misspellings for 15 and 30.
        /// </summary>
        static Dictionary<string, List<string>> MinutesMisspellings = new()
        {
            // We could put all 60 values here if we were particularly excited
            {"15", ["fifteen", "fiftene", "fiften", "fiveteen", "fiftheen", "fiftean", "fiftyn"] },
            {"30", ["thirty", "thirry", "thrity", "thirte", "thirtee", "thirtey", "thurty"] }
        };

        #endregion


        #region Helper functions

        static List<string> GetCleanWordsAndNumbers(string toClean)
        {
            // Replacing things with space helps with inputs like "7:30" or "on Monday,at 6" 
            string paddedToClean = new string(toClean.Select(c => char.IsPunctuation(c) ? ' ' : c).ToArray());
            string cleanSentence = new string(paddedToClean.Trim().ToLower().Where(c => char.IsLetterOrDigit(c) || c == ' ').ToArray());
            return cleanSentence.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        static bool CorrectSpellingFromDictionary(ref string candidate, Dictionary<string, List<string>> misspellings)
        {
            foreach (var targetWord in misspellings.Keys)
            {
                if (candidate == targetWord || misspellings[targetWord].Contains(candidate))
                {
                    candidate = targetWord;
                    return true;
                }
            }

            return false;
        }

        static DateTime ParseOneWordNaturalDate(string namedDay)
        {
            if (!CorrectSpellingFromDictionary(ref namedDay, DayMisspellings))
            {
                throw new ArgumentException($"Invalid spelling found: {namedDay}");
            }

            return DateTime.Today + TimeSpan.FromDays(GetDaysUntilNamedDay(false, namedDay));
        }

        static DateTime ParseTwoWordNaturalDate(string firstWord, string namedDay)
        {
            if (!IsValidPreposition(firstWord) && !IsValidWeekSpecifier(firstWord))
            {
                throw new ArgumentException("Invalid natural date format.");
            }

            if (!CorrectSpellingFromDictionary(ref namedDay, DayMisspellings))
            {
                throw new ArgumentException($"Invalid spelling found: {namedDay}");
            }

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

            if (!CorrectSpellingFromDictionary(ref namedDay, DayMisspellings))
            {
                throw new ArgumentException($"Invalid spelling found: {namedDay}");
            }

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

        static bool IsValidPreposition(string candidate)
        {
            return DatePrepositions.Contains(candidate);
        }

        static bool IsValidWeekSpecifier(string candidate)
        {
            return candidate == "this" || candidate == "next";
        }

        static bool GetMinutesFromString(string candidate, out int minutes)
        {
            if (int.TryParse(candidate, out minutes) && minutes >= 0 && minutes < 60)
            {
                return true;
            }
            else if (MinutesMisspellings["15"].Contains(candidate))
            {
                minutes = 15;
                return true;
            }
            else if (MinutesMisspellings["30"].Contains(candidate))
            {
                minutes = 30;
                return true;
            }

            return false;
        }

        static bool GetNaturalTimeWithinString(List<string> sentenceWords, out TimeSpan timeSpan, out int startIndex, out int numWords)
        {
            int totalWords = sentenceWords.Count;
            string candidate;
            startIndex = 0;
            numWords = 0;
            timeSpan = TimeSpan.Zero;

            for (startIndex = 0; startIndex < totalWords; ++startIndex)
            {
                int wordsUntilEnd = totalWords - startIndex;

                for (numWords = wordsUntilEnd; numWords >= 1; --numWords)
                {
                    candidate = string.Join(" ", sentenceWords.Skip(startIndex).Take(numWords));

                    // Not ideal for production: using exceptions for flow logic   :/
                    try
                    {
                        timeSpan = ParseNaturalTime(candidate);
                        return true;
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }
                }
            }

            return false;
        }

        static bool GetNaturalDateWithinString(List<string> sentenceWords, out DateTime dateTime, out int startIndex, out int numWords)
        {
            int totalWords = sentenceWords.Count;
            string candidate;
            startIndex = 0;
            numWords = 0;
            dateTime = DateTime.MinValue;

            for (startIndex = 0; startIndex < totalWords; ++startIndex)
            {
                int wordsUntilEnd = totalWords - startIndex;

                for (numWords = wordsUntilEnd; numWords >= 1; --numWords)
                {
                    candidate = string.Join(" ", sentenceWords.Skip(startIndex).Take(numWords));

                    try
                    {
                        dateTime = ParseNaturalDate(candidate);
                        return true;
                    }
                    catch (ArgumentException)
                    {
                        continue;
                    }
                }
            }

            return false;
        }

        #endregion
    }
}

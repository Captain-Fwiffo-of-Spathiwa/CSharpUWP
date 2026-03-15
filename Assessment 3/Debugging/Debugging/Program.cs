/*
    [✓] Create a project with the code (a console app will do).
    [✓] Document with comments exactly what the code is doing.
    [✓] Rename any badly named variables and methods.
    [✓] Comment every line of the ItemCollection unless it's blatantly obvious.
    
    To make sure you don’t accidentally break anything as you edit the code,
    you should display the results of the method to the console, save them
    somewhere and then check the results match when you’ve finished. 
*/


/* UPDATED CODE */
class DiceProbabilities
{
    /// <summary>
    /// Given a quantity of 6-sided dice, return a mapping of the probabilities
    /// of every possible sum of combined rolled die values.
    /// </summary>
    /// <param name="numDice">
    /// The number of dice whose rolled values to combine.
    /// </param>
    /// <return>
    /// A Dictionary with the probabilities of rolling each possible sum of the
    /// rolled dice.
    /// </return>
    public static Dictionary<int, Double> GetProbabilitiesDistribution(int numDice)
    {
        // Sure C#, you make async functions really, really easy.
        // But why'd you have to go ruin 'const'?
        int numFacesPerDie = 6;
        int minimumSum = numDice;
        int maximumSum = numDice * numFacesPerDie;

        // A map of every possible sum that can occur from the rolled dice, to
        // the number of different ways that sum can occur.
        // Eg. 5 = 1+4 or 2+3 or 3+2 or 4+1, giving 4 occurrences from 2 dice.
        // Therefore, for 2 dice, sumOccurrences[5] == 4
        Dictionary<int, int> sumOccurrences = new();

        // Create a new zeroed entry in the dictionary for every possible sum
        for (int sum = minimumSum; sum <= maximumSum; sum++)
        {
            sumOccurrences[sum] = 0;
        }

        // The collection of current face values is our current permutation
        int[] currentDicePermutation = new int[numDice];

        // Initialise all dice to their smallest value
        int minFaceValue = 1;
        for (int die = 0; die < numDice; die++)
        {
            currentDicePermutation[die] = minFaceValue;
        }

        // Sum every permutation of dice until there are none left to sum
        bool areAllPermutationsCounted = false;       
        while (!areAllPermutationsCounted)
        {
            int sumOfFaceValues = 0;

            // Read each face value to get the current permutation's sum
            foreach (int faceValue in currentDicePermutation)
            {
                sumOfFaceValues += faceValue;
            }

            // Increment the count of that sum in our dictionary
            sumOccurrences[sumOfFaceValues] += 1;

            // Adjust the set of dice to the next uncounted permutation
            int chosenDie = 0;
            bool isNewPermutationValid = false;

            // Tweak die values until we have a new valid permutation to count
            while (!isNewPermutationValid)
            {
                // Increment the chosen die to form a new permutation
                currentDicePermutation[chosenDie] += 1;

                // If the new value is still 6 or less, the new permutation is
                // valid to count, so we'll exit the inner loop.
                if (currentDicePermutation[chosenDie] <= 6)
                {
                    isNewPermutationValid = true;
                }
                // Else, if that was the last die, all permutations are counted
                else if (chosenDie == numDice - 1)
                {
                    areAllPermutationsCounted = true;
                    isNewPermutationValid = true;
                }
                // Else, wind the chosen die back to its minimum, and move to
                // the next die to try incrementing that one.
                else
                {
                    currentDicePermutation[chosenDie] = minFaceValue;
                    chosenDie++;
                }
            }
        }

        // A map of every possible sum that can occur the odds of it occurring
        Dictionary<int, Double> sumProbabilities = new();

        // The number of different permutations possible
        Double numPermutations = Math.Pow(6.0, (Double)numDice);
        
        // For every sum possible, calculate its probability
        for (int nextSum = minimumSum; nextSum <= maximumSum; nextSum++)
        {
            sumProbabilities[nextSum] = (Double)sumOccurrences[nextSum] / numPermutations;
        }
        
        return sumProbabilities;
    }



    /////////////////////////////////////////////
    /////////////////////////////////////////////
    //       Original un-commented code        //
    /////////////////////////////////////////////
    /////////////////////////////////////////////
    public static Dictionary<int, Double> calculateProbabilitiesForNumberOfDice(int n)
    {
        Dictionary<int, int> rc = new Dictionary<int, int>();
        int mn = n;
        int mx = n * 6;
        for (int sum = mn; sum <= mx; sum++)
        {
            rc[sum] = 0;
        }
        int[] d = new int[n];
        for (int sum = 0; sum < n; sum++)
        {
            d[sum] = 1;
        }
        bool finished1 = false;
        while (!finished1)
        {
            int total = 0;
            foreach (int faceValue in d)
            {
                total += faceValue;
            }
            rc[total] += 1;
            int sum = 0;
            bool finished2 = false;
            while (!finished2)
            {
                d[sum] += 1;
                if (d[sum] <= 6)
                {
                    finished2 = true;
                }
                else
                {
                    if (sum == n - 1)
                    {
                        finished1 = true;
                        finished2 = true;
                    }
                    else
                    {
                        d[sum] = 1;
                    }
                }
                sum++;
            }
        }
        Dictionary<int, Double> sumProbabilities = new Dictionary<int, double>();
        Double total2 = Math.Pow(6.0, (Double)n);
        for (int sum = mn; sum <= mx; sum++)
        {
            sumProbabilities[sum] = (Double)rc[sum] / total2;
        }
        return sumProbabilities;
    }
}




class TesterClass
{
    static void Main()
    {
        int maxDice = 5;

        Console.WriteLine($"Hello, World! Checking {maxDice} dice.\n");

        for (int numDice = 2; numDice < maxDice; ++numDice)
        {
            Console.WriteLine($"Rolling {numDice} dice:\n");

            var cleanProbabilities = DiceProbabilities.GetProbabilitiesDistribution(numDice);
            var dirtyProbabilities = DiceProbabilities.calculateProbabilitiesForNumberOfDice(numDice);

            foreach (var entry in cleanProbabilities)
            {
                if (!dirtyProbabilities.ContainsKey(entry.Key))
                {
                    Console.WriteLine("Oh dear. This shouldn't have happened!");
                }
            }

            if (dirtyProbabilities.Count != cleanProbabilities.Count)
            {
                Console.WriteLine("Oh dear. This also shouldn't happen!");
            }

            foreach (var entry in cleanProbabilities)
            {
                Console.WriteLine($"Clean code - Sum: {entry.Key}, Probability: {entry.Value}");
                Console.WriteLine($"Dirty code - Sum: {entry.Key}, Probability: {dirtyProbabilities[entry.Key]}");
            }

            Console.WriteLine("\n");
        }
    }
}

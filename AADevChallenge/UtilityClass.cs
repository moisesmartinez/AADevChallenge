using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AADevChallenge
{
    class UtilityClass
    {
        private const string vowels = "aeiouyAEIOUY";
        private static string[] englishWords = { "drool", "cats", "clean", "code", "dogs", "materials", "needed", "this", "is",
                                                "hard", "what", "are", "you", "smoking", "shot", "gun", "down", "river", "super",
                                                "man", "rule", "acklen", "developers", "are", "amazing", "home" };

        //Put the values in alphabetic order
        public static string[] AlphabeticSort(string[] values)
        {
            List<string> sortedList = values.ToList();
            sortedList.Sort();
            return sortedList.ToArray();
        }

        //Sort the values in reverse alphabetic order
        public static string[] ReverseAlphabeticSort(string[] values)
        {
            List<string> sortedList = values.ToList();
            sortedList.Sort();
            sortedList.Reverse();
            return sortedList.ToArray();
        }

        //At the beginning of each word, shift each vowel to the right by one letter (if at the end of the word, move it to the beginning)
        public static string[] ShiftVowels(string[] values)
        {
            int wordsCount = values.Length;
            for (int wordIndex = 0; wordIndex < wordsCount; wordIndex++)
            {
                //get one word from the values array
                string word = values[wordIndex];

                for (int i = 0; i < word.Length; i++)
                {
                    char currentCharacter = word[i];
                    if (vowels.Contains(currentCharacter))
                    {
                        //if the currentCharacter is a vowel and it's at the end of the string, then move the vowel to the beginning of the word..
                        if (i == word.Length - 1)
                        {
                            word = word.Insert(0, currentCharacter.ToString());
                            word = word.Remove(word.Length - 1, 1);
                        }
                        else//.. else, just shift the vowel one position to the right
                        {
                            word = word.Insert(i + 2, currentCharacter.ToString());
                            word = word.Remove(i, 1);
                            i++;
                        }
                    }
                }

                values[wordIndex] = word;
            }
            return values;
        }

        //Concatenate the words into one string, delimited by the ASCII integer value of the first character in the previous word
        //(for the first word, use the first character in the last word)
        public static string ConcatenateWords(string[] values)
        {
            string concatenatedWords = "";

            //Concatenate the first word, using the first character in the last word
            if (values.Length > 0)
            {
                int firstCharacterLastWord = values[values.Length - 1][0];
                concatenatedWords += values[0] + firstCharacterLastWord;
            }

            //Now concatenate the rest of the words, using the first character of the previous word
            for (int i = 1; i < values.Length; i++)
            {
                int firstCharacterPreviousWord = values[i - 1][0];
                concatenatedWords += values[i] + firstCharacterPreviousWord.ToString();
            }

            return concatenatedWords;
        }

        //Concatenate the words into one string, demilited by asterisks
        public static string ConcatenateWordsWithAsterik(string[] values)
        {
            string concatenatedWords = string.Join("*", values);
            return concatenatedWords;
        }

        //Base64 encode the value (UTF-8)
        public static string StringToBase64String(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        //For each value in the words array, if value contains more than one english word, split that value and add them to the array
        public static string[] SplitEnglishWords(string[] values)
        {
            int valuesCount = values.Length;
            List<string> splitWords = new List<string>();

            for (int valuesIndex = 0; valuesIndex < valuesCount; valuesIndex++)
            {
                string currentValue = values[valuesIndex];
                //Check if part(s) of the current values is an english word
                string possibleEnglishWord = "";
                for (int i = 0; i < currentValue.Length; i++)
                {
                    possibleEnglishWord += currentValue[i].ToString();
                    if (englishWords.Contains(possibleEnglishWord.ToLower()))
                    {
                        splitWords.Add(possibleEnglishWord);
                        possibleEnglishWord = "";
                    }
                }
                //if the variable possibleEnglishWord has any string, then that part of the string was not an english word. 
                //Hence, add the whole value to the returning arrary
                if (possibleEnglishWord != "")
                {
                    splitWords.Add(possibleEnglishWord);
                }
            }

            return splitWords.ToArray();
        }

        //For all consonants, alternate between uppercase and lowercase letters. 
        public static string[] AlternateConsonants(string[] values)
        {
            //starting with the case of the first letter given in the first word in the alphabetized list
            bool alternateToUpper = Char.IsUpper(values[0][0]);

            List<string> wordsWithAlternatedConsonants = new List<string>();
            int wordsCount = values.Length;
            for (int wordsIndex = 0; wordsIndex < wordsCount; wordsIndex++)
            {
                string word = values[wordsIndex];
                for (int i = 0; i < word.Length; i++)
                {
                    string currentCharacter = word[i].ToString();
                    if (!vowels.Contains(currentCharacter))
                    {
                        if (alternateToUpper)
                        {
                            currentCharacter = currentCharacter.ToUpper();
                        }
                        else
                        {
                            currentCharacter = currentCharacter.ToLower();
                        }
                        word = word.Remove(i, 1);
                        word = word.Insert(i, currentCharacter);
                        alternateToUpper = !alternateToUpper; //toggle the boolean value
                    }
                }
                wordsWithAlternatedConsonants.Add(word);
            }

            return wordsWithAlternatedConsonants.ToArray();
        }


        //Replace all vowels with Fibonacci numbers (in order) starting with given fibonacci number
        public static string[] ReplaceVowelsFibonacci(string[] values, double startingFibonacciNumber)
        {
            int wordsCount = values.Length;
            List<string> newWordsWithFibonacci = new List<string>();
            for (int wordsIndex = 0; wordsIndex < wordsCount; wordsIndex++ )
            {
                string word = values[wordsIndex];
                for (int i = 0; i < word.Length; i++)
                {
                    char currentCharacter = word[i];
                    if (vowels.Contains(currentCharacter))
                    {
                        word = word.Remove(i, 1);
                        //The first time it inserts the fibonacci number, it will be the starting number from the GET request
                        word = word.Insert(i, startingFibonacciNumber.ToString());
                        startingFibonacciNumber = GetNextFibonacciNumber(startingFibonacciNumber);
                    }
                }
                newWordsWithFibonacci.Add(word);
            }

            return newWordsWithFibonacci.ToArray();
        }

        //Get next fibonacci number from a starting fibonacci number (private function used by ReplaceVowelsFibonacci function)
        private static double GetNextFibonacciNumber(double startingFibonacciNumber)
        {
            if (startingFibonacciNumber == 0)
            {
                return 1;
            }
            double a = 0;
            double b = 1;
            double fibonacciNumber = a + b;
            //Calculate the fibonnaci number until it's greater than the starting fibonacci number
            while(fibonacciNumber <= startingFibonacciNumber)
            {
                a = b;
                b = fibonacciNumber;
                fibonacciNumber = a + b;
            }

            //fibonacciNumber contains the last calculated fibonacci number, b contains the previos fibonacci number.
            //Hence, if b is not the same as the startingFibonacciNumber, then the parameter is not a valid fibonacci number
            if (b != startingFibonacciNumber)
            {
                throw new Exception("Invalid fibonacci number");
            }
            return fibonacciNumber;
        }

    }
}

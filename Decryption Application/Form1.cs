using System.Text.RegularExpressions;
using System.Text;
namespace Decryption_Application
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Regex sWhitespace = new Regex(@"\s+");
        string ReplaceWhitespace(string input, string replacement)
        {
            return sWhitespace.Replace(input, replacement);
        }

        int[] createText(string text)
        {
            string noWhitespace = ReplaceWhitespace(text, "");
            StringBuilder sb = new StringBuilder();
            foreach (char c in noWhitespace)
            {
                if (!char.IsPunctuation(c))
                {
                    sb.Append(c);
                }
            }
            char[] tArray = new char[sb.Length];
            for (int i = 0; i < tArray.Length; i++)
            {
                tArray[i] = sb[i];
            }
            int[] nArray = new int[tArray.Length];
            for (int i = 0; i < tArray.Length; i++)
            {
                char c = tArray[i];
                int numberOfLetter = ((int)char.ToUpper(c)) - 65;
                nArray[i] = numberOfLetter;
            }
            int[] plainText = nArray;
            return plainText;
        }
        int[] generateCipherText(int[] plainText, int[] keyFinal)
        {
            int x = keyFinal.Length;
            int[] newArray = new int[x];
            for (int i = 0; i < x; i++)
            {
                int z = keyFinal[i] + plainText[i];
                newArray[i] = (z % 26);
            }
            return newArray;

        }
        int[] generatePlainText(int[] cipherText, int[] keyFinal)
        {
            int x = keyFinal.Length;
            int[] newArray = new int[x];
            for (int i = 0; i < x; i++)
            {
                int z = 26 + (cipherText[i] - keyFinal[i]);
                newArray[i] = (z % 26);
            }
            return newArray;
        }
        int[] generateKey(int[] plainText, int[] key)
        {
            int x = plainText.Length;
            int y = key.Length;
            int[] newArray = new int[x];
            for (int i = 0; i < x; i++)
            {
                newArray[i] = key[(i % y)];
            }
            return newArray;
        }
        string convertToString(int[] cipherText)
        {
            string[] Alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            string[] newArray = new string[cipherText.Length];

            for (int i = 0; i < cipherText.Length; i++)
            {
                int letterNumber = cipherText[i];
                newArray[i] = Alphabet[letterNumber];
            }
            string stringNewArray = string.Join("", newArray);
            return stringNewArray;

        }
        string Encipher(string text, string inputKey)
        {
            int[] plainText = createText(text);
            int[] key = createText(inputKey);
            int[] keyFinal = generateKey(plainText, key);
            int[] cipherText = generateCipherText(plainText, keyFinal);
            string output = convertToString(cipherText);
            return output;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox3.Text))
            {
                textBox2.Clear();
                Text = Encipher(textBox1.Text, textBox3.Text);

                textBox2.AppendText(Text);
            }
                
        }
        double[] englishProbDist = new double[26] { 0.08167, 0.01492, 0.02782, 0.04253, 0.12702, 0.02228, 0.02015, 0.06094, 0.06966, 0.00153, 0.00772, 0.04025, 0.02406, 0.06749, 0.07507, 0.01929, 0.00095, 0.05987, 0.06327, 0.09056, 0.02758, 0.00978, 0.02360, 0.00150, 0.01974, 0.00074 };

        int[] cipherTextShifted(int[] cipherText, int i)
        {
            int[] shiftedCipherText = new int[cipherText.Length];
            for (int run = 0; run < cipherText.Length; run++)
            {

                shiftedCipherText[run] = ((cipherText[(run + i) % cipherText.Length]));

            }
            return shiftedCipherText;
        }
        int[] generateIOC(int[] cipherText, int[] shiftedCipherText)
        {
            int m = cipherText.Length;
            int[] indicatorFunction = new int[1] { 0 };
            for (int i = 0; i < m; i++)
            {
                if (cipherText[i] == shiftedCipherText[i])
                {
                    indicatorFunction[0]++;
                }
                else
                {
                    continue;
                }

            }
            return indicatorFunction;
        }
        int[,] refineArray(int key, int[] firstArray)
        {
            int counter = 0;
            for (int i = 0; i < firstArray.Length; i++)
            {
                if ((i % key) == 0)
                {
                    counter++;
                }
            }
            int[,] finalArray = new int[counter, 2];

            int[] otherArray = new int[counter];
            for (int i = 0; i < firstArray.Length; i++)
            {
                if ((i % key) == 0)
                {
                    int index = Array.IndexOf(otherArray, 0);
                    otherArray[index] = firstArray[i];
                    finalArray[index, 0] = i;
                    finalArray[index, 1] = firstArray[i];
                }
            }
            return finalArray;

        }
        double nCr(int n, int r)
        {
            if (r == 0)
            {
                return 1;
            }
            else
            {
                double[] final = new double[r];
                for (int i = 0; i < r; i++)
                {
                    double x = n - i;
                    double y = i + 1;
                    double d = x / y;
                    final[i] = d;
                }
                double prod = 1;
                foreach (double i in final)
                {
                    prod *= i;
                }
                return prod;
            }
        }
        double power(int n, double p)
        {
            double k = Convert.ToDouble(n);

            double result = Math.Pow(p, k);
            return result;
        }
        double calculateCDF(double p, int characterset, int coincidences)
        {
            double[] PMF = new double[coincidences + 1];

            for (int i = 0; i < (coincidences + 1); i++)
            {
                double nChooseR = nCr(characterset, i);
                double result = nChooseR * power(i, p) * power(characterset - i, (1 - p));
                PMF[i] = result;

                continue;
            }
            double sum = 1 - PMF.Sum();
            return sum;
        }
        double averageValues(double[] probabilities)
        {
            double prod = 0;
            foreach (double value in probabilities)
            {
                prod += value;
            }
            double finalValue = prod / probabilities.Length;
            return finalValue;
        }
        double generateProbability(int n, int key, int[] IOCValues)
        {
            int[,] refinedIOC = refineArray(key, IOCValues);
            double[] probabilities = new double[refinedIOC.GetLength(0)];

            for (int i = 0; i < refinedIOC.GetLength(0); i++)
            {
                int coincidences = refinedIOC[i, 1];
                double probabilityCorrect = calculateCDF(0.0661, n, coincidences);
                double probabilityIncorrect = calculateCDF(0.0384615, n, coincidences);
                int index = Array.IndexOf(probabilities, 0);
                probabilities[index] = 1 - (probabilityIncorrect / probabilityCorrect);
            }
            double averageProbability = averageValues(probabilities);
            return averageProbability;

        }
        int[] splitCipher(int[] cipherText, int keySegment, int keyLength)
        {
            int counter = 0;
            for (int i = 0; i < cipherText.Length; i++)
            {
                if ((i + 1) % ((keyLength * counter) + keySegment) == 0)
                {
                    counter++;
                }
            }
            int[] monoalphabeticCipher = new int[counter + 1];
            int[] other = new int[counter + 1];
            int newCounter = 0;
            for (int i = 0; i < cipherText.Length; i++)
            {

                if (i % ((keyLength * newCounter) + keySegment) == 0)
                {
                    int index = Array.IndexOf(other, 0);
                    monoalphabeticCipher[index] = cipherText[i];
                    other[index] = 1;
                    newCounter++;
                }
            }
            return monoalphabeticCipher;
        }
        int[] findFrequencies(int[] cipherText)
        {
            int[] frequencies = new int[26];
            int counter = 0;
            for (int i = 0; i < 26; i++)
            {
                foreach (int j in cipherText)
                {
                    if (j == i)
                    {
                        counter++;
                    }
                }
                frequencies[i] = counter;
                counter = 0;
            }
            return frequencies;
        }
        double findChi(int[] charFrequencies, int j, int textLength)
        {
            double[] newEnglishProbDist = new double[26];
            for (int i = 0; i < 26; i++)
            {
                newEnglishProbDist[(i + j) % 26] = englishProbDist[i];
            }

            double[] otherArray = new double[26];
            for (int i = 0; i < 26; i++)
            {
                double value = Math.Pow((charFrequencies[i] * textLength) - (newEnglishProbDist[i] * textLength), 2) / (newEnglishProbDist[i] * textLength);
                otherArray[i] = value;
            }
            double sum = otherArray.Sum();
            return sum;

        }
        int[] findKey(int[] cipherText, int keyLength, int textLength)
        {
            double[] chiArray = new double[26];
            int[] key = new int[keyLength];
            for (int i = 1; i < (keyLength + 1); i++)
            {
                int[] monoAlphabeticSub = splitCipher(cipherText, i, keyLength);
                int[] charFrequencies = findFrequencies(monoAlphabeticSub);
                for (int j = 0; j < 26; j++)
                {
                    double chi = findChi(charFrequencies, j, textLength);
                    chiArray[j] = chi;
                }
                double lowestChi = chiArray.Min();
                int index = Array.IndexOf(chiArray, lowestChi);
                key[i - 1] = index;
            }
            int[] newKey = new int[keyLength];
            for (int i = 0; i < keyLength; i++)
            {
                newKey[(i + 1) % keyLength] = key[i];
            }
            string keyString = convertToString(newKey);
            Console.WriteLine(keyString);
            return newKey;
        }
        string decrypt(string firstCipherText, string firstKey)
        {
            int[] key = createText(firstKey);
            
            int[] cipherText = createText(firstCipherText);
            int[] finalKey = generateKey(cipherText, key);
            int[] plainText = generatePlainText(cipherText, finalKey);
            string output = convertToString(plainText);
            return output;
        }
        string Decipher(string firstCipherText)
        {
            
            int[] cipherText = createText(firstCipherText);
            int[] IOCValues = new int[51];
            for (int i = 0; i < 51; i++)
            {
                IOCValues[i] = i;
            }

            foreach (int i in IOCValues)
            {
                int[] shiftedCipherText = cipherTextShifted(cipherText, i);
                int[] indexOfCoincidence = generateIOC(cipherText, shiftedCipherText);
                IOCValues[i] = indexOfCoincidence[0];
            }
            int n = cipherText.Length;
            double[] probabilities = new double[21];

            for (int i = 1; i < 21; i++)
            {
                probabilities[i] = generateProbability(n, i, IOCValues);
            }

            double highestProbability = probabilities.Max();
            int keyLength = Array.IndexOf(probabilities, highestProbability);
            int[] key = findKey(cipherText, keyLength, n);
            
            string keyFinal = convertToString(key);
            return keyFinal;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                textBox3.Clear();
                textBox2.Clear();
                Text = Decipher(textBox1.Text);
                textBox3.AppendText(Text);
                Text = decrypt(textBox1.Text, Text);
                textBox2.AppendText(Text);
 
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

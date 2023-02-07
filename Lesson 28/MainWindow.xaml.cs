using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lesson_28
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void inPlain_TextChanged(object sender, TextChangedEventArgs e)
        {
            outErrors.Text = "";
            outRoundOne.Text = "";
            outShiftRows.Text = "";
            outSubBytes.Text = "";
            outMixColumns.Text = "";

            String plainTxt = inPlain.Text;

            //Restricting character input to hex characters
            foreach (char c in plainTxt)
            {
                if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f') || c == ' ') && plainTxt != String.Empty)
                {
                    inPlain.Text = plainTxt.Remove(plainTxt.Length - 1, 1);
                    inPlain.SelectionStart = inPlain.Text.Length;
                }
            }
        }

        private void inKey_TextChanged(object sender, TextChangedEventArgs e)
        {
            outErrors.Text = "";
            outRoundOne.Text = "";
            outShiftRows.Text = "";
            outSubBytes.Text = "";
            outMixColumns.Text = "";

            String keyTxt = inKey.Text;

            //Restricting character input to hex characters
            foreach (char c in keyTxt)
            {
                if (!((c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f') || c == ' ') && keyTxt != String.Empty)
                {
                    inKey.Text = keyTxt.Remove(keyTxt.Length - 1, 1);
                    inKey.SelectionStart = inKey.Text.Length;
                }
            }
        }

        private void calculate_Click(object sender, RoutedEventArgs e)
        {
            outErrors.Text = "";
            outRoundOne.Text = "";
            outShiftRows.Text = "";
            outSubBytes.Text = "";
            outMixColumns.Text = "";

            /**************
            TEXT FORMATTING
            **************/
            String plainTxt = inPlain.Text.ToLower();
            String keyTxt = inKey.Text.ToLower();
            plainTxt = Regex.Replace(plainTxt, "[^0-9a-f]", "");
            keyTxt = Regex.Replace(keyTxt, "[^0-9a-f]", "");

            /*************
            ERROR CHECKING
            *************/
            if (plainTxt.Length < 32 || plainTxt.Length > 32)
            {
                outErrors.Text = "Please enter a 128 bit (32 characters) hex string for the plaintext!!";
                return;
            }
            else if (keyTxt.Length < 32 || plainTxt.Length > 32)
            {
                outErrors.Text = "Please enter a 128 bit (32 characters) hex string for the key!!";
                return;
            }

            /******************************
            SPLITTING HEXES INTO 4X4 ARRAYS
            ******************************/
            String[,] splitPlain = new string[4, 4];
            String[,] splitKey = new string[4, 4];
            int counter = 0;
            for (int c = 0; c < 4; c++)
            {
                for (int r = 0; r < 4; r++)
                {
                    splitPlain[r, c] = plainTxt.Substring(counter, 2);
                    splitKey[r, c] = keyTxt.Substring(counter, 2);
                    counter += 2;
                }
            }

            /*****************
            START OF ROUND ONE
            *****************/
            String[,] roundOne = new string[4, 4];

            //XOR-ing the plaintext hexes with the key hexes
            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    roundOne[r, c] = (Convert.ToInt32(splitPlain[r, c], 16) ^ Convert.ToInt32(splitKey[r, c], 16)).ToString("X2");
                }
            }

            //Converting the array into a string for output
            String roundOneStr = "";
            for (int r = 0; r < 4; r++)
            {
                roundOneStr += "\n";
                for (int c = 0; c < 4; c++)
                {
                    roundOneStr += roundOne[r, c] + " ";
                }
            }
            outRoundOne.Text = roundOneStr;

            /********
            SUB BYTES
            ********/
            //sBoxes, taken from previous code
            String[,] sBoxes =
            {
                {"63" , "7c" , "77" , "7b" , "f2" , "6b" , "6f" , "c5" , "30" , "01" , "67" , "2b" , "fe" , "d7" , "ab" , "76"},
                {"ca" , "82" , "c9" , "7d" , "fa" , "59" , "47" , "f0" , "ad" , "d4" , "a2" , "af" , "9c" , "a4" , "72" , "c0"},
                {"b7" , "fd" , "93" , "26" , "36" , "3f" , "f7" , "cc" , "34" , "a5" , "e5" , "f1" , "71" , "d8" , "31" , "15"},
                {"04" , "c7" , "23" , "c3" , "18" , "96" , "05" , "9a" , "07" , "12" , "80" , "e2" , "eb" , "27" , "b2" , "75"},
                {"09" , "83" , "2c" , "1a" , "1b" , "6e" , "5a" , "a0" , "52" , "3b" , "d6" , "b3" , "29" , "e3" , "2f" , "84"},
                {"53" , "d1" , "00" , "ed" , "20" , "fc" , "b1" , "5b" , "6a" , "cb" , "be" , "39" , "4a" , "4c" , "58" , "cf"},
                {"d0" , "ef" , "aa" , "fb" , "43" , "4d" , "33" , "85" , "45" , "f9" , "02" , "7f" , "50" , "3c" , "9f" , "a8"},
                {"51" , "a3" , "40" , "8f" , "92" , "9d" , "38" , "f5" , "bc" , "b6" , "da" , "21" , "10" , "ff" , "f3" , "d2"},
                {"cd" , "0c" , "13" , "ec" , "5f" , "97" , "44" , "17" , "c4" , "a7" , "7e" , "3d" , "64" , "5d" , "19" , "73"},
                {"60" , "81" , "4f" , "dc" , "22" , "2a" , "90" , "88" , "46" , "ee" , "b8" , "14" , "de" , "5e" , "0b" , "db"},
                {"e0" , "32" , "3a" , "0a" , "49" , "06" , "24" , "5c" , "c2" , "d3" , "ac" , "62" , "91" , "95" , "e4" , "79"},
                {"e7" , "c8" , "37" , "6d" , "8d" , "d5" , "4e" , "a9" , "6c" , "56" , "f4" , "ea" , "65" , "7a" , "ae" , "08"},
                {"ba" , "78" , "25" , "2e" , "1c" , "a6" , "b4" , "c6" , "e8" , "dd" , "74" , "1f" , "4b" , "bd" , "8b" , "8a"},
                {"70" , "3e" , "b5" , "66" , "48" , "03" , "f6" , "0e" , "61" , "35" , "57" , "b9" , "86" , "c1" , "1d" , "9e"},
                {"e1" , "f8" , "98" , "11" , "69" , "d9" , "8e" , "94" , "9b" , "1e" , "87" , "e9" , "ce" , "55" , "28" , "df"},
                {"8c" , "a1" , "89" , "0d" , "bf" , "e6" , "42" , "68" , "41" , "99" , "2d" , "0f" , "b0" , "54" , "bb" , "16"}
            };

            //Code repurposed from Lesson 27, the pair from round one is subbed in with a pair given by the sBox, inserted into subBytes[,]
            String subByteStr = ""; //String for output to GUI
            String[,] subBytes = new string[4, 4]; //Array of subbed bytes

            for (int r = 0; r < 4; r++)
            {
                subByteStr += "\n";
                for (int c = 0; c < 4; c++)
                {
                    String pair = roundOne[r, c];
                    int row = Convert.ToInt32(pair[0].ToString(), 16);
                    int col = Convert.ToInt32(pair[1].ToString(), 16);

                    subBytes[r, c] += sBoxes[row, col];
                    subByteStr += sBoxes[row, col] + " ";
                }
            }
            outSubBytes.Text = subByteStr.ToUpper();

            /*********
            SHIFT ROWS
            *********/
            String[,] shiftRows = new string[4, 4];

            //Splitting off each row from subBytes[,], a 2D array, into four separate 1D arrays
            String[] subRow0 = Enumerable.Range(0, subBytes.GetUpperBound(1) + 1).Select(i => subBytes[0, i]).ToArray();
            String[] subRow1 = Enumerable.Range(0, subBytes.GetUpperBound(1) + 1).Select(i => subBytes[1, i]).ToArray();
            String[] subRow2 = Enumerable.Range(0, subBytes.GetUpperBound(1) + 1).Select(i => subBytes[2, i]).ToArray();
            String[] subRow3 = Enumerable.Range(0, subBytes.GetUpperBound(1) + 1).Select(i => subBytes[3, i]).ToArray();

            subRow1 = ShiftRows(subRow1); //Using ShiftRows() once, moving the first term to the end
            subRow2 = ShiftRows(ShiftRows(subRow2)); //Using ShiftRows() twice, moving the first two terms to the end
            subRow3 = ShiftRows(ShiftRows(ShiftRows(subRow3))); //Using ShiftRows() thrice, moving the first three terms to the end

            //Inserting the shifted rows into a new 2D array
            for (int i = 0; i < 4; i++)
            {
                shiftRows[0, i] = subRow0[i];
                shiftRows[1, i] = subRow1[i];
                shiftRows[2, i] = subRow2[i];
                shiftRows[3, i] = subRow3[i];
            }

            //Converting the array into a string for output
            String shiftRowStr = "";
            for (int r = 0; r < 4; r++)
            {
                shiftRowStr += "\n";
                for (int c = 0; c < 4; c++)
                {
                    shiftRowStr += shiftRows[r, c] + " ";
                }
            }
            outShiftRows.Text = shiftRowStr.ToUpper();

            /************
            MIXED COLUMNS
            ************/
            String[,] temp = new string[4, 3]; //Temp array; first column is the original from shiftRows, second is the bit shift, and third is the two XOR-ed
            String[,] mixedCols = new string[4, 4]; //Final mixed column array

            //Nested for loop traversing by columns
            for (int c = 0; c < 4; c++)
            {
                for (int r = 0; r < 4; r++)
                {
                    //Code repurposed from Lesson 26, using the bit shifting methods for the column
                    temp[r, 0] = shiftRows[r, c];
                    for (int i = 1; i < 2; i++)
                    {
                        int tempInt;
                        int tempXor;
                        String tempString = "";

                        //Checking if the first bit is 0
                        if (Convert.ToInt32(shiftRows[r, c], 16) < 128)
                        {
                            //Bitshift
                            tempInt = Convert.ToInt32(shiftRows[r, c], 16) << 1;
                            tempString = tempInt.ToString("X2");
                        }
                        //Else, the bit is 1
                        else
                        {
                            //XOR-ing and bitshift
                            tempInt = Convert.ToInt32(shiftRows[r, c], 16) << 1;
                            tempXor = tempInt ^ Convert.ToInt32("11B", 16);
                            tempString = tempXor.ToString("X2");
                        }

                        //Adding the shift/xor to the array
                        temp[r, i] = tempString;
                    }
                }
                //XOR-ing the original column with the bitshifted one, inserting the result into the third column 
                temp[0, 2] = (Convert.ToInt32(temp[0, 0], 16) ^ Convert.ToInt32(temp[0, 1], 16)).ToString("X2");
                temp[1, 2] = (Convert.ToInt32(temp[1, 0], 16) ^ Convert.ToInt32(temp[1, 1], 16)).ToString("X2");
                temp[2, 2] = (Convert.ToInt32(temp[2, 0], 16) ^ Convert.ToInt32(temp[2, 1], 16)).ToString("X2");
                temp[3, 2] = (Convert.ToInt32(temp[3, 0], 16) ^ Convert.ToInt32(temp[3, 1], 16)).ToString("X2");

                //Multiplication and XOR-ing; 0 = {01}, 1 = {02}, 2 = {03}
                mixedCols[0, c] = (Convert.ToInt32(temp[0, 1], 16) ^ Convert.ToInt32(temp[1, 2], 16) ^ Convert.ToInt32(temp[2, 0], 16) ^ Convert.ToInt32(temp[3, 0], 16)).ToString("X2"); // ({02} * column 0) ^ ({03} * column 1) ^ (column 2) ^ (column 3)
                mixedCols[1, c] = (Convert.ToInt32(temp[0, 0], 16) ^ Convert.ToInt32(temp[1, 1], 16) ^ Convert.ToInt32(temp[2, 2], 16) ^ Convert.ToInt32(temp[3, 0], 16)).ToString("X2"); // (column 0) ^ ({02} * column 1) ^ ({03} * column 2) ^ (column 3)
                mixedCols[2, c] = (Convert.ToInt32(temp[0, 0], 16) ^ Convert.ToInt32(temp[1, 0], 16) ^ Convert.ToInt32(temp[2, 1], 16) ^ Convert.ToInt32(temp[3, 2], 16)).ToString("X2"); // (column 0) ^ (column 1) ^ ({02} * column 2) ^ ({03} * column 3)
                mixedCols[3, c] = (Convert.ToInt32(temp[0, 2], 16) ^ Convert.ToInt32(temp[1, 0], 16) ^ Convert.ToInt32(temp[2, 0], 16) ^ Convert.ToInt32(temp[3, 1], 16)).ToString("X2"); // ({03} * column 0) ^ (column 1) ^ (column 2) ^ ({02} * column 3)
            }

            //Converting the array into a string for output
            String mixColStr = "";
            for (int r = 0; r < 4; r++)
            {
                mixColStr += "\n";
                for (int c = 0; c < 4; c++)
                {
                    mixColStr += mixedCols[r, c] + " ";
                }
            }
            outMixColumns.Text = mixColStr.ToUpper();

        }

        String[] ShiftRows (String[] row)
        {
            //Shifts the first term of the array to the end of the array
            String temp = "";
            String[] shiftRows = new string[4];
            temp = row[0];
            shiftRows[0] = row[1];
            shiftRows[1] = row[2];
            shiftRows[2] = row[3];
            shiftRows[3] = temp;

            return shiftRows;
        }
    }
}
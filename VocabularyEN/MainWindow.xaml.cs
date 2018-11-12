using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.IO;
using System.Threading;

namespace VocabularyEN
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] words;
        int choice_language;
        char[] pattern;
        int amount=0;

        public MainWindow()
        {
            InitializeComponent();

            if (!File.Exists("Słowa.txt")) File.Create("Słowa.txt");
        }

        private void new_words_b_Click(object sender, RoutedEventArgs e)
        {
            if (new_english.Text != "" && new_polish.Text != "")
            {
                using (StreamWriter sw = File.AppendText("Słowa.txt"))
                {
                    sw.WriteLine((new_english.Text + "," + new_polish.Text).Replace(", ",","));
                    new_english.Text = ""; new_polish.Text = "";
                    warning1.Visibility = Visibility.Hidden;
                    sw.Close();
                }
            }

            else
            {
                warning1.Visibility = Visibility.Visible;
                warning1.Content = "Wprowadź słowa";
            }
        }

        private void translate_tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            warning2.Content = "Tłumaczenie niepoprawne";
            warning2.Foreground = new SolidColorBrush(Colors.Red);

            if (choice_language == 1)
            {
                for (int i = 1; i < words.Length; i++)
                {
                    if (translate_tb.Text == words[i])
                    {
                        warning2.Content = "Tłumaczenie poprawne!";
                        warning2.Foreground = new SolidColorBrush(Colors.Green);
                    }
                }
            }

            else
            {
                if (translate_tb.Text == words[0])
                {
                    warning2.Content = "Tłumaczenie poprawne!";
                    warning2.Foreground = new SolidColorBrush(Colors.Green);
                }
            }

            warning2.Visibility = Visibility.Visible;
        }

        private void random_b_Click(object sender, RoutedEventArgs e)
        {
            if (File.ReadAllLines("Słowa.txt").Count() == 0)
            {
                warning1.Content = "Brak dodanych słów";
                warning1.Visibility = Visibility.Visible;
                return;
            }

            else warning1.Visibility = Visibility.Hidden;

            warning2.Visibility = Visibility.Hidden;

            string help;
            Random r = new Random();
            int random_number = r.Next(0, File.ReadLines("Słowa.txt").Count());
            choice_language = r.Next(1, 3);

            string line = File.ReadLines("Słowa.txt").Skip(random_number).Take(1).First();

            words = line.Split(',');

            random_info2_l.Visibility = Visibility.Visible;

            if (choice_language == 1)
            {
                random_info2_l.Content = words[0];
                help = words[words.Length - r.Next(0, words.Length - 1) - 1];
            }

            else
            {
                random_info2_l.Content = words[words.Length - r.Next(0, words.Length - 1) - 1];
                help = words[0];
            }

            pattern = new char[help.Length];
            for (int i = 0; i < help.Length; i++)
                pattern[i] = '*';

            help_l.Visibility = Visibility.Hidden;
            help_b.Visibility = Visibility.Visible;
            translate_tb.Text = "";
            amount = 0;
        }

        private void help_b_Click(object sender, RoutedEventArgs e)
        {
            int i;
            if (choice_language == 1) i = 1; else i = 0;

            Random r = new Random();
            int pattern_index = r.Next(0, pattern.Length);

            while(pattern[pattern_index]!='*')
            {
                pattern_index = r.Next(0, pattern.Length);
            }

            pattern[pattern_index] = words[i][pattern_index];
            char[] pattern2 = new char[pattern.Length * 2];
            string pattern3 = "";

            int k = 0;
            for(int j=0; j <= pattern2.Length-2; j = j+2)
            {
                pattern2[j] = pattern[k++];
                pattern2[j + 1] = ' ';
            }

            for (int j = 0; j < pattern2.Length; j++)
                pattern3 += pattern2[j];

            help_l.Content = pattern3;
            help_l.Visibility = Visibility.Visible;

            amount++;
            if (amount == pattern.Length) help_b.Visibility = Visibility.Hidden;
        }
    }
}

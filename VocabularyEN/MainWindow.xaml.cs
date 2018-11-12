using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

using Controls = System.Windows.Controls;

namespace VocabularyEN
{
    public partial class MainWindow : Window
    {
        private delegate void Warning_Info(Controls.Label label, string message, SolidColorBrush color);

        /* In each line of file "Słowa.txt" is one word in english and one or more translations for it.
        1 stands for first word (eng), 2 stands for translation(s) (pl). */
        int choice_of_language;

        char[] pattern;
        string[] words;
        int help_size;
        Warning_Info warning_info;

        public MainWindow()
        {
            InitializeComponent();

            if (!File.Exists("Słowa.txt")) File.Create("Słowa.txt");

            warning_info = (Controls.Label label, string message, SolidColorBrush color) =>
            {
                label.Content = message;
                label.Foreground = color;
            };
        }

        private void button_add_words_Click(object sender, RoutedEventArgs e)
        {
            if (textbox_new_english.Text != "" && textbox_new_polish.Text != "")
            {
                using (StreamWriter sw = File.AppendText("Słowa.txt"))
                {
                    sw.WriteLine((textbox_new_english.Text + "," + textbox_new_polish.Text).Replace(", ",","));
                    textbox_new_english.Text = ""; textbox_new_polish.Text = "";
                    label_warning_add.Visibility = Visibility.Hidden;
                    sw.Close();
                }
            }

            else
            {
                label_warning_add.Visibility = Visibility.Visible;
                warning_info(label_warning_add, "Wprowadź słowa", new SolidColorBrush(Colors.Red));
            }
        }

        private void textbox_translate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            warning_info(label_warning_translation, "Tłumaczenie niepoprawne", new SolidColorBrush(Colors.Red));

            if (choice_of_language == 1)
            {
                for (int i = 1; i < words.Length; i++)
                    if (textbox_translate.Text == words[i])
                        warning_info(label_warning_translation, "Tłumaczenie poprawne!", new SolidColorBrush(Colors.Green));
            }   

            else
            {
                if (textbox_translate.Text == words[0]) warning_info(label_warning_translation, "Tłumaczenie poprawne!", new SolidColorBrush(Colors.Green));
            }

            label_warning_translation.Visibility = Visibility.Visible;
        }

        private void button_random_Click(object sender, RoutedEventArgs e)
        {
            if (File.ReadAllLines("Słowa.txt").Count() == 0)
            {
                warning_info(label_warning_add, "Brak dodanych słów", new SolidColorBrush(Colors.Red));
                label_warning_add.Visibility = Visibility.Visible;
                return;
            }

            else label_warning_add.Visibility = Visibility.Hidden;

            label_warning_translation.Visibility = Visibility.Hidden;

            string help;
            Random r = new Random();
            int random_word = r.Next(0, File.ReadLines("Słowa.txt").Count());
            choice_of_language = r.Next(1, 3); //If 1 then user try to translate english word into polish, if 2 then vice versa.

            string line = File.ReadLines("Słowa.txt").Skip(random_word).Take(1).First();

            words = line.Split(',');

            label_random_info2.Visibility = Visibility.Visible;

            if (choice_of_language == 1)
            {
                label_random_info2.Content = words[0];
                help = words[words.Length - r.Next(0, words.Length - 1) - 1];
            }

            else
            {
                label_random_info2.Content = words[words.Length - r.Next(0, words.Length - 1) - 1];
                help = words[0];
            }

            pattern = new char[help.Length];
            for (int i = 0; i < help.Length; i++)
                pattern[i] = '*';

            label_help.Visibility = Visibility.Hidden;
            button_help.Visibility = Visibility.Visible;
            textbox_translate.Text = "";
            help_size = 0;
        }

        private void help_b_Click(object sender, RoutedEventArgs e)
        {
            int i;
            if (choice_of_language == 1) i = 1; else i = 0;

            Random r = new Random();
            int pattern_index = r.Next(0, pattern.Length);

            while(pattern[pattern_index]!='*')
                pattern_index = r.Next(0, pattern.Length);

            pattern[pattern_index] = words[i][pattern_index];

            label_help.Content = string.Join(" ", pattern);
            label_help.Visibility = Visibility.Visible;

            help_size++;
            if (help_size == pattern.Length) button_help.Visibility = Visibility.Hidden;
        }
    }
}

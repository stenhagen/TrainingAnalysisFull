using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrainingAnalysis.DataStorage;

namespace TrainingAnalysisGUI
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private const string USERNAME = "Username";
        private const string PASSWORD = "Password";
        private const double TEXTBOX_OPACITY_DEFAULT = 0.4;
        private const double TEXTBOX_OPACITY_CONTENT = 1;

        public Register()
        {
            InitializeComponent();
            NotUnique.Visibility = Visibility.Hidden;
        }

        private void SubmitRegister_Click(object sender, RoutedEventArgs e)
        {
            //bool success = User.Insert(Username.Text, Password.Text);
            //NotUnique.Visibility = success ? Visibility.Hidden : Visibility.Visible;
        }

        private void Username_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Username.Text == USERNAME)
            {
                Username.Text = "";
                Username.Foreground.Opacity = TEXTBOX_OPACITY_CONTENT;
            }
        }

        private void Username_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(Username.Text == "")
            {
                Username.Text = USERNAME;
                Username.Foreground.Opacity = TEXTBOX_OPACITY_DEFAULT;
            }
        }

        private void Password_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Password.Text == PASSWORD)
            {
                Password.Text = "";
                Password.Foreground.Opacity = TEXTBOX_OPACITY_CONTENT;
            }
        }

        private void Password_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Password.Text == "")
            {
                Password.Text = PASSWORD;
                Password.Foreground.Opacity = TEXTBOX_OPACITY_DEFAULT;
            }
        }
    }
}

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
        public Register()
        {
            InitializeComponent();
            NotUnique.Visibility = Visibility.Hidden;
        }

        private void SubmitRegister_Click(object sender, RoutedEventArgs e)
        {
            bool success = User.Insert(Username.Text, Password.Text);
            NotUnique.Visibility = success ? Visibility.Hidden : Visibility.Visible;
        }
    }
}

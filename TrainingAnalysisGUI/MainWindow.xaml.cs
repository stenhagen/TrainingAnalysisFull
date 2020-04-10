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
using TrainingAnalysis;

namespace TrainingAnalysisGUI
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

        private void SessionUpload_Click(object sender, RoutedEventArgs e)
        {
            FileReader f = new FileReader();
            string path = @sessionPath.Text;
            string sessionFile = f.readFile(path);
            Dictionary<string, string> header = TrainingSession.getHeaderInfo(sessionFile);
            string activity = header["activity"];
            TrainingSession session;
            switch (activity)
            {
                case "Running":
                    session = new RunningSession(header["startTime"]);
                    session.loadSessionBody(sessionFile);
                    MessageBox.Show("Number of data points in session: " + session.mTicks.ToString());
                    break;
                case "Biking":
                    session = new CyclingSession(header["startTime"]);
                    session.loadSessionBody(sessionFile);
                    MessageBox.Show("Number of data points in session: " + session.mTicks.ToString());
                    break;
                default:
                    break;
                    
            }
        }
    }
}

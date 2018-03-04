using BetterConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace WpfDemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MyBetterConfigProjectViewModel();
        }
    }

    public class MyBetterConfigProjectViewModel : INotifyPropertyChanged
    {
        private string settingKey;
        private string settingValue;
        private string projectSecret;
        private ushort pollInterval = 5;

        private BetterConfigClient betterConfigClient;

        public event PropertyChangedEventHandler PropertyChanged;

        public MyBetterConfigProjectViewModel()
        {
            
        }

        private void RaisePropertyChanged(string propertyName)
        {            
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string SettingKey
        {
            get { return settingKey; }
            set
            {
                if (settingKey != value)
                {
                    settingKey = value;
                    RaisePropertyChanged("SettingKey");
                    InitBcClient();
                }
            }
        }

        public string SettingValue
        {
            get { return settingValue; }
            set
            {
                if (settingValue != value)
                {
                    settingValue = value;
                    RaisePropertyChanged("SettingValue");                    
                }
            }
        }

        public string ProjectSecret
        {
            get { return projectSecret; }
            set
            {
                if (projectSecret != value)
                {
                    projectSecret = value;
                    RaisePropertyChanged("ProjectSecret");
                    InitBcClient();
                }
            }
        }

        //PollInterval
        public ushort PollInterval
        {
            get { return pollInterval; }
            set
            {
                if (pollInterval != value)
                {
                    pollInterval = value;
                    RaisePropertyChanged("PollInterval");
                    InitBcClient();
                }
            }
        }

        private void InitBcClient()
        {
            if (this.betterConfigClient != null)
            {
                this.betterConfigClient.ConfigurationChanged -= BetterConfigClient_ConfigurationChanged;
            }

            if (!string.IsNullOrEmpty(this.projectSecret))
            {
                this.betterConfigClient = new BetterConfigClient(
                    new BetterConfigClientConfiguration
                    {
                        PollIntervalSeconds = pollInterval,
                        ProjectSecret = this.projectSecret
                    });

                this.betterConfigClient.ConfigurationChanged += BetterConfigClient_ConfigurationChanged;
            }
        }

        private void BetterConfigClient_ConfigurationChanged(object sender, EventArgs e)
        {
            // refresh config

            SettingValue = this.betterConfigClient.GetValue(this.settingKey, "N/A");
        }        
    }
}

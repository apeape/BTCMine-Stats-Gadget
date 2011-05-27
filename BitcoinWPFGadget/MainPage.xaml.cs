using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Globalization;
//using Microsoft.Windows.Controls;

namespace BitcoinWPFGadget
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        const int apikey_length = 40;
        private DateTime LastUpdate;
        private Timer updateTimer;
        private Timer displayCountdownTimer;
        private TimeSpan timerUpdateRate = TimeSpan.FromMinutes(1);

        public ObservableCollection<BTCMine.UserStats> UserStats { get; set; }
        public ObservableCollection<BTCMine.Miner> MinerStats { get; set; }
        public ObservableCollection<BTCMine.Pool> PoolStats { get; set; }

        string apiKey = String.Empty;
        BTCGuild.Stats stats = default(BTCGuild.Stats);
        double currentDifficulty;
        MtGox.TickerData ticker = default(MtGox.TickerData);

        public static TimeSpan yellowIdleThreshold;
        public static TimeSpan redIdleThreshold;

        public MainPage()
        {
            this.DataContext = this;
            InitializeComponent();

            this.apikey.Text = Properties.Settings.Default.apikey;

            this.LastUpdate = DateTime.Now;

            this.UserStats = new ObservableCollection<BTCMine.UserStats>();
            this.MinerStats = new ObservableCollection<BTCMine.Miner>();
            this.PoolStats = new ObservableCollection<BTCMine.Pool>();

            this.userStats.ItemsSource = this.UserStats;
            this.workerStats.ItemsSource = this.MinerStats;
            this.poolStats.ItemsSource = this.PoolStats;
            // custom worker list background color based on last share time
            this.workerStats.ItemContainerStyleSelector = new WorkerListStyleSelector();

            this.workerTotalsStats.ItemsSource = this.UserStats;

            // start a timer to update the stats
            this.updateTimer = new Timer(this.UpdateTimerTick, null, TimeSpan.Zero, this.timerUpdateRate);

            // start a timer to update the update countdown display
            this.displayCountdownTimer = new Timer(UpdateCountdownDisplay, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public void UpdateTimerTick(object state)
        {
            this.LastUpdate = DateTime.Now;

            // don't update without a valid api key
            if (apiKey == string.Empty || apiKey.Length != apikey_length) return;

            try
            {
                var poolstats = BTCMine.GetPoolStats(apiKey);
                var userstats = BTCMine.GetUserStats(apiKey);
                var minerstats = BTCMine.GetMinerStats(apiKey);

                // grab stats from json api
                stats = BTCGuild.GetStats(apiKey);
                if (poolstats == default(BTCMine.Pool) || userstats == default(BTCMine.UserStats) || minerstats == default(BTCMine.MinerStats))
                    throw new System.Runtime.Serialization.SerializationException("Failed to deserialize from JSON!");

                // grab current difficulty
                currentDifficulty = Utility.Deserialize<double>("http://blockexplorer.com/q/getdifficulty");

                // grab current mtgox ticker
                ticker = MtGox.GetTickerData();

                UpdateGUI(() =>
                {
                    try
                    {
                        if (currentDifficulty == default(double))
                        {
                            // failed to retrieve difficulty, btc per day calculation not possible
                        }
                        else
                        {
                            // from the gribble bot:
                            // The expected generation output, at $1 Khps, given current difficulty of [bc,diff
                            // is [math calc 50*24*60*60 / (1/((2**224-1)/[bc,diff]*$1*1000/2**256))]
                            // BTC per day and [math calc 50*60*60 / (1/((2**224-1)/[bc,diff]*$1*1000/2**256))] BTC per hour.".
                            userstats.btc_per_day = 50 * TimeSpan.FromDays(1).TotalSeconds / (1 / (Math.Pow(2, 224) - 1)) / currentDifficulty * userstats.hashrate * 1000000 / Math.Pow(2, 256);
                            userstats.btc_per_hour = userstats.btc_per_day / 24f;
                        }
                        // update mtgox exchange rate
                        if (ticker == default(MtGox.TickerData))
                        {
                            // failed to retrieve mtgox data, leave current value
                        }
                        else
                        {
                            this.usd.Text = ticker.buy.ToString("C");
                            this.UpArrow.Visibility = this.ticker.buy > this.ticker.last ? Visibility.Visible : Visibility.Hidden;
                            this.DownArrow.Visibility = this.ticker.buy < this.ticker.last ? Visibility.Visible : Visibility.Hidden;
                            this.usd.Foreground = this.ticker.buy >= this.ticker.last ? Brushes.LightGreen : Brushes.LightPink;
                            userstats.usd_per_day_stats = ((decimal)userstats.btc_per_day * ticker.buy).ToString("C");
                        }

                        // bind user stats
                        this.UserStats.Clear();
                        this.UserStats.Add(userstats);

                        // bind pool stats
                        this.PoolStats.Clear();
                        this.PoolStats.Add(poolstats);

                        // bind worker stats
                        this.MinerStats.Clear();
                        minerstats.miners.ForEach(miner => this.MinerStats.Add(miner));

                        this.test.Text = "BTCMine Stats"; // reset error
                    }
                    catch (Exception e)
                    {
                        // probably an error updating the gui
                        this.test.Text = e.Message;
                    }
                });
            }
            catch (Exception e)
            {
                UpdateGUI(() =>
                {
                    // probably an error deserializing the json, or we timed out trying to read it
                    this.test.Text = e.Message;
                });
            }
        }

        public void UpdateCountdownDisplay(object state)
        {
            UpdateGUI(() =>
                {
                    this.Countdown.Text = (timerUpdateRate - (DateTime.Now - LastUpdate)).ToString(@"mm\:ss");
                });
        }

        /// <summary>
        /// In WPF, only the thread that created a DispatcherObject may access that object. 
        /// For example, a background thread that is spun off from the main UI thread cannot
        /// update the contents of a Button that was created on the UI thread.
        /// In order for the background thread to access the Content property of the Button,
        /// the background thread must delegate the work to the Dispatcher associated with the UI thread.
        /// </summary>
        /// <param name="action"></param>
        public void UpdateGUI(Action action)
        {
            this.Dispatcher.BeginInvoke((ThreadStart)delegate() { action();});
        }

        private void Donations_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Donations.SelectAll();
        }

        private void savebutton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.apikey = this.apikey.Text;
            Properties.Settings.Default.Save();
        }

        private void apikey_TextChanged(object sender, TextChangedEventArgs e)
        {
            // perform initial update after they paste/type in a new key
            if (this.apikey.Text.Length == apikey_length)
            {
                apiKey = this.apikey.Text;
                UpdateTimerTick(null);
            }
        }

        private void numbersOnlyTextbox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            UInt32 result;
            if (!UInt32.TryParse(e.Text, out result))
            {
                e.Handled = true;
            }
        }
    }
}

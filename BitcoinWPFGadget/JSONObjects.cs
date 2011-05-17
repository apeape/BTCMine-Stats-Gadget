using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace BitcoinWPFGadget
{
    public class MtGox
    {
        public static MtGox.TickerData GetTickerData()
        {
            return Utility.Deserialize<MtGox.Ticker>("https://mtgox.com/code/data/ticker.php").ticker;
        }

        public class Ticker
        {
            public TickerData ticker { get; set; }
        }
        public class TickerData
        {
            public decimal high { get; set; }
            public decimal low { get; set; }
            public double vol { get; set; }
            public decimal buy { get; set; }
            public decimal sell { get; set; }
            public decimal last { get; set; }
        }
    }
    public class BTCMine
    {
        public static BTCMine.Pool GetPoolStats(string apikey)
        {
            return Utility.Deserialize<BTCMine.Pool>("http://btcmine.com/api/getpoolstats/" + apikey);
        }

        public static BTCMine.UserBalance GetUserBalance(string apikey)
        {
            return Utility.Deserialize<BTCMine.UserBalance>("http://btcmine.com/api/getbalance/" + apikey);
        }

        public static BTCMine.UserStats GetUserStats(string apikey)
        {
            return Utility.Deserialize<BTCMine.UserStats>("http://btcmine.com/api/getstats/" + apikey);
        }

        public static BTCMine.MinerStats GetMinerStats(string apikey)
        {
            return Utility.Deserialize<BTCMine.MinerStats>("http://btcmine.com/api/getminerstats/" + apikey);
        }

        public class Pool
        {
            public UInt64 getworks { get; set; }
            public UInt64 round_shares { get; set; }
            public UInt64 miners { get; set; }
            public TimeSpan round_duration { get; set; }
            public UInt64 block { get; set; }
            public DateTime round_started { get; set; }
            public double hashrate { get; set; }

            public string getworks_stats { get { return getworks + " GW/s"; } }
            public string hashrate_stats { get { return (hashrate / 1000f).ToString("0.00") + " gH/s"; } }
        }

        public class UserBalance
        {
            public double confirmed { get; set; }
            public double unconfirmed { get; set; }

            public string confirmed_stats { get { return confirmed.ToString("0.00") + " BTC"; } }
            public string unconfirmed_stats { get { return unconfirmed.ToString("0.00") + " BTC"; } }
        }

        public class UserStats
        {
            public double total_bounty { get; set; }
            public double confirmed_bounty { get; set; }
            public UInt64 solved_blocks { get; set; }
            public UInt64 round_shares { get; set; }
            public double estimated_bounty { get; set; }
            public UInt64 solved_shares { get; set; }
            public double unconfirmed_bounty { get; set; }
            public double hashrate { get; set; }
            public bool online_status { get; set; }
            public double total_payout { get; set; }

            public string total_bounty_stats { get { return total_bounty.ToString("0.00") + " BTC"; } }
            public string confirmed_bounty_stats { get { return confirmed_bounty.ToString("0.00") + " BTC"; } }
            public string estimated_bounty_stats { get { return estimated_bounty.ToString("0.00") + " BTC"; } }
            public string unconfirmed_bounty_stats { get { return unconfirmed_bounty.ToString("0.00") + " BTC"; } }
            public string total_payout_stats { get { return total_payout.ToString("0.00") + " BTC"; } }
            public string hashrate_stats { get { return hashrate + " mH/s"; } }
            public string online_status_stats { get { return online_status ? "Online" : "Offline"; } }

            public double btc_per_day { get; set; }
            public string btc_per_day_stats { get { return btc_per_day.ToString("0.00") + " BTC"; } }
            public double btc_per_hour { get; set; }
            public string btc_per_hour_stats { get { return btc_per_hour.ToString("0.00") + " BTC"; } }
            public string usd_per_day_stats { get; set; }
        }

        public class MinerStats
        {
            public List<Miner> miners { get; set; }
        }

        public class Miner
        {
            public string name { get; set; }
            public DateTime date_connected { get; set; }
            public bool online_status { get; set; }
            public UInt64 solved_shares { get; set; }
            public UInt64 solved_blocks { get; set; }

            public string online_status_stats { get { return online_status ? "Online" : "Offline"; } }
            public SolidColorBrush onlineStatusColor { get { return online_status ? Brushes.LightGreen : Brushes.LightPink; } }
        }
    }

    public class BTCGuild
    {
        public static BTCGuild.Stats GetStats(string apikey)
        {
            return Utility.Deserialize<BTCGuild.Stats>("http://www.btcguild.com/api.php?api_key=" + apikey);
        }

        public class Stats
        {
            public Pool pool { get; set; }
            public User user { get; set; }
            public Dictionary<String, Worker> workers { get; set; }
        }

        public class Pool
        {
            public UInt64 active_workers { get; set; }
            public double hash_rate { get; set; }
            public UInt64 round_shares { get; set; }
            public TimeSpan round_time { get; set; }

            public string hash_rate_stats { get { return (hash_rate / 1000f).ToString("0.00") + " gH/s"; } }
        }

        public class User
        {
            public double confirmed_rewards { get; set; }
            public double estimated_rewards { get; set; }
            public double payouts { get; set; }
            public double unconfirmed_rewards { get; set; }

            public string confirmed_rewards_stats { get { return confirmed_rewards + " BTC"; } }
            public string estimated_rewards_stats { get { return estimated_rewards + " BTC"; } }
            public string payouts_stats { get { return payouts + " BTC"; } }
            public string unconfirmed_rewards_stats { get { return unconfirmed_rewards + " BTC"; } }
        }

        public class Worker
        {
            public UInt64 blocks_found { get; set; }
            public double hash_rate { get; set; }
            public TimeSpan last_share_timespan
            { 
                get {
                    TimeSpan res = new TimeSpan();
                    if (TimeSpan.TryParse(last_share, out res))
                        return res;
                    else return TimeSpan.MaxValue;
                }
            }
            public string last_share { get; set; }
            public UInt64 reset_shares { get; set; }
            public UInt64 reset_stales { get; set; }
            public UInt64 round_shares { get; set; }
            public UInt64 round_stales { get; set; }
            public UInt64 total_shares { get; set; }
            public UInt64 total_stales { get; set; }
            public string worker_name { get; set; }

            public string hash_rate_stats { get { return hash_rate + " mH/s"; } }
            public string reset_share_stats { get { return reset_shares + " (" + reset_stales + ")"; } }
            public string round_share_stats { get { return round_shares + " (" + round_stales + ")"; } }
            public string total_share_stats { get { return total_shares + " (" + total_stales + ")"; } }

            public SolidColorBrush statusColor
            {
                get
                {
                    if (last_share_timespan > MainPage.redIdleThreshold)
                        return Brushes.LightPink;
                    if (last_share_timespan > MainPage.yellowIdleThreshold)
                        return Brushes.LightYellow;
                    else return Brushes.LightGreen;
                }
            }
        }

        public class WorkerTotals
        {
            public double total_hash_rate { get; set; }
            public UInt64 round_shares_total { get; set; }
            public UInt64 round_stales_total { get; set; }
            public UInt64 reset_shares_total { get; set; }
            public UInt64 reset_stales_total { get; set; }
            public UInt64 total_shares_total { get; set; }
            public UInt64 total_stales_total { get; set; }

            public string hash_rate_total_stats { get { return total_hash_rate + " mH/s"; } }
            public string round_shares_total_stats { get { return round_shares_total + " (" + round_stales_total + ")"; } }
            public string reset_shares_total_stats { get { return reset_shares_total + " (" + reset_stales_total + ")"; } }
            public string total_shares_total_stats { get { return total_shares_total + " (" + total_stales_total + ")"; } }
            public UInt64 blocks_found_total { get; set; }

            public double btc_per_day { get; set; }
            public string btc_per_day_stats { get { return btc_per_day.ToString("0.00"); } }
        }
    }

    public class BitcoinCharts
    {
        public static List<BitcoinCharts.Market> GetMarkets()
        {
            return Utility.Deserialize<List<BitcoinCharts.Market>>("http://bitcoincharts.com/t/markets.json");
        }

        /// <summary>
        /// Get current weighted BTC prices from bitcoincharts
        /// </summary>
        public static BitcoinCharts.WeightedPrices GetWeightedPrices()
        {
            return Utility.Deserialize<BitcoinCharts.WeightedPrices>("http://bitcoincharts.com/t/weighted_prices.json");
        }

        public enum MarketSymbol
        {
            mtgoxUSD,
            bcmPPUSD,
            britcoinGBP,
            bcEUR,
            bcLREUR,
            bcLRUSD,
            btcexJPY,
            btcexWMR,
            bcmLRUSD,
            bcmPXGAU,
            btcexRUB,
            bcPGAU,
            btcexUSD,
            btcexEUR,
            btcexWMZ,
            btcexYAD,
            bcmBMUSD,
            bitomatPLN,
            bitmarketEUR,
            bitmarketGBP,
            bitmarketPLN,
            virwoxSLL,
        }

        public enum CurrencyType
        {
            USD, RUB, GAU, SLL, GBP, PLN, EUR, JPY
        }

        public class Market
        {
            public double high { get; set; }
            [JsonIgnore]
            public DateTime latestTrade
            {
                get { return Utility.DateTimeFromUnixTime(latest_trade_unixtime); }
            }
            [JsonProperty(PropertyName = "latest_trade")]
            public Int64 latest_trade_unixtime { get; set; }
            public double bid { get; set; }
            public double volume { get; set; }
            [JsonConverter(typeof(StringEnumConverter))]
            public CurrencyType currency { get; set; }
            public double low { get; set; }
            public double n_trades { get; set; }
            public double ask { get; set; }
            public double close { get; set; }
            public double open { get; set; }
            [JsonConverter(typeof(StringEnumConverter))]
            public MarketSymbol symbol { get; set; }
            public double currency_volume { get; set; }
        }

        public class WeightedPrices
        {
            public Currency USD { get; set; }
            public Currency RUB { get; set; }
            public Currency GAU { get; set; }
            public Currency SLL { get; set; }
            public Currency GBP { get; set; }
            public Currency PLN { get; set; }
            public Currency EUR { get; set; }
        }

        public class Currency
        {
            [JsonProperty(PropertyName = "7d")]
            public double Week { get; set; }
            [JsonProperty(PropertyName = "30d")]
            public double Month { get; set; }
            [JsonProperty(PropertyName = "24h")]
            public double Day { get; set; }
        }
    }
}

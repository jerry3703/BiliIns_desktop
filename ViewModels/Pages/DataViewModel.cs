using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows;

namespace Bililns_desktop.ViewModels.Pages
{
    public partial class DataViewModel : ObservableObject
    {

        // BV号
        [ObservableProperty] private string bv1;
        [ObservableProperty] private string bv2;
        [ObservableProperty] private string bv3;

        // 标题
        [ObservableProperty] private string title1;
        [ObservableProperty] private string title2;
        [ObservableProperty] private string title3;

        // 各项数据
        [ObservableProperty] private string playCount1;
        [ObservableProperty] private string playCount2;
        [ObservableProperty] private string playCount3;

        [ObservableProperty] private string likeCount1;
        [ObservableProperty] private string likeCount2;
        [ObservableProperty] private string likeCount3;

        [ObservableProperty] private string likeRate1;
        [ObservableProperty] private string likeRate2;
        [ObservableProperty] private string likeRate3;

        [ObservableProperty] private string coinCount1;
        [ObservableProperty] private string coinCount2;
        [ObservableProperty] private string coinCount3;

        [ObservableProperty] private string coinRate1;
        [ObservableProperty] private string coinRate2;
        [ObservableProperty] private string coinRate3;

        [ObservableProperty] private string favCount1;
        [ObservableProperty] private string favCount2;
        [ObservableProperty] private string favCount3;

        [ObservableProperty] private string commentCount1;
        [ObservableProperty] private string commentCount2;
        [ObservableProperty] private string commentCount3;

        [ObservableProperty] private string danmakuCount1;
        [ObservableProperty] private string danmakuCount2;
        [ObservableProperty] private string danmakuCount3;

        [ObservableProperty] private string interactRate1;
        [ObservableProperty] private string interactRate2;
        [ObservableProperty] private string interactRate3;

        [ObservableProperty] private string pubDate1;
        [ObservableProperty] private string pubDate2;
        [ObservableProperty] private string pubDate3;

        // 柱状图数据
        public SeriesCollection ChartSeries { get; set; } = new SeriesCollection();
        public string[] ChartLabels { get; set; } = new[] { "点赞", "投币", "收藏", "评论", "弹幕" };

        public DataViewModel()
        {
            // 初始化数据
            Title1 = "稿件1";
            Title2 = "稿件2";
            Title3 = "稿件3";
        }

        [RelayCommand]
        public async Task ClickAsync()
        {
            // 校验BV号是否为空
            if (string.IsNullOrWhiteSpace(Bv1) || string.IsNullOrWhiteSpace(Bv2) || string.IsNullOrWhiteSpace(Bv3))
            {
                MessageBox.Show("请填写完整的三个BV号！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 校验BV号格式（简单示例，B站BV号一般以BV开头且长度为12）
            if (!Bv1.StartsWith("BV") || Bv1.Length != 12 ||
                !Bv2.StartsWith("BV") || Bv2.Length != 12 ||
                !Bv3.StartsWith("BV") || Bv3.Length != 12)
            {
                MessageBox.Show("请输入正确格式的BV号（如BV1xxxxxxx）！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await FetchAndAnalyzeAsync(Bv1, 1);
            await FetchAndAnalyzeAsync(Bv2, 2);
            await FetchAndAnalyzeAsync(Bv3, 3);
            UpdateChart();

        }

        private async Task FetchAndAnalyzeAsync(string bv, int index)
        {
            if (string.IsNullOrWhiteSpace(bv)) return;
            string url = "https://api.bilibili.com/x/web-interface/view?bvid=" + bv;
            try
            {
                using HttpClient client = new HttpClient();
                string jsonResponse = await client.GetStringAsync(url);
                JObject json = JObject.Parse(jsonResponse);
                if (json["code"]?.ToString() == "0")
                {
                    var data = json["data"];
                    // 标题
                    SetProperty($"Title{index}", data["title"]?.ToString());
                    // BV号
                    SetProperty($"Bv{index}", bv);
                    // 播放量
                    SetProperty($"PlayCount{index}", data["stat"]["view"]?.ToString());
                    // 点赞数
                    SetProperty($"LikeCount{index}", data["stat"]["like"]?.ToString());
                    // 点赞率
                    SetProperty($"LikeRate{index}", CalcPercent(data["stat"]["like"], data["stat"]["view"]));
                    // 硬币数
                    SetProperty($"CoinCount{index}", data["stat"]["coin"]?.ToString());
                    // 投币率
                    SetProperty($"CoinRate{index}", CalcPercent(data["stat"]["coin"], data["stat"]["view"]));
                    // 收藏数
                    SetProperty($"FavCount{index}", data["stat"]["favorite"]?.ToString());
                    // 评论数
                    SetProperty($"CommentCount{index}", data["stat"]["reply"]?.ToString());
                    // 弹幕数
                    SetProperty($"DanmakuCount{index}", data["stat"]["danmaku"]?.ToString());
                    // 互动率
                    int view = data["stat"]["view"]?.ToObject<int>() ?? 0;
                    int danmaku = data["stat"]["danmaku"]?.ToObject<int>() ?? 0;
                    int reply = data["stat"]["reply"]?.ToObject<int>() ?? 0;
                    SetProperty($"InteractRate{index}", view > 0 ? $"{((danmaku + reply) * 100.0 / view):F2}%" : "0%");
                    // 发布时间
                    long pubdate = data["pubdate"]?.ToObject<long>() ?? 0;
                    var dt = DateTimeOffset.FromUnixTimeSeconds(pubdate).LocalDateTime;
                    SetProperty($"PubDate{index}", dt.ToString("yyyy年M月d日"));
                }
                else
                {
                    SetProperty($"Title{index}", "获取失败");
                }
            }
            catch (Exception ex)
            {
                SetProperty($"Title{index}", $"异常:{ex.Message}");
            }
        }

        private string CalcPercent(JToken num, JToken den)
        {
            int n = num?.ToObject<int>() ?? 0;
            int d = den?.ToObject<int>() ?? 0;
            return d > 0 ? $"{(n * 100.0 / d):F2}%" : "0%";
        }

        private void SetProperty(string prop, string value)
        {
            var property = GetType().GetProperty(prop);
            if (property != null)
                property.SetValue(this, value);
        }

        private void UpdateChart()
        {
            ChartSeries.Clear();
            ChartSeries.Add(new ColumnSeries
            {
                Title = "1",
                Values = new ChartValues<int>
                {
                    int.TryParse(LikeCount1, out var l1) ? l1 : 0,
                    int.TryParse(CoinCount1, out var c1) ? c1 : 0,
                    int.TryParse(FavCount1, out var f1) ? f1 : 0,
                    int.TryParse(CommentCount1, out var cm1) ? cm1 : 0,
                    int.TryParse(DanmakuCount1, out var d1) ? d1 : 0
                }
            });
            ChartSeries.Add(new ColumnSeries
            {
                Title = "2",
                Values = new ChartValues<int>
                {
                    int.TryParse(LikeCount2, out var l2) ? l2 : 0,
                    int.TryParse(CoinCount2, out var c2) ? c2 : 0,
                    int.TryParse(FavCount2, out var f2) ? f2 : 0,
                    int.TryParse(CommentCount2, out var cm2) ? cm2 : 0,
                    int.TryParse(DanmakuCount2, out var d2) ? d2 : 0
                }
            });
            ChartSeries.Add(new ColumnSeries
            {
                Title = "3",
                Values = new ChartValues<int>
                {
                    int.TryParse(LikeCount3, out var l3) ? l3 : 0,
                    int.TryParse(CoinCount3, out var c3) ? c3 : 0,
                    int.TryParse(FavCount3, out var f3) ? f3 : 0,
                    int.TryParse(CommentCount3, out var cm3) ? cm3 : 0,
                    int.TryParse(DanmakuCount3, out var d3) ? d3 : 0
                }
            });
        }
    }
}

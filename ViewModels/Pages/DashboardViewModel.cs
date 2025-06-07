using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text.Json; // 添加此命名空间以解决 JsonException 的问题
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
namespace Bililns_desktop.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private  string bvtext;
        [ObservableProperty]
        private string testtext;
        [ObservableProperty]
        private string aid;
        [ObservableProperty]
        private string timeout;
        [ObservableProperty]
        private string title;
        [ObservableProperty]
        private string view;
        [ObservableProperty]
        private string danmaku;
        [ObservableProperty]
        private string like;
        [ObservableProperty]
        private string detail;//视频简介
        [ObservableProperty]
        private string bvtextplus;//bv type +
        [ObservableProperty]
        private string owner;//owner type +
        [ObservableProperty]
        private string timeoutplus;//owner type +
        [ObservableProperty]
        private string createtime;//owner type +
        /*
        [ObservableProperty]
        private string coinandfav;//owner type +
        [ObservableProperty]
        private string revandshare;//owner type +
        */
        //数据分析
        [ObservableProperty]
        private string coin;
        [ObservableProperty]
        private string fav;
        [ObservableProperty]
        private string rev;
        [ObservableProperty]
        private string share;

        [ObservableProperty]
        private double likepercent;
        [ObservableProperty]
        private double coinpercent;
        [ObservableProperty]
        private double handpercent;//互动率
        [ObservableProperty]
        private static string airesultout;//ai分析结果
        [RelayCommand]
        private void checkbut()
        {
            testtext = bvtext;
            OnPropertyChanged(nameof(Testtext));
            // MessageBox.Show(testtext);
            FetchAndParseJsonAsync();
            Ai(bvtext);
           // System.Diagnostics.Debug.WriteLine(airesultout);
            //OnPropertyChanged(nameof(Airesultout));

        }



       async Task Ai(string bvcode)
        {
            //获取评论
            string bvid = bvcode; // 替换为你要查询的视频BV号
            var comments = await CommentAcquisition.GetData(bvcode);

            if (comments == null || comments.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("没有获取到评论");
                airesultout = "没有获取到评论,检查BV号/网络？";
                return;
            }

            // 如果评论数量大于60，随机选60条，否则返回全部
            List<Comment> result;
            if (comments.Count > 60)
            {
                Random rnd = new Random();
                result = comments.OrderBy(x => rnd.Next()).Take(60).ToList();
            }
            else
            {
                result = comments;
            }

            // 输出结果
            foreach (var comment in result)
            {
                System.Diagnostics.Debug.WriteLine($"{comment.user_name}: {comment.content}");
            }

            /*
            // 使用 DeepSeek API 生成文本
            var apiKey = "";
            var modelName = "deepseek-chat";
            var prompt = "对评论进行分析(结合分析, 不可逐条分析, 不可说明该分析是针对哪几条评论), 输出不可涉及评论内容; 仅对情感和正反风向进行200字以内简要分析，以下是评论"+comments;    
            var deepSeek = new DeepSeekApi(apiKey);
            string resultai = await deepSeek.GenerateTextAsync(modelName, prompt);
            
            */
            string resultai = "未启用AI分析功能！";
            airesultout = resultai;
            OnPropertyChanged(nameof(Airesultout));
            // MessageBox.Show(resultai);
            System.Diagnostics.Debug.WriteLine(resultai);
            return  ;
           // OnPropertyChanged(nameof(Timeout));
        }



        async Task FetchAndParseJsonAsync()
        {
            // 定义 API URL
            string url = "https://api.bilibili.com/x/web-interface/view?bvid=";
            try
            {
                // 创建 HttpClient 并发送请求
                using HttpClient client = new HttpClient();
                string jsonResponse = await client.GetStringAsync(url + bvtext);

                // 解析 JSON 响应
                JObject json = JObject.Parse(jsonResponse);

                // 提取 message 字段
                string value1 = json["message"]?.ToString();
                testtext = value1;
                OnPropertyChanged(nameof(Testtext));

                if (value1 == "0")
                {
                    airesultout = "分析中。。。。。";
                    OnPropertyChanged(nameof(Airesultout));

                    // 提取并格式化 JSON 数据
                    string aidin = json["data"]["aidin"]?.ToString();
                    title = json["data"]["title"]?.ToString();

                    view = json["data"]["stat"]["view"]?.ToString();
                    view = "播放量：" + view + "   ";

                    danmaku = json["data"]["stat"]["danmaku"]?.ToString();
                    danmaku = "弹幕：" + danmaku + "   ";

                    like = json["data"]["stat"]["like"]?.ToString();
                    like = "点赞：" + like + "   ";

                    detail = json["data"]["desc"]?.ToString();

                    owner = json["data"]["owner"]["name"]?.ToString();
                    owner = "UP主：" + owner + "   ";

                    bvtextplus = "bv号：" +bvtext; 

                    string coinnum = json["data"]["stat"]["coin"]?.ToString();
                    string favnum = json["data"]["stat"]["favorite"]?.ToString();
                    //coinandfav = "       "+coinnum + "        " + favnum + "   ";
                    coin = coinnum;
                    fav = favnum;

                    string revnum = json["data"]["stat"]["reply"]?.ToString();
                    string sharenum = json["data"]["stat"]["share"]?.ToString();
                    rev = revnum;
                    share = sharenum;
                    //revandshare = "       " + revnum + "        " + sharenum + "   ";

                    // 获取并转换发布时间戳
                    long pubdatein = json["data"]["pubdate"]?.ToObject<long>() ?? 0;
                    System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                    var time = startTime.AddSeconds(pubdatein);
                    timeout = time.ToString();
                    timeoutplus = "发布时间：" + timeout;
                    // 创建时间戳处理
                    long createti = json["data"]["ctime"]?.ToObject<long>() ?? 0;
                    System.DateTime startTime2 = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                    var time2 = startTime2.AddSeconds(pubdatein);
                    createtime = time2.ToString();
                    createtime = "创建时间：" + createtime;

                    // 计算百分比
         
  // 计算百分比
                    int likeInt = 0, viewInt = 0, coinInt = 0, favInt = 0, danmakuInt = 0, replyInt = 0, shareInt = 0;
                    int.TryParse(json["data"]["stat"]["like"]?.ToString(), out likeInt);
                    int.TryParse(json["data"]["stat"]["view"]?.ToString(), out viewInt);
                    int.TryParse(json["data"]["stat"]["coin"]?.ToString(), out coinInt);
                    int.TryParse(json["data"]["stat"]["favorite"]?.ToString(), out favInt);
                    int.TryParse(json["data"]["stat"]["danmaku"]?.ToString(), out danmakuInt);
                    int.TryParse(json["data"]["stat"]["reply"]?.ToString(), out replyInt);
                    int.TryParse(json["data"]["stat"]["share"]?.ToString(), out shareInt);

                    likepercent = viewInt > 0 ? Math.Round((double)likeInt / viewInt * 100, 2) : 0;
                    coinpercent = viewInt > 0 ? Math.Round((double)coinInt / viewInt * 100, 2) : 0;
                    handpercent = viewInt > 0
                        ? Math.Round(
                            ((double)danmakuInt + replyInt) / viewInt * 100, 2)
                        : 0;


                    // 更新前端绑定属性
                    OnPropertyChanged(nameof(Timeout));
                    OnPropertyChanged(nameof(Title));
                    OnPropertyChanged(nameof(View));
                    OnPropertyChanged(nameof(Danmaku));
                    OnPropertyChanged(nameof(Like));
                    OnPropertyChanged(nameof(Detail));
                    OnPropertyChanged(nameof(bvtextplus));
                    OnPropertyChanged(nameof(Owner));
                    OnPropertyChanged(nameof(Timeoutplus));
                    OnPropertyChanged(nameof(Createtime));
                    //OnPropertyChanged(nameof(Coinandfav));
                    //OnPropertyChanged(nameof(Revandshare));
                    OnPropertyChanged(nameof(Likepercent));
                    OnPropertyChanged(nameof(Coinpercent));
                    OnPropertyChanged(nameof(Handpercent));
                    OnPropertyChanged(nameof(Coin));
                    OnPropertyChanged(nameof(Fav));
                    OnPropertyChanged(nameof(Share));
                    OnPropertyChanged(nameof(Rev));
                }
                else
                {
                    airesultout = "错误BV号/网络错误";
                    // 处理错误信息
                    title = "获取失败，错误信息：" + value1;
                    view = "播放量：未知  ";
                    danmaku = "弹幕：未知    ";
                    like = "点赞：未知   ";
                    timeout = "未知   ";
                    bvtextplus = "未知    ";
                    detail = "未知   ";
                    owner = "未知   ";
                    timeoutplus = "未知   ";
                    createtime = "未知   ";
                    //coinandfav = "未知   ";
                    //revandshare = "未知   ";
                    coin = "未知   ";
                    fav = "未知   ";
                    share = "未知   ";
                    rev = "未知   ";
                    likepercent = 0;
                    coinpercent = 0;
                    handpercent = 0;
                    // 更新前端绑定属性
                    OnPropertyChanged(nameof(Airesultout));
                    OnPropertyChanged(nameof(Timeout));
                    OnPropertyChanged(nameof(Title));
                    OnPropertyChanged(nameof(View));
                    OnPropertyChanged(nameof(Danmaku));
                    OnPropertyChanged(nameof(Like));
                    OnPropertyChanged(nameof(Detail));
                    OnPropertyChanged(nameof(bvtextplus));
                    OnPropertyChanged(nameof(Owner));
                    OnPropertyChanged(nameof(Timeoutplus));
                    OnPropertyChanged(nameof(Createtime));
                   // OnPropertyChanged(nameof(Coinandfav));
                   // OnPropertyChanged(nameof(Revandshare));
                    OnPropertyChanged(nameof(Likepercent));
                    OnPropertyChanged(nameof(Coinpercent));
                    OnPropertyChanged(nameof(Handpercent));
                    OnPropertyChanged(nameof(Timeoutplus));
                    OnPropertyChanged(nameof(Coin));
                    OnPropertyChanged(nameof(Fav));
                    OnPropertyChanged(nameof(Share));
                    OnPropertyChanged(nameof(Rev));

                }
            }
            catch (HttpRequestException ex)
            {
                // 处理网络请求异常
                Testtext = $"网络错误: {ex.Message}";
            }
            catch (JsonException ex)
            {
                // 处理 JSON 解析异常
                Testtext = $"JSON 解析错误: {ex.Message}";
            }
        }
    }
}

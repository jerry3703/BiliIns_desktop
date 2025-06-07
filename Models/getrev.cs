using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;

/// 获取评论，计算相似度
/// 评论数据结构
public class Comment
{
    public string user_name { get; set; }
    public string user_id { get; set; }
    public string content { get; set; }
    public int like_count { get; set; }
    public int reply_count { get; set; }
    public string comment_time { get; set; }
}

public class CommentAcquisition
{
    private static readonly HttpClient client = new HttpClient();

    // 获取视频信息
    public static async Task<(string aid, string cid, string title, int reply_count)> GetBvidInfo(string bvid)
    {
        var url = $"https://api.bilibili.com/x/web-interface/view?bvid={bvid}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        request.Headers.Add("Referer", $"https://www.bilibili.com/video/{bvid}");

        try
        {
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            if ((int)json["code"] == 0)
            {
                var data = json["data"];
                return (
                    data["aid"].ToString(),
                    data["cid"].ToString(),
                    data["title"].ToString(),
                    (int)data["stat"]["reply"]
                );
            }
        }
        catch { }
        return (null, null, null, 0);
    }

    // 获取评论（异步版本）
    public static async Task<JObject> GetCommentsAsync(string bvid, string aid, int page = 1, int size = 20)
    {
        var url = $"https://api.bilibili.com/x/v2/reply/main?jsonp=jsonp&next={page}&type=1&oid={bvid}&mode=3&plat=1&size={size}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        request.Headers.Add("Referer", $"https://www.bilibili.com/video/BV{bvid}");
        request.Headers.Add("Cookie", "SESSDATA=27edbd17%2C1764770797%2C9e6be%2A61CjDPEUME7uy0yMx5DtAKRE5PTo5jYAVC0eJYHitlCeSVWr08EYSUm18hJ0IhEhga6d4SVnQyMU1HT3pUaFlXNC04Ynpvd19iUUV0Tk1hZS15cGNtTlNZTFU2amlxQWcwWGNLRXgzSXRIc2VjbmJUR0JVZnc1SGZoZ09xSUdIamhvcWJTQV91bDZnIIEC;bili_jct=c7c62ad434a131a872587fffd1703111;");
        try
        {

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var jsonStr = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(jsonStr); // 临时输出到控制台
            //return JObject.Parse(jsonStr);
            return JObject.Parse(jsonStr);
            response.EnsureSuccessStatusCode();
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }
        catch { return null; }
    }
        
    // 解析评论
    public static List<Comment> ParseComments(JObject jsonData)
    {
        var comments = new List<Comment>();
        if (jsonData != null && (int)jsonData["code"] == 0)
        {
            var replies = jsonData["data"]?["replies"];
            if (replies != null)
            {
                foreach (var reply in replies)
                {
                    comments.Add(new Comment
                    {
                        user_name = reply["member"]["uname"].ToString(),
                        user_id = reply["member"]["mid"].ToString(),
                        content = reply["content"]["message"].ToString(),
                        like_count = (int)reply["like"],
                        reply_count = (int)reply["rcount"],
                        comment_time = DateTimeOffset.FromUnixTimeSeconds((long)reply["ctime"]).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }
            }
        }
        return comments;
    }

    // 过滤评论：去除重复次数大于5的评论
    public static List<Comment> FilterComments(List<Comment> comments)
    {
        var contentCounts = comments.GroupBy(c => c.content)
                                   .ToDictionary(g => g.Key, g => g.Count());
        return comments.Where(c => contentCounts[c.content] <= 5).ToList();
    }

    // 计算评论内容的简单相似度（可用分词+余弦相似度，示例用Jaccard）
    public static double ComputeSimilarity(string a, string b)
    {
        var setA = new HashSet<string>(a.Split(' '));
        var setB = new HashSet<string>(b.Split(' '));
        var intersect = setA.Intersect(setB).Count();
        var union = setA.Union(setB).Count();
        return union == 0 ? 0 : (double)intersect / union;
    }

    // 计算相似度矩阵
    public static double[,] ComputeSimilarityMatrix(List<Comment> comments)
    {
        int n = comments.Count;
        var matrix = new double[n, n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                matrix[i, j] = ComputeSimilarity(comments[i].content, comments[j].content);
        return matrix;
    }

    // 选择内容多样的评论
    public static List<Comment> SelectDiverseComments(List<Comment> comments, double[,] similarityMatrix, int targetCount = 30)
    {
        if (comments.Count <= targetCount) return comments;

        var selected = new List<int>();
        // 先选点赞最多的
        selected.Add(comments.Select((c, i) => (c, i)).OrderByDescending(x => x.c.like_count).First().i);

        while (selected.Count < targetCount)
        {
            double minAvgSim = double.MaxValue;
            int nextIdx = -1;
            for (int i = 0; i < comments.Count; i++)
            {
                if (selected.Contains(i)) continue;
                double avgSim = selected.Select(j => similarityMatrix[i, j]).Average();
                if (avgSim < minAvgSim)
                {
                    minAvgSim = avgSim;
                    nextIdx = i;
                }
            }
            if (nextIdx == -1) break;
            selected.Add(nextIdx);
        }
        return selected.Select(i => comments[i]).ToList();
    }

    // 主函数：获取并筛选评论
    public static async Task<List<Comment>> GetData(string bvid, int sampleQuantity = 30)
    {
        // 验证BV号格式
        if (!System.Text.RegularExpressions.Regex.IsMatch(bvid, @"^BV[A-Za-z0-9]{10}$"))
            return new List<Comment>();

        var (aid, cid, title, reply_count) = await GetBvidInfo(bvid);
        if (string.IsNullOrEmpty(aid) || string.IsNullOrEmpty(cid))
            return new List<Comment>();

        var allComments = new List<Comment>();
        int page = 1;
        while (true)
        {
            var jsonData = await GetCommentsAsync(bvid, aid, page, 20);
            if (jsonData == null || (int)jsonData["code"] != 0 || jsonData["data"]?["replies"] == null)
                break;

            var comments = ParseComments(jsonData);
            if (comments.Count == 0) break;

            allComments.AddRange(comments);

            if (comments.Count < 20 || page == 30) break;
            page++;
        }

        if (allComments.Count == 0) return new List<Comment>();

        var filtered = FilterComments(allComments);
        if (filtered.Count > sampleQuantity)
        {
            var simMatrix = ComputeSimilarityMatrix(filtered);
            return SelectDiverseComments(filtered, simMatrix, sampleQuantity);
        }
        else
        {
            return filtered;
        }
    }
}
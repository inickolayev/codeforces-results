using CodeforcesApi.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeforcesResults.Services
{
    public class ContestWatcherService
    {
        public Dictionary<string, Dictionary<int, Dictionary<int, int>>> Table { get; set; }
        public IList<KeyValuePair<string, int>> ResultTable { get; set; }
        public List<int> Ids { get; set; } = new List<int>
            {
                254437
                ,256327
                ,258122
                ,259870
                ,261200
                ,262801
                ,264099
            };

        public ContestWatcherService(IContestApi contestApi)
        {
            _contestApi = contestApi;
            var t = GenerateRowsAsync();
        }

        private readonly IContestApi _contestApi;

        private async Task GenerateRowsAsync()
        {
            try
            {
                Table = new Dictionary<string, Dictionary<int, Dictionary<int, int>>>();
                foreach (var hwId in Ids)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    var res = await _contestApi.GetStandingsAsync(hwId);
                    foreach (var r in res.Result.Rows)
                    {
                        var handle = r.Party.Members.Single().Handle;
                        var points = (int)r.Points;
                        if (!Table.ContainsKey(handle))
                            Table.Add(handle, new Dictionary<int, Dictionary<int, int>>());
                        if (!Table[handle].ContainsKey(hwId))
                            Table[handle].Add(hwId, new Dictionary<int, int>());
                        var results = r.ProblemResults
                            .Select((v, i) => (v, i))
                            .Where(_ => 
                                _.v.Points > 0
                            );
                        foreach (var p in results)
                        {
                            if (!Table[handle][hwId].ContainsKey(p.i))
                                Table[handle][hwId].Add(p.i, 1);
                        }
                    }
                }
                 ResultTable = Table
                    .Select(kvp =>
                        KeyValuePair.Create(
                            kvp.Key,
                            kvp.Value.Select(it => it.Value.Count()).Sum()
                        )
                    )
                    .OrderByDescending(kvp => kvp.Value)
                     .ToList();
            }
            catch(Exception e)
            { }
        }
    }
}

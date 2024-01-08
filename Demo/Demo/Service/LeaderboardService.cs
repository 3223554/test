using System;
using Demo.Model;
using System.Collections.Concurrent;

namespace Demo.Service
{
    /**
     * 排名服务
     * 
     */

	public class LeaderboardService
	{
        private readonly ConcurrentDictionary<long, Customer> leaderboard = new ConcurrentDictionary<long, Customer>();
        //private readonly object updateRanksLock = new object();

        public decimal UpdateScore(long customerId, decimal score)
        {
            leaderboard.AddOrUpdate(customerId,
                addValueFactory: id => new Customer { CustomerID = id, Score = score },
                updateValueFactory: (id, existingCustomer) =>
                {
                    // 乐观锁
                    existingCustomer.Score += score; 
                    return existingCustomer;
                });

            UpdateRanks();
            return leaderboard[customerId].Score;
        }

        public List<Customer> GetCustomersByRank(int start, int end)
        {
            var sortedLeaderboard = leaderboard.Values.OrderByDescending(c => c.Score)
                                                    .ThenBy(c => c.CustomerID)
                                                    .ToList();

            return sortedLeaderboard.Where(c => c.Rank >= start && c.Rank <= end).ToList();
        }

        public List<Customer> GetCustomersByCustomerId(long customerId, int high, int low)
        {
            if (leaderboard.TryGetValue(customerId, out var customer))
            {
                var startIndex = Math.Max(0, customer.Rank - low - 1);
                var endIndex = Math.Min(leaderboard.Count - 1, customer.Rank + high - 1);

                var sortedLeaderboard = leaderboard.Values.OrderByDescending(c => c.Score)
                                                    .ThenBy(c => c.CustomerID)
                                                    .ToList();

                return sortedLeaderboard.GetRange(startIndex, endIndex - startIndex + 1);
            }

            return new List<Customer>();
        }

        private void UpdateRanks()
        {
          
                var sortedLeaderboard = leaderboard.Values.OrderByDescending(c => c.Score)
                                                   .ThenBy(c => c.CustomerID)
                                                   .ToList();

                for (int i = 0; i < sortedLeaderboard.Count; i++)
                {
                    var customer = sortedLeaderboard[i];
                    customer.Rank = i + 1;
                }
            
        }
    }
}


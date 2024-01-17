using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Demo.Model;

namespace Demo.Service
{
    /**
     * 排名服务
     */
    public class LeaderboardService
    {
        private readonly ConcurrentDictionary<long, Customer> leaderboard = new ConcurrentDictionary<long, Customer>();
        private readonly SortedList<int, Customer> sortedLeaderboard = new SortedList<int, Customer>();
        private readonly object updateLock = new object();


        public decimal UpdateScore(long customerId, decimal score)
        {
            leaderboard.AddOrUpdate(customerId,
                addValueFactory: id => new Customer { CustomerID = id, Score = score },
                updateValueFactory: (id, existingCustomer) =>
                {
                    existingCustomer.Score += score;
                    return existingCustomer;
                });

            UpdateRanks();
            return leaderboard[customerId].Score;
        }


        public List<Customer> GetCustomersByRank(int start, int end)
        {
            return sortedLeaderboard.Values.Skip(start - 1).Take(end - start + 1).ToList();
        }

        public List<Customer> GetCustomersByCustomerId(long customerId, int high, int low)
        {
            if (leaderboard.TryGetValue(customerId, out var customer))
            {
                var startIndex = Math.Max(0, customer.Rank - high - 1);
                var endIndex = Math.Min(sortedLeaderboard.Count - 1, customer.Rank + low - 1);

                return sortedLeaderboard.Values.Skip(startIndex).Take(endIndex - startIndex + 1).ToList();
            }

            return new List<Customer>();
        }

        private void UpdateRanks()
        {
            if (!Monitor.IsEntered(updateLock)) // 检查是否存在锁
            {
                lock (updateLock)
                {
                    var sortedCustomers = leaderboard.Values.OrderByDescending(c => c.Score)
                                         .ThenBy(c => c.CustomerID)
                                         .ToList();

                    for (int i = 0; i < sortedCustomers.Count; i++)
                    {
                        var customer = sortedCustomers[i];
                        customer.Rank = i + 1;
                    }

                    sortedLeaderboard.Clear();
                    foreach (var customer in sortedCustomers)
                    {
                        sortedLeaderboard.Add(customer.Rank, customer);
                    }
                }
            }
         
        }
    }
}

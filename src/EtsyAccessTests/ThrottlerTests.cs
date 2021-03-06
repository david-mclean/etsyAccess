﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EtsyAccess.Models;
using FluentAssertions;
using NUnit.Framework;

namespace EtsyAccessTests
{
	public class ThrottlerTests : BaseTest
	{
		[ Test ]
		public void TestThrottler()
		{
			ThreadPool.SetMinThreads(100, 4);
			System.Net.ServicePointManager.DefaultConnectionLimit = 100;
			List<Shop> shops = new List<Shop>();
			int requestsAmount = 20;

			List<Task> tasks = new List<Task>();

			for (int i = 0; i < requestsAmount; i++)
			{
				int j = i;
				var task = Task.Run(async () =>
				{
					Stopwatch stopwatch = Stopwatch.StartNew();

					Trace.WriteLine($"[{ DateTime.Now }] Started making call #{ j + 1 } to get shop info");
					
					var result = await this.EtsyAdminService.GetShopInfo( this.ShopName, CancellationTokenSource.Token );
					
					Trace.WriteLine($"[{ DateTime.Now }] Completed making call #{ j + 1 } to get shop info. Took { stopwatch.ElapsedMilliseconds } ms to complete");
					shops.Add(result);
					
				});
				
				tasks.Add(task);
			}

			//CancellationTokenSource.CancelAfter( 40 * 1000 );

			Task.WaitAll(tasks.ToArray());

			shops.Count.Should().Be( requestsAmount );
		}
	}
}

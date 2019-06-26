using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EtsyAccess.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace EtsyAccessTests
{
	public class ItemsServiceTests : BaseTest
	{
		private const string Sku = "testsku1";

		[ Test ]
		public void GetListingsGetListingProductBySkuBySku()
		{
			var listing = this.EtsyItemsService.GetListingProductBySku( Sku, CancellationToken.None ).Result;

			Assert.IsNotNull( listing );
			Assert.AreEqual( Sku, listing.Sku );
		}

		[ Test ]
		public void GetListingsBySkus()
		{
			var listing = this.EtsyItemsService.GetListingsBySkus( new List<string> { Sku }, CancellationToken.None ).Result.ToList();

			Assert.IsTrue( listing.Any() );
			Assert.AreEqual( Sku, listing[0].Sku[0] );
		}

		[ Test ]
		public void UpdateSkuQuantity()
		{
			int quantity = 12;

			this.EtsyItemsService.UpdateSkuQuantity( Sku, quantity, CancellationToken.None );

			// assert
			var inventory = this.EtsyItemsService.GetListingProductBySku( Sku, CancellationToken.None ).GetAwaiter().GetResult();

			inventory.Should().NotBeNull();
			inventory.Offerings.Should().NotBeNullOrEmpty();
			inventory.Offerings.First().Quantity.Should().Be( quantity );
		}

		[ Test ]
		public void UpdateSkuQuantityToZero()
		{
			int quantity = 0;
			const string message = "offering must have quantity greater than 0";

			// assert
			var etsyException = Assert.Throws< EtsyException >( () =>
			{
				this.EtsyItemsService.UpdateSkuQuantity( Sku, quantity, CancellationToken.None );
			});

			Assert.That( etsyException.ToString().Contains( message ) );
		}

		[ Test ]
		public void UpdateSkusQuantities()
		{
			string sku = "testSku1";
			int skuQuantity = 6;
			string sku2 = "B07DBJSDPN-20BR";
			int sku2Quantity = 98;

			var quantities = new Dictionary<string, int>
			{
				{ sku, skuQuantity },
				{ sku2, sku2Quantity }
			};

			this.EtsyItemsService.UpdateSkusQuantityAsync(quantities, CancellationToken.None).GetAwaiter().GetResult();

			// assert
			var skuInventory = this.EtsyItemsService.GetListingProductBySku( sku, CancellationToken.None ).GetAwaiter().GetResult();

			skuInventory.Should().NotBeNull();
			skuInventory.Offerings.Should().NotBeNullOrEmpty();
			skuInventory.Offerings.First().Quantity.Should().Be( skuQuantity );

			var sku2Inventory = this.EtsyItemsService.GetListingProductBySku( sku2, CancellationToken.None ).GetAwaiter().GetResult();

			sku2Inventory.Should().NotBeNull();
			sku2Inventory.Offerings.Should().NotBeNullOrEmpty();
			sku2Inventory.Offerings.First().Quantity.Should().Be( sku2Quantity );
		}
	}
}
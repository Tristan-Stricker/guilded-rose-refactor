namespace Tests
{
    using Core;
    using Xunit;

    public class Tests
    {
        [Theory]
        [InlineData(16, 14, "Normal", -1)]  // Once the sell by date has passed, Quality degrades twice as fast
        [InlineData(0, 0, "Normal", 0)]  // The Quality of an item is never negative
        [InlineData(2, 3, "Aged Brie", 2)]  // "Aged Brie" actually increases in Quality the older it gets
        [InlineData(50, 50, "Aged Brie", 2)]  // The Quality of an item is never more than 50
        [InlineData(3, 3, "Sulfuras, Hand of Ragnaros", 2)]  // "Sulfuras", being a legendary item, never has to be sold or decreases in Quality
        [InlineData(3, 4, "Backstage passes to a TAFKAL80ETC concert", 15)]  // ""Backstage passes", like aged brie, increases in Quality as its SellIn value approaches;
        [InlineData(3, 5, "Backstage passes to a TAFKAL80ETC concert", 10)]  // "Backstage passes", Quality increases by 2 when there are 10 days or less
        [InlineData(3, 6, "Backstage passes to a TAFKAL80ETC concert", 5)]  // "Backstage passes", Quality increases by 3 when there are 5 days or less
        [InlineData(3, 0, "Backstage passes to a TAFKAL80ETC concert", 0)]  // "Backstage passes", Quality drops to 0 after the concert

        public void Quality_Changes_When_Updated(int startingQuality, int endingQuality, string productName, int sellIn)
        {
            Item item = new Item { Name = productName, Quality = startingQuality, SellIn = sellIn };
            GildedRose app = new GildedRose(new[] { item });

            // act
            app.UpdateQuality();

            // assert
            Assert.Equal(endingQuality, item.Quality);
        }

        [Theory]
        [InlineData("Normal",                                     1,0)]
        [InlineData("Sulfuras, Hand of Ragnaros",                 1,1)] // "Sulfuras", being a legendary item, never has to be sold or decreases in Quality
        [InlineData("Backstage passes to a TAFKAL80ETC concert",  1,0)]

        public void Sell_In_Changes_On_Update(string productName, int startingSellIn, int expectedSellIn)
        {
            Item item = new Item { Name = productName, Quality = 2, SellIn = startingSellIn };
            GildedRose app = new GildedRose(new[] { item });

            // act
            app.UpdateQuality();

            // assert
            Assert.Equal(expectedSellIn, item.SellIn);
        }
    }
}

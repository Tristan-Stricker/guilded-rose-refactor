using System;
using System.Collections.Generic;

namespace Core
{
    public class GildedRose
    {
        IList<Item> Items { get; }

        public GildedRose(IList<Item> Items)
        {
            this.Items = Items;
        }

        public void UpdateQuality()
        {
            foreach (var item in Items)
            {
                var rules = new List<QualityRule>
                {
                    new QualityRule {
                        Name = "Once the sell by date has passed, Quality degrades twice as fast",
                        ApplyWhen = item.IsPassedSellByDate(),
                        Adjustment = (item) => item.Quality -= 2
                    },
                    new QualityRule {
                        Name = "Aged Brie actually increases in Quality the older it gets",
                        ApplyWhen = item.Name == "Aged Brie",
                        Adjustment = (item) => item.Quality += 1
                    },
                    new QualityRule {
                        Name = "Backstage passes, like aged brie, increases in Quality as its SellIn value approaches",
                        ApplyWhen = item.IsBackStagePass() && item.SellIn > 10,
                        Adjustment = (item) => item.Quality += 1
                    },
                    new QualityRule {
                        Name = "Backstage passes, Quality increases by 2 when there are 10 days or less",
                        ApplyWhen = item.IsBackStagePass() && (item.SellIn > 5 && item.SellIn <= 10),
                        Adjustment = (item) => item.Quality += 2
                    },
                    new QualityRule {
                        Name = "Backstage passes Quality increases by 3 when there are 5 days or less",
                        ApplyWhen = item.IsBackStagePass() && (item.SellIn > 0 && item.SellIn <= 5),
                        Adjustment = (item) => item.Quality += 3
                    },
                    new QualityRule {
                        Name = "Backstage passes, Quality drops to 0 after the concert",
                        ApplyWhen = item.IsBackStagePass() && item.SellIn <= 0,
                        Adjustment = (item) => item.Quality = 0
                    },
                    new QualityRule {
                        Name = "The Quality of an item is never negative",
                        ApplyWhen = item.Quality < 0,
                        Adjustment = (item) => item.Quality = 0
                    },
                    new QualityRule {
                        Name = "The Quality of an item is never more than 50",
                        ApplyWhen = item.Quality > 50,
                        Adjustment = (item) => item.Quality = 50
                    },
                    new QualityRule {
                        Name = "Sulfuras, being a legendary item, never has to be sold or decreases in Quality",
                        ApplyWhen = item.IsSulfurasHandOfRagnaros(),
                        Adjustment = (item) => item.Quality = item.Quality
                    }
                };

                foreach (var rule in rules)
                {
                    if (rule.ApplyWhen)
                    {
                        rule.Adjustment.Invoke(item);
                    }
                }

                item.SellIn -= item.SellIn == 0 && !item.IsSulfurasHandOfRagnaros() ? 0 : 1;
            }
        }

        internal record QualityRule
        {
            public string Name { get; init; }
            public bool ApplyWhen { get; init; }

            public Action<Item> Adjustment { get; set; }
        }
    }

    internal static class ItemExtensions
    {
        public static bool IsBackStagePass(this Item item)
        {
            return item.Name == "Backstage passes to a TAFKAL80ETC concert";
        }

        public static bool IsPassedSellByDate(this Item item)
        {
            return item.SellIn <= 0;
        }

        public static bool IsSulfurasHandOfRagnaros(this Item item)
        {
            return item.Name == "Sulfuras, Hand of Ragnaros";
        }
    }
}

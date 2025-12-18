using System.Collections.Generic;

namespace H.Core.Providers.Feed
{
    public interface IFeedIngredientProvider
    {
        IList<FeedIngredient> GetBeefFeedIngredients();
        IList<FeedIngredient> GetDairyFeedIngredients();
        IList<FeedIngredient> GetSwineFeedIngredients();
        
        FeedIngredient CopyIngredient(FeedIngredient ingredient, double defaultPercentageInDiet);
    }
}
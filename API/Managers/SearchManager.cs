using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.API.Models;
using InventoryManagement.API.DataProvider.SQL;
using NLog;
using System.Linq;

namespace InventoryManagement.API.Managers
{
    public class SearchManager : ISearchManager
    {
        private readonly ISearchRepository _searchRepository;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public SearchManager(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }


        public async Task<RequestResponse<List<SearchResult>>> GetItemsByGroupCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var searchCollection = new RequestResponse<List<SearchResult>>();
            var dataItemCollection = new RequestResponse<ItemByAttributeSearchDataCollection>();

            try
            {
                dataItemCollection = await _searchRepository.GetItemsByGroupCollection(subscriptionId, searchRequest).ConfigureAwait(true);

                var matchedGroups = new List<Group>();
                var matchedTags = new List<Tag>();
                searchCollection.Item = new List<SearchResult>();
                searchCollection.PagedCollection = dataItemCollection.PagedCollection;
                searchCollection.Success = dataItemCollection.Success;
                
                foreach (Item currentItem in dataItemCollection.Item.Items)
                {
                    //  Populate Tags collection for all items
                    var tagQuery = from itemTags in dataItemCollection.Item.ItemTags
                                   join tag in dataItemCollection.Item.Tags on
                                   itemTags.TagId equals tag.Id
                                   where itemTags.ItemId == currentItem.Id
                                   select new Tag
                                   {
                                       Id = tag.Id,
                                       Name = tag.Name,
                                       AttributeCount = tag.AttributeCount,
                                       CreateDate = tag.CreateDate,
                                       UpdateDate = tag.UpdateDate,
                                       IsActive = tag.IsActive,
                                       SubscriptionId = tag.SubscriptionId
                                   };

                    foreach (var matchResult in tagQuery)
                    {
                        if (!matchedTags.Any(mt => mt.Id == matchResult.Id))
                        {
                            matchedTags.Add(matchResult);
                        }
                    }


                    //  Populate Groups collection for all items
                    var groupQuery = from itemGroups in dataItemCollection.Item.ItemGroups
                                     join g in dataItemCollection.Item.Groups on
                                     itemGroups.GroupId equals g.Id
                                     where itemGroups.ItemId == currentItem.Id
                                     select new Group
                                     {
                                         Id = g.Id,
                                         Name = g.Name,
                                         AttributeCount = g.AttributeCount,
                                         CreateDate = g.CreateDate,
                                         UpdateDate = g.UpdateDate,
                                         IsActive = g.IsActive,
                                         SubscriptionId = g.SubscriptionId
                                     };

                    foreach (var matchResult in groupQuery)
                    {
                        if (!matchedGroups.Any(mt => mt.Id == matchResult.Id))
                        {
                            matchedGroups.Add(matchResult);
                        }
                    }


                    searchCollection.Item.Add(new SearchResult { Id = currentItem.Id, Description = currentItem.Description, Name = currentItem.Name, AmountValue = currentItem.AmountValue, Tags = new List<Tag>(matchedTags), Groups = new List<Group>(matchedGroups) });
                    matchedTags.Clear();
                }
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                searchCollection.Success = false;
            }
            return searchCollection;
        }

        public async Task<RequestResponse<List<SearchResult>>> GetItemsByTagCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var searchCollection = new RequestResponse<List<SearchResult>>();
            var dataItemCollection = new RequestResponse<ItemByAttributeSearchDataCollection>();

            try
            {
                dataItemCollection = await _searchRepository.GetItemsByTagCollection(subscriptionId, searchRequest).ConfigureAwait(true);

                var matchedTags = new List<Tag>();
                var matchedGroups = new List<Group>();
                searchCollection.Item = new List<SearchResult>();
                searchCollection.PagedCollection = dataItemCollection.PagedCollection;
                searchCollection.Success = dataItemCollection.Success;

                //  Populate Tags collection for all items
                foreach (Item currentItem in dataItemCollection.Item.Items)
                {
                    var tagQuery = from itemTags in dataItemCollection.Item.ItemTags
                                join tag in dataItemCollection.Item.Tags on
                                itemTags.TagId equals tag.Id
                                where itemTags.ItemId == currentItem.Id
                                select new Tag
                                {
                                    Id = tag.Id,
                                    Name = tag.Name,
                                    AttributeCount = tag.AttributeCount,
                                    CreateDate = tag.CreateDate,
                                    UpdateDate = tag.UpdateDate,
                                    IsActive = tag.IsActive,
                                    SubscriptionId = tag.SubscriptionId
                                };

                    foreach (var matchResult in tagQuery)
                    {
                        if (!matchedTags.Any(mt => mt.Id == matchResult.Id))
                        {
                            matchedTags.Add(matchResult);
                        }
                    }
                    //  Populate Groups collection for all items
                    var groupQuery = from itemGroups in dataItemCollection.Item.ItemGroups
                                join g in dataItemCollection.Item.Groups on
                                itemGroups.GroupId equals g.Id
                                where itemGroups.ItemId == currentItem.Id
                                select new Group 
                                { 
                                    Id = g.Id, 
                                    Name = g.Name, 
                                    AttributeCount = g.AttributeCount, 
                                    CreateDate = g.CreateDate, 
                                    UpdateDate = g.UpdateDate, 
                                    IsActive = g.IsActive, 
                                    SubscriptionId = g.SubscriptionId
                                };

                    foreach (var matchResult in groupQuery)
                    {
                        if (!matchedGroups.Any(mt => mt.Id == matchResult.Id))
                        {
                            matchedGroups.Add(matchResult);
                        }
                    }


                    searchCollection.Item.Add(new SearchResult { Id = currentItem.Id, Description = currentItem.Description, Name = currentItem.Name, AmountValue = currentItem.AmountValue, Tags = new List<Tag>(matchedTags), Groups = new List<Group>(matchedGroups) });
                    matchedTags.Clear();
                }
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                searchCollection.Success = false;
            }
            return searchCollection;
        }

        public async Task<RequestResponse<List<SearchResult>>> GetLowQuantityItemCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var searchCollection = new RequestResponse<List<SearchResult>>();

            try
            {
                var dataItemCollection = await _searchRepository.GetLowQuantityItemCollection(subscriptionId, searchRequest).ConfigureAwait(true);

                var matchedTags = new List<Tag>();
                var matchedGroups = new List<Group>();
                searchCollection.Item = new List<SearchResult>();
                searchCollection.PagedCollection = dataItemCollection.PagedCollection;
                searchCollection.Success = dataItemCollection.Success;

                //  Populate Tags collection for all items
                foreach (SearchResult currentItem in dataItemCollection.Item.SearchResults)
                {
                    if (currentItem.ResultType == SearchResult.SearchResultType.Item.ToString().ToLowerInvariant())
                    {
                        var tagQuery = from itemTags in dataItemCollection.Item.ItemTags
                                       join tag in dataItemCollection.Item.Tags on
                                       itemTags.TagId equals tag.Id
                                       where itemTags.ItemId == currentItem.Id
                                       select new Tag
                                       {
                                           Id = tag.Id,
                                           Name = tag.Name,
                                           AttributeCount = tag.AttributeCount,
                                           CreateDate = tag.CreateDate,
                                           UpdateDate = tag.UpdateDate,
                                           IsActive = tag.IsActive,
                                           SubscriptionId = tag.SubscriptionId
                                       };

                        foreach (var matchResult in tagQuery)
                        {
                            if (!matchedTags.Any(mt => mt.Id == matchResult.Id))
                            {
                                matchedTags.Add(matchResult);
                            }
                        }
                        //  Populate Groups collection for all items
                        var groupQuery = from itemGroups in dataItemCollection.Item.ItemGroups
                                         join g in dataItemCollection.Item.Groups on
                                         itemGroups.GroupId equals g.Id
                                         where itemGroups.ItemId == currentItem.Id
                                         select new Group
                                         {
                                             Id = g.Id,
                                             Name = g.Name,
                                             AttributeCount = g.AttributeCount,
                                             CreateDate = g.CreateDate,
                                             UpdateDate = g.UpdateDate,
                                             IsActive = g.IsActive,
                                             SubscriptionId = g.SubscriptionId
                                         };

                        foreach (var matchResult in groupQuery)
                        {
                            if (!matchedGroups.Any(mt => mt.Id == matchResult.Id))
                            {
                                matchedGroups.Add(matchResult);
                            }
                        }

                        searchCollection.Item.Add(
                            new SearchResult
                            {
                                ResultType = currentItem.ResultType,
                                ResultWeight = currentItem.ResultWeight,
                                Id = currentItem.Id,
                                Description = currentItem.Description,
                                Name = currentItem.Name,
                                ResultId = currentItem.ResultId,
                                Tags = new List<Tag>(matchedTags),
                                Groups = new List<Group>(matchedGroups),
                                AmountValue = currentItem.AmountValue
                            });
                        matchedTags.Clear();
                        matchedGroups.Clear();
                    }
                    else
                    {
                        searchCollection.Item.Add(
                            new SearchResult
                            {
                                ResultType = currentItem.ResultType,
                                ResultWeight = currentItem.ResultWeight,
                                Id = currentItem.Id,
                                ResultId = currentItem.ResultId,
                                Description = currentItem.Description,
                                Name = currentItem.Name,
                                AmountValue = currentItem.AmountValue
                            });
                    }
                }

            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                searchCollection.Success = false;
            }
            return searchCollection;
        }


        public async Task<RequestResponse<List<SearchResult>>> GetSearchResultCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var searchCollection = new RequestResponse<List<SearchResult>>();

            try
            {
                var dataItemCollection = await _searchRepository.GetSearchResultCollection(subscriptionId, searchRequest).ConfigureAwait(true);

                var matchedTags = new List<Tag>();
                var matchedGroups = new List<Group>();
                searchCollection.Item = new List<SearchResult>();
                searchCollection.PagedCollection = dataItemCollection.PagedCollection;
                searchCollection.Success = dataItemCollection.Success;

                //  Populate Tags collection for all items
                foreach (SearchResult currentItem in dataItemCollection.Item.SearchResults)
                {
                    if (currentItem.ResultType == SearchResult.SearchResultType.Item.ToString().ToLowerInvariant())
                    {
                        var tagQuery = from itemTags in dataItemCollection.Item.ItemTags
                                       join tag in dataItemCollection.Item.Tags on
                                       itemTags.TagId equals tag.Id
                                       where itemTags.ItemId == currentItem.Id
                                       select new Tag
                                       {
                                           Id = tag.Id,
                                           Name = tag.Name,
                                           AttributeCount = tag.AttributeCount,
                                           CreateDate = tag.CreateDate,
                                           UpdateDate = tag.UpdateDate,
                                           IsActive = tag.IsActive,
                                           SubscriptionId = tag.SubscriptionId
                                       };

                        foreach (var matchResult in tagQuery)
                        {
                            if (!matchedTags.Any(mt => mt.Id == matchResult.Id))
                            {
                                matchedTags.Add(matchResult);
                            }
                        }
                        //  Populate Groups collection for all items
                        var groupQuery = from itemGroups in dataItemCollection.Item.ItemGroups
                                         join g in dataItemCollection.Item.Groups on
                                         itemGroups.GroupId equals g.Id
                                         where itemGroups.ItemId == currentItem.Id
                                         select new Group
                                         {
                                             Id = g.Id,
                                             Name = g.Name,
                                             AttributeCount = g.AttributeCount,
                                             CreateDate = g.CreateDate,
                                             UpdateDate = g.UpdateDate,
                                             IsActive = g.IsActive,
                                             SubscriptionId = g.SubscriptionId
                                         };

                        foreach (var matchResult in groupQuery)
                        {
                            if (!matchedGroups.Any(mt => mt.Id == matchResult.Id))
                            {
                                matchedGroups.Add(matchResult);
                            }
                        }
                        searchCollection.Item.Add(
                            new SearchResult
                            {
                                ResultType = currentItem.ResultType,
                                ResultWeight = currentItem.ResultWeight,
                                Id = currentItem.Id,
                                Description = currentItem.Description,
                                Name = currentItem.Name,
                                ResultId = currentItem.ResultId,
                                Tags = new List<Tag>(matchedTags),
                                Groups = new List<Group>(matchedGroups),
                                AmountValue = currentItem.AmountValue
                            });
                        matchedTags.Clear();
                        matchedGroups.Clear();
                    }
                    else
                    {
                        searchCollection.Item.Add(
                            new SearchResult
                            {
                                ResultType = currentItem.ResultType,
                                ResultWeight = currentItem.ResultWeight,
                                Id = currentItem.Id,
                                ResultId = currentItem.ResultId,
                                Description = currentItem.Description,
                                Name = currentItem.Name,
                                AmountValue = currentItem.AmountValue
                            });
                    }
                }

            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                searchCollection.Success = false;
            }
            return searchCollection;
        }
    }
}

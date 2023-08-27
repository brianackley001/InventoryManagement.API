using InventoryManagement.API.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.UnitTest.Managers.TestSetup
{
    [ExcludeFromCodeCoverage]
    public static class SearchManagerSetUp
    {
        public enum AttributeType { Tag, Group }
        public static ItemByAttributeSearchDataCollection GetItemsByGroup()
        {
            var uniqueItemCollection = GetUniqueItems();

            var groupCollection = new List<Group>
            {
                new Group { Id=20,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Group 1_s1", UpdateDate=DateTime.Now.AddDays(-12), CreateDate=DateTime.Now.AddDays(-11)},
                new Group { Id=30,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Group 2_s1", UpdateDate=DateTime.Now.AddDays(-22), CreateDate=DateTime.Now.AddDays(-12)},
                new Group { Id=40,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Group 3_s1", UpdateDate=DateTime.Now.AddDays(-32), CreateDate=DateTime.Now.AddDays(-21)},
                new Group { Id=50,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Group 4_s1", UpdateDate=DateTime.Now.AddDays(-44), CreateDate=DateTime.Now.AddDays(-33)}
            };
            var tagCollection = new List<Tag>
            {
                new Tag { Id=110,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Tag 1_s1", UpdateDate=DateTime.Now.AddDays(-12), CreateDate=DateTime.Now.AddDays(-11)},
                new Tag { Id=130,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Tag 2_s1", UpdateDate=DateTime.Now.AddDays(-22), CreateDate=DateTime.Now.AddDays(-12)},
                new Tag { Id=140,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Tag 3_s1", UpdateDate=DateTime.Now.AddDays(-32), CreateDate=DateTime.Now.AddDays(-21)},
                new Tag { Id=150,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Tag 4_s1", UpdateDate=DateTime.Now.AddDays(-44), CreateDate=DateTime.Now.AddDays(-33)}
            };

            var itemGroupCollection = new List<ItemGroup> {
                new ItemGroup{Id=9987, IsActive=true, GroupId=20, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=22222, IsActive=true, GroupId=20, ItemId=171,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=23423, IsActive=true, GroupId=20, ItemId=181,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=939383, IsActive=true, GroupId=20, ItemId=199,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=8887, IsActive=true, GroupId=30, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=12, IsActive=true, GroupId=40, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=987345, IsActive=true, GroupId=50, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=99387, IsActive=true, GroupId=30, ItemId=171,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=234324, IsActive=true, GroupId=40, ItemId=181,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)}
            };

            var itemTagCollection = new List<ItemTag>
            {
                new ItemTag{Id=9987, IsActive=true, TagId=110, ItemId=171,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemTag{Id=8887, IsActive=true, TagId=130, ItemId=171,CreateDate=DateTime.Now.AddDays(-22), UpdateDate=DateTime.Now.AddDays(-23)},
                new ItemTag{Id=12987, IsActive=true, TagId=130, ItemId=181,CreateDate=DateTime.Now.AddDays(-23), UpdateDate=DateTime.Now.AddDays(-26)},
                new ItemTag{Id=232323, IsActive=true, TagId=140, ItemId=181,CreateDate=DateTime.Now.AddDays(-11), UpdateDate=DateTime.Now.AddDays(-11)},
                new ItemTag{Id=565656, IsActive=true, TagId=150, ItemId=131,CreateDate=DateTime.Now.AddDays(-54), UpdateDate=DateTime.Now.AddDays(-55)},
                new ItemTag{Id=774545, IsActive=true, TagId=110, ItemId=199,CreateDate=DateTime.Now.AddDays(-74), UpdateDate=DateTime.Now.AddDays(-75)}

            };

            return new ItemByAttributeSearchDataCollection(items: uniqueItemCollection, groups: groupCollection, itemGroups: itemGroupCollection, itemTags: itemTagCollection, tags: tagCollection);
        }
        public static ItemByAttributeSearchDataCollection GetItemsByTag()
        {
            var uniqueItemCollection = GetUniqueItems();

            var tagCollection = new List<Tag>
            {
                new Tag { Id=110,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Tag 1_s1", UpdateDate=DateTime.Now.AddDays(-12), CreateDate=DateTime.Now.AddDays(-11)},
                new Tag { Id=130,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Tag 2_s1", UpdateDate=DateTime.Now.AddDays(-22), CreateDate=DateTime.Now.AddDays(-12)},
                new Tag { Id=140,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Tag 3_s1", UpdateDate=DateTime.Now.AddDays(-32), CreateDate=DateTime.Now.AddDays(-21)},
                new Tag { Id=150,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Tag 4_s1", UpdateDate=DateTime.Now.AddDays(-44), CreateDate=DateTime.Now.AddDays(-33)}
            };
            var groupCollection = new List<Group>
            {
                new Group { Id=20,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Group 1_s1", UpdateDate=DateTime.Now.AddDays(-12), CreateDate=DateTime.Now.AddDays(-11)},
                new Group { Id=30,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Group 2_s1", UpdateDate=DateTime.Now.AddDays(-22), CreateDate=DateTime.Now.AddDays(-12)},
                new Group { Id=40,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Group 3_s1", UpdateDate=DateTime.Now.AddDays(-32), CreateDate=DateTime.Now.AddDays(-21)},
                new Group { Id=50,AttributeCount=1, SubscriptionId=1, IsActive=true, Name="Group 4_s1", UpdateDate=DateTime.Now.AddDays(-44), CreateDate=DateTime.Now.AddDays(-33)}
            };

            var itemTagCollection = new List<ItemTag>
            {
                new ItemTag{Id=9987, IsActive=true, TagId=110, ItemId=171,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemTag{Id=8887, IsActive=true, TagId=130, ItemId=171,CreateDate=DateTime.Now.AddDays(-22), UpdateDate=DateTime.Now.AddDays(-23)},
                new ItemTag{Id=12987, IsActive=true, TagId=130, ItemId=181,CreateDate=DateTime.Now.AddDays(-23), UpdateDate=DateTime.Now.AddDays(-26)},
                new ItemTag{Id=232323, IsActive=true, TagId=140, ItemId=181,CreateDate=DateTime.Now.AddDays(-11), UpdateDate=DateTime.Now.AddDays(-11)},
                new ItemTag{Id=565656, IsActive=true, TagId=150, ItemId=131,CreateDate=DateTime.Now.AddDays(-54), UpdateDate=DateTime.Now.AddDays(-55)},
                new ItemTag{Id=774545, IsActive=true, TagId=110, ItemId=199,CreateDate=DateTime.Now.AddDays(-74), UpdateDate=DateTime.Now.AddDays(-75)}

            };

            var itemGroupCollection = new List<ItemGroup> {
                new ItemGroup{Id=9987, IsActive=true, GroupId=20, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=22222, IsActive=true, GroupId=20, ItemId=171,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=23423, IsActive=true, GroupId=20, ItemId=181,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=939383, IsActive=true, GroupId=20, ItemId=199,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=8887, IsActive=true, GroupId=30, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=12, IsActive=true, GroupId=40, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=987345, IsActive=true, GroupId=50, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=99387, IsActive=true, GroupId=30, ItemId=171,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)},
                new ItemGroup{Id=234324, IsActive=true, GroupId=40, ItemId=181,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12)}
            };

            return new ItemByAttributeSearchDataCollection(items: uniqueItemCollection, groups: groupCollection, itemGroups: itemGroupCollection, itemTags: itemTagCollection, tags: tagCollection);
        }

        public static RequestResponse<ItemByAttributeSearchDataCollection> GetItemsPageOneRequestResponse(AttributeType attributeType)
        {
            var itemsByAttributeCollection = attributeType == AttributeType.Tag ? GetItemsByTag() : GetItemsByGroup();

            var pageRequestResponse = new RequestResponse<ItemByAttributeSearchDataCollection>
            {
                Item = itemsByAttributeCollection,
                PagedCollection = new PagedCollection { CollectionTotal = 6, PageNumber = 1, PageSize = 3 },
                Success = true
            };

            pageRequestResponse
                .Item.Items = new List<Item>
            {
                new Item{
                    Id = itemsByAttributeCollection.Items[0].Id,
                    AmountValue=itemsByAttributeCollection.Items[0].AmountValue,
                    SubscriptionId = itemsByAttributeCollection.Items[0].SubscriptionId,
                    Description = itemsByAttributeCollection.Items[0].Description,
                    Name=itemsByAttributeCollection.Items[0].Name,
                    Groups=new List<Group>(),
                    Tags=new List<Tag>(),
                    IsActive=true
                },
                new Item{
                    Id = itemsByAttributeCollection.Items[1].Id,
                    AmountValue=itemsByAttributeCollection.Items[1].AmountValue,
                    SubscriptionId = itemsByAttributeCollection.Items[1].SubscriptionId,
                    Description = itemsByAttributeCollection.Items[1].Description,
                    Name=itemsByAttributeCollection.Items[1].Name,
                    Groups=new List<Group>(),
                    Tags=new List<Tag>(),
                    IsActive=true
                },
                new Item{
                    Id = itemsByAttributeCollection.Items[2].Id,
                    AmountValue=itemsByAttributeCollection.Items[2].AmountValue,
                    SubscriptionId = itemsByAttributeCollection.Items[2].SubscriptionId,
                    Description = itemsByAttributeCollection.Items[2].Description,
                    Name=itemsByAttributeCollection.Items[2].Name,
                    Groups=new List<Group>(),
                    Tags=new List<Tag>(),
                    IsActive=true
                }
            };

            return pageRequestResponse;

        }
        public static RequestResponse<ItemByAttributeSearchDataCollection> GetItemsPageTwoRequestResponse(AttributeType attributeType)
        {
            var itemsByAttributeCollection = attributeType == AttributeType.Tag ? GetItemsByTag() : GetItemsByGroup();

            var pageRequestResponse = new RequestResponse<ItemByAttributeSearchDataCollection>
            {
                Item = itemsByAttributeCollection,
                PagedCollection = new PagedCollection { CollectionTotal = 6, PageNumber = 2, PageSize = 3 },
                Success = true
            };

            pageRequestResponse
                .Item.Items = new List<Item>
            {
                new Item{
                    Id = itemsByAttributeCollection.Items[3].Id,
                    AmountValue=itemsByAttributeCollection.Items[3].AmountValue,
                    SubscriptionId = itemsByAttributeCollection.Items[3].SubscriptionId,
                    Description = itemsByAttributeCollection.Items[3].Description,
                    Name=itemsByAttributeCollection.Items[3].Name,
                    Groups=new List<Group>(),
                    Tags=new List<Tag>(),
                    IsActive=true
                },
                new Item{
                    Id = itemsByAttributeCollection.Items[4].Id,
                    AmountValue=itemsByAttributeCollection.Items[4].AmountValue,
                    SubscriptionId = itemsByAttributeCollection.Items[4].SubscriptionId,
                    Description = itemsByAttributeCollection.Items[4].Description,
                    Name=itemsByAttributeCollection.Items[4].Name,
                    Groups=new List<Group>(),
                    Tags=new List<Tag>(),
                    IsActive=true
                },
                new Item{
                    Id = itemsByAttributeCollection.Items[5].Id,
                    AmountValue=itemsByAttributeCollection.Items[5].AmountValue,
                    SubscriptionId = itemsByAttributeCollection.Items[5].SubscriptionId,
                    Description = itemsByAttributeCollection.Items[5].Description,
                    Name=itemsByAttributeCollection.Items[5].Name,
                    Groups=new List<Group>(),
                    Tags=new List<Tag>(),
                    IsActive=true
                }
            };

            return pageRequestResponse;

        }

        public static RequestResponse<SearchResultDataCollection>  GetSearchResultsPageOne()
        {
            var resultsCollection = GetSearchResultsData();

            var pageRequestResponse = new RequestResponse<SearchResultDataCollection>
            {
                Item = resultsCollection,
                PagedCollection = new PagedCollection { CollectionTotal = 6, PageNumber = 1, PageSize = 3 },
                Success = true
            };

            pageRequestResponse
                .Item.SearchResults = new List<SearchResult>
                {
                    new SearchResult
                    {
                        Id = resultsCollection.SearchResults[0].Id,
                        ResultId = resultsCollection.SearchResults[0].ResultId,
                        Description= resultsCollection.SearchResults[0].Description,
                        Name= resultsCollection.SearchResults[0].Name,
                        ResultType= resultsCollection.SearchResults[0].ResultType,
                        ResultWeight= resultsCollection.SearchResults[0].ResultWeight
                    },
                    new SearchResult
                    {
                        Id = resultsCollection.SearchResults[1].Id,
                        ResultId = resultsCollection.SearchResults[1].ResultId,
                        Description= resultsCollection.SearchResults[1].Description,
                        Name= resultsCollection.SearchResults[1].Name,
                        ResultType= resultsCollection.SearchResults[1].ResultType,
                        ResultWeight= resultsCollection.SearchResults[1].ResultWeight
                    },
                    new SearchResult
                    {
                        Id = resultsCollection.SearchResults[2].Id,
                        ResultId = resultsCollection.SearchResults[2].ResultId,
                        Description= resultsCollection.SearchResults[2].Description,
                        Name= resultsCollection.SearchResults[2].Name,
                        ResultType= resultsCollection.SearchResults[2].ResultType,
                        ResultWeight= resultsCollection.SearchResults[2].ResultWeight
                    }
                };

            return pageRequestResponse;
        }
        public static RequestResponse<SearchResultDataCollection> GetSearchResultsPageTwo()
        {
            var resultsCollection = GetSearchResultsData();

            var pageRequestResponse = new RequestResponse<SearchResultDataCollection>
            {
                Item = resultsCollection,
                PagedCollection = new PagedCollection { CollectionTotal = 6, PageNumber = 1, PageSize = 3 },
                Success = true
            };

            pageRequestResponse
                .Item.SearchResults = new List<SearchResult>
                {
                    new SearchResult
                    {
                        Id = resultsCollection.SearchResults[3].Id,
                        ResultId = resultsCollection.SearchResults[3].ResultId,
                        Description= resultsCollection.SearchResults[3].Description,
                        Name= resultsCollection.SearchResults[3].Name,
                        ResultType= resultsCollection.SearchResults[3].ResultType,
                        ResultWeight= resultsCollection.SearchResults[3].ResultWeight
                    },
                    new SearchResult
                    {
                        Id = resultsCollection.SearchResults[4].Id,
                        ResultId = resultsCollection.SearchResults[4].ResultId,
                        Description= resultsCollection.SearchResults[4].Description,
                        Name= resultsCollection.SearchResults[4].Name,
                        ResultType= resultsCollection.SearchResults[4].ResultType,
                        ResultWeight= resultsCollection.SearchResults[4].ResultWeight
                    },
                    new SearchResult
                    {
                        Id = resultsCollection.SearchResults[5].Id,
                        ResultId = resultsCollection.SearchResults[5].ResultId,
                        Description= resultsCollection.SearchResults[5].Description,
                        Name= resultsCollection.SearchResults[5].Name,
                        ResultType= resultsCollection.SearchResults[5].ResultType,
                        ResultWeight= resultsCollection.SearchResults[5].ResultWeight
                    }
                };

            return pageRequestResponse;
        }

        private static SearchResultDataCollection GetSearchResultsData()
        {
            var results = GetSearchItemResults();

            var itemGroupCollection = new List<ItemGroup> {
                new ItemGroup{Id=9987, IsActive=true, GroupId=900, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12), GroupName="First Group"},
                new ItemGroup{Id=22222, IsActive=true, GroupId=901, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12), GroupName="Second Group"},
                new ItemGroup{Id=23423, IsActive=true, GroupId=902, ItemId=102,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12), GroupName="Third  Group"}
            };

            var itemTagCollection = new List<ItemTag>
            {
                new ItemTag{Id=19987, IsActive=true, TagId=110, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12), TagName="Tag #1"},
                new ItemTag{Id=18887, IsActive=true, TagId=130, ItemId=101,CreateDate=DateTime.Now.AddDays(-22), UpdateDate=DateTime.Now.AddDays(-23), TagName="Tag #2"},
                new ItemTag{Id=112987, IsActive=true, TagId=140, ItemId=101,CreateDate=DateTime.Now.AddDays(-23), UpdateDate=DateTime.Now.AddDays(-26), TagName="Tag #3"}

            };

            return new SearchResultDataCollection
            {
                ItemGroups = itemGroupCollection,
                ItemTags = itemTagCollection,
                SearchResults = results
            };
        }
        private static List<Item> GetUniqueItems()
        {
            return new List<Item>
            {
                new Item{Id=100, AmountValue= 1, CreateDate=DateTime.Now.AddDays(-25), UpdateDate= DateTime.Now.AddDays(-22), Description="Test Item #1", Name="Test Item #1 Name", IsActive=true, SubscriptionId=1},
                new Item{Id=131, AmountValue= 2, CreateDate=DateTime.Now.AddDays(-45), UpdateDate= DateTime.Now.AddDays(-33), Description="Test Item #2", Name="Test Item #2 Name", IsActive=true, SubscriptionId=1},
                new Item{Id=151, AmountValue= 0, CreateDate=DateTime.Now.AddDays(-55), UpdateDate= DateTime.Now.AddDays(-46), Description="Test Item #3", Name="Test Item #3 Name", IsActive=true, SubscriptionId=1},
                new Item{Id=171, AmountValue= 1, CreateDate=DateTime.Now.AddDays(-85), UpdateDate= DateTime.Now.AddDays(-5), Description="Test Item #4", Name="Test Item #4 Name", IsActive=true, SubscriptionId=1},
                new Item{Id=181, AmountValue= 0, CreateDate=DateTime.Now.AddDays(-33), UpdateDate= DateTime.Now.AddDays(-12), Description="Test Item #5", Name="Test Item #5 Name", IsActive=true, SubscriptionId=1},
                new Item{Id=199, AmountValue= 2, CreateDate=DateTime.Now.AddDays(-99), UpdateDate= DateTime.Now.AddDays(-99), Description="Test Item #6", Name="Test Item #6 Name", IsActive=true, SubscriptionId=1}
            };
        }
        private static List<SearchResult> GetSearchItemResults()
        {
            return new List<SearchResult>
            {
                new SearchResult{ Id=1, ResultId=100, Description="Item Apple 1 Description", Name="Futz", ResultType="item", ResultWeight=1},
                new SearchResult{ Id=2, ResultId=101, Description="Item Thing Description", Name="Crap 1", ResultType="item", ResultWeight=1},
                new SearchResult{ Id=3, ResultId=102, Description="Item Blah Blah App Blah Description", Name="Stuff 1", ResultType="item", ResultWeight=1},
                new SearchResult{ Id=4, ResultId=400, Description="", Name="Ap Group 1", ResultType="group", ResultWeight=2},
                new SearchResult{ Id=5, ResultId=500, Description="", Name="Crap Tag 1", ResultType="tag", ResultWeight=3},
                new SearchResult{ Id=6, ResultId=700, Description="", Name="Zap List", ResultType="list", ResultWeight=4},
            };
        }
    }
}

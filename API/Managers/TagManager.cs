using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.API.Models;
using InventoryManagement.API.DataProvider.SQL;
using NLog;

namespace InventoryManagement.API.Managers
{
    public class TagManager : ITagManager
    {
        private readonly ITagRepository _tagRepository;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public TagManager(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<Tag>>> GetTagCollection(int subscriptionId, int pageNumber, int pageSize)
        {
            var tagCollection = new RequestResponse<List<Tag>>();

            try
            {
                tagCollection = await _tagRepository.GetTagCollection(subscriptionId, pageNumber, pageSize).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                tagCollection.Success = false;
            }
            return tagCollection;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<Tag>>> GetTagCollectionWithItemCounts(int subscriptionId, SearchRequest searchRequest)
        {
            var tagCollection = new RequestResponse<List<Tag>>();

            try
            {
                tagCollection = await _tagRepository.GetTagCollectionWithItemCounts(subscriptionId, searchRequest).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                tagCollection.Success = false;
            }
            return tagCollection;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<Tag>>> GetTags(int subscriptionId)
        {
            var tags = new RequestResponse<List<Tag>>();

            try
            {
                tags = await _tagRepository.GetTags(subscriptionId).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                tags.Success = false;
            }

            return tags;
        }

        public async Task<RequestResponse<Tag>> UpsertTag(Tag tag)
        {
            var insertTagId = -1;

            try
            {
                insertTagId = await _tagRepository.UpsertTag(tag).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
            }

            if (insertTagId > -1 && tag.Id < 1)
            {
                tag.Id = insertTagId;
                tag.CreateDate = DateTime.Now;
            }
            tag.UpdateDate = DateTime.Now;
            return new RequestResponse<Tag> { Item = tag, Success = insertTagId > -1 };
        }
    }
}

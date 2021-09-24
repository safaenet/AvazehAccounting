﻿using System.Net.Http;
using System.Threading.Tasks;

namespace AvazehWpfApiClient.DataAccess.Interfaces
{
    public interface IApiProcessor
    {
        Task<T> GetCollectionAsync<T>(string Key, int Page = 1, string SearchText = "", int PageSize = 50) where T : class;
        Task<T> GetItemAsync<T>(string Key, int Id) where T : class;
        Task<U> CreateItemAsync<T, U>(string Key, T model) where U : class;
        Task<U> UpdateItemAsync<T, U>(string Key, int Id, T model) where U : class;
        Task<bool> DeleteItemAsync(string Key, int Id);
    }
}
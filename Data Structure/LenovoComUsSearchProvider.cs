using Data.Entities;
using DataExtraction.Services.HttpRootDomainsProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;

namespace DataExtraction.Services
{
    public class LenovoComUsSearchProvider : ISearchProvider
    {
        private readonly string searchRequest = @"https://openapi.lenovo.com/us/en/ofp/search/global/cache/products/get/_tsc?text=$term&rows=20";
        private bool useUnblocker = true;

        public async Task<List<SearchData>> FetchItems(string term, string rootDomain, int page = 1, string zipCode = null)
        {
            string formattedEndpoint = searchRequest.Replace("$term", WebUtility.UrlEncode(term));

            var response = await AsyncHttpRequest.GetRequestAsync(formattedEndpoint,
                new HttpRequestSettings() { SetFingerprint = false, UseUnblocker = useUnblocker });

            if (!response.IsSuccessStatusCode)
            {
                throw new DataServicesException($"Lenovo search API failed. [rootDomain={rootDomain}, term={term}]",
                     response.HttpStatusCode,
                     response.Exception);
            }

            var responseData = JsonConvert.DeserializeObject<LenovoComUsSearchResponse>(response.Content);
            List<SearchData> searchData = new List<SearchData>();
            var i = 1;
            var skus = responseData?.data?.data;

            if (skus != null && skus.Any())
            {
                foreach (var sku in skus)
                {
                    if (sku?.products == null) continue;

                    foreach (var product in sku.products)
                    {
                        if (product == null) continue;

                        searchData.Add(new SearchData
                        {
                            Rank = i++,
                            Rootdomain = rootDomain,
                            Sku = product.productCode ?? "No SKU",
                            Url = "https://www.lenovo.com/us/en" + product.url??"No Url",
                        });
                    }
                }
            }
            return searchData;
        }

        public async Task<List<SearchData>> FetchItems(string term, string rootDomain)
        {
            return await FetchItems(term, rootDomain, 1);
        }

        public Task<List<SearchData>> FetchItems(string term, string rootDomain, string zipCode)
        {
            throw new NotImplementedException();
        }
    }
}

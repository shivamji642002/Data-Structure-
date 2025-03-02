using Common.Services;
using Data.Entities;
using DataExtraction.Services.CanadianTire;
using DataExtraction.Services.HttpRootDomainsProviders;
using DataExtraction.Services.Takealot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DataExtraction.Services
{
    public class FrigidaireComSearchProvider : ISearchProvider
    {
        private readonly string searchRequest = @"https://apolloapi.electrolux.com/occ/v2/frigidaire/users/anonymous/eluxproducts/search?fields=FULL%2Cproducts(FULL)%2Cfacets&query=$term&pageSize=12&currentPage=$page&sort=approvalStatusSort&searchType=FINISHED_GOODS&lang=en&curr=USD";
        private bool useUnblocker = true;
        private int offset = 15;

        public async Task<List<SearchData>> FetchItems(string term, string rootDomain, int page = 1, string zipCode = null)
        {
            string formattedEndpoint = searchRequest.Replace("$domain", rootDomain)
                                                    .Replace("$term", term)
                                                    .Replace("$offset", offset.ToString())
                                                    .Replace("$page",(page ).ToString());

            var headers = new WebHeaderCollection();
            var response = await AsyncHttpRequest.GetRequestAsync(formattedEndpoint,
                new HttpRequestSettings() { SetFingerprint = false, UseUnblocker = useUnblocker, Headers = headers });

            if (!response.IsSuccessStatusCode)
            {
                throw new DataServicesException($"Frigidaire search API failed retrieving item. [rootDomain={rootDomain}, Search term={term}]",
                     response.HttpStatusCode,
                     response.Exception);
            }

            var responseData = JsonConvert.DeserializeObject<FrigidaireComSearchResponse>(response.Content);

            List<SearchData> searchData = new List<SearchData>();
            var i = 1;
            var skus = responseData?.products; 
            if (skus != null && skus.Any())
            {
                foreach (var sku in skus)
                {
                    searchData.Add(new SearchData
                    {
                        Rank = i++,
                        Rootdomain = rootDomain,
                        Title = sku?.name ?? null,
                        Price = sku?.colorVariants?.FirstOrDefault()?.Price?.value ?? 0,
                        Brand = sku?.brand ?? null,
                        Url = "frigidaire.com" + sku?.url ?? null,
                        Sku = sku?.code,
                        ListPrice = sku?.colorVariants?.FirstOrDefault()?.mapUSD,
                        SellerSku = new SellerSku
                        {
                            SkuEntry = new SkuEntry
                            {
                                RootDomain = rootDomain,
                                ImageUrl = sku?.colorVariants?.FirstOrDefault()?.plpImage ?? "No image",
                                Currency = sku?.colorVariants?.FirstOrDefault()?.currencyIso ?? "USD",
                                Category = sku?.categoryName ?? "Uncategorized",
                                Name = sku?.name?? "No name",                               
                            }
                        }
                    });
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

    
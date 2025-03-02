using Data.Entities;
using DataExtraction.Services.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataExtraction.Services
{
    public class FrigidaireComSearchProvider : ISearchProvider
    {
        private string _frigidaireComSearch = "https://apolloapi.electrolux.com/occ/v2/frigidaire/users/anonymous/eluxproducts/search?fields=FULL%2Cproducts(FULL)%2Cfacets&query=%3Arelevance%3AallCategories%3AM_Accessories_FoodPreservation_ConsumablesandAccessories_WaterFilters%3AisConsumerVisUS%3Atrue&pageSize=18&zipCode=28088&lang=en&curr=USD";
        public async Task<List<SearchData>> FetchItems(string term, string rootDomain, int page, string zipCode = null)
        {
            string url = string.Format(_frigidaireComSearch, page, term);
            var headers = new WebHeaderCollection { { "Content-Type", "application/json" } };

            var response = await AsyncHttpRequest.GetRequestAsync(url, new HttpRequestSettings() { });
            if (!response.IsSuccessStatusCode)
            {
                throw new DataServicesException($"Boutiquerugs.com search API failed searching term. [Rootdomain={rootDomain}, Term={term}]",
                       response.HttpStatusCode,
                       response.Exception);
            }

            var data = JsonConvert.DeserializeObject<BoutiquerugsSearchResponse>(response.Content);
            var results = ExtractSearchItems(data, "frigidaire.com");

            return results;
        }

        public Task<List<SearchData>> FetchItems(string term, string rootDomain, string zipCode = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<SearchData>> FetchItems(string url, string zipCode = null)
        {
            throw new NotImplementedException();
        }

        public List<SearchData> ExtractSearchItems(BoutiquerugsSearchResponse response, string rootDomain)
        {
            var items = new List<SearchData>();
            if (response.results == null || (response?.results?.Count() ?? 0) == 0)
                return items;

            int i = 1;
            foreach (var sku in response.results)
            {
                string mainSku = sku.handle;
                string variant = sku.ss_id;
                string fullSku = VariantSkuUtils.BuildVariantSku(mainSku, variant);
                items.Add(new SearchData
                {
                    Rank = i++,
                    Rootdomain = rootDomain,
                    Sku = fullSku,
                    Title = sku.name,
                    Url = "http://frigidaire.com" + sku.url,
                    Price = float.Parse(sku.price),
                    ListPrice = float.Parse(sku.msrp),
                    SellerSku = new SellerSku
                    {
                        Price = float.Parse(sku.price),
                        Url = "http://frigidaire.com" + sku.url,
                        SkuEntry = new SkuEntry
                        {
                            RootDomain = rootDomain,
                            Sku = fullSku,
                            Name = sku.name,
                            Brand = sku.brand,
                            Url = "http://frigidaire.com" + sku.url,
                            Price = float.Parse(sku.price),
                            AverageCustomerReview = float.Parse(sku.rating),
                            NumberOfCustomerReviews = float.Parse(sku.ratingCount)
                        }
                    }
                });
            }
            return items;
        }
    }
}





///
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
        private readonly string reviewRequest = @"https://api.bazaarvoice.com/data/statistics.json?apiversion=5.4&passkey=74tu5sx5g02dyd24juarrwcsl&stats=Reviews&filter=ContentLocale:en_US&filter=ProductId:fhpc4260ls,fhwc3650rs,fhwc3660ls,fhwc3655ls,fhwc3050rs,fhwc3055ls,fhwc3060ls,ucvh2001as,fcvw3062as,fcvw3662as";
        private bool useUnblocker = true;
        private int offset = 15;

        public async Task<List<SearchData>> FetchItems(string term, string rootDomain, int page = 1, string zipCode = null)
        {
            string formattedEndpoint = searchRequest.Replace("$domain", rootDomain)
                                                    .Replace("$term", term)
                                                    .Replace("$offset", offset.ToString())
                                                    .Replace("$page", (page).ToString());

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
                                Name = sku?.name ?? "No name",
                            }
                        }
                    });
                }
            }
            return searchData;
        }

        public async Task<List<SearchData>> FetchItems2(string term, string rootDomain, int page = 1, string zipCode = null)
        {
            string formattedEndpoint = reviewRequest.Replace("$domain", rootDomain)
                                                    .Replace("$term", term)
                                                    .Replace("$offset", offset.ToString())
                                                    .Replace("$page", (page).ToString());

            var headers = new WebHeaderCollection();
            var response = await AsyncHttpRequest.GetRequestAsync(formattedEndpoint,
                new HttpRequestSettings() { SetFingerprint = false, UseUnblocker = useUnblocker, Headers = headers });

            if (!response.IsSuccessStatusCode)
            {
                throw new DataServicesException($"Frigidaire search API failed retrieving item. [rootDomain={rootDomain}, Search term={term}]",
                     response.HttpStatusCode,
                     response.Exception);
            }

            var responseData = JsonConvert.DeserializeObject<FrigidaireComReviewApiResponse>(response.Content);

            List<SearchData> searchData = new List<SearchData>();
            var i = 1;
            var skus = responseData?.Results;
            if (skus != null && skus.Any())
            {
                foreach (var sku in skus)
                {
                    searchData.Add(new SearchData
                    {
                        Rank = i++,
                        Rootdomain = rootDomain,
                        
                        SellerSku = new SellerSku
                        {
                            SkuEntry = new SkuEntry
                            {
                                RootDomain = rootDomain,
                                NumberOfCustomerRatings = sku?.ProductStatistics?.ReviewStatistics?.AverageOverallRating ?? 0.0,
                                NumberOfCustomerReviews = sku?.ProductStatistics?.ReviewStatistics?.TotalReviewCount ??0.0,

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

        public async Task<List<SearchData>> FetchItems2(string term, string rootDomain)
        {
            return await FetchItems(term, rootDomain, 1);
        }
        public Task<List<SearchData>> FetchItems2(string term, string rootDomain, string zipCode)
        {
            throw new NotImplementedException();
        }
    }
}


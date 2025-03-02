using Common.Services;
using Data.Entities;
using DataExtraction.Services.HttpRootDomainsProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExtraction.Services
{
    public class FrigidaireComApiProvider : ApiProviderBase
    {
        private string productReviewRequestUrl = @"https://api.bazaarvoice.com/data/batch.json?passkey=74tu5sx5g02dyd24juarrwcsl&apiversion=5.5&resource.q0=products&filter.q0=id:eq:$sku&stats.q0=reviews";
        private IRootDomainXPathCrawlerFactory rdr;

        public FrigidaireComApiProvider(IRootDomainXPathCrawlerFactory rdr)
        {
            this.rdr = rdr;
        }
        protected async override Task<ApiResponse> GetSellerSkuHelper(List<string> skus, string rootDomain, ProductInformation productInformation = ProductInformation.ProductSummary, Dictionary<string, string> options = null)
        {
            if (skus == null || skus.Count() == 0)
                return new ApiResponse();

            string sku = skus[0];
            bool useUnblocker = true;

            if (skus.Count > 1)
            {
                throw new DataServicesException($"Frigidaire API failed, received more than a single sku per request. [Skus={String.Join(',', skus)}]",
                    System.Net.HttpStatusCode.BadRequest);
            }
            if (!rdr.TryGetProductPage(rootDomain, sku, out string url))
            {
                throw new DataServicesException($"Internal error - rootDomain not supported. [rootDomain={rootDomain}, Sku ={sku}]",
                        DataServicesHttpStatusCode.InvalidUrl);
            }

            var requestSettings = new HttpRequestSettings { UseUnblocker = useUnblocker,SetFingerprint = true };

            var response = await AsyncHttpRequest.GetRequestAsync(url, requestSettings);
            if (!response.IsSuccessStatusCode)
            {
                throw new DataServicesException($"Can't load webpage. [Sku={sku}]",
                        response.HttpStatusCode,
                        response.Exception);
            }

            var items = HttpDataExtractor.ExtractProductDataItems(rootDomain, response.Content, rdr);


            string urlSku = sku.ToLower().Contains("prod") ? sku.ToLower() : sku;
            string formattedEndpoint = productReviewRequestUrl.Replace("$sku", urlSku);
            var apiResponse = await AsyncHttpRequest.GetRequestAsync(formattedEndpoint,
                new HttpRequestSettings() { SetFingerprint = true, UseUnblocker = useUnblocker });
            if (!apiResponse.IsSuccessStatusCode)
            {
                throw new DataServicesException($"ReviewController.Get failed. [rootDomain={rootDomain}, sku={sku}]",
                    apiResponse.HttpStatusCode,
                    apiResponse.Exception);
            }

            string jsonObject = apiResponse.Content;


            SellerSku sellerSku = new SellerSku { SkuEntry = new SkuEntry { RootDomain = rootDomain } };

            var responseData = JsonConvert.DeserializeObject<FrigidaireComApiStarRatingResponse>(jsonObject);

            
            ShippingOption sp = null;
            var freeship = items.GetValueOrDefault("product.shipping_options", null) as List<Dictionary<string, object>>;

            if (!freeship.IsNullOrEmpty())
            {
                sp = new ShippingOption
                {
                    Cost = freeship.First().GetValueOrDefault("cost", null)?.ToString(),
                    Time = freeship.First().GetValueOrDefault("time", null)?.ToString()
                };
            }

            List<Dictionary<string, object>> attributesArray = (List<Dictionary<string, object>>)items.GetValueOrDefault("product.attributes.information", null);
            Dictionary<string, object> attributes = new Dictionary<string, object>();
            
            if (!attributesArray.IsNullOrEmpty())
            {
                foreach (Dictionary<string, object> attrib in attributesArray)
                {
                    string key = attrib.First().Value.ToString();
                    string value = string.Join(",", (attrib.Last().Value as List<string>) ?? new List<string> { attrib.Last().Value.ToString() });
                    attributes.Add(key, value);
                   
                }
            }

            List<Dictionary<string, object>> variantattributesArray = (List<Dictionary<string, object>>)items.GetValueOrDefault("product.variant_attributes", null);
            Dictionary<string, object> variantAttributes = new Dictionary<string, object>();

            if (!variantattributesArray.IsNullOrEmpty())
            {
                foreach (Dictionary<string, object> attrib in variantattributesArray)
                {
                    string key = attrib.First().Value.ToString();
                    string value = string.Join(",", (attrib.Last().Value as List<string>) ?? new List<string> { attrib.Last().Value.ToString() });
                    variantAttributes.Add(key, value);

                }
            }

            Currency cur = Currency.Unknown;
            string szCur = (string)items.GetValueOrDefault("product.currency", null);
            Enum.TryParse(szCur, out cur);
            ProductAvailability pa = ProductAvailability.InStock;

            string szRating = (string)items.GetValueOrDefault("product.reviews.average", null);
            double? rating = null;
            if (double.TryParse(szRating, CultureInfo.InvariantCulture, out double dRating))
            {
                rating = dRating;
            }
            string szReviews = (string)items.GetValueOrDefault("product.reviews.amount", null);
            int? reviews = null;
            if (int.TryParse(szReviews, out int nReviews))
            {
                reviews = nReviews;
            }
            List<string> productImages = (List<string>)items.GetValueOrDefault("product.productimages", null);
            List<string> category = (List<string>)items.GetValueOrDefault("product.category", null);


            string szList_price = (string)items.GetValueOrDefault("product.list_price", null);
            double? list_price = null;
            if (double.TryParse(szList_price, out double list_price1)) list_price = list_price1;
            var responseRatingData = responseData?.BatchedResults?.q0?.Results?.FirstOrDefault()?.ReviewStatistics;

            var ratingsDict = new Dictionary<string, StarRatingData>();

            if (responseRatingData?.RatingDistribution != null && responseRatingData.RatingDistribution.Any())
            {
                // Calculate total ratings
                int totalRatings = responseRatingData.RatingDistribution.Sum(rd => rd.Count);

                // Build the dictionary
                ratingsDict = responseRatingData.RatingDistribution.ToDictionary(
                    rd => $"{rd.RatingValue} Star",
                    rd => new StarRatingData
                    {
                        Number = rd.Count,
                        Percentage = totalRatings > 0 ? Math.Round((double)rd.Count / totalRatings * 100, 2) : 0
                    });
            }


            var apiFinalResponse = new ApiResponse()
            {
                Items = new Dictionary<string, ApiSkuData>()
            };

            apiFinalResponse.Items.Add(sku, new ApiSkuData(
                title: string.Join(" ", (string)items.GetValueOrDefault("product.name", null), (string)items.GetValueOrDefault("product.subtitle", null)).Trim(),
                price: (string)items.GetValueOrDefault("product.price", null),
                brand: (string)items.GetValueOrDefault("product.brand", null),
                currency: cur,
                sku: sku,
                category: category.IsNullOrEmpty() ? null : category.ToArray(),
                upc: null,
                ean: null,
                starRatingDistribution: ratingsDict,
                mpn: null,
                model: null,
                images: productImages,
                description: (string)items.GetValueOrDefault("product.description", null),
                features: (List<string>)items.GetValueOrDefault("product.featurebullets", null),
                condition: ProductCondition.Unknown,
                variants: (List<string>)items.GetValueOrDefault("product.variants", null),
                quantitySold: null,
                availability: pa,
               // variantAttributes: variantAttributes.IsNullOrEmpty() ? null : variantAttributes,
                attributes: attributes.IsNullOrEmpty() ? null : attributes,
                dealType: null,
                promoText: (string)items.GetValueOrDefault("product.promotext", null),
                listPrice: list_price,
                priceByUnit: null,
                imageCount: productImages.IsNullOrEmpty() ? null : productImages.Count(),
                videoCount: (int)items.GetValueOrDefault("product.videoCount", null),
                numberOfCustomerReviews: reviews,
                averageCustomerReview: rating,
                priceHistory: null,
                shipping_options: sp == null ? null : new List<ShippingOption>() { sp },
                skuOffer: null,
                sellerStats: null,
                buyBoxWinnerHistory: null
            ));

            return apiFinalResponse;
        }

        public override Task<ApiCategoryProductsListResponse> ProductFinder(string rootDomain, long rootCategory, int lowerBound, int upperBound, string categoriesInclude, string policy, int perPage)
        {
            throw new NotImplementedException();
        }


        public override Task<ApiSellersInfoResponse> GetSellerInfo(List<string> id, string rootDomain)
        {
            throw new NotImplementedException();
        }
    }
}


using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataExtraction.Services.ReviewsProviders
{
    public class LenovoComUsReviewsResponce
   

    
    {
        public LenovoComUsReview[] Results { get; set; }

    }

    public class LenovoComUsReview
    {
        public string Title { get; set; }
        public string UserNickname { get; set; }
        public string ReviewText { get; set; }
        public int? Rating { get; set; }
        public string ProductId { get; set; }
        public int Id { get; set; }
        public string OriginalProductName { get; set; }
        public string SubmissionTime { get; set; }
        public string UserLocation { get; set; }
        public string IsSyndicated { get; set; }
        public string[] BadgesOrder { get; set; }
        public Dictionary<string, LenovoComUsBadge> Badges { get; set; }
        public LenovoComUsReviewClientResonses[] ClientResponses { get; set; }
        public string IsRecommended { get; set; }

        public LenovoComUsReviewPhoto[] Photos { get; set; }

        public List<string> ReviewImageUrl => Photos?.Select(p => p.Sizes.normal.Url).ToList();
        public class LenovoComUsReviewPhoto
        {
            public LenovoComUsReviewPhotoSize Sizes { get; set; }
        }
        public class LenovoComUsReviewPhotoSize
        {
            public LenovoComUsReviewPhotoSizeNormal normal { get; set; }
        }
        public class LenovoComUsReviewPhotoSizeNormal
        {
            public string Url { get; set; }
        }

        public class LenovoComUsReviewClientResonses
        {
            public string Response { get; set; }
            //public string Date { get; set; }

        }
        public class LenovoComUsBadge
        {
            public string ContentType { get; set; }
            public string Id { get; set; }    
        }
    }
}





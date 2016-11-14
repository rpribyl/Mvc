using CMS.OnlineForms.Types;

using DancingGoat.Models.Cafes;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Implements an ICafeFeedbackRepository as a Kentico CafeeFeedback form item.
    /// </summary>
    public class KenticoCafeFeedbackRepository : ICafeFeedbackRepository
    {
        /// <summary>
        /// Saves a new form record from the view model data.
        /// </summary>
        /// <param name="message">The "Contact us" form data.</param>
        public void InsertCafeFeedbackFormItem(CafeFeedback feedback)
        {
            var item = new CafeFeedbackItem
            {
                CafeId = feedback.CafeId,
                OverallRating = feedback.OverallRating,
                ReviewText = feedback.ReviewText,
                Email = feedback.Email
            };

            item.Insert();
        }
    }
}

using DancingGoat.Models.Cafes;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Abstraction of a cafe feedback form item.
    /// </summary>
    public interface ICafeFeedbackRepository
    {
        /// <summary>
        /// Saves a new form record from the view model data.
        /// </summary>
        /// <param name="feedback">Individual feedback data</param>
        void InsertCafeFeedbackFormItem(CafeFeedback feedback);
    }
}
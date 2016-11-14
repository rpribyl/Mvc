using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.Base;
using CMS.DocumentEngine.Types;
using CMS.Globalization;
using CMS.Helpers;

using DancingGoat.Infrastructure;
using DancingGoat.Models.Cafes;
using DancingGoat.Models.Contacts;
using DancingGoat.Repositories;

namespace DancingGoat.Controllers
{
    public class CafesController : Controller
    {
        private readonly ICafeRepository mCafeRepository;
        private readonly ICountryRepository mCountryRepository;
        private readonly IOutputCacheDependencies mOutputCacheDependencies;
        private readonly ICafeFeedbackRepository mCafeFeedbackRepository;


        public CafesController(ICafeRepository cafeRepository, ICountryRepository countryRepository, IOutputCacheDependencies outputCacheDependencies, ICafeFeedbackRepository cafeFeedbackRepository)
        {
            mCountryRepository = countryRepository;
            mCafeRepository = cafeRepository;
            mOutputCacheDependencies = outputCacheDependencies;
            mCafeFeedbackRepository = cafeFeedbackRepository;
        }


        // GET: Cafes
        [OutputCache(CacheProfile = "Default", VaryByParam = "none")]
        public ActionResult Index()
        {
            var companyCafes = mCafeRepository.GetCompanyCafes(4);
            var partnerCafes = mCafeRepository.GetPartnerCafes();

            var model = new Models.Cafes.IndexViewModel
            {
                CompanyCafes = GetCompanyCafesModel(companyCafes),
                PartnerCafes = GetPartnerCafesModel(partnerCafes)
            };

            mOutputCacheDependencies.AddDependencyOnPages<Cafe>();
            mOutputCacheDependencies.AddDependencyOnInfoObjects<CountryInfo>();
            mOutputCacheDependencies.AddDependencyOnInfoObjects<StateInfo>();

            return View(model);
        }


        private Dictionary<string, List<ContactModel>> GetPartnerCafesModel(IEnumerable<Cafe> cafes)
        {
            var cityCafes = new Dictionary<string, List<ContactModel>>();

            // Group partner cafes by their location
            foreach (var cafe in cafes)
            {
                var city = cafe.City.ToLowerCSafe();
                var contact = CreateContactModel(cafe);

                if (cityCafes.ContainsKey(city))
                {
                    cityCafes[city].Add(contact);
                }
                else
                {
                    cityCafes.Add(city, new List<ContactModel> {contact});
                }
            }

            return cityCafes;
        }


        private IEnumerable<CafeModel> GetCompanyCafesModel(IEnumerable<Cafe> cafes)
        {
            return cafes.Select(cafe => new CafeModel
            {
                Photo = cafe.Fields.Photo,
                Note = cafe.Fields.AdditionalNotes,
                Contact = CreateContactModel(cafe),
                CafeFeedback = new Models.Cafes.CafeFeedback()
                {
                    CafeId = cafe.CafeID,
                    OverallRating = 10,
                    ReviewText = string.Empty,
                    Email = string.Empty
                }
            });
        }


        private ContactModel CreateContactModel(IContact contact)
        {
            var countryStateName = CountryStateName.Parse(contact.Country);
            var country = mCountryRepository.GetCountry(countryStateName.CountryName);
            var state = mCountryRepository.GetState(countryStateName.StateName);

            var model = new ContactModel(contact)
            {
                CountryCode = country.CountryTwoLetterCode,
                Country = ResHelper.LocalizeString(country.CountryDisplayName)
            };

            if (state != null)
            {
                model.StateCode = state.StateName;
                model.State = ResHelper.LocalizeString(state.StateDisplayName);
            }

            return model;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendFeedback(CafeFeedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }
            else
            {
                try
                {
                    mCafeFeedbackRepository.InsertCafeFeedbackFormItem(feedback);
                }
                catch
                {
                    return View("~/Views/Contacts/Error.cshtml");
                }

                return RedirectToAction("ThankYou", "Contacts");
            }
        }
    }
}
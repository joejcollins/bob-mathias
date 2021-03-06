﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Predictions;

namespace DataAccess
{
    [Table("Months")]
    public class Month
    {
        [Key]
        public virtual Guid Id { get; set; }
        // Foreign key and navigation property back to site
        public virtual Guid SiteId { get; set; }
        [ForeignKey("SiteId")]
        public virtual Site Site { get; set; }
        [Display(Name = "Month")]
        [Column(TypeName = "DateTime2")]
        public virtual DateTime MonthTime { get; set; }

        public int IncomeTotal
        {
            get
            {
                var incomeTotal = this.IncomeAdmissions ?? default(int);
                incomeTotal += this.IncomeRetail ?? default(int);
                incomeTotal += this.IncomeRefreshment ?? default(int);
                incomeTotal += this.IncomeDonation ?? default(int); 
                incomeTotal += this.IncomeAdditionalEvents ?? default(int);
                return incomeTotal;
            }
        }

        #region ATTENTION
        [Display(Name = "Marketing Effort", Description = "Approximately how many person hours were spent on marketing tasks this month?")]
        public virtual int? MarketingEffort { get; set; }
        [Display(Name = "Website Visitors", Description = "How many visitors did your website get this month?")]
        public virtual int? WebsiteVisitors { get; set; }

        [Display(Name = "Regional TV", Description = "Has your site featured on regional TV this month?")]
        public virtual bool RegionalTv { get; set; }
        [Display(Name = "National TV", Description = "Has your site featured on national TV this month?")]
        public virtual bool NationalTv { get; set; }
        [Display(Name = "Overseas TV", Description = "Has your site featured on TV in another country this month?")]
        public virtual bool OverseasTv { get; set; }
        [Display(Name = "Regional Radio", Description = "Has your site featured on a regional radio station this month?")]
        public virtual bool RegionalRadio { get; set; }
        [Display(Name = "National Radio", Description = "Has your site featured on national radio this month?")]
        public virtual bool NationalRadio { get; set; }
        [Display(Name = "Regional Newspaper", Description = "Has your been site featured in a regional newspaper or magazine?")]
        public virtual bool RegionalNewsPaper { get; set; }
        [Display(Name = "National Newspaper", Description = "Has your site featured in a national newspaper or magazine?")]
        public virtual bool NationalNewsPaper { get; set; }

        [Display(Name = "Website Url", Description = "What is your web site address?")]
        public virtual String WebsiteUrl { get; set; }
        public Boolean IsWebsitePresent
        {
            get { return !String.IsNullOrEmpty(WebsiteUrl); }
        }

        [Display(Name = "Facebook Url", Description = "If you have a Facebook page, what it the address?")]
        public virtual String FacebookUrl { get; set; }
        [Display(Name = "Twitter Account", Description = "If you have a Twitter account, what is your account name?")]
        public virtual String TwitterUrl { get; set; }
        [Display(Name = "Youtube Url", Description = "If you have a Youtube page, what it the address?")]
        public virtual String YoutubeUrl { get; set; }
        #endregion

        #region ARRIVING
        [Display(Name = "General Visitors", Description = "How many general visitors came to your site this month?")]
        public virtual int? VisitorsGeneral { get; set; }
        [Display(Name = "Member Visitors", Description = "How many visitors were members/season ticket holders at your site this month?")]
        public virtual int? VisitorsMember { get; set; }
        [Display(Name = "Schools Visitors", Description = "How many school visitors came to your site this month?")]
        public virtual int? VisitorsSchool { get; set; }
        #region Visitor Calculations
        public int VisitorsTotal
        {
            get
            {
                // this.NumberVisitorsAdditional is not included because it wasn't in the modelling
                var visitorsTotal = this.VisitorsGeneral ?? default(int);
                visitorsTotal += this.VisitorsMember ?? default(int);
                visitorsTotal += this.VisitorsSchool ?? default(int);
                return visitorsTotal;
            }
        }

        /// <summary>
        /// Calculations for the total number of visitors require the month
        /// and the site.
        /// </summary>
        private VisitorsTotal _visitorsTotal
        {
            get
            {
                // When the month is being saved the site is null so this calculation
                // throws an exception.
                if (this.Site == null) return new VisitorsTotal();
                var visitorsTotal = new VisitorsTotal
                {
                    MonthNumber = this.MonthTime.Month,
                    IsMuseum = Convert.ToInt32(this.Site.IsMuseum),
                    IsCastle = Convert.ToInt32(this.Site.IsCastle),
                    IsWorldHeritageSite = Convert.ToInt32(this.Site.IsWorldHeritageSite),
                    AreaIndoorSquareMetres = this.Site.AreaIndoorSquareMetres,
                    IsWebsitePresent = Convert.ToInt32(this.IsWebsitePresent),
                    IsRefreshment = Convert.ToInt32(this.IsRefreshment ?? true)
                };
                return visitorsTotal;
            }
        }

        public double VisitorsTotalModel
        {
            get
            {
                var visitorsTotal = this._visitorsTotal;
                return Math.Round(visitorsTotal.Predicted);
            }
        }

        public double VisitorsTotalModelUpper
        {
            get
            {
                var visitorsTotal = this._visitorsTotal;
                return Math.Round(visitorsTotal.PredictedUpper);
            }
        }
        #endregion

        [Display(Name = "Admission Income", Description = "What was the gross income (not profit) from visitor admissions this month? Excluding VAT")]
        public virtual int? IncomeAdmissions { get; set; }
        #region Admission Calculations
        private AdmissionsIncomeTotal _admissionsIncomeTotal
        {
            get
            {
                if (this.Site == null) return new AdmissionsIncomeTotal();
                var admissionsIncomeTotal = new AdmissionsIncomeTotal
                {
                    VisitorsTotal = this.VisitorsTotal,
                    AreaIndoorSquareMetres = this.Site.AreaIndoorSquareMetres,
                    WardDensity = this.Site.SiteCircumstance.WardDensity,
                    WardApproximatedSocialGradeAllCategories =
                        this.Site.SiteCircumstance.WardApproximatedSocialGradeAllCategories,
                    WardApproximatedSocialGradeC2 =
                        this.Site.SiteCircumstance.WardApproximatedSocialGradeC2,
                    MarketingEffort = Convert.ToInt32(this.MarketingEffort ?? 0),
                    IsRefreshment = Convert.ToInt32(this.IsRefreshment ?? true)
                };
                return admissionsIncomeTotal;
            }
        }

        public double IncomeAdmissionsModel
        {
            get
            {
                var admissionsIncomeTotal = this._admissionsIncomeTotal;
                return Math.Round(admissionsIncomeTotal.Predicted);
            }
        }

        public double IncomeAdmissionsModelUpper
        {
            get
            {
                var admissionsIncomeTotal = this._admissionsIncomeTotal;
                return Math.Round(admissionsIncomeTotal.PredictedUpper);
            }
        }
        #endregion

        [Display(Name = "Not with Family", Description = "What percentage of visitors were not with a family? If you don't know please estimate")]
        public virtual int VisitorsPercentNoFamily { get; set; }
        [Display(Name = "In a Family", Description = "What percentage of visitors were with a family? If you don't know please estimate")]
        public virtual int VisitorsPercentFamily { get; set; }

        [Display(Name = "Children", Description = "What percentage of visitors were children under 16 years old? If you don't know please estimate")]
        public virtual int VisitorsPercentChildren { get; set; }
        [Display(Name = "Working Age Adults", Description = "What percentage of visitors were adults of working age? If you don't know please estimate")]
        public virtual int VisitorsPercentAdult { get; set; }
        [Display(Name = "Retired", Description = "What percentage of visitors were over 65, retired or semi-retired? If you don't know please estimate")]
        public virtual int VisitorsPercentRetired { get; set; }

        [Display(Name = "Hours Monday", Description = "How many hours were you open on Mondays?")]
        [Range(minimum: 0, maximum: 24, ErrorMessage = "Enter between 0 and 24 hours")]
        public virtual float HoursMonday { get; set; }
        [Display(Name = "Hours Tuesday", Description = "How many hours were you open on Tuesdays?")]
        [Range(minimum: 0, maximum: 24, ErrorMessage = "Enter between 0 and 24 hours")]
        public virtual float HoursTuesday { get; set; }
        [Display(Name = "Hours Wednesday", Description = "How many hours were you open on Wednesdays?")]
        [Range(minimum: 0, maximum: 24, ErrorMessage = "Enter between 0 and 24 hours")]
        public virtual float HoursWednesday { get; set; }
        [Display(Name = "Hours Thursday", Description = "How many hours were you open on Thursdays?")]
        [Range(minimum: 0, maximum: 24, ErrorMessage = "Enter between 0 and 24 hours")]
        public virtual float HoursThursday { get; set; }
        [Display(Name = "Hours Friday", Description = "How many hours were you open on Fridays?")]
        [Range(minimum: 0, maximum: 24, ErrorMessage = "Enter between 0 and 24 hours")]
        public virtual float HoursFriday { get; set; }
        [Display(Name = "Hours Saturday", Description = "How many hours were you open on Saturdays?")]
        [Range(minimum: 0, maximum: 24, ErrorMessage = "Enter between 0 and 24 hours")]
        public virtual float HoursSaturday { get; set; }
        [Display(Name = "Hours Sunday", Description = "How many hours were you open on Sundays?")]
        [Range(minimum: 0, maximum: 24, ErrorMessage = "Enter between 0 and 24 hours")]
        public virtual float HoursSunday { get; set; }

        #endregion

        #region SHOPPING
        [Display(Name = "Do you have a shop?", Description = "Was there a shop or retail outlet open on your site this month?")]
        public virtual bool? IsRetail { get; set; }
        [Display(Name = "Retail Income", Description = "How much was your retail gross income (not profit) this month? Excluding VAT")]
        public virtual int? IncomeRetail { get; set; }
        [Display(Name = "Shop behind pay barrier?", Description = "Do visitors have to pay to get to the shop?")]
        public virtual bool PayToShop { get; set; }
        [Display(Name = "Shop visible from entrance?", Description = "Can you see the shop before you enter?")]
        public virtual bool ShopVisibleFromEntrance { get; set; }
        [Display(Name = "Exit through the shop?", Description = "Do visitors have leave via the shop?")]
        public virtual bool ExitViaShop { get; set; }
        [Display(Name = "Distance to Shop", Description = "How far is it from the entrance to the shop?")]
        public virtual int? DistanceToShop { get; set; }
        public virtual int DistanceToShopUnits { get; set; }
        [Display(Name = "Area of Shop", Description = "How big is the shop?  In square feet or square metres.")]
        public virtual int? AreaShop { get; set; }
        public virtual int AreaShopUnits { get; set; }
        [Display(Name = "Number of Products", Description = "How many product lines does the shop have?")]
        public virtual int? NumberProducts { get; set; }
        [Display(Name = "Exhibition Related Products", Description = "How many of those product lines are directly related to your exhibitions this month?")]
        public virtual int? PercentageRelatedProducts { get; set; }
        #region Retail Income Calculations

        public double RetailIncomePerVisitor
        {
            get
            {
                // if the retail income is empty assume it is zero
                // cast the values to doubles, otherwise you get integer division
                var incomeRetail = Convert.ToDouble(this.IncomeRetail ?? 0.0);
                var retailIncomePerVisitor = Convert.ToDouble(incomeRetail/this.VisitorsTotal);
                // In case the IncomeRetail or VisitorsTotal are zero (= infinity)
                retailIncomePerVisitor = Double.IsInfinity(retailIncomePerVisitor) ? 0.0 : retailIncomePerVisitor;
                // In case both IncomeRetail and VisitorsTotal are zero (= NAN)
                retailIncomePerVisitor = Double.IsNaN(retailIncomePerVisitor) ? 0.0 : retailIncomePerVisitor;
                return Math.Round(retailIncomePerVisitor, 2);
            }
        }

        // TODO: Move the type conversions into the prediction object.
        private RetailIncomePerVisitor _retailIncomePerVisitor
        {
            get
            {
                if (this.Site == null) return new RetailIncomePerVisitor();
                var retailIncomePerVisitor = new RetailIncomePerVisitor
                {
                    VisitorsTotal = this.VisitorsTotal, 
                    IsPayToEnter = (this.IncomeAdmissions > 0) ? 1: 0, 
                    IsArtsCentre = Convert.ToInt32(this.Site.IsArtsCentre), 
                    IsMuseum = Convert.ToInt32(this.Site.IsMuseum), 
                    IsWebsitePresent = String.IsNullOrWhiteSpace(this.WebsiteUrl) ? 0: 1, 
                    PayToShop = Convert.ToInt32(this.PayToShop), 
                    ShopVisibleFromEntrance = Convert.ToInt32(this.ShopVisibleFromEntrance), 
                    IncomeRefreshment = Convert.ToDouble(this.IncomeRefreshment), 
                    IsAdditionalEvents = Convert.ToInt32(this.IsAdditionalEvents)
                };
                return retailIncomePerVisitor;
            }
        }
        public double RetailIncomePerVisitorModel
        {
            get
            {
                var retailIncomePerVisitor = this._retailIncomePerVisitor;
                return Math.Round(retailIncomePerVisitor.Predicted, 2);
            }
        }

        public double RetailIncomePerVisitorModelUpper
        {
            get
            {
                var retailIncomePerVisitor = this._retailIncomePerVisitor;
                return Math.Round(retailIncomePerVisitor.PredictedUpper, 2);
            }
        }

        #endregion
        #endregion

        #region REFRESHMENT

        [Display(Name = "Refreshments Available?", Description = "Did you provide refreshments for visitors this month?")]
        public bool? IsRefreshment { get; set; }
        [Display(Name = "Refreshments Income", Description = "What was your gross income (not profit) from refreshments sales this month? Excluding VAT")]
        public virtual int? IncomeRefreshment { get; set; }
        [Display(Name = "Behind Pay Barrier?", Description = "Do visitors have to pay to enter before they can get refreshments?")]
        public virtual bool PayToRefreshment { get; set; }
        [Display(Name = "Visible from Entrance?", Description = "Can you see the refreshment service before you enter the site?")]
        public virtual bool RefreshmentVisibleFromEntrance { get; set; }
        [Display(Name = "Distance to Refreshments", Description = "How far is it from the entrance to the refreshment service?")]
        public virtual int? DistanceToRefreshment { get; set; }
        public virtual int DistanceToRefreshmentUnits { get; set; }
        [Display(Name = "Indoor Seats", Description = "How many people can the café or restaurant seat indoors?")]
        public virtual int? NumberSeatsIndoors { get; set; }
        [Display(Name = "Outdoor Seats", Description = "How seats are available outside?")]
        public virtual int? NumberSeatsOutside { get; set; }

        [Display(Name = "Vending Machines", Description = "Are refreshment provided by vending machines?")]
        public virtual bool IsVending { get; set; }
        [Display(Name = "Cafeteria", Description = "Do visitors select and collect their own refreshments?")]
        public virtual bool IsCafeteria { get; set; }
        [Display(Name = "Table Service", Description = "Do you provide a waiter table service?")]
        public virtual bool IsTableService { get; set; }
        [Display(Name = "Outside Hours", Description = "Is your refreshment service open outside normal site hours?")]
        public virtual bool IsRefreshmentOutsideHours { get; set; }

        [Display(Name = "Teas and Coffee", Description = "Are tea and coffee available?")]
        public virtual bool IsTeaAndCoffee { get; set; }
        [Display(Name = "Cakes and Desserts", Description = "Are cakes, desserts and cookies available?")]
        public virtual bool IsCakeAndBiscuit { get; set; }
        [Display(Name = "Light Meals", Description = "Are snacks, sandwiches and light meals available?")]
        public virtual bool IsLightMeals { get; set; }
        [Display(Name = "Full Meals", Description = "Do you provide large or main meals?")]
        public virtual bool IsFullMeal { get; set; }

        [Display(Name = "Organic or Local", Description = "Are any of your refreshments organic or locally sourced?")]
        public virtual bool IsOrganic { get; set; }
        [Display(Name = "Vegetarian or Vegan", Description = "Are vegetarian or vegan options available?")]
        public virtual bool IsVegetarian { get; set; }
        [Display(Name = "Gluten Free", Description = "Are gluten free options available?")]
        public virtual bool IsGlutenFree { get; set; }
        [Display(Name = "Alcohol", Description = "Do you serve alcohol?")]
        public virtual bool IsAlcohol { get; set; }
        [Display(Name = "In House", Description = "Are refreshments provided by in house staff?")]
        public virtual bool IsCateringInHouse { get; set; }
        [Display(Name = "Out sourced", Description = "Are the refreshments out sourced to another organisation?")]
        public virtual bool IsCateringOutSourced { get; set; }
        #region Refreshment Income Calculations

        public double RefreshmentIncomePerVisitor
        {
            get
            {
                // if the refreshment income is empty assume it is zero
                // cast the values to doubles to avoid integer division
                var incomeRefreshment = Convert.ToDouble(this.IncomeRefreshment ?? 0.0);
                var refreshmentIncomePerVisitor = Convert.ToDouble(incomeRefreshment / this.VisitorsTotal);
                // In case the IncomeRetail or VisitorsTotal are zero (= infinity)
                refreshmentIncomePerVisitor = Double.IsInfinity(refreshmentIncomePerVisitor) ? 0.0 : refreshmentIncomePerVisitor;
                // In case both IncomeRetail and VisitorsTotal are zero (= NAN)
                refreshmentIncomePerVisitor = Double.IsNaN(refreshmentIncomePerVisitor) ? 0.0 : refreshmentIncomePerVisitor;
                return Math.Round(refreshmentIncomePerVisitor, 2);
            }
        }

        private RefreshmentIncomePerVisitor _refreshmentIncomePerVisitor
        {
            get
            {
                if (this.Site == null) return new RefreshmentIncomePerVisitor();
                var refreshmentIncomePerVisitor = new RefreshmentIncomePerVisitor
                {
                    IsPark = Convert.ToInt32(this.Site.IsPark), 
                    IsHistoricHouse = Convert.ToInt32(this.Site.IsHistoricHouse), 
                    IsWorldHeritageSite = Convert.ToInt32(this.Site.IsWorldHeritageSite), 
                    GoogleRating = this.Site.SiteCircumstance.GoogleRating ?? 4.5, // Assume 4.5 if not given
                    AuthorityDensity = this.Site.SiteCircumstance.AuthorityDensity, 
                    IncomeRetail = this.IncomeRetail ?? 0, // Assume no income if not given
                    VisitorsTotal = this.VisitorsTotal,
                    IsVending = Convert.ToInt32(this.IsVending), 
                    IsTableService = Convert.ToInt32(this.IsTableService), 
                    IsTeaAndCoffee = Convert.ToInt32(this.IsTeaAndCoffee), 
                    IsCakeAndBiscuit = Convert.ToInt32(this.IsCakeAndBiscuit),
                    IsFullMeal = Convert.ToInt32(this.IsFullMeal), 
                    IsVegetarian = Convert.ToInt32(this.IsVegetarian), 
                    IsAlcohol = Convert.ToInt32(this.IsAlcohol)
                };
                return refreshmentIncomePerVisitor;
            }
        }

        public double RefreshmentIncomePerVisitorModel
        {
            get
            {
                var refreshmentIncomePerVisitor = this._refreshmentIncomePerVisitor;
                return Math.Round(refreshmentIncomePerVisitor.Predicted, 2);
            }
        }

        public double RefreshmentIncomePerVisitorModelUpper
        {
            get
            {
                var refreshmentIncomePerVisitor = this._refreshmentIncomePerVisitor;
                return Math.Round(refreshmentIncomePerVisitor.PredictedUpper, 2);
            }
        }

        #endregion
        #endregion

        #region DONATION
        [Display(Name = "Donation Opportunity?", Description = "Do you provide visitors with opportunites to make donations?")]
        public virtual bool? IsDonationOpportunity { get; set; }

        [Display(Name = "Donation Income", Description = "What was your gross income from donations for this month? Excluding Gift Aid")]
        public virtual int? IncomeDonation { get; set; }
        [Display(Name = "Behind Pay Barrier?", Description = "Do visitors have to pay to enter before they can make a donation?")]
        public virtual bool PayToDonate { get; set; }
        [Display(Name = "Visible from Entrance?", Description = "Can you see the donation opportunity before you enter the site?")]
        public virtual bool DonationVisibleFromEntrance { get; set; }
        [Display(Name = "Distance to Donation", Description = "How far is it from the entrance to the first opportunity to make a donation?")]
        public virtual int? DistanceToDonation { get; set; }
        public virtual int DistanceToDonationUnits { get; set; }
        [Display(Name = "Number of Donation Boxes", Description = "How many donation opportunites does your site offer?")]
        public virtual int? NumberDonationOpportunities { get; set; }
        #endregion
        
        #region EXPERIENCE
        [Display(Name = "Events?", Description = "Did you have any special events this month? e.g. after hours tours, venue hire, weddings etc...")]
        public virtual bool? IsAdditionalEvents { get; set; }

        [Display(Name = "Events", Description = "How many events did you run at your site this month?")]
        public virtual int? NumberAdditionalEvents { get; set; }
        [Display(Name = "Events Numbers", Description = "How many visitors took part in events at your site this month?")]
        public virtual int? NumberVisitorsAdditional { get; set; }
        [Display(Name = "Events Income", Description = "What was the income from events this month?")]
        public virtual int? IncomeAdditionalEvents { get; set; }

        [Display(Name = "Number of Artefacts", Description = "Approximately how many artefacts are in your collections?")]
        public virtual int? NumberArtefacts { get; set; }
        [Display(Name = "Artefacts on Display", Description = "Approximately what percentage of the artefacts are on display?")]
        public virtual int? ArtefactsDisplay { get; set; }
        [Display(Name = "Designated Collections", Description = "Do you have any collections designated 'Outstanding' by the Arts Council?")]
        public virtual bool IsCollectionsOutstanding { get; set; }
        [Display(Name = "Describe your Programme", Description = "On this scale from niche interest to mass appeal?")]
        public virtual int ProgrammeMassAppeal { get; set; }
        #endregion

        /// <summary>
        /// Copy the stuff that probably hasn't changed.
        /// </summary>
        /// <returns></returns>
        public Month ShallowCopy()
        {
            var newMonth = new Month()
            {
                // Attention
                WebsiteUrl = this.WebsiteUrl,
                FacebookUrl = this.FacebookUrl,
                TwitterUrl = this.TwitterUrl,
                YoutubeUrl = this.YoutubeUrl,
                // Arriving
                VisitorsPercentNoFamily = this.VisitorsPercentNoFamily,
                VisitorsPercentFamily = this.VisitorsPercentFamily,
                VisitorsPercentChildren = this.VisitorsPercentChildren,
                VisitorsPercentAdult = this.VisitorsPercentAdult,
                VisitorsPercentRetired = this.VisitorsPercentRetired,
                HoursMonday = this.HoursMonday,
                HoursTuesday = this.HoursTuesday,
                HoursWednesday = this.HoursWednesday,
                HoursThursday = this.HoursThursday,
                HoursFriday = this.HoursFriday,
                HoursSaturday = this.HoursSaturday,
                HoursSunday = this.HoursSunday,  
                // Shop
                IsRetail = this.IsRetail,
                PayToShop = this.PayToShop,
                ShopVisibleFromEntrance = this.ShopVisibleFromEntrance,
                ExitViaShop = this.ExitViaShop,
                DistanceToShop = this.DistanceToShop,
                DistanceToShopUnits = this.DistanceToShopUnits,
                AreaShop = this.AreaShop,
                AreaShopUnits = this.AreaShopUnits,
                NumberProducts = this.NumberProducts,
                PercentageRelatedProducts = this.PercentageRelatedProducts,
                // Refreshment
                IsRefreshment = this.IsRefreshment,
                PayToRefreshment = this.PayToRefreshment,
                RefreshmentVisibleFromEntrance = this.RefreshmentVisibleFromEntrance,
                DistanceToRefreshment = this.DistanceToRefreshment,
                DistanceToRefreshmentUnits = this.DistanceToRefreshmentUnits,
                NumberSeatsIndoors = this.NumberSeatsIndoors,
                NumberSeatsOutside = this.NumberSeatsOutside,
                IsVending = this.IsVending,
                IsCafeteria = this.IsCafeteria,
                IsTableService = this.IsTableService,
                IsRefreshmentOutsideHours = this.IsRefreshmentOutsideHours,
                IsTeaAndCoffee = this.IsTeaAndCoffee,
                IsCakeAndBiscuit  = this.IsCakeAndBiscuit,
                IsLightMeals = this.IsLightMeals,
                IsFullMeal = this.IsFullMeal,
                IsOrganic = this.IsOrganic,
                IsVegetarian = this.IsVegetarian,
                IsGlutenFree = this.IsGlutenFree,
                IsAlcohol = this.IsAlcohol,
                IsCateringInHouse = this.IsCateringInHouse,
                IsCateringOutSourced = this.IsCateringOutSourced,
                // Donation 
                IsDonationOpportunity = this.IsDonationOpportunity,
                NumberDonationOpportunities = this.NumberDonationOpportunities,
                DonationVisibleFromEntrance = this.DonationVisibleFromEntrance,
                PayToDonate = this.PayToDonate,
                DistanceToDonation = this.DistanceToDonation,
                DistanceToDonationUnits = this.DistanceToDonationUnits,
                
                // Experience    
                IsAdditionalEvents = this.IsAdditionalEvents,
                NumberArtefacts = this.NumberArtefacts,
                ArtefactsDisplay = this.ArtefactsDisplay,
                IsCollectionsOutstanding = this.IsCollectionsOutstanding,
                ProgrammeMassAppeal = this.ProgrammeMassAppeal
            };
            return newMonth;
        }
    }
}


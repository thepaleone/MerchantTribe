﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MerchantTribeStore.Controllers.Shared;
using MerchantTribeStore.Models;
using MerchantTribe.Commerce;
using MerchantTribe.Commerce.Catalog;
using MerchantTribe.Commerce.Utilities;
using MerchantTribe.Commerce.Content;

namespace MerchantTribeStore.Controllers
{
    public class ProductReviewsController : BaseStoreController
    {
        //
        // GET: /ProductReviews/
        public ActionResult Index(string slug)
        {
            ProductReviewsViewModel model = new ProductReviewsViewModel();

            Product p = GetProductFromSlug(slug);
            if (p == null) return Redirect("~/");

            model.ProductView = new SingleProductViewModel(p, MTApp);
            
            // Titles
            ViewBag.Title = p.MetaTitle;
            if (((string)ViewBag.Title).Trim().Length < 1) ViewBag.Title = p.ProductName;
            ViewBag.Title = "Reviews: " + ViewBag.Title;
            ViewBag.MetaTitle = "Reviews: " + p.MetaTitle;
            ViewBag.MetaDescription = "Reviews: " + p.MetaDescription;
            ViewBag.MetaKeywords = "reviews," + p.MetaKeywords;

            List<ProductReview> reviews = p.ReviewsApproved;
            if (reviews == null) return Redirect("~/");
            if (reviews.Count < 1) return Redirect("~/");
            model.Reviews = reviews;

            // Load ratings buttons
            ThemeManager tm = MTApp.ThemeManager();
            ViewBag.Star0Url = tm.ButtonUrl("Stars0", Request.IsSecureConnection);
            ViewBag.Star1Url = tm.ButtonUrl("Stars1", Request.IsSecureConnection);
            ViewBag.Star2Url = tm.ButtonUrl("Stars2", Request.IsSecureConnection);
            ViewBag.Star3Url = tm.ButtonUrl("Stars3", Request.IsSecureConnection);
            ViewBag.Star4Url = tm.ButtonUrl("Stars4", Request.IsSecureConnection);
            ViewBag.Star5Url = tm.ButtonUrl("Stars5", Request.IsSecureConnection);

            ViewBag.AvgLabel = SiteTerms.GetTerm(SiteTermIds.AverageRating);
            int avg = CalculateAverageRating(reviews);
            ViewBag.Avg = avg;
            ViewBag.AvgImage = tm.ButtonUrl("Stars" + avg.ToString(), Request.IsSecureConnection);

            return View(model);
        }
        private int CalculateAverageRating(List<ProductReview> reviews)
        {
            int AverageRating = 3;
            double tempRating = 3.0;
            double sumRatings = 0.0;
            for (int i = 0; i <= reviews.Count - 1; i++)
            {
                sumRatings += (int)reviews[i].Rating;
            }
            if (sumRatings > 0)
            {
                tempRating = sumRatings / reviews.Count;
                AverageRating = (int)Math.Ceiling(tempRating);
            }


            return AverageRating;
        }
        private Product GetProductFromSlug(string slug)
        {
            Product result = null;
            result = MTApp.CatalogServices.Products.FindBySlug(slug);
            return result;
        }
    }
}

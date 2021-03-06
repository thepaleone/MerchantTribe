﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using MerchantTribe.Commerce;
using MerchantTribe.Commerce.Catalog;

namespace MerchantTribeStore.code.TemplateEngine.TagHandlers
{
    public class Link : ITagHandler
    {

        public string TagName
        {
            get { return "sys:link"; }
        }

        public void Process(List<ITemplateAction> actions, MerchantTribe.Commerce.MerchantTribeApplication app, ITagProvider tagProvider, ParsedTag tag, string innerContents)
        {
            

            string mode = tag.GetSafeAttribute("mode");
            string href = string.Empty;
            string sysid = tag.GetSafeAttribute("sysid");

            switch (mode.Trim().ToLowerInvariant())
            {
                case "home":
                    href = app.CurrentStore.RootUrl();
                    break;
                case "checkout":
                    href = app.CurrentStore.RootUrlSecure() + "checkout";
                    if (innerContents == string.Empty) innerContents = "<span>Checkout</span>";
                    break;
                case "cart":
                    href = app.CurrentStore.RootUrl() + "cart";
                    if (innerContents == string.Empty)
                    {
                        string itemCount = "0";
                        string subTotal = "$0.00";

                        if (SessionManager.CurrentUserHasCart(app.CurrentStore))
                        {
                            itemCount =  SessionManager.GetCookieString(WebAppSettings.CookieNameCartItemCount(app.CurrentStore.Id), app.CurrentStore);
                            subTotal = SessionManager.GetCookieString(WebAppSettings.CookieNameCartSubTotal(app.CurrentStore.Id), app.CurrentStore);
                            if (itemCount.Trim().Length < 1) itemCount = "0";
                            if (subTotal.Trim().Length < 1) subTotal = "$0.00";
                        }

                        innerContents = "<span>View Cart: " + itemCount + " items</span>";

                    }
                    break;
                case "category":
                    Category cat = app.CatalogServices.Categories.Find(sysid);
                    href = app.CurrentStore.RootUrl() + cat.RewriteUrl;
                    break;
                case "product":
                    Product p = app.CatalogServices.Products.Find(sysid);
                    href = app.CurrentStore.RootUrl() + p.UrlSlug;
                    break;
                case "":
                    string temp = tag.GetSafeAttribute("href");
                    if (temp.StartsWith("http://") || temp.StartsWith("https://"))
                    {
                        href = temp;
                    }
                    else
                    {
                        href = app.CurrentStore.RootUrl() + temp.TrimStart('/');
                    }
                    break;
                case "myaccount":
                    href = app.CurrentStore.RootUrlSecure() + "account";
                    if (innerContents == string.Empty) innerContents = "<span>My Account</span>";
                    break;
                case "signin":
                    string currentUserId = app.CurrentCustomerId;
                    if (currentUserId == string.Empty)
                    {
                        href = app.CurrentStore.RootUrlSecure() + "signin";
                        if (innerContents == string.Empty) innerContents = "<span>Sign In</span>";
                    }
                    else
                    {
                        href = app.CurrentStore.RootUrlSecure() + "signout";
                        if (innerContents == string.Empty) innerContents = "<span>Sign Out</span>";
                    }

                    break;
            }

            //if (href.Trim().Length > 0)
            //{
                StringBuilder sb = new StringBuilder();
                sb.Append("<a href=\"" + href + "\"");
                PassAttribute(ref sb, tag, "id");
                PassAttribute(ref sb, tag, "title");
                PassAttribute(ref sb, tag, "style");
                PassAttribute(ref sb, tag, "class");
                PassAttribute(ref sb, tag, "dir");
                PassAttribute(ref sb, tag, "lang");
                PassAttribute(ref sb, tag, "target");
                PassAttribute(ref sb, tag, "rel");
                PassAttribute(ref sb, tag, "media");
                PassAttribute(ref sb, tag, "hreflang");
                PassAttribute(ref sb, tag, "type");
                PassAttribute(ref sb, tag, "name");
                sb.Append(">");
                actions.Add(new Actions.LiteralText(sb.ToString()));

                // Process any inner tags
                Processor proc = new Processor(app, innerContents, tagProvider);
                var subActions = proc.RenderForDisplay();                
                actions.AddRange(subActions);

                actions.Add(new Actions.LiteralText("</a>"));
            //}            
            
        }

        private void PassAttribute(ref StringBuilder sb, ParsedTag tag, string name)
        {
            string value = tag.GetSafeAttribute(name);
            if (value.Length > 0)
            {
                sb.Append(" " + name + "=\"" + value + "\"");
            }
        }
         
    }
}
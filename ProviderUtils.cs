using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using NBrightCore.common;
using NBrightDNN;
using Nevoweb.DNN.NBrightBuy.Components;
using DotNetNuke.Common.Utilities;

namespace NBrightPayBox.DNN.NBrightStore
{
    public class ProviderUtils
    {


        public static String GetTemplateData(String templatename,NBrightInfo pluginInfo)
        {
            var controlMapPath = HttpContext.Current.Server.MapPath("/DesktopModules/NBright/NBrightPayBox");
            var templCtrl = new NBrightCore.TemplateEngine.TemplateGetter(PortalSettings.Current.HomeDirectoryMapPath, controlMapPath, "Themes\\config", "");
            var templ = templCtrl.GetTemplateData(templatename, Utils.GetCurrentCulture());
            var dic = new Dictionary<String, String>();
            foreach (var d in StoreSettings.Current.Settings())
            {
                dic.Add(d.Key, d.Value);
            }
            foreach (var d in pluginInfo.ToDictionary())
            {
                if (dic.ContainsKey(d.Key))
                    dic[d.Key] = d.Value;
                else
                    dic.Add(d.Key, d.Value);
            }
            templ = Utils.ReplaceSettingTokens(templ, dic);
            templ = Utils.ReplaceUrlTokens(templ);
            return templ;
        }

        public static String GetBankRemotePost(OrderData orderData)
        {
            var rPost = new RemotePost();

            var settings = ProviderUtils.GetData(orderData.Lang);

                        var param = new string[3];
            param[0] = "orderid=" + orderData.PurchaseInfo.ItemID.ToString("");
            param[1] = "status=1";
            var pbxeffectue = Globals.NavigateURL(StoreSettings.Current.PaymentTabId, "", param);
            param[0] = "orderid=" + orderData.PurchaseInfo.ItemID.ToString("");
            param[1] = "status=0";
            var pbxrefuse = Globals.NavigateURL(StoreSettings.Current.PaymentTabId, "", param);
            var appliedtotal = orderData.PurchaseInfo.GetXmlPropertyDouble("genxml/appliedtotal").ToString("0.00").Replace(",","").Replace(".",""); ;
            var postUrl = settings.GetXmlProperty("genxml/textbox/mainurl");

            WebRequest request = WebRequest.Create("https://tpeweb.paybox.com/load.html");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response == null || response.StatusCode != HttpStatusCode.OK) postUrl = settings.GetXmlProperty("genxml/textbox/backupurl");                

            if (settings.GetXmlPropertyBool("genxml/checkbox/preproduction"))
            {
                postUrl = settings.GetXmlProperty("genxml/textbox/preprodurl");                
            }

            rPost.Url = postUrl;

            rPost.Add("PBX_SITE", settings.GetXmlProperty("genxml/textbox/pbxsite"));
            rPost.Add("PBX_RANG", settings.GetXmlProperty("genxml/textbox/pbxrang"));
            rPost.Add("PBX_DEVISE", settings.GetXmlProperty("genxml/textbox/pbxdevise"));
            rPost.Add("PBX_TOTAL", appliedtotal);
            rPost.Add("PBX_IDENTIFIANT", settings.GetXmlProperty("genxml/textbox/pbxidentifiant"));
            rPost.Add("PBX_CMD", orderData.PurchaseInfo.ItemID.ToString(""));
            rPost.Add("PBX_PORTEUR", orderData.GetClientEmail());
            rPost.Add("PBX_RETOUR", settings.GetXmlProperty("genxml/textbox/pbxretour"));
            rPost.Add("PBX_EFFECTUE", pbxeffectue);
            rPost.Add("PBX_REFUSE", pbxrefuse);
            rPost.Add("PBX_ANNULE", pbxrefuse);
            rPost.Add("PBX_REPONDRE_A", Utils.ToAbsoluteUrl("/DesktopModules/NBright/NBrightPayBox/notify.ashx"));
            rPost.Add("PBX_HASH", "SHA512");
            rPost.Add("PBX_TIME", DateTime.UtcNow.ToString("o"));

            rPost.Add("PBX_HMAC", rPost.GetHmac(settings.GetXmlProperty("genxml/textbox/hmackey")).ToUpper());
            

            //Build the re-direct html 
            var rtnStr = "";
            rtnStr = rPost.GetPostHtml("/DesktopModules/NBright/NBrightPayBox/Themes/config/img/" + settings.GetXmlProperty("genxml/dropdownlist"));

            if (settings.GetXmlPropertyBool("genxml/checkbox/debugmode"))
            {
                File.WriteAllText(PortalSettings.Current.HomeDirectoryMapPath + "\\debug_NBrightPayBoxpost.html", rtnStr);
            }
            return rtnStr;
        }

        public static NBrightInfo GetData(string lang)
        {
            var objCtrl = new NBrightBuyController();
            var info = objCtrl.GetByGuidKey(PortalSettings.Current.PortalId, -1, "NBrightPayBoxPAYMENT", "NBrightPayBoxpayment");
            if (info == null)
            {
                info = new NBrightInfo(true);
                info.GUIDKey = "NBrightPayBoxpayment";
                info.TypeCode = "NBrightPayBoxPAYMENT";
                info.ModuleId = -1;
                info.PortalId = PortalSettings.Current.PortalId;
                var pid = objCtrl.Update(info);
                info = new NBrightInfo(true);
                info.GUIDKey = "";
                info.TypeCode = "NBrightPayBoxPAYMENTLANG";
                info.ParentItemId = pid;
                info.Lang = lang;
                info.ItemID = objCtrl.Update(info);
            }

            // do edit field data if a itemid has been selected
            var nbi = objCtrl.Get(info.ItemID, "NBrightPayBoxPAYMENTLANG", lang);
            return nbi;
        }

        public static string SaveData(HttpContext context)
        {
            try
            {

                var objCtrl = new NBrightBuyController();

                //get uploaded params
                var ajaxInfo = NBrightBuyUtils.GetAjaxFields(context);
                var lang = NBrightBuyUtils.SetContextLangauge(ajaxInfo); // Ajax breaks context with DNN, so reset the context language to match the client.

                var itemid = ajaxInfo.GetXmlProperty("genxml/hidden/itemid");
                if (Utils.IsNumeric(itemid))
                {
                    var nbi = objCtrl.Get(Convert.ToInt32(itemid));
                    // get data passed back by ajax
                    var strIn = HttpUtility.UrlDecode(Utils.RequestParam(context, "inputxml"));
                    // update record with ajax data
                    nbi.UpdateAjax(strIn);
                    objCtrl.Update(nbi);

                    // do langauge record
                    var nbi2 = objCtrl.GetDataLang(Convert.ToInt32(itemid), lang);
                    nbi2.UpdateAjax(strIn);
                    objCtrl.Update(nbi2);

                    DataCache.ClearCache(); // clear ALL cache.
                }
                return "";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }


    }
}

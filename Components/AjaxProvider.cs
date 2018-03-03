﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Xml;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using NBrightCore.common;
using NBrightDNN;
using NBrightPayBox.DNN.NBrightStore;
using Nevoweb.DNN.NBrightBuy.Components;
using Nevoweb.DNN.NBrightBuy.Components.Products;
using Nevoweb.DNN.NBrightBuy.Components.Interfaces;
using RazorEngine.Compilation.ImpromptuInterface.InvokeExt;

namespace NBrightPayBox
{
    public class AjaxProvider : AjaxInterface
    {
        public override string Ajaxkey { get; set; }

        public override string ProcessCommand(string paramCmd, HttpContext context, string editlang = "")
        {
            var ajaxInfo = NBrightBuyUtils.GetAjaxFields(context);
            var lang = NBrightBuyUtils.SetContextLangauge(ajaxInfo); // Ajax breaks context with DNN, so reset the context language to match the client.
            var objCtrl = new NBrightBuyController();

            var strOut = "PayBox Ajax Error";
            switch (paramCmd)
            {
                case "nbrightpayboxajax_savesettings":
                    strOut = objCtrl.SavePluginSinglePageData(context);
                    break;
                case "nbrightpayboxajax_selectlang":
                    objCtrl.SavePluginSinglePageData(context);
                    var nextlang = ajaxInfo.GetXmlProperty("genxml/hidden/nextlang");
                    var info = objCtrl.GetPluginSinglePageData("NBrightPayBoxpayment", "NBrightPayBoxPAYMENT", nextlang);
                    strOut = NBrightBuyUtils.RazorTemplRender("settingsfields.cshtml", 0, "", info, "/DesktopModules/NBright/NBrightPayBox", "config", nextlang, StoreSettings.Current.Settings());
                    break;                                        
            }

            return strOut;

        }

        public override void Validate()
        {
            throw new NotImplementedException();
        }

    }
}

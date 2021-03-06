
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using NBrightCore.common;
using NBrightCore.render;
using NBrightDNN;

using Nevoweb.DNN.NBrightBuy.Base;
using Nevoweb.DNN.NBrightBuy.Components;
using NBrightPayBox.DNN.NBrightStore;
using DataProvider = DotNetNuke.Data.DataProvider;

namespace NBrightPayBox.DNN.NBrightStore
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ViewNBrightGen class displays the content
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class NBrightPayBoxPayment : NBrightBuyAdminBase
    {

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
                base.OnLoad(e);

                if (Page.IsPostBack == false)
                {
                    PageLoad();
                }
        }

        private void PageLoad()
        {
            if (NBrightBuyUtils.CheckRights())
            {
                var objCtrl = new NBrightBuyController();
                var info = objCtrl.GetPluginSinglePageData("NBrightPayBoxpayment", "NBrightPayBoxPAYMENT", Utils.GetCurrentCulture());
                var strOut = NBrightBuyUtils.RazorTemplRender("settings.cshtml", 0, "", info, ControlPath, "config", Utils.GetCurrentCulture(), StoreSettings.Current.Settings());
                var l = new Literal();
                l.Text = strOut;
                Controls.Add(l);
            }
        }

        #endregion


    }

}

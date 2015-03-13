﻿using System;
using System.Runtime.Remoting.Contexts;
using System.Web;
using NBrightCore.common;
using Nevoweb.DNN.NBrightBuy.Components;

namespace NBrightPayBox.DNN.NBrightStore
{
    /// <summary>
    /// Summary description for XMLconnector
    /// </summary>
    public class NBrightPayBoxNotify : IHttpHandler
    {
        private String _lang = "";




        /// <summary>
        /// This function needs to process and returned message from the bank.
        /// Thsi processing may vary widely between banks.
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            var modCtrl = new NBrightBuyController();
            var info = ProviderUtils.GetProviderSettings("NBrightPayBoxpayment");

            try
            {
                var debugMode = info.GetXmlPropertyBool("genxml/checkbox/debugmode");

                var debugMsg = "START CALL" + DateTime.Now.ToString("s") + " </br>";
                debugMsg += "returnmessage: " + context.Request.Form.Get("returnmessage") + "</br>";
                if (debugMode)
                {
                    info.SetXmlProperty("genxml/debugmsg", debugMsg);
                    modCtrl.Update(info);
                }

                debugMsg = "NBrightPayBox DEBUG: " + DateTime.Now.ToString("s") + " </br>";


                var rtnMsg = "version=2" + Environment.NewLine + "cdr=1";

                // ------------------------------------------------------------------------
                // In this case the payment provider passes back data via form POST.
                // Get the data we need.
                string returnmessage = "";
                int NBrightPayBoxStoreOrderID = 0;
                string NBrightPayBoxCartID = "";
                string NBrightPayBoxClientLang = "";

                if ((context.Request.Form.Get("returnmessage") != null))
                {
                    returnmessage = context.Request.Form.Get("returnmessage");

                    var orderid = Utils.RequestQueryStringParam(context, "ref");

                    if (Utils.IsNumeric(orderid))
                    {
                        var authcode = Utils.RequestQueryStringParam(context, "auto");
                        var errcode = Utils.RequestQueryStringParam(context, "rtnerr");

                        NBrightPayBoxStoreOrderID = Convert.ToInt32(orderid);
                        // ------------------------------------------------------------------------

                        debugMsg += "OrderId: " + orderid + " </br>";
                        if (debugMode)
                        {
                            info.SetXmlProperty("genxml/debugmsg", debugMsg);
                            modCtrl.Update(info);
                        }

                        var orderData = new OrderData(NBrightPayBoxStoreOrderID);


                        if (authcode == "")
                            rtnMsg = "KO";
                        else
                            rtnMsg = "OK";

                        debugMsg += "authcode: " + authcode + " </br>";
                        if (debugMode)
                        {
                            info.SetXmlProperty("genxml/debugmsg", debugMsg);
                            modCtrl.Update(info);
                        }

                        if (authcode == "")
                        {
                            orderData.PaymentFail();
                        }
                        else
                        {
                            if (errcode == "00000")
                            {
                                orderData.PaymentOk();
                            }
                            else if (errcode == "99999")
                            {
                                orderData.PaymentOk("050");
                            }
                            else
                            {
                                orderData.PaymentFail();
                            }
                        }
                    }
                }
                if (debugMode)
                {
                    debugMsg += "Return Message: " + rtnMsg;
                    info.SetXmlProperty("genxml/debugmsg", debugMsg);
                    modCtrl.Update(info);
                }


                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Write(rtnMsg);
                HttpContext.Current.Response.ContentType = "text/plain";
                HttpContext.Current.Response.CacheControl = "no-cache";
                HttpContext.Current.Response.Expires = -1;
                HttpContext.Current.Response.End();

            }
            catch (Exception ex)
            {
                if (!ex.ToString().StartsWith("System.Threading.ThreadAbortException"))  // we expect a thread abort from the End response.
                {
                    info.SetXmlProperty("genxml/debugmsg", "NBrightPayBox ERROR: " + ex.ToString());
                    modCtrl.Update(info);
                }
            }


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


    }
}
NBrightPayBox
-------------

Upgrade to v1.1 - In v1.1 backward compatiblity with the previous version DB has been broken.  

Installing the new module will try and fix this in the DB, but you might need to delete the current DB records and recreate.

delete [NBrightBuy] where typecode like '%paybox%'


You will need to ensure the ajax provider is registered in the plugins.  Move the "pluginPayBoxAjax.xml" to the NBrughtBuy/plugin folder.


NBrightPayBox
-------------

Upgrade to v1.1 - v1.1 is NOT backward compatiblity with the previous versions, the DB has been altered.  

Installing the new module will try and fix this in the DB, but you might need to delete the current DB records and recreate.

delete [NBrightBuy] where typecode like '%paybox%'



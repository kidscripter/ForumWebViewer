# ForumWebViewer

![Screenshot](https://github.com/kidscripter/ForumWebViewer/blob/main/Screenshot.PNG?raw=true)

Forum Web Viewer is a web based viewer that uses the Zeiss Forum database and image repository.

To get started you will need to use Visual Studio and edit the following in the web.config

```
<appSettings>
  If you want to use Azure authentication you can create an app registration and fill in the ClientID, Tenant, and redirectUri.  If you do not want to use that then you need to remove [Authorize] from each ActionResult in the Home     
  controller.
<connectionStrings>
  Edit the connectionString with your forum database information.
```

Add a read only user to the Forum database: https://stackoverflow.com/questions/1708409/how-to-start-mysql-with-skip-grant-tables

Tested with Forum 4.2 running on IIS.

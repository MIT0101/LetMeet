# LetMeet
#Deployment and defult Values

1-Attach (LetMeetDb,LetMeetIdentityDb) Databases to SQL Server form DefultDb Folder , folow this link to learn how to attach database
https://www.youtube.com/watch?v=s7m0hDeLonM 

** if you can't attach db you can apply sql scripte to genrate tables and defult db values from SqlScripts folder inside DfultDb folder**


2-depoly app to iis on windows be sure you install .net 6 at least on machine, folow this link
https://www.compilemode.com/2021/04/how-to-host-asp-net-mvc-web-app-on-local-iis.html

**If you have an error with access tha data base you have to add "IIS APPPOOL\LetMeet" 
to sql server security login and give it premission to readDb and writeDb to (LetMeetDb,LetMeetIdentityDb)**


3-defualt sigin in info 
Email Address:default@user.com
Password :nfSOqBFkSzr43#yU
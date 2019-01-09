#Equitable Ping Pong#
Thanks for the opportunity  to participate in your test. The specification was laid out short and sweet - no problems understanding it.

##Implementation##

### Back End ###
I used Visual Studio 2015, and selected Visual C# / Web / ASP.Net Web Application / Empty (with WebAPI checked).

To this point I haven't worked with Code First Entity Framework (or any other Entity Framework).  I have my own working routines (in use for about 13 years now on various production systems) and have just never spent the time to learn these frameworks.  The key routines are in DBLib/Utilility.cs

### Front End ###
I would have preferred to use Angular (version 7) but the October update seems to have created problems with rxjs(6.3.3) and @Angular/Materials and my installation wasn't working properly.  I did a bit of Googling on that one, but couldn't find any effective solutions, so I dropped back to Angularjs (1.6) and went with that.

### Database ###
I have SQLExpress (2014) installed and used that.  The actual table and stored procedures are trivial.  Rather than cluttering up the repository with a SQL database project, I have included the scripts in the DBSql directory. Just create the pingpong database, add the pingpong1 login/user and run the scripts.  And, of course, update the connection string at the top of web.config.

### Unit Tests ###
As mentioned in my resume, I have **NO** experience with unit testing and have not included any in this project.  Both Angular and Visual Studio are now including them to some degree, but I've been waiting for the environment to settle down before wading into that.

## General Comments ##
The UI is fairly clean but quite simple.  Generally, my background is making a website work and I leave it up to others to make it pretty. I did throw in the "current date/time" at the top of the pages as I had it laying around.

I have set up the program structure to support a larger project, even though the number of files is small.  I've learned the hard way that no project stays small, so you better structure things go big from the beginning.

My first thought was to go with a home page list with an edit dialog, but you specified two pages so I set it up with routing to handle that.

This is what I came up with "on the fly".  One of my particular strengths is to be able to take existing code, understand it and modify it in the same style as it was originally written in.  It is important for code to adhere to a common style throughout a project. (Neatness counts. I have found that tidy, well laid out code tends to be easier to understand - and its much more likely to actually work.)

Thanks again.

**Alan G. Stewart**


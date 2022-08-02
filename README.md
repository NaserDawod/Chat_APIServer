# Chat_APIServer

Server for a chat app, the server is built with C# in ASP.Net core MVC shuch that the design of the web page is in the View, the logic in the Controller and data in the Model, also the server dose implemnt a API service that the view can use to get and put data in the database, the implemntaion of the API it self is via a built in Service and for the DataBase im using Microsoft EntityFramework </br>

I built this server with both my partners Hiba and Nsr for a group project for my algorithmic programing class

the web site offers the ability to create accounts and add different contacts(even contacts from different servers with the same API) and most important Real Time messages! (thanks to SignalR).</br>
and to mention that we also built a android app that uses the same server</br>

running the server is simple:</br>
&emsp;Linux:</br>
&emsp;&emsp;1- download or clone the repository</br>
&emsp;&emsp;2- opne the terminal from the project folder</br>
&emsp;&emsp;3- type: dotnet ef update database</br>
&emsp;&emsp;4- type: dotnet run</br>
&emsp;Windows:</br>
&emsp;&emsp;1- download or clone the repository</br>
&emsp;&emsp;2- opne the cmd from the project folder</br>
&emsp;&emsp;3- type: Update-DataBase</br>
&emsp;&emsp;4- type: dotnet run</br>
here is a short video of how to run the server and creat a new account: https://mega.nz/file/y1tnmaIa#KhXe4UQlz7Ao3k0balvJxFvFROD6CdZ_Rzm3JMQdd4w </br>
the server localhost is : localhost:7000

now after you run the app you can creat an account via the register page then login and start using the app as normal you can add contacts via the add button next to tha name, edit the contact, delete him and you can send, edit, delete messages a huge part was to check the api but since you didnt specify the way that you will check it here are tow ways that you can check that it is working

1- you can use the cilent side site after loging into your account the app supports all the abilities working with contacts messages and real time messages to different servers

here is an example of using the cient side site https://mega.nz/file/ftVmSbAQ#y61nbn_SVls-ohzJcG_SitAW95Yl81SGJwHHW13_CXw

2- you can use any api service that are online to check the server and since you need to authorize the user that is logged in we have put the token that was created with the jwt on the url after your loging in so you can take in then use it as you want here is an example of using the api https://mega.nz/file/2tVWkZII#Hsgjz9Y2K-aKoi2C47J9auEvpZ1YohQZDVRXaX-WjlM

Note: in the previos assignment we were 2 in the group and we didnt use react so we had to move our frontend from the previos assignment and use it here

code structure: first is the signin were we have a signcontroller wich use the jwt authintication method after loging in you will be moved to the chat section along with the authorize token from the jwt the chat page is controlled by the chat controller and the contacts api wich in turn use the UserService to read and wtite data from the site and we use the SignalR method for sending notifications.

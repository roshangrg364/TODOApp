# TODOApp (Admin Roshan Gurung)
## OverView
- Todo App is a app that allows user to register and keep record of their daily task in a organized way
- User can share their todo with other and collaborate with them.
- User also can set remainder to notify them about updates on todo

## Features
- Ability to create account easily through create acccount in login page.
- Permission Based Authorization. Superadmin can create user and assign them roles to manage their user.
- Todo Creation, Report of Active, Completed and Shared Todo.
- Ability to share todo with other users.
- Real Time Dahboard update and notification on sharing todo.
- Ability to comment on todo to create a conversation between shared user
- History of todo workflow.

## System Requirements
- visual studio 2022
- dot net core 7
- SQl Server 2017 and above

## Architecture, pattern ,technologies used
- App is made using clean architecture .
- Repository pattern and UnitOfWork Pattern is used.
- SignalR used for real time notification and dashboard update
- IhostedService used for cron job to notify reminder
## System Installation
- Clone the Repository 
- Setup Database Connection In appsetting.json file as per your system ("Server=.;Database=TODO;Enlist=false;Encrypt=False;Trusted_Connection=True;")
- Run the system. After Build Database with the provided Database name (TODO as per above connectionstring) will be created and a default superadmin user will be created (Credential of superadmin :   Username= admin@gmail.com  Password = admin)
-  User can create their own user with create account option in login.
-  On Login from admin user. Admin Will have access to User and Role Management.
-  Admin can create User and Roles , lock and unlock user, assign roles to user, assign permission to user.

  ## Todo AppGuide
   ### Dashboard Section
   - Dashboard contain count for Total User(only seen to superadmin), Total Todos, Total Active and Completed Todos, Priority Wise Todo.
   - On clicking the link it redirects to respective sections
   - Dashboard also have two chart - One provide monthly graph of todo and other is the pie chart representation.
   - On the top right corner. it contains the option to logout, edit user profile and change password of current user.

  ### User and Role Management Section
  - This section is accessible to only superadmin and those having role that have permission to view role and user.
  - Under User Section, is the list of user and functionality to create, update, lock and unlock user.
  - Under Role Section, is the list of role and functionality to create, edit and assign permission to role

  ### TODO Section
  - This is the main application section where all the todo related works can be done
  - Todo list menu show list of all todo of user (Exception: Superadmin can view Todos of all users) shown on the basis of priority level
  - User can create new todo using add button in todolist
  - There are two actions on todo List page - View And Edit
  - Edit Button is visible on to the creator of todo.
  - View Button provides overall detail and history of todo
  - on top right part of VIew Page ther is Set reminder button which is visible only to the creator or any one whom the todo is shared with.
  - on the bottom there are 4 button - Close todo, comment, Share and delete
  - Close button and comment button is eligble to all the user whom the tod is shared
  - share and delete button is eligble only to the creator of todo.
   #### How to share todo?
   - todo can be shared only by creator of todo to multiple user.
   - on clicking share todo .. user is redirected to view page where they can input the email of user and comment.
   - if user of that email exist todo will be shared to that user and a realtime notification is seen by the user whom the todo is shared along with the change in dashboard data.
   - NOTE: Email is used to share the todo as listing all users in this system was not viable as it do not contains feature to manage groups
   #### How to Delete Todo?
   - On View Page of todo delete button is eligible to the creator of todo.
   - On delete all the data related to the todo will be deleted.


 #### How to Comment on todo?
  - User whom the todo is shared and the creator are accessible to comment on todo.
  - User can place a comment and click on comment  todo  button to comment on the todo
  - After comment all the  user linked with the todo will get live notification and comment will be updated automatically to their todo page.

  #### How to complete todo?
  - User whom the todo is shared and the creator are accessible to close todo.
  - User can place a comment and click on close todo to complete the todo
  - - After closing todo all the user linked with the todo will get live notification and comment will be updated automatically to their todo page.


  #### How to Set Reminder ?
  - Creator of todo and user whom the the todo is can set reminder for todo .
  - Once reminder is set the particular user will be notified by a popup message at the time set as reminder



# Have Fun !!
  
    
  


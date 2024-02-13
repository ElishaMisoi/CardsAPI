
## Cards RESTful Challenge

### Description

Assume an application named Cards that allows users to create and manage tasks in the form of cards:

- Application users are identified uniquely by their mail address, have a role (Member or Admin) and use a password to authenticate themselves before accessing cards 
    * Members have access to cards they created 
    * Admins have access to all cards

- A user creates a card by providing a name for it and, optionally, a description and a color 
    * Name is mandatory 
    * Color, if provided, should conform to a “6 alphanumeric characters prefixed with a #“ format 
    * Upon creation, the status of a card is To Do

- A user can search through cards they have access to 
    * Filters include name, color, status and date of creation 
    * Optionally limit results using page & size or offset & limit options 
    * Results may be sorted by name, color, status, date of creation

- A user can request a single card they have access to

- A user can update the name, the description, the color and/or the status of a card they have access to 
    * Contents of the description and color fields can be cleared out 
    * Available statuses are To Do, In Progress and Done

Build a RESTful web service that satisfies the aforementioned requirements using C#, .NET and an SQL database of your choice

### Notes
- Signing users up is out of scope for this assignment, feel free to populate your database with a fixed set of users by hand 
- Regarding authentication, you are encouraged to generate a JSON Web Token to store user info - said token can be included in the “Authorization“ header (as an OAuth 2.0 Bearer Token) in subsequent API calls to identify the calling user 
- Optionally document the REST API using a tool of your choice

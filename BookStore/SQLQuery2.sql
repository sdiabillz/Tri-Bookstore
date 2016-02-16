create table [User](
UserID int not null primary key Identity (1,1),
pword nvarchar(10),
Securitylevel nvarchar(10),--General or Admin
Email nvarchar(50)
);

create table Institution(
Institution_ID int not null primary key Identity (1,1),
Name nvarchar(100)
);



create table Posting(
Book_ID int not NULL , 
UserID int not null , 
Posting_Date datetime,
ExpiryDate datetime,
PostTitle nvarchar(50), 
PostDescription nvarchar(255),
price float, 
img image,
Primary key (Book_ID, UserID),
foreign key (Book_ID) REFERENCES Book(Book_ID), 
foreign key (UserID) REFERENCES [User] (UserID)
);


create table Book(
Book_ID int not null primary key, 
Title nvarchar(50), 
author nvarchar(50),
genre nvarchar(50), 
publisher nvarchar(50)
);

create table OfficialBook (
Book_ID int not null, 
Course_ID nvarchar (50),
foreign key (Book_ID) REFERENCES Book(Book_ID), 
);
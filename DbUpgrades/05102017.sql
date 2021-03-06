----sumanth change----
GO
ALTER procedure [dbo].[InsertUpdateCustomers]  
(  
 @CustId int,  
 @firstName varchar(max),  
 @lastName varchar(max),  
 @Phone1 nvarchar(max),  
 @Phone2 nvarchar(max),  
 @email nvarchar(100),  
 @address1 nvarchar(100),  
 @address2 nvarchar(100),  
 @city nvarchar(50),  
 @state nvarchar(50),  
 @CustomerType nvarchar(100),  
 @CustomerAdded datetime,  
 @CardNumber varchar(50),  
 @Zip varchar(50) ,
 @ExpireDate varchar(64)
)  
as  
Begin  
	IF exists(select CustomerId from IcsCustomers where CustomerID = @CustId)  
	Begin  
		Declare @currentTime datetime;  
		Set @currentTime = (SELECT SWITCHOFFSET(SYSDATETIMEOFFSET(), '+05:30'));  
  
		Update ICSCustomers set FirstName=@firstName, LastName=@lastName, PhoneNumber=@Phone1, Phone2=@Phone2, Email=@email, Address1=@address1, Address2=@address2, City=@city, State=@state, CustomerType=@CustomerType, CustomerAdded=@CustomerAdded, 
		CardNumber=@CardNumber, Notes1='',IsUpdated=0,LastUpdatedOn=@currentTime,zip = @Zip where CustomerId = @CustId;  
  
		IF exists(select CustomerId from DeviceTokens where customerid =@CustId)  
		Begin  
			update DeviceTokens set DeviceToken='',DeviceType=null, email=@email,ActivationCode=null,Verified=0,PrefferredStore=null,DeviceId=null,LastUpdatedOn=@currentTime where CustomerId = @CustId;  
		End  
		Else  
		Begin  
			Insert into Devicetokens select @CustId,'',null,@email,null,0,null,null,@currentTime;  
		End  
		Select 2;  
	End  
	Else  
	Begin  
		Insert into ICSCustomers select @CustId,@firstName,@lastName,@Phone1,@Phone2,@email,@address1,@address2,@city,@state,@CustomerType,@CustomerAdded,@CardNumber,'',0,@currentTime,@Zip,@ExpireDate;  
		If exists(select CustomerId from DeviceTokens where customerid =@CustId)  
		Begin  
			Update DeviceTokens set DeviceToken='',DeviceType=null, email=@email,ActivationCode=null,Verified=0,PrefferredStore=null,DeviceId=null,LastUpdatedOn=@currentTime where CustomerId = @CustId;  
		End  
		Else  
		Begin  
			Insert into Devicetokens select @CustId,'',null,@email,null,0,null,null,@currentTime;  
		End  
		Select 1;  
	End  
End  
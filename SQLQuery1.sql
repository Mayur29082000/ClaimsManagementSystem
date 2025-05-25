
/* ===============================
   SCHEMA + DATA + PROCEDURES SCRIPT
   Simulates a Claims Handling System
   =============================== */

/* ====== CREATE TABLES ====== */


USE [master]
GO

IF (EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = N'ClaimsSystemsDB'OR name = N'ClaimsSystemsDB')))
DROP DATABASE ClaimsSystemsDB
go

create database ClaimsSystemsDB
go

use ClaimsSystemsDB
go

/*
    MSSQL-Compatible Customer Claims Backend Schema & Sample Data
    Converted from MySQL to Microsoft SQL Server syntax
    Includes: Table definitions, constraints, sample inserts, triggers, stored procedures, and comments.
*/

-- Drop tables if they exist (to make script re-runnable)
IF OBJECT_ID('policies', 'U') IS NOT NULL DROP TABLE claim_score_alcon
go

IF OBJECT_ID('claim_history', 'U') IS NOT NULL DROP TABLE claim_history
go

IF OBJECT_ID('policies', 'U') IS NOT NULL DROP TABLE receipts
go

IF OBJECT_ID('policies', 'U') IS NOT NULL DROP TABLE payments
go

IF OBJECT_ID('claims', 'U') IS NOT NULL DROP TABLE claims
go

IF OBJECT_ID('policies', 'U') IS NOT NULL DROP TABLE score_team
go

IF OBJECT_ID('policies', 'U') IS NOT NULL DROP TABLE policies
go

IF OBJECT_ID('customers', 'U') IS NOT NULL DROP TABLE customers
go


/*
Customers table
-- Customer like MRF tyre  make policies from Any Insurance company
--  MRF tyres works with the buyer Bajaj Automobile they sell their tyre to bajaj
-- As they are in continuous business so bajaj not paying the everytime while taking the tyres from mrf
-- if bajaj buyer not paying them and they make fraud with mrf so they loss their money thaswhy they make policies with insurance company

*/
CREATE TABLE customers (
    customer_id VARCHAR(10) PRIMARY KEY,
    name VARCHAR(100),
    email VARCHAR(100),
    phone VARCHAR(15),
    description varchar(100)
)
go

--Create the Auto Increment CustmerId

CREATE OR ALTER FUNCTION dbo.fn_get_next_customer_id()
RETURNS VARCHAR(10)
AS
BEGIN
    DECLARE @next_id VARCHAR(10) 
    DECLARE @max_id INT 

    SELECT @max_id = MAX(CAST(SUBSTRING(customer_id, 2, LEN(customer_id)) AS INT)) 
    FROM customers 

    IF @max_id IS NULL
        SET @next_id = 'C001' 
    ELSE
        IF @max_id + 1 < 1000
            SET @next_id = 'C' + RIGHT('000' + CAST(@max_id + 1 AS VARCHAR), 3) 
        ELSE
             SET @next_id = 'C' + CAST(@max_id + 1 AS VARCHAR)   -- No padding

    RETURN @next_id 
END 
GO



-- Insert sample customers
INSERT INTO customers (customer_id,name, email, phone, description) VALUES
(dbo.fn_get_next_customer_id(),'Bajaj Auto Ltd.', 'claims@bajajauto.com', '02212345678', 'Manufacturer of two-wheelers and three-wheelers in India') 
INSERT INTO customers (customer_id,name, email, phone, description) VALUES
(dbo.fn_get_next_customer_id(),'Tata Steel Ltd.', 'support@tatasteel.com', '03387654321', 'Leading steel manufacturing and engineering company') 
INSERT INTO customers (customer_id,name, email, phone, description) VALUES
(dbo.fn_get_next_customer_id(),'Mahindra Logistics', 'logistics@mahindra.com', '01112349876', 'Provider of supply chain and transportation logistics solutions') 
INSERT INTO customers (customer_id,name, email, phone, description) VALUES
(dbo.fn_get_next_customer_id(),'Godrej Industries', 'support@godrej.com', '04422331155', 'Diversified company in chemicals, consumer products, and real estate') 
INSERT INTO customers (customer_id,name, email, phone, description) VALUES
(dbo.fn_get_next_customer_id(),'Adani Enterprises', 'contact@adani.com', '07933225566', 'Conglomerate in energy, logistics, agribusiness, and infrastructure') 
go


--Insert new customer by using stored procedure
CREATE OR ALTER PROCEDURE usp_AddCustomer
(
    @Name VARCHAR(100),
    @Email VARCHAR(100),
    @Phone VARCHAR(15),
    @Description VARCHAR(255)
    
)
AS
BEGIN
    SET NOCOUNT ON

    BEGIN TRY

         -- Check if claim exists
        IF EXISTS (SELECT 1 FROM customers WHERE name = @Name and email= @Email and phone=@Phone and description =@Description)
            RETURN -1 -- Claim not found

        -- Insert new customer
        INSERT INTO customers (customer_id, name, email, phone, description)
        VALUES (dbo.fn_get_next_customer_id(), @Name, @Email, @Phone, @Description)

      RETURN 1  -- Success
    END TRY
    BEGIN CATCH
        return -99  -- Error code for unexpected exception
    END CATCH
END
GO


select * from customers 
go


/*
Policies table

-- Customer like MRF tyre  make policies from Any Insurance company
--  MRF tyres works with the buyer Bajaj Automobile they sell their tyre to bajaj
-- As they are in continuous business so bajaj not paying the everytime while taking the tyres from mrf
-- if bajaj buyer not paying them and they make fraud with mrf so they loss their money thaswhy they make policies with insurance company

*/

CREATE TABLE policies (
    policy_id  VARCHAR(10) PRIMARY KEY,
    customer_id  VARCHAR(10) FOREIGN KEY REFERENCES customers(customer_id),
    policy_number VARCHAR(50),
    policy_type VARCHAR(50),
    issue_date DATE,
    expiry_date DATE,  -- manually entered
    coverage_amount DECIMAL(12,2),
    is_active BIT       -- will be calculated by trigger
) 
go 

--Create the Auto Increment PolicyId

CREATE OR ALTER FUNCTION dbo.fn_get_next_policy_id()
RETURNS VARCHAR(10)
AS
BEGIN
    DECLARE @next_id VARCHAR(10) 
    DECLARE @max_id INT 

    SELECT @max_id = MAX(CAST(SUBSTRING(policy_id, 2, LEN(policy_id)) AS INT)) 
    FROM policies 

    IF @max_id IS NULL
        SET @next_id = 'P001' 
    ELSE
        IF @max_id + 1 < 1000
            SET @next_id = 'P' + RIGHT('000' + CAST(@max_id + 1 AS VARCHAR), 3) 
        ELSE
             SET @next_id = 'P' + CAST(@max_id + 1 AS VARCHAR)   -- No padding

    RETURN @next_id 
END 
GO


-- is_active column is automatically filled by expry date 
--if expiry date is more than today so is active and set 1 else 0

CREATE TRIGGER trg_set_is_active
ON policies
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE p
    SET is_active = CASE 
        WHEN p.expiry_date >= CAST(GETDATE() AS DATE) THEN 1 
        ELSE 0 
    END
    FROM policies p
    INNER JOIN inserted i ON p.policy_id = i.policy_id 
END
go


-- Insert policy for Bajaj Auto (buyer) purchasing from Tata Steel (supplier)
INSERT INTO policies (policy_id,customer_id, policy_number, policy_type, issue_date, expiry_date, coverage_amount)
VALUES (dbo.fn_get_next_policy_id(),'C001', 'BAJ-TATA-2024-001', 'Commercial Purchase Protection', '2025-01-01', '2026-01-01', 500000.00) 
INSERT INTO policies (policy_id,customer_id, policy_number, policy_type, issue_date, expiry_date, coverage_amount)
VALUES (dbo.fn_get_next_policy_id(),'C002', 'TATA-GODREJ-2024-002', 'Supplier Warranty Cover', '2024-03-01', '2025-03-01', 300000.00) 
INSERT INTO policies (policy_id,customer_id, policy_number, policy_type, issue_date, expiry_date, coverage_amount)
VALUES (dbo.fn_get_next_policy_id(),'C003', 'MAH-TATA-2024-003', 'Fleet Damage Insurance', '2023-09-15', '2028-09-15', 750000.00) 
INSERT INTO policies (policy_id,customer_id, policy_number, policy_type, issue_date, expiry_date, coverage_amount)
VALUES (dbo.fn_get_next_policy_id(),'C004', 'GODREJ-ADANI-2024-004', 'Industrial Loss Protection', '2018-04-01', '2021-04-01', 1000000.00) 
INSERT INTO policies (policy_id,customer_id, policy_number, policy_type, issue_date, expiry_date, coverage_amount)
VALUES (dbo.fn_get_next_policy_id(),'C005', 'ADA-MAH-2024-005', 'Freight Loss Cover', '2023-12-10', '2026-12-10', 450000.00) 
go


--Add Policy by using a stored procedure

CREATE OR ALTER PROCEDURE usp_AddPolicy
(
    @CustomerId VARCHAR(10),
    @PolicyNumber VARCHAR(50),
    @PolicyType VARCHAR(50),
    @IssueDate DATE,
    @ExpiryDate DATE,
    @CoverageAmount DECIMAL(12,2)
    
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        -- Validate Customer Exists
        IF NOT EXISTS (SELECT 1 FROM customers WHERE customer_id = @CustomerId)
        BEGIN
            RETURN -1 -- Customer not found
            
        END

        --Validate Coverage Amount
        IF @CoverageAmount <= 0
        BEGIN
            RETURN -2 -- Invalid amount
          
        END

        -- Validate Expiry after Issue Date
        IF @ExpiryDate < @IssueDate
        BEGIN
            RETURN -3 -- Expiry date before issue date
            
        END

        -- ✅ Insert into policies
        INSERT INTO policies (policy_id,customer_id, policy_number, policy_type, issue_date, expiry_date, coverage_amount)
        VALUES (
            dbo.fn_get_next_policy_id(),
            @CustomerId,
            @PolicyNumber,
            @PolicyType,
            @IssueDate,
            @ExpiryDate,
            @CoverageAmount
            
        )

        RETURN 1 -- Success
    END TRY
    BEGIN CATCH
        RETURN -99 -- Unexpected error
    END CATCH
END
GO




select * from policies 
go


/*
SCORE TEAM TABLE

--if customer raised the claim like they give some extra period to buyer to pay the amount
--after also buyer will not paying amount then customer raised the collection claim 
--collection team like broker it collects the payment from buyer 

*/
CREATE TABLE score_team (
    reviewer_id VARCHAR(10) PRIMARY KEY,
    name VARCHAR(100),
    role VARCHAR(100),
    email VARCHAR(100)
) 
go


--Create the Auto Increment ScoreId
CREATE OR ALTER FUNCTION dbo.fn_get_next_score_id()
RETURNS VARCHAR(10)
AS
BEGIN
    DECLARE @next_id VARCHAR(10) 
    DECLARE @max_id INT 

    SELECT @max_id = MAX(CAST(SUBSTRING(reviewer_id, 3, LEN(reviewer_id)) AS INT)) 
    FROM score_team 

    IF @max_id IS NULL
        SET @next_id = 'SC001' 
    ELSE
        IF @max_id + 1 < 1000
            SET @next_id = 'SC' + RIGHT('000' + CAST(@max_id + 1 AS VARCHAR), 3) 
        ELSE
             SET @next_id = 'SC' + CAST(@max_id + 1 AS VARCHAR)   -- No padding

    RETURN @next_id 
END 
go

-- insert some records into SCORE TEAM
INSERT INTO score_team (reviewer_id,name, role, email) VALUES
(dbo.fn_get_next_score_id(),'John Reviewer', 'Senior Analyst', 'john@review.com') 
INSERT INTO score_team (reviewer_id,name, role, email) VALUES
(dbo.fn_get_next_score_id(),'Jane Smith', 'Risk Manager', 'jane@review.com') 
INSERT INTO score_team (reviewer_id,name, role, email) VALUES
(dbo.fn_get_next_score_id(),'Alice Review', 'Claims Auditor', 'alice@review.com') 
INSERT INTO score_team (reviewer_id,name, role, email) VALUES
(dbo.fn_get_next_score_id(),'Bob Check', 'Fraud Investigator', 'bob@review.com') 
INSERT INTO score_team (reviewer_id,name, role, email) VALUES
(dbo.fn_get_next_score_id(),'Eve Verify', 'Policy Analyst', 'eve@review.com') 
go


select * from score_team 
go




/*
Claims table
-- if customer raised the claim like they give some extra period to buyer to pay the amount
-- after also buyer will not paying amount then customer raised any type of claim 
-- claim has three type Claim means normal claim, Monitor means insurance team just monitor the case not involving actively
-- collection means payment will collected by the broker or score team
-- status is default set to inprogress and if the amount is settled then claims will be closed
-- client directly if collection then add into the claims_reviews

*/
CREATE TABLE claims (
    claim_id VARCHAR(10) PRIMARY KEY,
    policy_id VARCHAR(10) FOREIGN KEY REFERENCES policies(policy_id),
     claim_type VARCHAR(50) CHECK (claim_type IN ('Claims', 'Monitor', 'Collection')),
    comment TEXT,
    amount DECIMAL(12, 2),
    status VARCHAR(30) DEFAULT 'In Processing',
    submitted_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE()
) 
go 

--Create the Auto Increment claim_id
CREATE OR ALTER FUNCTION dbo.fn_get_next_claims_id()
RETURNS VARCHAR(10)
AS
BEGIN
    DECLARE @next_id VARCHAR(10) 
    DECLARE @max_id INT 

    SELECT @max_id = MAX(CAST(SUBSTRING(claim_id, 3, LEN(claim_id)) AS INT)) 
    FROM claims 

    IF @max_id IS NULL
        SET @next_id = 'CL001' 
    ELSE
        IF @max_id + 1 < 1000
            SET @next_id = 'CL' + RIGHT('000' + CAST(@max_id + 1 AS VARCHAR), 3) 
        ELSE
             SET @next_id = 'CL' + CAST(@max_id + 1 AS VARCHAR)   -- No padding

    RETURN @next_id 
END 
go 



/*
claim_score_alcon TABLE
--Automatically inserted the data into table by using trigger trg_handle_collection_claim
-- if the claim is raised then it will assigned with some alcon id and assign with score team member
-- this table is used for assign the score member to the claim having claim type is collection
-- and if not collection then assign the score member as null.
*/


CREATE TABLE claim_score_alcon (
    alcon_id VARCHAR(10) PRIMARY KEY,
    --review_id VARCHAR(10) PRIMARY KEY,
    claim_id VARCHAR(10) FOREIGN KEY REFERENCES claims(claim_id),
    reviewer_id VARCHAR(10) FOREIGN KEY REFERENCES score_team(reviewer_id),
    risk_score INT,
    decision VARCHAR(50),
    comments TEXT,
    reviewed_at DATETIME DEFAULT GETDATE()
) 

go 


--Create the Auto Increment alcon_id
CREATE OR ALTER FUNCTION dbo.fn_get_next_alcon_id()
RETURNS VARCHAR(10)
AS
BEGIN
    DECLARE @next_id VARCHAR(10) 
    DECLARE @max_id INT 

    SELECT @max_id = MAX(CAST(SUBSTRING(alcon_id, 3, LEN(alcon_id)) AS INT)) 
    FROM claim_score_alcon 

    IF @max_id IS NULL
        SET @next_id = 'AL001' 
    ELSE
        IF @max_id + 1 < 1000
            SET @next_id = 'AL' + RIGHT('000' + CAST(@max_id + 1 AS VARCHAR), 3) 
        ELSE
             SET @next_id = 'AL' + CAST(@max_id + 1 AS VARCHAR)   -- No padding

    RETURN @next_id 
END 
go 


--if the claim type from claims table is collection then assign random score member to claim case
-- if not collection then assign the score member as null.
CREATE OR ALTER TRIGGER trg_handle_collection_claim
ON claims
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON 

    DECLARE @claim_id VARCHAR(10), @claim_type VARCHAR(50) 

    SELECT TOP 1 @claim_id = claim_id, @claim_type = claim_type
    FROM inserted 

    -- Check if claim_type is 'Collection'
    IF @claim_type = 'Collection'
    BEGIN
        DECLARE @random_reviewer VARCHAR(10) 

        -- Pick a random reviewer from score_team
        SELECT TOP 1 @random_reviewer = reviewer_id
        FROM score_team
        ORDER BY NEWID() 

        INSERT INTO claim_score_alcon (alcon_id,claim_id, reviewer_id, risk_score, decision, comments)
        VALUES (
            dbo.fn_get_next_alcon_id(),
            @claim_id, 
            @random_reviewer, 
            FLOOR(RAND() * 100), 
            'Pending', 
            'Auto-assigned for Collection'
        ) 
    END
    ELSE
    BEGIN
        -- For other claim types, reviewer is NULL and risk_score is 0
        INSERT INTO claim_score_alcon (alcon_id,claim_id, reviewer_id, risk_score, decision, comments)
        VALUES (
            dbo.fn_get_next_alcon_id(),
            @claim_id,
            NULL,
            0,
            'Auto-Skipped',
            'Claim not of type Collection'
        ) 
    END
END 
go


-- insert some records into claims table
INSERT INTO claims (claim_id,policy_id, claim_type, comment, amount) VALUES
(dbo.fn_get_next_claims_id(),'P001', 'Collection', 'Damaged steel coils from Tata Steel', 1250000.00) 
INSERT INTO claims (claim_id,policy_id, claim_type, comment, amount) VALUES
(dbo.fn_get_next_claims_id(),'P002', 'Claims', 'Warranty claim for defective batch', 450000.00) 
INSERT INTO claims (claim_id,policy_id, claim_type, comment, amount) VALUES
(dbo.fn_get_next_claims_id(),'P003', 'Collection', 'Fleet damage due to loading error', 800000.00) 
INSERT INTO claims (claim_id,policy_id, claim_type, comment, amount, status) VALUES
(dbo.fn_get_next_claims_id(),'P004', 'Monitor', 'Machinery failure under coverage', 620000.00, 'Monitor') 
INSERT INTO claims (claim_id,policy_id, claim_type, comment, amount) VALUES
(dbo.fn_get_next_claims_id(),'P005', 'Claims', 'Freight delayed beyond threshold', 150000.00) 
go

--Manual Inserting the data to raise the claims using the stored procedure
CREATE OR ALTER PROCEDURE usp_AddClaim
(
    @PolicyId VARCHAR(10),
    @ClaimType VARCHAR(50),
    @Comment TEXT,
    @Amount DECIMAL(12, 2)
 
)
AS
BEGIN
    SET NOCOUNT ON

    BEGIN TRY
        -- 1. Check required values
        IF NOT EXISTS (SELECT 1 FROM policies WHERE policy_id = @PolicyId)
        BEGIN
             -- Policy not found
            RETURN -1
        END

        IF @Amount <= 0
        BEGIN
           RETURN -2 -- Invalid amount
            
        END

        IF @ClaimType NOT IN ('Claims', 'Monitor', 'Collection')
        BEGIN
            RETURN -3 -- Invalid claim type
            
        END

       

        -- 3. Insert into claims table
        INSERT INTO claims (claim_id, policy_id, claim_type, comment, amount)
        VALUES (dbo.fn_get_next_claims_id(), @PolicyId, @ClaimType, @Comment, @Amount)

        RETURN 1 -- Success
    END TRY
    BEGIN CATCH
        RETURN -99 -- Unexpected error
    END CATCH
END
GO



SELECT * FROM CLAIMS 
go

--Automatically inserted data 
SELECT * FROM claim_score_alcon 
go



-- Table to record all payments (either buyer-Insurance company-customer or through score team-Insurance company-customer)
CREATE TABLE payments (
    payment_id VARCHAR(10) PRIMARY KEY,
    claim_id VARCHAR(10) FOREIGN KEY REFERENCES claims(claim_id),
    payer_type VARCHAR(20) CHECK (payer_type IN ('Buyer', 'Broker')),
    amount_paid DECIMAL(12,2),
    payment_date DATETIME DEFAULT GETDATE(),
    notes VARCHAR(255)
) 
go



--Create the Auto Increment payment_id
CREATE OR ALTER FUNCTION dbo.fn_get_next_payment_id()
RETURNS VARCHAR(10)
AS
BEGIN

    DECLARE @next_id VARCHAR(10) 
    DECLARE @max_id INT 

    SELECT @max_id = MAX(CAST(SUBSTRING(payment_id, 3, LEN(payment_id)) AS INT)) 
    FROM payments 

    IF @max_id IS NULL
        SET @next_id = 'PA001' 
    ELSE
        IF @max_id + 1 < 1000
            SET @next_id = 'PA' + RIGHT('000' + CAST(@max_id + 1 AS VARCHAR), 3) 
        ELSE
             SET @next_id = 'PA' + CAST(@max_id + 1 AS VARCHAR)   -- No padding
    
    RETURN @next_id 
END 
go 



-- Table to track receipts generated from payments
--for every payment it eill generated the recipt
CREATE TABLE receipts (
    receipt_id VARCHAR(10) PRIMARY KEY,
    payment_id VARCHAR(10) FOREIGN KEY REFERENCES payments(payment_id),
    claim_id VARCHAR(10) FOREIGN KEY REFERENCES claims(claim_id) ,
    amount DECIMAL(12,2),
    remaining_amount DECIMAL(12,2),
    buyer_fees DECIMAL(12,2) DEFAULT 0.00,
    status VARCHAR(20),
    created_at DATETIME DEFAULT GETDATE()
) 
go


--Create the Auto Increment receipt_id
CREATE OR ALTER FUNCTION dbo.fn_get_next_receipt_id()
RETURNS VARCHAR(10)
AS
BEGIN
    DECLARE @next_id VARCHAR(10) 
    DECLARE @max_id INT 

    SELECT @max_id = MAX(CAST(SUBSTRING(receipt_id, 3, LEN(receipt_id)) AS INT)) 
    FROM receipts 

    IF @max_id IS NULL
        SET @next_id = 'RE001' 
    ELSE
        IF @max_id + 1 < 1000
            SET @next_id = 'RE' + RIGHT('000' + CAST(@max_id + 1 AS VARCHAR), 3) 
        ELSE
             SET @next_id = 'RE' + CAST(@max_id + 1 AS VARCHAR)   -- No padding

    RETURN @next_id 
END 
go 


--If the all payment was raised by the customer in the claims table done by the claient by any way.
-- They It will automatially inserted into the claims history 
--if payment complete so insert data

CREATE TABLE claim_history (
    history_id VARCHAR(10) PRIMARY KEY,
    claim_id VARCHAR(10) FOREIGN KEY REFERENCES claims(claim_id),
    status VARCHAR(30) default 'Completed',
    date_of_closed DATETIME DEFAULT GETDATE(),
    comment VARCHAR(255) default 'Payment Done'
) 

GO 


--Create the Auto Increment history_id
CREATE OR ALTER FUNCTION dbo.fn_get_next_history_id()
RETURNS VARCHAR(10)
AS
BEGIN
    DECLARE @next_id VARCHAR(10) 
    DECLARE @max_id INT 

    SELECT @max_id = MAX(CAST(SUBSTRING(history_id, 3, LEN(history_id)) AS INT)) 
    FROM claim_history 

    IF @max_id IS NULL
        SET @next_id = 'CH001' 
    ELSE
        IF @max_id + 1 < 1000
            SET @next_id = 'CH' + RIGHT('000' + CAST(@max_id + 1 AS VARCHAR), 3) 
        ELSE
             SET @next_id = 'CH' + CAST(@max_id + 1 AS VARCHAR)   -- No padding

    RETURN @next_id 
END 
go 

select * from claim_history
go

select * from payments
go

select * from receipts
go

--===================================completed============================================


--drop trigger dbo.trg_create_receipt 

-- Trigger to create receipt on new payment
CREATE or alter TRIGGER trg_create_receipt
ON payments
AFTER INSERT
AS
BEGIN
    
    DECLARE @claim_id VARCHAR(10) , @amount_paid DECIMAL(12,2), @payment_id VARCHAR(10) , 
            @total_claim DECIMAL(12,2), @remaining DECIMAL(12,2), 
            @payer_type VARCHAR(20), @buyer_fees DECIMAL(12,2) 

    SELECT TOP 1 
        @claim_id = claim_id, 
        @amount_paid = amount_paid, 
        @payment_id = payment_id,
        @payer_type = payer_type
    FROM inserted 

  

    SELECT @total_claim = amount FROM claims WHERE claim_id = @claim_id 
     
    SELECT @remaining = @total_claim - SUM(amount_paid) FROM payments WHERE claim_id = @claim_id 
 
    -- Calculate buyer fees only if payer is 'buyer'
    SET @buyer_fees = CASE 
        WHEN @payer_type = 'Broker' THEN ROUND(@amount_paid * 0.02, 2)
        ELSE 0.00
    END 
 
   IF @remaining = 0
    BEGIN
       
        INSERT INTO claim_history (history_id,claim_id)
         VALUES (dbo.fn_get_next_history_id(),@claim_id) 

        update claims set status ='Completed - Closed' where claim_id=@claim_id 
    END 

   
    INSERT INTO receipts (receipt_id,payment_id, claim_id, amount, remaining_amount, status, buyer_fees)
    VALUES (
        dbo.fn_get_next_receipt_id(),
        @payment_id, 
        @claim_id, 
        @amount_paid, 
        @remaining,
        CASE WHEN @remaining <= 0 THEN 'Fully Paid' ELSE 'Partial' END,
        @buyer_fees
    ) 

END 

GO 

-- Stored procedure to insert payment
CREATE OR ALTER PROCEDURE InsertPayment
    @claim_id VARCHAR(10) ,
    @amount_paid DECIMAL(12,2),
    @notes VARCHAR(255)
AS
BEGIN
    DECLARE @new_payment_id VARCHAR(10) = dbo.fn_get_next_payment_id();

    Declare @payer_type VARCHAR(20) 
    Declare @claim_type VARCHAR(20) 

    select  @claim_type = claim_type from claims where claim_id=@claim_id
    
    SET @payer_type = CASE 
        WHEN @claim_type = 'Collection' THEN 'Broker' 
        else 'Buyer' 
    END

    INSERT INTO payments (payment_id, claim_id, payer_type, amount_paid, notes)
    VALUES (@new_payment_id,@claim_id, @payer_type, @amount_paid, @notes) 

    
END 
go


--Partial payment for CL001 claim
EXEC InsertPayment @claim_id = 'CL001', @amount_paid = 300000, @notes = 'first insatllation of payment' 
go



--Partial payment for CL002 claim
EXEC InsertPayment @claim_id ='CL002' , @amount_paid = 150000, @notes = 'first insatllation of payment' 
go

--Partial payment for CL001 claim
EXEC InsertPayment @claim_id ='CL001', @amount_paid = 350000, @notes = 'Second insatllation of payment' 
go



--update claim type from monitor, claim to Collection for given ClaimID
CREATE OR ALTER PROCEDURE usp_UpdateClaimTypeToCollection
(
    @claim_id VARCHAR(10)
)
AS
BEGIN
    BEGIN TRY
        
    
        -- Check if claim exists
        IF NOT EXISTS (SELECT 1 FROM claims WHERE claim_id = @claim_id)
            RETURN -1 -- Claim not found

        -- Check if claim type is already 'Collection'
        IF EXISTS (SELECT 1 FROM claims WHERE claim_id = @claim_id AND claim_type = 'Collection')
            RETURN -2  -- Already of type 'Collection'

        
        -- Update claim type
        UPDATE claims
        SET claim_type = 'Collection',
            updated_at = GETDATE()
        WHERE claim_id = @claim_id;

         DECLARE @random_reviewer VARCHAR(10) 

        -- Pick a random reviewer from score_team
        SELECT TOP 1 @random_reviewer = reviewer_id
        FROM score_team
        ORDER BY NEWID() 

        INSERT INTO claim_score_alcon (alcon_id,claim_id, reviewer_id, risk_score, decision, comments)
        VALUES (
            dbo.fn_get_next_alcon_id(),
            @claim_id, 
            @random_reviewer, 
            FLOOR(RAND() * 100), 
            'Auto-assigned due to type change',
            'Claim type was changed to Collection automatically'
        ) 
        RETURN 1 -- Success
    END TRY
    BEGIN CATCH
        RETURN -99 -- Unexpected error
    END CATCH
END
GO

/*
--To test the procedure usp_UpdateClaimTypeToCollection for change claim type to collection 
DECLARE @result INT;
EXEC @result = usp_UpdateClaimTypeToCollection @claim_id = 'CL002';
PRINT 'Result: ' + CAST(@result AS VARCHAR);
go


--Complete payment for CL002 claim
EXEC InsertPayment @claim_id ='CL002' , @amount_paid = 300000, @notes = 'complete payment' 
go

*/


select * from claim_history
go

select * from payments
go

select * from receipts
go

select * from claims
go

select * from customers
go


--Get all claims assigned to a specific reviewer.

CREATE OR ALTER PROCEDURE usp_GetReviewerClaims
    @alcon_id VARCHAR(10)
AS
BEGIN
    SELECT cr.alcon_id, cr.claim_id, c.comment, c.amount, cr.risk_score, cr.decision, cr.comments, cr.reviewed_at
    FROM claim_score_alcon cr
    INNER JOIN claims c ON cr.claim_id = c.claim_id
    WHERE cr.alcon_id = @alcon_id;
END;
GO


--Function to calculate remaining amount for a claim.

CREATE OR ALTER FUNCTION fn_get_remaining_balance (
    @ClaimId VARCHAR(10)
)
RETURNS DECIMAL(12,2)
AS
BEGIN
    DECLARE @total_claim DECIMAL(12,2);
    DECLARE @total_paid DECIMAL(12,2);

    SELECT @total_claim = amount FROM claims WHERE claim_id = @ClaimId;
    SELECT @total_paid = SUM(amount_paid) FROM payments WHERE claim_id = @ClaimId;

    IF @total_paid IS NULL SET @total_paid = 0;

    RETURN @total_claim - @total_paid;
END;
GO

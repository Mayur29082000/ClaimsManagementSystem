# Claims Management System

Welcome to this demo of my Claims Management System, a robust backend solution built using **C#**, **SQL Server**, and exposed via **Swagger Web API**.

In today’s demo, I’ll walk you through a real-world Claims Management System, designed for scenarios where large companies manage credit-based trade with buyers.

Let’s take an example — MRF Tyres sells products to Bajaj Auto on credit. But if Bajaj fails to pay on time or defaults, MRF risks losing huge amounts.

That’s where insurance policies come in. MRF signs a commercial insurance agreement to cover these trade risks.

In this project, we simulate that entire lifecycle:

* Customers like MRF raise claims if a buyer defaults.
* Claims are categorized as normal, monitor, or collection.
* If the issue escalates, it's assigned to a Score Team or broker, who steps in like a recovery agent.
* Payments made against claims generate receipts.
* Once fully paid, the claim is closed and added to the Claim History — all handled automatically by triggers.

Everything is managed through SQL Server, C# backend, and exposed via Swagger APIs.

So let’s explore how all these layers come together — from database to console and then to web.

## 🎥 Project Overview Video

Watch a detailed walkthrough of the Claims Management System:

[**Watch Project Demo on Google Drive**](https://drive.google.com/file/d/1x2CQcdNsTWFpx8MHgrV9N0OhZaGgAtx3/view?usp=drive_link)

## ✨ System Architecture & Workflow

### 🔹 Step 1: SQL Setup
We begin with the SQL backend.
I’ve created tables for customers, policies, claims, payments, and receipts.
Each claim has a status. When fully paid, it's moved to the claim history table.
That happens automatically through a trigger — no manual effort needed.
I’ve also added stored procedures to insert customers, raise claims, and record payments.
This ensures validation and keeps the database consistent.

### 🔹 Step 2: Data Access Layer (DAL)
Now we move to the C# DAL — or Data Access Layer.
Here, repository classes manage the logic.
We use Entity Framework for LINQ-based queries, and ADO.NET to call stored procedures.
For example — we call `usp_AddCustomer` to insert a new customer.
We can also fetch policies, claims, and update records — all from C#.

### 🔹 Step 3: Console App Testing
Let’s test this logic using a .NET Console App.
We start by adding a customer.
Then, we issue a policy to that customer.
Next, we raise a new claim.
We also test converting that claim to Collection — for high-risk tracking.
Finally, we record a payment.
Behind the scenes, triggers handle the receipt, update the remaining balance, and close the claim if fully paid.
All these actions show real-time output in the console.

### 🔹 Step 4: Swagger Web API
Now let’s switch to Swagger.
Here, we expose every action as a Web API endpoint.
Customers can create claims, view their policies, and check their claim status.
Insurers can issue and update policies.
Score Team reviewers can see assigned claims and update decisions.
Payments can be made via API, and receipts are returned automatically.
All logic is handled by backend SQL, making this system incredibly reliable.

## 🚀 Final Recap

This system automates the entire lifecycle of a claim.
From policy creation to claim closure — every step is secured, validated, and exposed via clean APIs.
It’s a complete backend solution for real-world insurance workflows.

---
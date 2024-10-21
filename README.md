
# Booking Service System

## Overview

The **Booking Service System** is a platform that automates and manages the booking process for various services, such as appointments, event reservations, and resource management. This system provides user-friendly tools to handle scheduling, user notifications, and background tasks seamlessly. Designed with scalability in mind, it can be customized and extended for multiple use cases.

### Key Features:
- **User Management**: Authentication and role-based access using ASP.NET Core Identity.
- **Booking and Scheduling**: Efficiently manage bookings, cancellations, and rescheduling.
- **Automated Background Tasks**: Using Quartz for scheduled tasks like reminders and reporting.
- **Localization Support**: Multi-language support for broader audience reach.
- **Email Notifications**: Integrated SendGrid for email-based communications (reminders, confirmations).
  
## Technologies & Tools

The project leverages modern web development technologies to provide a reliable and scalable solution:

- **ASP.NET Core**: The backbone of the web application, known for its speed and performance.
- **Entity Framework Core**: ORM for managing database operations with ease and flexibility.
- **Quartz.NET**: Scheduling framework for handling background tasks like email notifications and job automation.
- **SendGrid**: Cloud-based email service for sending out notification emails and updates.
- **Localization Tools**: Libraries to support multi-language user interfaces.

### Required Packages:

To ensure the system operates properly, several key NuGet packages were used. Below is a list of packages critical to the system's functionality:

1. **Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore (8.0.8)**  
   Provides diagnostic middleware that helps in managing and logging Entity Framework Core-related exceptions.

2. **Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.8)**  
   Integrates ASP.NET Core Identity with Entity Framework Core, allowing for easy handling of user authentication and role management with a SQL database as storage.

3. **Microsoft.AspNetCore.Identity.UI (8.0.8)**  
   Adds the necessary user interface components to handle identity features, like login pages, registration forms, password resets, etc.

4. **Microsoft.AspNetCore.Localization (2.1.1)**  
   Provides localization capabilities for building multi-language support into the web application, making it accessible to users in different regions.

5. **Microsoft.EntityFrameworkCore.SqlServer (8.0.8)**  
   The Entity Framework Core provider for SQL Server, used to interact with the SQL database.

6. **Microsoft.EntityFrameworkCore.Tools (8.0.8)**  
   Provides tools for managing Entity Framework Core migrations, allowing for easy database updates and schema changes.

7. **Microsoft.Extensions.Localization (8.0.8)**  
   Extends localization functionality within ASP.NET Core to handle multi-language scenarios.

8. **Microsoft.VisualStudio.Web.CodeGeneration.Design (8.0.5)**  
   Helps in generating controllers, views, and models via command-line scaffolding, speeding up development.

9. **Quartz (3.13.0)**  
   A popular open-source job scheduling library used to schedule and run background tasks like sending out email reminders or updating data at specific intervals.

10. **Quartz.Extensions.DependencyInjection (3.13.0)**  
    Allows the integration of Quartz with ASP.NET Core’s dependency injection system, ensuring jobs are handled efficiently within the framework.

11. **Quartz.Extensions.Hosting (3.13.0)**  
    Adds support for running Quartz as a hosted service, making it easier to manage long-running background processes.

12. **SendGrid (9.29.3)**  
    This package integrates with SendGrid, enabling the system to send email notifications for various events, such as bookings, cancellations, and reminders.

## Setup Instructions

To set up and run the Booking Service System locally, follow these steps:

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/AbdoOo20/Booking-Service-System.git
   ```

2. **Install Dependencies**:
   After cloning the repository, ensure all necessary NuGet packages are installed by running:
   ```bash
   dotnet restore
   ```

3. **Database Setup**:
   The system uses Entity Framework Core for database management. Ensure the database is set up and updated by running:
   ```bash
   dotnet ef database update
   ```

4. **Run the Project**:
   Once the packages are installed and the database is updated, you can run the project using:
   ```bash
   dotnet run
   ```

## Important Notes:
- **Database Connection**: Ensure that your database connection string in `appsettings.json` is configured correctly for your local environment.
- **Quartz Scheduling**: The system uses Quartz for job scheduling. This requires proper setup to ensure background tasks, such as email notifications, are run on time.
- **Local APIs**: Many of the project’s APIs are set to localhost. Ensure that your machine’s environment is configured properly to handle these local API requests.

## Developers

This project was developed by a talented team of developers, each contributing significantly to different aspects of the system:

- **MohamedSha22 (#1)**: Lead developer responsible for the core architecture and ensuring the system's scalability.
- **AbdoOo20 (#2)**: Focused on backend development, particularly on API integration and scheduling using Quartz.NET.
- **AmgadEzzat (#3)**: Handled front-end design, ensuring the user interface is intuitive and responsive.
- **Mohamed15101997 (#4)**: Managed user authentication and security features, integrating ASP.NET Identity.
- **Eslam-waheed (#5)**: Worked on system integration, ensuring smooth communication between various modules and handling localization.
- **BasmaElmahany (#6)**: Handled email notifications and external service integration, particularly with SendGrid.
- **Mahmoud-Abdelstar1997 (#7)**: Assisted with testing and ensuring the system performs optimally under different conditions.

## Final Notes

This system is designed for flexibility and scalability, with features that can be adapted for various industries. Make sure to properly configure your environment and dependencies to get the most out of this system.

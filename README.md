# Booking Service System

## Project Overview
The Booking Service System is a comprehensive web-based application designed to facilitate service booking across various providers. It enables users to explore different services, manage their bookings, and access detailed information about each service. The system streamlines the process of booking services by offering a user-friendly interface and a robust backend that handles all operations related to bookings, scheduling, and service management.

## Tech Stack and Tools
This project was developed using the following key technologies:

- **ASP.NET Core MVC** for building the web application.
- **Entity Framework Core (EF Core)** for handling data persistence and communication with SQL Server.
- **RESTful APIs** were created to enable interactions between the frontend (built with Angular) and the backend, ensuring smooth data exchange.
- **Quartz.NET** for job scheduling, managing tasks like booking updates, and other background services.
- **SendGrid** for sending email notifications related to booking confirmations, cancellations, and reminders.

## Important Packages
To ensure the project runs smoothly, the following packages must be installed:

- `Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore (8.0.8)` – Handles exception details related to EF Core in development environments.
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.8)` – Integrates ASP.NET Core Identity with EF Core.
- `Microsoft.AspNetCore.Localization (2.1.1)` – Adds localization support for handling multiple languages.
- `Microsoft.EntityFrameworkCore.SqlServer (8.0.8)` – Provides EF Core support for SQL Server databases.
- `Quartz.Extensions.DependencyInjection (3.13.0)` – Enables dependency injection for Quartz-based jobs.
- `SendGrid (9.29.3)` – Used for sending emails, such as booking confirmations or reminders.

## Development Team
This project was developed by a collaborative team of talented developers:

- **MohamedSha22**: Lead developer, contributed significantly to core functionality and code optimizations.
- **AbdoOo20**: Backend specialist, focused on data handling, API development, and database management.
- **AmgaadEzzat**: Contributed to frontend development and UI/UX improvements.
- **Mohamed15101997**: Assisted in backend API integrations and database optimization tasks.
- **Eslam-waheed**: Contributed to both backend and frontend, ensuring seamless interaction between them.
- **BasmaElmahany**: Frontend developer, focused on building responsive and dynamic UI components.
- **Mahmoud-Abdelstar1997**: Assisted in project testing, debugging, and overall quality assurance.

Each developer played a crucial role in bringing this project to life with their unique skill sets and dedication.

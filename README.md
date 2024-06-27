# Survey System

This project implements a Survey System using ASP.NET Core for handling survey creation and email notifications.

## Features

- **Create Survey**: Allows Admin to share surveys with a specified URL and send notifications to designated domain administrators.
- **Email Notification**: Sends an email notification to each domain administrator with a link to the survey.
- **Validation**: Validates survey URLs and email addresses using regular expressions.

## Technologies Used

- ASP.NET Core
- Serilog for logging
- System.Net.Mail for email functionality
- Regular Expressions for input validation

## Getting Started

To run this project locally, follow these steps:

1. **Clone the repository:**

   ```bash
   git clone https://github.com/alnutayfi/SurveySystem.git
   cd SurveySystem
   ```

2. NET SDK :
   Ensure that you have the .NET SDK (Software Development Kit) installed on your machine. The .NET SDK includes everything you need to build and run .NET Core applications.
   You can download the .NET SDK from the official .NET website throw follwoing url https://dotnet.microsoft.com/en-us/download then choose .NET8
   After installation, verify that the .NET CLI (Command-Line Interface) commands (dotnet) are available in your terminal or command prompt by running dotnet --version.

4. **Build and Run:**

  - Open the Soultion 'SurveySystem.sln' using your IDE 'ex. jetbrins-rider or VisualStudio' 
  - Then click on Run
  - Or throw .NET CLI to build and run the project.

   ```bash
   dotnet build
   dotnet run
   ```


5. **Usage:**

   - Access the application through `http://localhost:5003/Index` to start creating surveys.
   - open survey page using 'click on' url 
   - Fill in the survey URL and add domain-admin email pairs to send out survey invitations.

---

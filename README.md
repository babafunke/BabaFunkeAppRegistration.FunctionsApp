# BabaFunkeAppRegistration.FunctionsApp
A Registration backend using Azure Durable Functions

[![Blog Inspiration (BabaFunkeAppRegistration.FunctionsApp)](https://img.shields.io/badge/Blog-Inspiration-yellowgreen.svg?style=flat-square)](https://daddycreates.com/a4k-app-creating-a-registration-backend-using-azure-durable-functions/)
# Introduction 
BabaFunkeAppRegistration.FunctionsApp is a Registration backend for an App. It uses the combination of Azure Functions and Table Storage to register users via an App. Specifically, it uses Durable Functions to manage the registration workflow ensuring state management across the validation, persistence and email confirmation steps. The solution includes 4 projects - a shared class library, 2 Functions App projects and 1 Test project. It was inspired by the need to create a scalable yet secure cost-effective Registration backend for an App that I'm currently working on for my daughter.

# Solution
There is a single solution file which contains the projects below
* Shared - a Class library project that holds the models, entitity, enums and constants used across the solution.
* BabaFunkeEmailConfirmation - this is an HttpTrigger Azure Functions project. It provides the URL endpoint that gets called for confirming emails and updating the user's record in Table Storage.
* BabaFunkeAppRegistration - this is an Azure Functions project which uses Durable Functions. It consists of the workflow for validating, persisting, registering and completing the initial process.
* BabaFunkeAppRegistration.Tests - an Xunit test project for the Functions App projects

# Build and Test
Clone this git repo and open the solution in Visual Studio. Update the local.settings.json files with your details where applicable. This includes the SendGrid API keys, and sender email addresses. Also, make sure you change the value for the property **FunctionAppEmailHost** in the local.settings.json file of the BabaFunkeAppRegistration project to correspond to the local URL of the BabaFunkeEmailConfirmation project. 


# Background
For more on the project's background and walkthrough, read here
* [A4K App: creating a Registration Backend using Durable Functions](https://daddycreates.com/a4k-app-creating-a-registration-backend-using-azure-durable-functions/)

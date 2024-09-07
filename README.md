# OnlyBalds :bald_man:

![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=for-the-badge&logo=blazor&logoColor=white)
![API](https://img.shields.io/badge/API-3C873A?style=for-the-badge&logo=api&logoColor=white)

OnlyBalds is a Blazor application with a dedicated Web API for backend operations. It's designed to provide a seamless user experience for our bald community.

## :file_folder: Projects

The solution contains three projects:

1. **Blazor Server**: This project handles the server-side rendering of the Blazor application.
2. **Blazor Client**: This project is responsible for the client-side logic and UI of the Blazor application.
3. **Web API**: This project provides the backend for frontend (BFF) implementations, serving as the bridge between the Blazor application and the database.

## :rocket: Getting Started

To get started with the OnlyBalds application, follow these steps:

1. Clone the repository: `git clone https://github.com/bobbylite/OnlyBalds.git`
2. Navigate to the project directory: `cd OnlyBalds`
3. Restore the packages: `dotnet restore`
4. Run the application: `dotnet run`

## :computer: Running the Project Locally

To run the OnlyBalds application locally, follow these steps:

### Step 1: Run the BFF Backend

First, you need to run the Backend for Frontend (BFF) which is the Web API project. This project handles the backend operations and serves as the bridge between the Blazor application and the database.

1. Open a terminal or command prompt.
2. Navigate to the project directory: `cd OnlyBalds`
3. Run the BFF backend with the following command:
```sh
dotnet run --project OnlyBalds.Api --launch-profile https
```



### Step 2: Run the BFF Backend

First, you need to run the Backend for Frontend (BFF) which is the Web API project. This project handles the backend operations and serves as the bridge between the Blazor application and the database.

To run the OnlyBalds application locally, follow these steps:
1. Create a launch profile in the properties directory.
3. Use VS Code's debug section to run the profile created in the previous steps.

## :gear: Built With

- [.NET 8](https://dotnet.microsoft.com/en-us/)
- [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
- [.NET Core Web API](https://dotnet.microsoft.com/en-us/apps/aspnet/apis)
- [Auth0](https://auth0.com/)

## :handshake: Contributing

Contributions, issues, and feature requests are welcome! Feel free to check [issues page](https://github.com/bobbylite/OnlyBalds/issues).

## :memo: License

This project is [MIT](https://choosealicense.com/licenses/mit/) licensed.
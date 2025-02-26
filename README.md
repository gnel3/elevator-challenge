# Elevator Challenge

## Description
The Elevator Challenge is an application that simulates the movement of elevators within a building. This project aims to provide a realistic and efficient simulation of elevator operations, including handling multiple floors, managing elevator requests, and optimizing elevator movements to reduce wait times.

## Table of Contents
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
    - [Running the Simulation](#running-the-simulation)
    - [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Features
- Simulate elevator movements in a multi-story building
- Handle multiple elevator requests from different floors
- Optimize elevator operations to reduce wait times
- Real-time visualization of elevator positions and movements
- Configurable number of elevators and floors

## Technologies Used
- C#
- .NET 9
- Console Application

## Getting Started

### Prerequisites

- .NET Core SDK, you can download it from [here](https://dotnet.microsoft.com/download).

### Installation

To install the application, follow these steps:

1. Clone the repository:
    ```bash
    git clone https://github.com/gnel3/elevator-challenge.git
    ```
2. Navigate to the project directory:
    ```bash
    cd elevator-challenge
    ```
3. Build the project and specify an output directory:
    ```bash
    dotnet build .\src\Elevator.Challenge.Presentation\ -o ./build
    ```

### Running the Simulation

To run the application, follow these steps:

1. After building the project, navigate to the output directory:
    ```bash
    cd build
    ```
2. Run the executable:
   ```bash
      .\Elevator.Challenge.Presentation.exe
   ```   
3. Follow the on-screen instructions

### Configuration
You can customize the application's settings by editing the `appsettings.json` file located in src/Elevator.Challenge.Presentation.

Example `appsettings.json`:
```json
{
   "ElevatorSettings": {
      "NumberOfFloors": 10,
      "NumberOfElevators": 3,
      "MaxPassengers": 20,
      "SimulateMovement": true
   }
}
```

To modify the settings, simply open the `appsettings.json` file in a text editor and adjust the values as needed. Save the file and re-run the build to apply the changes.

## Contributing

We welcome contributions to the Elevator Challenge project! If you have suggestions, bug reports, or feature requests,
please open an issue or submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

If you have any questions or suggestions, please open an issue or contact the repository owner.
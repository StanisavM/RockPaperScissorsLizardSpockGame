# Rock Paper Scissors Lizard Spock Game API

This is a .NET 9 Web API that implements the classic "Rock, Paper, Scissors, Lizard, Spock" game, inspired by the TV show *The Big Bang Theory*. It's designed with best practices in mind to demonstrate clean architecture, MediatR, testing, and Docker deployment. It also includes a persistent scoreboard using LiteDB to track the 10 most recent game results.

---

## Features

- Play the RPSLS game with computer opponent.
- Random number generator service for computer move selection.
- Persistent scoreboard using LiteDB.
- View the 10 most recent game results (globally or by user).
- Reset the scoreboard by user.
- Dockerized for easy deployment.
- Swagger UI for testing and documentation.
- Full unit test suite with xUnit and Moq.

---

## Technologies Used

- **ASP.NET Core 9** – Web API framework
- **CQRS pattern** (with MediatR)
- **LiteDB** – Local embedded NoSQL database
- **Docker / Docker Compose** – Containerization and orchestration
- **Swagger (Swashbuckle)** – API documentation and UI

---

## Key NuGet Packages

- **MediatR**: Implements the mediator pattern for in-process messaging and CQRS.
- **LiteDB**: Lightweight NoSQL database for persisting game results.
- **Swashbuckle.AspNetCore**: Generates OpenAPI/Swagger documentation and UI.
- **Serilog.AspNetCore** & **Serilog.Sinks.Console**: Structured logging to console.
- **Polly**: Resilience and transient-fault-handling (retries, circuit breakers) for HTTP clients.
- **FluentValidation**: Fluent, expressive validation for configuration and DTOs.
- **xUnit**: Unit testing framework.
- **Moq**: Mocking library for unit tests.
- **coverlet.collector**: Code coverage collection for .NET test projects.

> For a full list of dependencies and their versions, see the `.csproj` files in each project.

---
---

## Endpoints Overview

### `/play` `POST`

Play a round against the computer.

**Request:**
```json
{
  "player": 1,
  "email": "player@example.com" // optional
}
```

**Response:**
```json
{
  "player": 1,
  "computer": 3,
  "results": "win",
  "funFact": "Rock Paper Scissors Lizard Spock was invented by Sam Kass and Karen Bryla and made popular by *The Big Bang Theory"
}
```

---

### `/choices` `GET`

Returns the list of all possible moves: Rock, Paper, Scissors, Lizard, Spock.

---

### `/choice` `GET`

Returns a randomly selected move.

---

### `/scoreboard` `GET`

Get the 10 most recent games (optional email filter).

**Query:**
```
/scoreboard?email=player@example.com
```

---

### `/scoreboard/clear` `POST`

Clear scoreboard by player email.

**Request:**
```json
{
  "email": "player@example.com"
}
```

---

## Project Structure

```
├── RockPaperScissorsLizardSpockGame.Api
│   └── Controllers / Dockerfile
├── RockPaperScissorsLizardSpockGame.Application
│   ├── Commands / Handlers / Queries / Services / DTOs
├── RockPaperScissorsLizardSpockGame.Domain
│   └── Models (GameMove, GameRules, ScoreEntry)
├── RockPaperScissorsLizardSpockGame.Infrastructure
│   └── LiteDbScoreboardService.cs
├── RockPaperScissorsLizardSpockGame.Tests
│   └── Unit Tests (xUnit)
├── docker-compose.yml
```

---

## Run via Docker

1. Ensure Docker is installed and running.
2. In the project root, run:

```bash
docker-compose up --build
```

3. Access the API at: https://localhost:8081/swagger/index.html

---

## Persistent Data

The scoreboard is stored in a `LiteDB` file inside the container and mounted to a volume so data survives restarts.

---

## Running Tests

From the test project folder:

```bash
dotnet test
```

---

## Bonus Features

✔ Scoreboard with 10 most recent results (by user or global) 
✔ Scoreboard can be reset per user  
✔ Multiple users supported via email
✔ Dockerized deployment

---

## Author

**Stanisav Milanovic**  
[GitHub](https://github.com/stanisavm)
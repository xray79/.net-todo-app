# .NET Todo App (Incremental Production-Style Evolution)

This repository keeps the original Todo app structure while incrementally improving backend boundaries, testing, CI, and local developer workflows.

## Repository Structure

- `TodoApi/`: ASP.NET Core API (controllers, application handlers, services, models)
- `TodoApi.UnitTests/`: xUnit + Moq + FluentAssertions unit tests
- `TodoApi.IntegrationTests/`: WebApplicationFactory-based API integration tests
- `todo-ui/`: Angular frontend and Cypress end-to-end tests
- `.github/workflows/ci.yml`: CI workflow for PR validation and publish artifacts
- `docker-compose.yml`: local container orchestration (API + optional infra)

## Current Architecture (Pragmatic Modular Monolith)

- HTTP concerns remain in `Controllers`.
- Application logic is split into command/query handlers under `TodoApi/Application/Todos`.
- In-memory data behavior stays in `ITodoService` + `InMemoryTodoService` to minimize disruption.
- Lightweight domain event foundation exists under:
  - `TodoApi/Domain/Events`
  - `TodoApi/Application/Events`
  - `TodoApi/Infrastructure/Events`
- Domain events are published for create/update/complete/delete and currently logged via `LoggingEventPublisher`.

For details, see [docs/architecture.md](docs/architecture.md).

## Running Locally (Non-Docker)

1. Backend:
   - `dotnet run --project TodoApi`
2. Frontend:
   - `cd todo-ui`
   - `npm ci`
   - `npm start`
3. Open `http://localhost:4200`

Default API URL is `http://localhost:5080`.

## Testing

- Unit tests:
  - `dotnet test TodoApi.UnitTests/TodoApi.UnitTests.csproj`
- Integration tests:
  - `dotnet test TodoApi.IntegrationTests/TodoApi.IntegrationTests.csproj`
- Angular unit tests:
  - `cd todo-ui && npm run test -- --watch=false`
- Cypress e2e:
  - `cd todo-ui && npm run e2e`

Note: Current Cypress coverage uses a practical, stable subset against the running app stack (app availability + core todo API-driven flows).

## Docker

- Validate compose:
  - `docker compose config`
- Run API only:
  - `docker compose up --build todo-api`
- Run API + optional infra:
  - `docker compose --profile infra up --build`

Services:
- `todo-api` on `http://localhost:5080`
- optional `redis` on `6379`
- optional `rabbitmq` on `5672` (`15672` management UI)

## CI Behavior

Defined in `.github/workflows/ci.yml`:

- Pull requests to `main`:
  - restore/build backend
  - run backend unit + integration tests
  - install/build/test Angular frontend
- Pushes to `main`:
  - build + publish `TodoApi`
  - upload publish artifact

## What Intentionally Stays Simple

- Data store is still in-memory.
- No external message broker integration in request flow yet.
- No service split yet; architecture remains single API + UI for learning value and low friction.

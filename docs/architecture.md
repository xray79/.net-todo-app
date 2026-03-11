# Architecture Notes

## Goals

Evolve the original Todo API + Angular UI toward production-style patterns without rewriting the repository.

## Applied Incremental Changes

## 1) Backend layering improvements

- Controllers delegate to application handlers:
  - `ITodoCommandHandler`
  - `ITodoQueryHandler`
- Handlers encapsulate use-case behavior and call `ITodoService`.
- Controllers remain thin and HTTP-focused.

## 2) CQRS-style split (lightweight)

- Write paths are represented as commands.
- Read paths are represented as queries.
- The split is intentionally lightweight and keeps the project approachable.

## 3) Event foundation (internal, not distributed)

- Domain event contracts in `TodoApi/Domain/Events/TodoEvents.cs`.
- Publisher abstraction in `TodoApi/Application/Events/IEventPublisher.cs`.
- Logging implementation in `TodoApi/Infrastructure/Events/LoggingEventPublisher.cs`.
- `TodoCommandHandler` publishes events for:
  - `TodoCreatedEvent`
  - `TodoUpdatedEvent`
  - `TodoCompletedEvent`
  - `TodoDeletedEvent`

This allows future RabbitMQ/Redis integrations without immediate complexity.

## 4) Validation and DTO hardening

- Request models now include string length constraints to avoid bad/oversized titles.

## 5) Test strategy

- Unit tests verify controller behavior and command-handler event publication.
- Integration tests exercise end-to-end API flows with `WebApplicationFactory`.
- Frontend unit tests validate Angular component behavior.
- Cypress provides practical end-to-end flow checks in a stable configuration.

## 6) Local infrastructure trajectory

- `docker-compose.yml` now supports:
  - `todo-api` runtime
  - optional `redis` and `rabbitmq` via `infra` profile
- This preserves local simplicity while allowing gradual event/caching expansion.

## Non-Goals (Current Stage)

- No microservice split.
- No persistent database migration yet.
- No distributed event bus wiring in the hot path.

## Suggested Next Increment

1. Add persistence (SQLite/PostgreSQL) behind repository abstraction.
2. Introduce outbox-style event persistence before broker publishing.
3. Add a background worker for one event consumer scenario (analytics/reminders).

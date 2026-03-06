# Phase 1: Architecture Assessment and Adaptation Plan

## 1. Current Structure Summary

### Backend
- Existing backend project: `TodoApi`
- Existing layers/folders: `Controllers`, `Models`, `Services`, `Program.cs`, `appsettings*.json`
- Current style: MVC controllers + service layer + EF/Identity-style direction already possible in this layout

### Tests
- `TodoApi.UnitTests` exists and should remain
- `TodoApi.IntegrationTests` exists and should remain

### Frontend
- Existing Angular app: `todo-ui`
- Cypress-related files/config already present and should be preserved

### CI
- Existing workflow: `.github/workflows/ci.yml`
- Should be evolved, not replaced wholesale

## 2. Target (Minimal-Change) Structure

Keep existing projects, evolve internals:

- `TodoApi`
  - `Controllers` (HTTP transport only)
  - `Features`
    - `Todos`
      - `Commands`
      - `Queries`
      - `Dtos`
      - `Validation`
    - `Auth`
  - `Domain`
    - `Entities`
    - `Events`
  - `Infrastructure`
    - `Persistence`
    - `Events`
    - `Caching` (optional, introduced only when useful)

- `TodoApi.UnitTests`
- `TodoApi.IntegrationTests`
- `todo-ui`

This keeps the repository recognizable while introducing clearer boundaries.

## 3. What Should Stay As-Is

- Project names and major folders (`TodoApi`, `TodoApi.UnitTests`, `TodoApi.IntegrationTests`, `todo-ui`)
- Existing test stack (xUnit/Moq/FluentAssertions, WebApplicationFactory, Cypress)
- Existing CI file path and overall workflow approach
- Core Todo app behavior and local development workflow

## 4. What Should Be Refactored (Incrementally)

- Reduce controller responsibility by moving use-case logic into feature handlers/services
- Introduce clearer DTO + validation boundaries for write operations
- Add CQRS-style separation where useful (commands vs queries) without forcing framework-heavy MediatR patterns immediately
- Introduce explicit domain/application/infrastructure folders inside `TodoApi`
- Introduce lightweight event contracts and publishing abstraction

## 5. New Pieces to Add (Practical Only)

- Root-level docs for architecture and local dev
- `docs/` architectural decision and roadmap docs
- Optional (later):
  - Dockerfile + docker-compose
  - Redis-backed read cache for query hot paths
  - RabbitMQ-backed event publisher/consumer for selected events

## 6. Risks and Tradeoffs

- Over-architecting too early reduces teaching clarity
- Full microservices split now would add high complexity with low immediate value
- Adding RabbitMQ/Redis before event contracts are stable may create avoidable churn
- Excessive renaming/restructuring will hurt continuity for learners

## 7. Recommended Constraints for Future Phases

- Prefer additive refactors over broad rewrites
- Preserve API contracts unless breaking change is justified and documented
- Keep frontend flows stable while improving maintainability
- Only adopt infrastructure components when tied to concrete use-cases

## 8. Explicit “Do Not Change” Guardrails

- Do not move to multi-repo
- Do not replace Angular/Cypress stack
- Do not rename major project directories without strong reason
- Do not implement complexity without practical learning value

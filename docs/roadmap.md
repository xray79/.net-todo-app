# Incremental Implementation Roadmap

This roadmap is intentionally evolutionary and low-disruption.

## Phase 2: Backend Structure Hardening

Goals:
- keep existing endpoints intact
- move business logic out of controllers
- introduce feature slices for Todos/Auth

Likely touched:
- `TodoApi/Controllers/*`
- `TodoApi/Services/*`
- `TodoApi/Models/*`
- new `TodoApi/Features/*`

## Phase 3: Testing Expansion

Goals:
- strengthen unit and integration coverage for full todo lifecycle:
  - create
  - list/get
  - update
  - complete
  - delete
- preserve existing tests and extend them

Likely touched:
- `TodoApi.UnitTests/*`
- `TodoApi.IntegrationTests/*`
- `todo-ui/cypress/e2e/*`
- `todo-ui/src/app/*.spec.ts`

## Phase 4: Frontend Maintainability Improvements

Goals:
- keep UI simple and functional
- introduce better API integration patterns (service separation)
- preserve current user flows

Likely touched:
- `todo-ui/src/app/*`
- new `todo-ui/src/app/services/*`

## Phase 5: CI Improvement

Goals:
- improve confidence while keeping workflow understandable
- ensure backend + frontend checks reflect real app health
- keep PR checks practical and stable

Likely touched:
- `.github/workflows/ci.yml`

## Phase 6: Event-Driven Foundation (Modular Monolith First)

Goals:
- add internal domain/integration event contracts:
  - `TodoCreated`
  - `TodoUpdated`
  - `TodoCompleted`
  - `TodoDeleted`
  - optional `TodoDueSoon`
- implement in-process dispatch first

Likely touched/added:
- `TodoApi/Domain/Events/*`
- `TodoApi/Infrastructure/Events/*`

## Phase 7: Optional Local Infrastructure

Goals:
- add Redis/RabbitMQ only if justified by concrete app behavior
- keep local dev easy

Likely added:
- `docker-compose.yml`
- `TodoApi/Dockerfile`
- optional simple worker project

## Phase 8: Documentation and Architecture Narrative

Goals:
- explain decisions and intentional simplifications
- map current design to production-oriented concepts without pretending full enterprise complexity

Likely touched/added:
- `README.md`
- `docs/architecture.md`
- `docs/local-development.md`

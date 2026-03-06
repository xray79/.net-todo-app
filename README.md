# .NET Todo Platform (Incremental Architecture Evolution)

This repository is intentionally being evolved in small, practical steps from a simple Todo API + Angular UI toward a production-style task platform architecture.

This commit delivers **Phase 1 (assessment and plan)** only:
- current-state assessment
- target incremental architecture direction
- phased implementation roadmap
- scope guardrails (what to keep, what to change, what not to change)

No runtime behavior changes are introduced in this phase.

## Current Projects

- `TodoApi` - ASP.NET Core backend API
- `TodoApi.UnitTests` - xUnit/Moq/FluentAssertions unit tests
- `TodoApi.IntegrationTests` - integration tests with WebApplicationFactory
- `todo-ui` - Angular frontend (with Cypress setup)
- `.github/workflows/ci.yml` - CI pipeline

## Phase 1 Documents

- [Architecture Assessment and Adaptation Plan](docs/architecture.md)
- [Incremental Roadmap](docs/roadmap.md)

## Intended Evolution Principles

- preserve repository shape and naming where possible
- evolve by refactoring, not replacement
- prioritize teaching value and working software
- add complexity only when it unlocks clear practical value
- move toward modular boundaries and event-driven foundations incrementally

# Gate Automation — Live Task

> Your interviewer has briefed you verbally on the domain. Ask for clarification any time during the exercise.

## Setup (1 min)

```bash
cd GateAutomationLive
dotnet run
```

API: **http://localhost:5050** (Swagger opens automatically).
Four sample gate passes are seeded. Three endpoints already exist — see Swagger.

## File layout

```
GateAutomationLive/
├── Controllers/
│   └── GatePassController.cs       <- Task 1 lives here
├── Services/
│   ├── IGatePassService.cs
│   └── GatePassService.cs
├── Infrastructure/                 <- don't modify
│   └── GatePassStore.cs
├── Models.cs
└── Program.cs                      <- DI wiring, don't modify
```

Your interviewer will walk you through this briefly before you start.

---

## Task — POST exit endpoint (~10 min)

Add a new endpoint in `Controllers/GatePassController.cs`:

> **`POST /api/gate-pass/{id}/exit`**
>
> - Marks a gate pass as exited.
> - Only allowed if the pass is currently `Active`.
> - Set `ExitTime` to current UTC.
> - Return the updated pass.

Follow the same pattern the existing endpoints use — delegate work to the service. You will likely need to extend `IGatePassService` and `GatePassService`.

Verify in Swagger.

---

## Time & expectations

- Think out loud. Ask clarifying questions whenever something is ambiguous — that's part of what we want to see.
- Care about correctness and edge cases. Don't worry about polish.

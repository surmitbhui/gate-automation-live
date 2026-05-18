# Gate Automation — Live Task

## Setup (2 minutes)

You need .NET 8 SDK installed (`dotnet --version` should show `8.x.x`).

```bash
dotnet run
```

The API will start at **http://localhost:5050**. Swagger UI will open automatically at **http://localhost:5050/swagger**.

You can test the existing endpoints from Swagger directly — no Postman needed. Four sample gate passes are seeded on startup.

---

## Project context

This is a small gate automation system. A `GatePass` is created when a vehicle enters the plant and represents the vehicle's authorization to be on-site. The gate pass goes through a lifecycle:

- **Active** — the vehicle is currently inside.
- **Exited** — the vehicle has left the plant.
- **Cancelled** — the gate pass was cancelled (e.g., wrong vehicle, mistake at entry).

There are 3 endpoints already implemented:

| Method | Route                        | Purpose                         |
|--------|------------------------------|---------------------------------|
| GET    | `/api/gate-pass/{id}`        | Get a single gate pass by id    |
| GET    | `/api/gate-pass?status=...`  | List gate passes, optional filter |
| POST   | `/api/gate-pass`             | Create a new gate pass          |

You can find them in `Program.cs`.

---

## Your task

Add a new endpoint:

> **`POST /api/gate-pass/{id}/exit`**
>
> Marks a gate pass as exited.
> A gate pass can only be exited if it is currently active.
> Set the exit time to the current time.
> Return the updated gate pass.

---

## What we're looking for

- Read the requirement carefully before writing code.
- Feel free to ask clarifying questions about anything you find ambiguous.
- Think out loud as you work — we're more interested in how you reason about the problem than in raw typing speed.

You have approximately **25-30 minutes** for this task.

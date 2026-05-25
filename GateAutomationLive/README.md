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

## Task 1 — POST exit endpoint (~5 min)

In `Program.cs`, add:

> **`POST /api/gate-pass/{id}/exit`**
>
> - Marks a gate pass as exited.
> - Only allowed if the pass is currently `Active`.
> - Set `ExitTime` to current UTC.
> - Return the updated pass.

Verify in Swagger.

---

## Task 2 — RFID consumer (~20-25 min)

Open `RfidConsumer.cs`. Task statement is in the file header.

You'll verify your work by watching gate pass statuses change at `GET /api/gate-pass` while the consumer runs.

---

## Time & expectations

- Total: **30-40 minutes**.
- Think out loud. Ask clarifying questions whenever something is ambiguous — that's part of what we want to see.
- Care about correctness and edge cases. Don't worry about polish.

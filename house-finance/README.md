# House Finance

Self-hosted web app for tracking shared household expenses.

## Features (v1)

- Add expenses with **who paid**, **amount**, and **description**
- View recent expenses and running total
- SQLite storage for simple self-hosting
- API stubs for future Gmail bill/invoice import

## Stack

- **Frontend:** React + TypeScript (Vite)
- **Backend:** Express + TypeScript
- **Database:** SQLite

## Local development

```bash
cd house-finance
npm install
npm run dev
```

- Web UI: http://localhost:5173
- API: http://localhost:3001

## Self-host with Docker

```bash
cd house-finance
docker compose up --build -d
```

Open http://localhost:3001

Data is stored in the `house-finance-data` Docker volume.

## API

- `GET /api/expenses` — list expenses
- `POST /api/expenses` — create expense
- `DELETE /api/expenses/:id` — delete expense
- `GET /api/gmail/status` — Gmail integration status (placeholder)

## Roadmap: Gmail integration

Planned next steps:

1. Google Cloud project + Gmail API
2. OAuth flow for household Gmail accounts
3. Parse bills/invoices from labeled emails or attachments
4. Create expenses with `source: "gmail"`

import Database from "better-sqlite3";
import fs from "node:fs";
import path from "node:path";
import type { CreateExpenseInput, Expense, ExpenseSource } from "./types.js";

const dbPath = process.env.DATABASE_PATH ?? path.join(process.cwd(), "data", "house-finance.db");

fs.mkdirSync(path.dirname(dbPath), { recursive: true });

const db = new Database(dbPath);

db.pragma("journal_mode = WAL");

db.exec(`
  CREATE TABLE IF NOT EXISTS expenses (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    paid_by TEXT NOT NULL,
    amount REAL NOT NULL CHECK(amount > 0),
    description TEXT NOT NULL,
    source TEXT NOT NULL DEFAULT 'manual' CHECK(source IN ('manual', 'gmail')),
    created_at TEXT NOT NULL DEFAULT (datetime('now'))
  );

  CREATE TABLE IF NOT EXISTS household_members (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE
  );

  INSERT OR IGNORE INTO household_members (name) VALUES ('Alex'), ('Partner');
`);

const listStmt = db.prepare(`
  SELECT id, paid_by AS paidBy, amount, description, source, created_at AS createdAt
  FROM expenses
  ORDER BY datetime(created_at) DESC, id DESC
`);

const insertStmt = db.prepare(`
  INSERT INTO expenses (paid_by, amount, description, source)
  VALUES (@paidBy, @amount, @description, @source)
`);

const deleteStmt = db.prepare(`DELETE FROM expenses WHERE id = ?`);

const membersStmt = db.prepare(`SELECT name FROM household_members ORDER BY name`);

export function listExpenses(): Expense[] {
  return listStmt.all() as Expense[];
}

export function createExpense(input: CreateExpenseInput, source: ExpenseSource = "manual"): Expense {
  const result = insertStmt.run({
    paidBy: input.paidBy.trim(),
    amount: input.amount,
    description: input.description.trim(),
    source,
  });

  const expense = db
    .prepare(
      `SELECT id, paid_by AS paidBy, amount, description, source, created_at AS createdAt
       FROM expenses WHERE id = ?`,
    )
    .get(result.lastInsertRowid) as Expense;

  return expense;
}

export function deleteExpense(id: number): boolean {
  const result = deleteStmt.run(id);
  return result.changes > 0;
}

export function listHouseholdMembers(): string[] {
  const rows = membersStmt.all() as { name: string }[];
  return rows.map((row) => row.name);
}

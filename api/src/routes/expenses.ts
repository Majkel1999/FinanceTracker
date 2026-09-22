import { Router } from "express";
import { createExpense, deleteExpense, listExpenses, listHouseholdMembers } from "../db.js";
import type { CreateExpenseInput } from "../types.js";

export const expensesRouter = Router();

expensesRouter.get("/", (_req, res) => {
  res.json({ expenses: listExpenses() });
});

expensesRouter.get("/members", (_req, res) => {
  res.json({ members: listHouseholdMembers() });
});

expensesRouter.post("/", (req, res) => {
  const input = req.body as Partial<CreateExpenseInput>;

  if (!input.paidBy?.trim()) {
    res.status(400).json({ error: "paidBy is required" });
    return;
  }

  if (typeof input.amount !== "number" || !Number.isFinite(input.amount) || input.amount <= 0) {
    res.status(400).json({ error: "amount must be a positive number" });
    return;
  }

  if (!input.description?.trim()) {
    res.status(400).json({ error: "description is required" });
    return;
  }

  const expense = createExpense({
    paidBy: input.paidBy,
    amount: input.amount,
    description: input.description,
  });

  res.status(201).json({ expense });
});

expensesRouter.delete("/:id", (req, res) => {
  const id = Number(req.params.id);

  if (!Number.isInteger(id) || id <= 0) {
    res.status(400).json({ error: "invalid expense id" });
    return;
  }

  const deleted = deleteExpense(id);

  if (!deleted) {
    res.status(404).json({ error: "expense not found" });
    return;
  }

  res.status(204).send();
});

export type ExpenseSource = "manual" | "gmail";

export interface Expense {
  id: number;
  paidBy: string;
  amount: number;
  description: string;
  source: ExpenseSource;
  createdAt: string;
}

export interface CreateExpenseInput {
  paidBy: string;
  amount: number;
  description: string;
}

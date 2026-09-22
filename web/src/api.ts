export interface Expense {
  id: number;
  paidBy: string;
  amount: number;
  description: string;
  source: "manual" | "gmail";
  createdAt: string;
}

export interface CreateExpenseInput {
  paidBy: string;
  amount: number;
  description: string;
}

async function request<T>(url: string, init?: RequestInit): Promise<T> {
  const response = await fetch(url, init);
  if (!response.ok) {
    const body = await response.json().catch(() => ({}));
    throw new Error(body.error ?? `Request failed (${response.status})`);
  }
  if (response.status === 204) {
    return undefined as T;
  }
  return response.json() as Promise<T>;
}

export async function fetchExpenses(): Promise<Expense[]> {
  const data = await request<{ expenses: Expense[] }>("/api/expenses");
  return data.expenses;
}

export async function fetchMembers(): Promise<string[]> {
  const data = await request<{ members: string[] }>("/api/expenses/members");
  return data.members;
}

export async function createExpense(input: CreateExpenseInput): Promise<Expense> {
  const data = await request<{ expense: Expense }>("/api/expenses", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(input),
  });
  return data.expense;
}

export async function deleteExpense(id: number): Promise<void> {
  await request<void>(`/api/expenses/${id}`, { method: "DELETE" });
}

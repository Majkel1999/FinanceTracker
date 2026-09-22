import { useEffect, useState } from "react";
import {
  createExpense,
  deleteExpense,
  fetchExpenses,
  fetchMembers,
  type CreateExpenseInput,
  type Expense,
} from "./api";
import { ExpenseForm } from "./components/ExpenseForm";
import { ExpenseList } from "./components/ExpenseList";

export function App() {
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [members, setMembers] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    async function load() {
      try {
        const [expenseData, memberData] = await Promise.all([fetchExpenses(), fetchMembers()]);
        setExpenses(expenseData);
        setMembers(memberData);
      } catch (loadError) {
        setError(loadError instanceof Error ? loadError.message : "Failed to load data.");
      } finally {
        setLoading(false);
      }
    }

    void load();
  }, []);

  async function handleCreate(input: CreateExpenseInput) {
    const expense = await createExpense(input);
    setExpenses((current) => [expense, ...current]);
  }

  async function handleDelete(id: number) {
    await deleteExpense(id);
    setExpenses((current) => current.filter((expense) => expense.id !== id));
  }

  return (
    <main className="page">
      <header className="hero">
        <div>
          <p className="eyebrow">Self-hosted</p>
          <h1>House Finance</h1>
          <p className="subtitle">Track who paid what around the house. Gmail bill import is planned next.</p>
        </div>
      </header>

      {loading ? <p className="loading">Loading...</p> : null}
      {error ? <p className="error banner">{error}</p> : null}

      {!loading && !error ? (
        <div className="layout">
          <ExpenseForm members={members} onSubmit={handleCreate} />
          <ExpenseList expenses={expenses} onDelete={handleDelete} />
        </div>
      ) : null}
    </main>
  );
}

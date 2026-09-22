import { FormEvent, useState } from "react";
import type { CreateExpenseInput } from "../api";

interface ExpenseFormProps {
  members: string[];
  onSubmit: (input: CreateExpenseInput) => Promise<void>;
}

export function ExpenseForm({ members, onSubmit }: ExpenseFormProps) {
  const [paidBy, setPaidBy] = useState(members[0] ?? "");
  const [amount, setAmount] = useState("");
  const [description, setDescription] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setError(null);

    const parsedAmount = Number(amount);
    if (!paidBy.trim()) {
      setError("Choose who paid.");
      return;
    }
    if (!Number.isFinite(parsedAmount) || parsedAmount <= 0) {
      setError("Enter a valid amount greater than zero.");
      return;
    }
    if (!description.trim()) {
      setError("Add a short description.");
      return;
    }

    setSubmitting(true);
    try {
      await onSubmit({
        paidBy,
        amount: parsedAmount,
        description,
      });
      setAmount("");
      setDescription("");
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : "Could not save expense.");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <section className="card">
      <h2>Add expense</h2>
      <form className="expense-form" onSubmit={handleSubmit}>
        <label>
          Paid by
          <select value={paidBy} onChange={(event) => setPaidBy(event.target.value)}>
            {members.map((member) => (
              <option key={member} value={member}>
                {member}
              </option>
            ))}
          </select>
        </label>

        <label>
          Amount
          <input
            type="number"
            min="0.01"
            step="0.01"
            placeholder="0.00"
            value={amount}
            onChange={(event) => setAmount(event.target.value)}
          />
        </label>

        <label>
          Description
          <input
            type="text"
            placeholder="Groceries, electricity bill, etc."
            value={description}
            onChange={(event) => setDescription(event.target.value)}
          />
        </label>

        {error ? <p className="error">{error}</p> : null}

        <button type="submit" disabled={submitting}>
          {submitting ? "Saving..." : "Add expense"}
        </button>
      </form>
    </section>
  );
}

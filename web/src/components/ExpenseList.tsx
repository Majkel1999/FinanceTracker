import type { Expense } from "../api";

interface ExpenseListProps {
  expenses: Expense[];
  onDelete: (id: number) => Promise<void>;
}

function formatCurrency(amount: number): string {
  return new Intl.NumberFormat(undefined, {
    style: "currency",
    currency: "USD",
  }).format(amount);
}

function formatDate(value: string): string {
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}

export function ExpenseList({ expenses, onDelete }: ExpenseListProps) {
  const total = expenses.reduce((sum, expense) => sum + expense.amount, 0);

  return (
    <section className="card">
      <div className="list-header">
        <h2>Recent expenses</h2>
        <p className="total">{formatCurrency(total)} total</p>
      </div>

      {expenses.length === 0 ? (
        <p className="empty">No expenses yet. Add your first one above.</p>
      ) : (
        <ul className="expense-list">
          {expenses.map((expense) => (
            <li key={expense.id} className="expense-item">
              <div>
                <p className="expense-description">{expense.description}</p>
                <p className="expense-meta">
                  {expense.paidBy} · {formatDate(expense.createdAt)}
                  {expense.source === "gmail" ? " · Gmail" : ""}
                </p>
              </div>
              <div className="expense-actions">
                <span className="expense-amount">{formatCurrency(expense.amount)}</span>
                <button type="button" onClick={() => onDelete(expense.id)} aria-label="Delete expense">
                  Delete
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}

import { Router } from "express";

/**
 * Placeholder routes for future Gmail bill/invoice integration.
 * Planned flow:
 * 1. OAuth connect via Google Cloud project
 * 2. Background sync of labeled emails or invoice attachments
 * 3. Map parsed amounts into expenses with source = 'gmail'
 */
export const gmailRouter = Router();

gmailRouter.get("/status", (_req, res) => {
  res.json({
    connected: false,
    message: "Gmail integration is not configured yet.",
    nextSteps: [
      "Create a Google Cloud project and enable the Gmail API",
      "Set GOOGLE_CLIENT_ID and GOOGLE_CLIENT_SECRET",
      "Complete OAuth and store refresh tokens securely",
    ],
  });
});

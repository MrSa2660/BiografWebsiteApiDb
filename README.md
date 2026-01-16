# Cinema Website To-Do

## Backend (ASP.NET)

- Auth: signup/login/logout, hashed passwords, JWT cookies/tokens + refresh, password reset email; roles (user/admin); wire middleware/policies in `Backend/BiografOpgave.API/BiografOpgave.API/Program.cs`.
- Users: profile view/update, change password, booking history; extend `User` model, add migrations/seeding in `...Domain` and `...Infrastructure`.
- Movies: CRUD endpoints (`MoviesController`), poster/trailer storage, metadata (title/description/genre/duration/rating/cast).
- Screens/Showtimes: models for screens with seat maps; schedule showtimes with conflict checks and price tiers; `ShowtimesController`.
- Booking/Seats: hold and reserve seats per showtime with timeout release; create bookings/tickets with QR/code; `BookingsController`; email payload builder.
- Admin: admin-only endpoints for movies/showtimes/screens/users/bookings/promos; audit logs for admin actions.
- Notifications: email service for signup verification, booking confirmation, cancellations, password reset; SMTP settings in `appsettings*.json`.
- Observability/Security: request logging, error handling middleware, health checks; input validation, rate limiting on auth, secure cookies/HTTPS.
- Data/Seed: seed movies/showtimes/users; update `DbContext`; maintain migrations.
- Tests: unit/integration tests for controllers/services/repos (booking/seat/payment flows) in `Backend/BiografOpgave.API/BiografOpgave.Test`.

## Frontend (Angular)

- Auth UI: signup/login/logout/reset with validation; JWT persistence/interceptor; route guards.
- Layout: header/nav/footer, featured carousel, responsive design; shared components in `Frontend/src/app/shared`.
- Movies: list/search/filter/sort (now/soon); movie detail with trailer, cast, and ratings.
- Showtimes/Seats: showtime selection per movie; seat map with availability/holds; display price tiers.
- Booking/Checkout: cart/summary, promo code entry, payment handoff, success page with ticket/QR; error states.
- Profile: view/update profile, change password, booking history with ticket download/resend.
- Admin UI: protected area for managing movies/showtimes/screens/users/bookings/promos; forms/tables with validation.
- UX/Status: toasts/snackbars, loading skeletons, empty/error states; custom 404/500 pages; accessibility (WCAG).
- Tests: component/unit tests (`ng test`) and E2E booking flow (auth → select showtime/seats → pay → confirmation) with Cypress/Playwright.

## CI/CD & Ops

- README updates for setup/run/test (front/back), API docs (Swagger/Scalar), ERD and API reference; scripts for seeding/migrations.
- Dockerize front/back; env configs for staging/prod; pipeline to build/test/deploy; database backups.

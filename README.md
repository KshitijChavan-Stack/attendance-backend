# TrackBase - Backend (C#/.NET 8 + PostgreSQL)

This is the backend for an employee attendance management system that allows employees to log entry and exit times and provides monthly analytics. It includes role-based login (Employee/Manager) and connects to a PostgreSQL database.

## Tech Stack
- **Language**: C#
- **Framework**: ASP.NET Core Web API (.NET 8)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Testing Tool**: Swagger / Postman

---

## API Features Implemented

### User & Manager Authentication
- **Login API**
  - Manager and Employee login with separate logic.
  - Manager credentials are stored in the `Manager` table.
  - As per login seperate dashboards will be accessed
- **Register API**
  - Asks whether to register as **Manager** or **Employee**.
  - Stores the user in the respective table based on input.
  - Breaked my head lots of times just to create the condtion and seperate the entry's
  - Seperate Tables created as well as models 

---

### Attendance Tracking
- **Mark Entry API**
  - Records the user's entry time.
  - Prevents duplicate entries without an exit.

- **Mark Exit API**
  - Records the user's exit time.
  - Calculates duration between entry and exit.
  - Prevents marking exit if already marked.

---

### Analytics & Reports
- **Get All Records (Manager View)**
  - Manager can view attendance of all users & as wll as himself.
-**Get specific employee/managers data
  - From id,name & email
- **Get User Attendance**
  - Fetches all attendance data for a specific user.

- **Get Monthly Attendance**
  - Retrieves attendance based on user ID, month, and year.
  - Gets the days present and hours worked in a month
  - Useful for showing graphs on the frontend.

---

## ⚙️ Models Implemented

- `User` – For employees (ID, Name, Email, Password).
- `Manager` – For managers (ID, Name, Email, Password).
- `Attendance` – Tracks attendance logs with entry/exit times and duration.

---

## Changes & Improvements
- Switched from `EmployeeName` to `UserId` for accurate identification.
- Used `DateTime.Now` to capture entry/exit times.
- Handled duplicate entry/exit cases gracefully.
- Database connected via `AttendanceDbContext` using PostgreSQL.
- Migration and schema generation configured with Entity Framework.

---

## 🌐 Hosting (Remaining Step)

We will host the API using **Render**.

### Hosting Status: 🚧 *In Progress*

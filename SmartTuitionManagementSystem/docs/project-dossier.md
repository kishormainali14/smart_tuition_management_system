# Smart Tuition Management System — Complete Project Dossier

**Generated:** June 11, 2026  
**Project Version:** 1.0.0  
**Framework:** .NET 10.0 (ASP.NET Core MVC)  
**Database:** PostgreSQL (via Npgsql)  
**ORM:** Entity Framework Core 10.0.8  

---

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Architecture](#2-architecture)
3. [Entity Relationship Diagram](#3-entity-relationship-diagram)
4. [Complete Database Schema](#4-complete-database-schema)
5. [Migration History](#5-migration-history)
6. [Controllers and Actions](#6-controllers-and-actions)
7. [Service Layer](#7-service-layer)
8. [View Structure](#8-view-structure)
9. [Configuration](#9-configuration)
10. [Route Map](#10-route-map)
11. [Key Architecture Notes](#11-key-architecture-notes)
12. [Source Code Listings](#12-source-code-listings)

---

## 1. Project Overview

### 1.1 Project Identity

| Field | Value |
|-------|-------|
| **Project Name** | Smart Tuition Management System |
| **Solution File** | `Smart Tuition Management System.sln` |
| **Project File** | `SmartTuitionManagementSystem.csproj` |
| **Root Namespace** | `SmartTuitionManagementSystem` |
| **Target Framework** | `net10.0` |
| **Language** | C# (Implicit usings enabled, Nullable enabled) |
| **Template** | ASP.NET Core MVC (not Minimal API) |

### 1.2 Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| ASP.NET Core MVC | 10.0 | Web framework |
| Entity Framework Core | 10.0.8 | ORM / data access |
| Npgsql (PostgreSQL provider) | 10.0.2 | Database driver |
| Newtonsoft.Json | 13.0.4 | JSON serialization |
| Bootstrap (via CDN in layout) | 5.x | Frontend UI |
| Font Awesome (via CDN) | 6.x | Icons |

### 1.3 Database

| Field | Value |
|-------|-------|
| **Database Engine** | PostgreSQL |
| **Database Name** | `SmartTuitionManagementDB` |
| **Host** | `localhost` |
| **Connection String** | `Host=localhost;Database=SmartTuitionManagementDB;Username=postgres;Password=Pass@123` |

### 1.4 Project Directory Structure

```
SmartTuitionManagementSystem/
├── Constants/
│   └── AttendanceStatus.cs
├── Controllers/
│   ├── AccountController.cs
│   ├── ClassesController.cs
│   ├── ExamsController.cs
│   ├── FeesController.cs
│   ├── HomeController.cs
│   ├── StudentAttendanceController.cs
│   ├── Studentscontroller.cs
│   ├── TeacherAttendanceController.cs
│   ├── Teacherscontroller.cs
│   └── UserManagementController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Entities/
│   ├── ClassEntity.cs
│   ├── ExamEntity.cs
│   ├── FeeCollectionEntity.cs
│   ├── FeeStructureEntity.cs
│   ├── FeeTypeEntity.cs
│   ├── PaymentTransactionEntity.cs
│   ├── StudentAttendanceEntity.cs
│   ├── StudentEntity.cs
│   ├── TeacherAttendanceEntity.cs
│   ├── TeacherEntity.cs
│   └── UserEntity.cs
├── Migrations/
│   ├── 20260609071202_initialcreate.cs
│   ├── 20260609071202_initialcreate.Designer.cs
│   ├── 20260610070635_InitialTeacherAttendance.cs
│   ├── 20260610070635_InitialTeacherAttendance.Designer.cs
│   ├── 20260611020534_AddRollNumberToStudents.cs
│   ├── 20260611020534_AddRollNumberToStudents.Designer.cs
│   └── ApplicationDbContextModelSnapshot.cs
├── Models/
│   ├── ClassViewModel.cs
│   ├── ErrorViewModel.cs
│   ├── ExamDashboardViewModel.cs
│   ├── ExamViewModel.cs
│   ├── FeeViewModels.cs
│   ├── ForgotPasswordViewModel.cs
│   ├── LoginViewModel.cs
│   ├── MarkAttendanceViewModel.cs
│   ├── RegisterviewModel.cs
│   ├── ResetPasswordViewModel.cs
│   ├── ResponseModels.cs
│   ├── StudentAttendanceViewModel.cs
│   ├── StudentViewModel.cs
│   ├── Students.cs
│   ├── Teacher.cs
│   ├── TeacherAttendance.cs
│   ├── TeacherAttendanceViewModel.cs
│   ├── TeacherViewModel.cs
│   └── UserViewModel.cs
├── Services/
│   ├── Interface/
│   │   ├── IAttendanceService.cs
│   │   ├── IClassService.cs
│   │   ├── IExamService.cs
│   │   ├── IFeeService.cs
│   │   ├── IStudentService.cs
│   │   ├── ITeacherAttendanceService.cs
│   │   ├── ITeacherService.cs
│   │   └── Iuserservice.cs
│   ├── AttendanceService.cs
│   ├── ClassService.cs
│   ├── ExamService.cs
│   ├── FeeService.cs
│   ├── StudentService.cs
│   ├── TeacherAttendanceService.cs
│   ├── TeacherService.cs
│   └── UserService.cs
├── Views/
│   ├── Account/
│   │   ├── ForgetPassword.cshtml
│   │   ├── Login.cshtml
│   │   └── ResetPassword.cshtml
│   ├── Classes/
│   │   ├── AddClass.cshtml
│   │   ├── ClassList.cshtml
│   │   ├── EditClass.cshtml
│   │   └── Index.cshtml
│   ├── Exams/
│   │   ├── Create.cshtml
│   │   ├── Dashboard.cshtml
│   │   ├── Details.cshtml
│   │   ├── Edit.cshtml
│   │   └── Index.cshtml
│   ├── Fees/
│   │   ├── Collections.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Index.cshtml
│   │   ├── Payment.cshtml
│   │   ├── Receipt.cshtml
│   │   └── Reports.cshtml
│   ├── Home/
│   │   ├── Dashboard.cshtml
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Error.cshtml
│   ├── StudentAttendance/
│   │   ├── AttendanceReport.cshtml
│   │   └── StudentAttendance.cshtml
│   ├── Students/
│   │   ├── AddStudent.cshtml
│   │   ├── EditStudent.cshtml
│   │   └── StudentList.cshtml
│   ├── TeacherAttendance/
│   │   ├── Edit.cshtml
│   │   ├── Index.cshtml
│   │   ├── MonthlyReport.cshtml
│   │   ├── SalaryCalculation.cshtml
│   │   └── SelectTeacher.cshtml
│   ├── Teachers/
│   │   ├── AddTeacher.cshtml
│   │   ├── EditTeacher.cshtml
│   │   └── TeacherList.cshtml
│   ├── UserManagement/
│   │   ├── AddUser.cshtml
│   │   ├── EditUser.cshtml
│   │   └── UserList.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── Properties/
│   └── launchSettings.json
├── appsettings.Development.json
├── appsettings.json
├── Program.cs
└── SmartTuitionManagementSystem.csproj
```

---

## 2. Architecture

### 2.1 Architectural Pattern

The application follows a standard **MVC (Model-View-Controller)** pattern with a **Service Layer**:

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│   Browser    │ ──▶ │  Controller  │ ──▶ │   Service    │ ──▶ │  DbContext   │
│   (Razor)    │ ◀── │    Layer     │ ◀── │    Layer     │ ◀── │  (EF Core)   │
└──────────────┘     └──────────────┘     └──────────────┘     └──────┬───────┘
                                                                       │
                                                                  ┌────▼───────┐
                                                                  │ PostgreSQL  │
                                                                  │  Database   │
                                                                  └────────────┘
```

### 2.2 Request Lifecycle

```
HTTP Request
    │
    ▼
Program.cs Middleware Pipeline
    ├── Exception Handler (Dev vs Production)
    ├── HttpsRedirection
    ├── StaticFiles
    ├── Routing
    ├── Session
    │
    ▼
Controller Action
    │
    ├── Session-based Auth Check
    ├── Call Service Method
    │       │
    │       ▼
    │   Service Layer
    │       ├── Business Logic
    │       ├── Validation
    │       └── DbContext Operations
    │
    ├── TempData for Success/Error Messages
    └── Return View(model)
```

### 2.3 Authentication Model

**Custom session-based authentication** (not ASP.NET Core Identity):

- **Login**: Email/password → SHA256 hash → verify → store user data in session
- **Session variables**: `UserId`, `Username`, `UserEmail`, `UserRole`, `UserName`
- **Session timeout**: 30 minutes (sliding expiration)
- **Role check**: Controllers check `HttpContext.Session.GetString("UserRole")` for "Admin"
- **Registration**: Disabled for public; admin-only user creation
- **Password reset**: Simple email-verify-then-reset (no email service, no security questions)

---

## 3. Entity Relationship Diagram

### 3.1 Complete ER Diagram

```mermaid
erDiagram
    USERS {
        int Id PK
        string Name
        string Email UK
        string Username UK
        string PasswordHash
        string Contact
        string Address
        string Role
        bool IsActive
        datetime CreatedAt
        datetime LastLoginDate
    }

    TEACHERS_ENTITY {
        int Id PK
        string FullName
        string Email UK
        string PhoneNumber
        string Address
        string Qualification
        int ExperienceYears
        string Specialization
        date HireDate
        decimal Salary "decimal(18,2)"
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    TEACHER_MODEL {
        int Id PK
        string FirstName
        string LastName
        string Email
        string PhoneNumber
        string Address
        string Qualification
        int ExperienceYears
        string Specialization
        date HireDate
        decimal Salary "decimal(18,2)"
        decimal PerDayRate "decimal(18,2)"
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
        int CreatedBy
        int UpdatedBy
    }

    CLASSES {
        int Id PK
        string ClassName
        string ClassCode
        string Section
        int MaxCapacity "Default: 40"
        int CurrentStrength "Default: 0"
        string RoomNumber
        string Description
        int ClassTeacherId FK "Nullable"
        string AcademicYear
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    STUDENTS {
        int Id PK
        string FullName
        string RollNumber
        string Email
        string PhoneNumber
        string Address
        date DateOfBirth "Nullable"
        string ParentName
        string ParentPhone
        date EnrollmentDate
        int ClassId FK
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    STUDENT_ATTENDANCE {
        int Id PK
        int StudentId FK
        date AttendanceDate
        string Status "Default: Present"
        string Remarks
        datetime MarkedAt
        int MarkedBy "Nullable"
    }

    EXAMS {
        int Id PK
        string ExamName
        string ExamCode UK
        string ExamType
        string AcademicYear
        string Section
        string Description
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    TEACHER_ATTENDANCE {
        int Id PK
        int TeacherId FK
        date Date UK "(TeacherId, Date)"
        int Status "Enum: Present=1..Leave=5"
        time CheckInTime
        time CheckOutTime
        decimal TotalWorkingHours "decimal(5,2)"
        decimal OvertimeHours "decimal(5,2)"
        string Remarks
        bool IsApproved
        int ApprovedBy
        datetime ApprovedDate
        string RejectionReason
        string DocumentPath
        datetime CreatedAt
        int CreatedBy
        datetime UpdatedAt
        int UpdatedBy
        byte[] RowVersion "Timestamp"
    }

    FEE_TYPES {
        int Id PK
        string FeeTypeName
        string Description
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    FEE_STRUCTURES {
        int Id PK
        int ClassId FK UK "(ClassId, FeeTypeId)"
        int FeeTypeId FK
        decimal Amount "decimal(18,2)"
        date DueDate "Nullable"
        string Description
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    FEE_COLLECTIONS {
        int Id PK
        int StudentId FK UK "(StudentId, FeeStructureId)"
        int FeeStructureId FK
        decimal TotalAmount "decimal(18,2)"
        decimal PaidAmount "decimal(18,2)"
        decimal RemainingBalance "decimal(18,2)"
        date DueDate
        string Status "Default: Pending"
        string Remarks
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    PAYMENT_TRANSACTIONS {
        int Id PK
        int FeeCollectionId FK
        int StudentId FK
        decimal AmountPaid "decimal(18,2)"
        date PaymentDate
        string PaymentMethod "Default: Cash"
        string TransactionReference
        string Remarks
        bool IsCancelled
        datetime CancelledAt
        int CancelledBy
        datetime CreatedAt
    }

    %% ===== RELATIONSHIPS =====
    CLASSES ||--o{ TEACHERS_ENTITY : "ClassTeacherId (FK, nullable)"
    CLASSES ||--o{ STUDENTS : "has students"
    CLASSES ||--o{ FEE_STRUCTURES : "has fee structures"

    STUDENTS ||--o{ STUDENT_ATTENDANCE : "attendance records"
    STUDENTS ||--o{ FEE_COLLECTIONS : "fee collections"
    STUDENTS ||--o{ PAYMENT_TRANSACTIONS : "payment history"

    TEACHER_MODEL ||--o{ TEACHER_ATTENDANCE : "attendance records"

    FEE_TYPES ||--o{ FEE_STRUCTURES : "fee type definition"

    FEE_STRUCTURES ||--o{ FEE_COLLECTIONS : "collection instances"

    FEE_COLLECTIONS ||--o{ PAYMENT_TRANSACTIONS : "payment installments"
```

### 3.2 Relationship Summary

| # | Parent | Child | FK Column | Type | Delete Behavior |
|---|--------|-------|-----------|------|-----------------|
| 1 | `ClassEntity` | `StudentEntity` | `Students.ClassId` | 1:M | Cascade |
| 2 | `ClassEntity` | `FeeStructureEntity` | `FeeStructures.ClassId` | 1:M | Restrict |
| 3 | `TeacherEntity` _(Entities)_ | `ClassEntity` | `Classes.ClassTeacherId` | 1:M | None (nullable FK) |
| 4 | `StudentEntity` | `StudentAttendanceEntity` | `Attendance.StudentId` | 1:M | Cascade |
| 5 | `StudentEntity` | `FeeCollectionEntity` | `FeeCollections.StudentId` | 1:M | Restrict |
| 6 | `StudentEntity` | `PaymentTransactionEntity` | `PaymentTransactions.StudentId` | 1:M | Restrict |
| 7 | `Teacher` _(Models)_ | `TeacherAttendance` | `TeacherAttendances.TeacherId` | 1:M | Restrict |
| 8 | `FeeTypeEntity` | `FeeStructureEntity` | `FeeStructures.FeeTypeId` | 1:M | Restrict |
| 9 | `FeeStructureEntity` | `FeeCollectionEntity` | `FeeCollections.FeeStructureId` | 1:M | Restrict |
| 10 | `FeeCollectionEntity` | `PaymentTransactionEntity` | `PaymentTransactions.FeeCollectionId` | 1:M | Restrict |

### 3.3 Standalone Entities (No Foreign Keys)

- `UserEntity` (table: `Users`)
- `ExamEntity` (table: `Exams`)

---

## 4. Complete Database Schema

### 4.1 Table: `Users`

| Column | Type | Constraints | Default |
|--------|------|-------------|---------|
| `Id` | `integer` | `PRIMARY KEY`, `GENERATED BY DEFAULT AS IDENTITY` | |
| `Name` | `text` | `NOT NULL` | |
| `Email` | `text` | `NOT NULL`, `UNIQUE` | |
| `Username` | `text` | `NOT NULL`, `UNIQUE` | |
| `PasswordHash` | `text` | `NOT NULL` | |
| `Contact` | `text` | `NOT NULL` | |
| `Address` | `text` | `NOT NULL` | |
| `Role` | `text` | | `'User'` |
| `IsActive` | `boolean` | `NOT NULL` | `true` |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | `CURRENT_TIMESTAMP` |
| `LastLoginDate` | `timestamp with time zone` | | |

**Indexes:**
- `IX_Users_Username` — UNIQUE on `Username`
- `IX_Users_Email` — UNIQUE on `Email`

### 4.2 Table: `Teachers` (Entities.TeacherEntity)

| Column | Type | Constraints | Default |
|--------|------|-------------|---------|
| `Id` | `integer` | `PRIMARY KEY`, `GENERATED BY DEFAULT AS IDENTITY` | |
| `FullName` | `character varying(100)` | `NOT NULL` | |
| `Email` | `character varying(100)` | `NOT NULL`, `UNIQUE` | |
| `PhoneNumber` | `character varying(20)` | `NOT NULL` | |
| `Address` | `character varying(500)` | | |
| `Qualification` | `character varying(200)` | `NOT NULL` | |
| `ExperienceYears` | `integer` | `NOT NULL` | |
| `Specialization` | `character varying(200)` | | |
| `HireDate` | `timestamp with time zone` | `NOT NULL` | `CURRENT_TIMESTAMP` |
| `Salary` | `numeric(18,2)` | `NOT NULL` | |
| `IsActive` | `boolean` | `NOT NULL` | `true` |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | `CURRENT_TIMESTAMP` |
| `UpdatedAt` | `timestamp with time zone` | | |

**Indexes:**
- `IX_Teachers_Email` — UNIQUE on `Email`

### 4.3 Table: `Teacher` (Models.Teacher)

| Column | Type | Constraints | Default |
|--------|------|-------------|---------|
| `Id` | `integer` | `PRIMARY KEY`, `GENERATED BY DEFAULT AS IDENTITY` | |
| `FirstName` | `character varying(50)` | `NOT NULL` | |
| `LastName` | `character varying(50)` | `NOT NULL` | |
| `Email` | `character varying(100)` | `NOT NULL` | |
| `PhoneNumber` | `character varying(20)` | `NOT NULL` | |
| `Address` | `character varying(500)` | | |
| `Qualification` | `character varying(200)` | `NOT NULL` | |
| `ExperienceYears` | `integer` | `NOT NULL` | |
| `Specialization` | `character varying(200)` | | |
| `HireDate` | `timestamp with time zone` | `NOT NULL` | |
| `Salary` | `numeric(18,2)` | `NOT NULL` | |
| `PerDayRate` | `numeric(18,2)` | `NOT NULL` | |
| `IsActive` | `boolean` | `NOT NULL` | `true` |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | |
| `UpdatedAt` | `timestamp with time zone` | | |
| `CreatedBy` | `integer` | | |
| `UpdatedBy` | `integer` | | |

### 4.4 Table: `Classes`

| Column | Type | Constraints | Default |
|--------|------|-------------|---------|
| `Id` | `integer` | `PRIMARY KEY`, `GENERATED BY DEFAULT AS IDENTITY` | |
| `ClassName` | `character varying(50)` | `NOT NULL` | |
| `ClassCode` | `character varying(20)` | | |
| `Section` | `character varying(10)` | | |
| `MaxCapacity` | `integer` | `NOT NULL` | `40` |
| `CurrentStrength` | `integer` | `NOT NULL` | `0` |
| `RoomNumber` | `character varying(20)` | | |
| `Description` | `character varying(500)` | | |
| `ClassTeacherId` | `integer` | `REFERENCES "Teachers"("Id")` | |
| `AcademicYear` | `character varying(20)` | | |
| `IsActive` | `boolean` | `NOT NULL` | `true` |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | |
| `UpdatedAt` | `timestamp with time zone` | | |

**Foreign Keys:**
- `ClassTeacherId` → `Teachers(Id)` (SET NULL on delete)

**Indexes:**
- `IX_Classes_ClassTeacherId`

### 4.5 Table: `Students`

| Column | Type | Constraints | Default |
|--------|------|-------------|---------|
| `Id` | `integer` | `PRIMARY KEY`, `GENERATED BY DEFAULT AS IDENTITY` | |
| `FullName` | `character varying(100)` | `NOT NULL` | |
| `RollNumber` | `character varying(20)` | | (added in migration 3) |
| `Email` | `character varying(100)` | `NOT NULL` | |
| `PhoneNumber` | `character varying(20)` | `NOT NULL` | |
| `Address` | `character varying(500)` | | |
| `DateOfBirth` | `timestamp with time zone` | | |
| `ParentName` | `character varying(100)` | | |
| `ParentPhone` | `character varying(20)` | | |
| `EnrollmentDate` | `timestamp with time zone` | `NOT NULL` | `CURRENT_TIMESTAMP` |
| `ClassId` | `integer` | `NOT NULL`, `REFERENCES "Classes"("Id")` | |
| `IsActive` | `boolean` | `NOT NULL` | `true` |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | `CURRENT_TIMESTAMP` |
| `UpdatedAt` | `timestamp with time zone` | | |

**Foreign Keys:**
- `ClassId` → `Classes(Id)` (CASCADE on delete)

**Indexes:**
- `IX_Students_ClassId`
- `IX_Students_Email`

### 4.6 Table: `Attendance` (Student Attendance)

| Column | Type | Constraints | Default |
|--------|------|-------------|---------|
| `Id` | `integer` | `PRIMARY KEY`, `GENERATED BY DEFAULT AS IDENTITY` | |
| `StudentId` | `integer` | `NOT NULL`, `REFERENCES "Students"("Id")` | |
| `AttendanceDate` | `timestamp with time zone` | `NOT NULL` | |
| `Status` | `text` | `NOT NULL` | `'Present'` |
| `Remarks` | `text` | | |
| `MarkedAt` | `timestamp with time zone` | `NOT NULL` | |
| `MarkedBy` | `integer` | | |

**Foreign Keys:**
- `StudentId` → `Students(Id)` (CASCADE on delete)

**Indexes:**
- `IX_Attendance_StudentId`

### 4.7 Table: `Exams`

| Column | Type | Constraints | Default |
|--------|------|-------------|---------|
| `Id` | `integer` | `PRIMARY KEY`, `GENERATED BY DEFAULT AS IDENTITY` | |
| `ExamName` | `character varying(100)` | `NOT NULL` | |
| `ExamCode` | `character varying(20)` | `NOT NULL` | |
| `ExamType` | `character varying(50)` | `NOT NULL` | |
| `AcademicYear` | `character varying(20)` | `NOT NULL` | |
| `Section` | `character varying(20)` | `NOT NULL` | |
| `Description` | `character varying(500)` | | |
| `IsActive` | `boolean` | `NOT NULL` | `true` |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | |
| `UpdatedAt` | `timestamp with time zone` | | |

### 4.8 Table: `TeacherAttendances`

| Column | Type | Constraints | Default |
|--------|------|-------------|---------|
| `Id` | `integer` | `PRIMARY KEY`, `GENERATED BY DEFAULT AS IDENTITY` | |
| `TeacherId` | `integer` | `NOT NULL`, `REFERENCES "Teacher"("Id")` | |
| `Date` | `date` | `NOT NULL` | |
| `Status` | `integer` | `NOT NULL` | |
| `CheckInTime` | `timestamp with time zone` | | |
| `CheckOutTime` | `timestamp with time zone` | | |
| `TotalWorkingHours` | `numeric(5,2)` | | |
| `OvertimeHours` | `numeric(5,2)` | | |
| `Remarks` | `character varying(500)` | | |
| `IsApproved` | `boolean` | `NOT NULL` | `false` |
| `ApprovedBy` | `integer` | | |
| `ApprovedDate` | `timestamp with time zone` | | |
| `RejectionReason` | `character varying(200)` | | |
| `DocumentPath` | `character varying(500)` | | |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | `CURRENT_TIMESTAMP` |
| `CreatedBy` | `integer` | | |
| `UpdatedAt` | `timestamp with time zone` | | |
| `UpdatedBy` | `integer` | | |
| `RowVersion` | `bytea` | | |

**Foreign Keys:**
- `TeacherId` → `Teacher(Id)` (RESTRICT on delete)

**Indexes:**
- `IX_TeacherAttendance_Unique` — UNIQUE on `(TeacherId, Date)`
- `IX_TeacherAttendance_TeacherId`
- `IX_TeacherAttendance_Date`
- `IX_TeacherAttendance_Status`

### 4.9 Table: `FeeTypes`

| Column | Type | Constraints | Default |
|--------|------|-------------|---------|
| `Id` | `integer` | `PRIMARY KEY`, `GENERATED BY DEFAULT AS IDENTITY` | |
| `FeeTypeName` | `character varying(100)` | `NOT NULL` | |
| `Description` | `character varying(500)` | | |
| `IsActive` | `boolean` | `NOT NULL` | `true` |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | `CURRENT_TIMESTAMP` |
| `UpdatedAt` | `timestamp with time zone` | | |

### 4.10 Table: `FeeStructures`

| Column | Type | Constraints | Default |
|--------|------|-------------|---------|
| `Id` | `integer` | `PRIMARY KEY`, `GENERATED BY DEFAULT AS IDENTITY` | |
| `ClassId` | `integer` | `NOT NULL`, `REFERENCES "Classes"("Id")` | |
| `FeeTypeId` | `integer` | `NOT NULL`, `REFERENCES "FeeTypes"("Id")` | |
| `Amount` | `numeric(18,2)` | `NOT NULL` | |
| `DueDate` | `timestamp with time zone` | | |
| `Description` | `character varying(500)` | | |
| `IsActive` | `boolean` | `NOT NULL` | `true` |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | `CURRENT_TIMESTAMP` |
| `UpdatedAt` | `timestamp with time zone` | | |

**Foreign Keys:**
- `ClassId` → `Classes(Id)` (RESTRICT on delete)
- `FeeTypeId` → `FeeTypes(Id)` (RESTRICT on delete)

**Indexes:**
- UNIQUE on `(ClassId, FeeTypeId)`

### 4.11 Table: `FeeCollections`

| Column | Type | Constraints | Default |
|--------|------|-------------|---------|
| `Id` | `integer` | `PRIMARY KEY`, `GENERATED BY DEFAULT AS IDENTITY` | |
| `StudentId` | `integer` | `NOT NULL`, `REFERENCES "Students"("Id")` | |
| `FeeStructureId` | `integer` | `NOT NULL`, `REFERENCES "FeeStructures"("Id")` | |
| `TotalAmount` | `numeric(18,2)` | `NOT NULL` | |
| `PaidAmount` | `numeric(18,2)` | `NOT NULL` | `0` |
| `RemainingBalance` | `numeric(18,2)` | `NOT NULL` | |
| `DueDate` | `timestamp with time zone` | `NOT NULL` | |
| `Status` | `character varying(20)` | `NOT NULL` | `'Pending'` |
| `Remarks` | `character varying(500)` | | |
| `IsActive` | `boolean` | `NOT NULL` | `true` |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | `CURRENT_TIMESTAMP` |
| `UpdatedAt` | `timestamp with time zone` | | |

**Foreign Keys:**
- `StudentId` → `Students(Id)` (RESTRICT on delete)
- `FeeStructureId` → `FeeStructures(Id)` (RESTRICT on delete)

**Indexes:**
- UNIQUE on `(StudentId, FeeStructureId)`

### 4.12 Table: `PaymentTransactions`

| Column | Type | Constraints | Default |
|--------|------|-------------|---------|
| `Id` | `integer` | `PRIMARY KEY`, `GENERATED BY DEFAULT AS IDENTITY` | |
| `FeeCollectionId` | `integer` | `NOT NULL`, `REFERENCES "FeeCollections"("Id")` | |
| `StudentId` | `integer` | `NOT NULL`, `REFERENCES "Students"("Id")` | |
| `AmountPaid` | `numeric(18,2)` | `NOT NULL` | |
| `PaymentDate` | `timestamp with time zone` | `NOT NULL` | |
| `PaymentMethod` | `character varying(50)` | `NOT NULL` | `'Cash'` |
| `TransactionReference` | `character varying(100)` | | |
| `Remarks` | `character varying(500)` | | |
| `IsCancelled` | `boolean` | `NOT NULL` | `false` |
| `CancelledAt` | `timestamp with time zone` | | |
| `CancelledBy` | `integer` | | |
| `CreatedAt` | `timestamp with time zone` | `NOT NULL` | `CURRENT_TIMESTAMP` |

**Foreign Keys:**
- `FeeCollectionId` → `FeeCollections(Id)` (RESTRICT on delete)
- `StudentId` → `Students(Id)` (RESTRICT on delete)

**Indexes:**
- `IX_PaymentTransactions_PaymentDate`

---

## 5. Migration History

### 5.1 Migration 1: `20260609071202_initialcreate`

**Date:** June 9, 2026

**Tables Created:**
- `Teachers` (Entities.TeacherEntity)
- `Users`
- `Classes`
- `Students`
- `Attendance`

**Foreign Keys Added:**
- `Classes.ClassTeacherId` → `Teachers.Id` (optional)
- `Students.ClassId` → `Classes.Id` (CASCADE)
- `Attendance.StudentId` → `Students.Id` (CASCADE)

**Indexes Created:**
| Index Name | Table | Columns | Unique |
|------------|-------|---------|--------|
| `IX_Attendance_StudentId` | Attendance | StudentId | No |
| `IX_Classes_ClassTeacherId` | Classes | ClassTeacherId | No |
| `IX_Students_ClassId` | Students | ClassId | No |
| `IX_Students_Email` | Students | Email | No |
| `IX_Teachers_Email` | Teachers | Email | Yes |
| `IX_Users_Email` | Users | Email | Yes |
| `IX_Users_Username` | Users | Username | Yes |

### 5.2 Migration 2: `20260610070635_InitialTeacherAttendance`

**Date:** June 10, 2026

**Tables Created:**
- `Exams`
- `Teacher` (Models.Teacher — distinct from `Teachers` table)
- `TeacherAttendances`

**Foreign Keys Added:**
- `TeacherAttendances.TeacherId` → `Teacher.Id` (RESTRICT)

**Indexes Created:**
| Index Name | Table | Columns | Unique |
|------------|-------|---------|--------|
| `IX_TeacherAttendance_Unique` | TeacherAttendances | TeacherId, Date | Yes |
| `IX_TeacherAttendance_TeacherId` | TeacherAttendances | TeacherId | No |
| `IX_TeacherAttendance_Date` | TeacherAttendances | Date | No |
| `IX_TeacherAttendance_Status` | TeacherAttendances | Status | No |

### 5.3 Migration 3: `20260611020534_AddRollNumberToStudents`

**Date:** June 11, 2026

**Changes:**
- Added `RollNumber` column (`character varying(20)`, nullable) to `Students` table

### 5.4 Current Database State (After All Migrations)

```
Tables in database: [Attendance, Classes, Exams, Students, Teacher, TeacherAttendances, Teachers, Users]
Tables NOT yet migrated (code only): [FeeTypes, FeeStructures, FeeCollections, PaymentTransactions]
```

---

## 6. Controllers and Actions

### 6.1 `AccountController` — Route: `/Account`

| HTTP | Action | Route | Auth Required | Description |
|------|--------|-------|---------------|-------------|
| GET | `Login` | `/Account/Login` | No | Login page (redirects to Dashboard if already logged in) |
| POST | `Login` | `/Account/Login` | No | Authenticate user via email/password |
| GET | `Register` | `/Account/Register` | No | Disabled — redirects to Login |
| POST | `Register` | `/Account/Register` | No | Disabled |
| GET | `AddUser` | `/Account/AddUser` | Admin | Loads user creation form |
| POST | `AddUser` | `/Account/AddUser` | Admin | Creates new user |
| GET | `Users` | `/Account/Users` | Admin | Lists all users |
| GET | `CheckUsernameAvailability` | `/Account/CheckUsernameAvailability` | No | AJAX username availability check |
| GET | `CheckEmailAvailability` | `/Account/CheckEmailAvailability` | No | AJAX email availability check |
| POST | `Logout` | `/Account/Logout` | Yes | Clears session, redirects to Login |
| GET | `AccessDenied` | `/Account/AccessDenied` | No | Access denied page |
| GET | `ForgetPassword` | `/Account/ForgetPassword` | No | Password reset request form |
| POST | `ForgetPassword` | `/Account/ForgetPassword` | No | Verify email for password reset |
| GET | `ResetPassword` | `/Account/ResetPassword` | No | Password reset form |
| POST | `ResetPassword` | `/Account/ResetPassword` | No | Save new password |

### 6.2 `HomeController` — Route: `/Home`

| HTTP | Action | Route | Auth Required | Description |
|------|--------|-------|---------------|-------------|
| GET | `Dashboard` | `/Home/Dashboard` | Yes | Main dashboard with statistics |
| GET | `Privacy` | `/Home/Privacy` | No | Privacy page |
| GET | `Error` | `/Home/Error` | No | Error page |

### 6.3 `ClassController` — Route: `/Class`

| HTTP | Action | Route | Auth Required | Description |
|------|--------|-------|---------------|-------------|
| GET | `Index` | `/Class/Index` | Yes | List all classes |
| GET | `AddClass` | `/Class/AddClass` | Yes | Load add class form |
| POST | `AddClass` | `/Class/AddClass` | Yes | Create new class |
| GET | `EditClass` | `/Class/EditClass/{id}` | Yes | Load edit class form |
| POST | `EditClass` | `/Class/EditClass/{id}` | Yes | Update class |
| POST | `DeleteClass` | `/Class/DeleteClass/{id}` | Yes | Soft-delete class (AJAX) |
| GET | `Details` | `/Class/Details/{id}` | Yes | Class details page |

### 6.4 `StudentsController` — Route: `/Students`

| HTTP | Action | Route | Auth Required | Description |
|------|--------|-------|---------------|-------------|
| GET | `AddStudent` | `/Students/AddStudent` | Yes | Load add student form |
| POST | `AddStudent` | `/Students/AddStudent` | Yes | Create new student |
| GET | `StudentList` | `/Students/StudentList` | Yes | List all students with search |
| GET | `EditStudent` | `/Students/EditStudent/{id}` | Yes | Load edit student form |
| POST | `EditStudent` | `/Students/EditStudent/{id}` | Yes | Update student |
| POST | `DeleteStudent` | `/Students/DeleteStudent/{id}` | Yes | Soft-delete student (AJAX) |
| GET | `Details` | `/Students/Details/{id}` | Yes | Student details page |
| GET | `ExportToExcel` | `/Students/ExportToExcel` | Yes | Export students to CSV |
| GET | `Index` | `/Students/Index` | Yes | Redirects to StudentList |

### 6.5 `TeachersController` — Route: `/Teachers`

| HTTP | Action | Route | Auth Required | Description |
|------|--------|-------|---------------|-------------|
| GET | `Index` | `/Teachers` | Yes | Redirects to TeacherList |
| GET | `TeacherList` | `/Teachers/TeacherList` | Yes | List all teachers |
| GET | `AddTeacher` | `/Teachers/AddTeacher` | Yes | Load add teacher form |
| POST | `AddTeacher` | `/Teachers/AddTeacher` | Yes | Create new teacher (Entities.TeacherEntity) |
| GET | `EditTeacher` | `/Teachers/EditTeacher/{id}` | Yes | Load edit teacher form |
| POST | `EditTeacher` | `/Teachers/EditTeacher/{id}` | Yes | Update teacher |
| POST | `DeleteTeacher` | `/Teachers/DeleteTeacher/{id}` | Yes | Soft-delete teacher (AJAX) |

### 6.6 `StudentAttendanceController` — Route: `/StudentAttendance`

| HTTP | Action | Route | Auth Required | Description |
|------|--------|-------|---------------|-------------|
| GET | `StudentAttendance` | `/StudentAttendance/StudentAttendance` | Yes | Daily attendance sheet |
| POST | `SaveAttendance` | `/StudentAttendance/SaveAttendance` | Yes | Save/update attendance |
| GET | `StudentHistory` | `/StudentAttendance/StudentHistory/{id}` | Yes | Student attendance history |
| POST | `CopyFromPrevious` | `/StudentAttendance/CopyFromPrevious` | Yes | Copy attendance from previous day |
| GET | `AttendanceReport` | `/StudentAttendance/AttendanceReport` | Yes | Attendance report |
| GET | `CheckStatus` | `/StudentAttendance/CheckStatus` | Yes | Check if attendance already marked |

### 6.7 `TeacherAttendanceController` — Route: `/TeacherAttendance`

| HTTP | Action | Route | Auth Required | Description |
|------|--------|-------|---------------|-------------|
| GET | `Index` | `/TeacherAttendance/Index` | Yes | Daily teacher attendance marking |
| POST | `MarkAttendance` | `/TeacherAttendance/MarkAttendance` | Yes | Mark single teacher attendance |
| POST | `MarkBulkAttendance` | `/TeacherAttendance/MarkBulkAttendance` | Yes | Mark bulk attendance |
| GET | `TeacherWise` | `/TeacherAttendance/TeacherWise` | Yes | Teacher-wise attendance summary |
| GET | `MonthlyReport` | `/TeacherAttendance/MonthlyReport` | Yes | Monthly attendance report |
| GET | `SalaryCalculation` | `/TeacherAttendance/SalaryCalculation` | Yes | Salary calculation |
| GET | `Edit` | `/TeacherAttendance/Edit/{id}` | Yes | Edit attendance record |
| POST | `Edit` | `/TeacherAttendance/Edit/{id}` | Yes | Update attendance record |
| POST | `Delete` | `/TeacherAttendance/Delete/{id}` | Yes | Delete attendance record |
| POST | `Approve` | `/TeacherAttendance/Approve/{id}` | Yes | Approve attendance |
| POST | `Reject` | `/TeacherAttendance/Reject/{id}` | Yes | Reject attendance with reason |
| GET | `ExportMonthlyReport` | `/TeacherAttendance/ExportMonthlyReport` | Yes | Export monthly report to CSV |
| GET | `ExportSalaryReport` | `/TeacherAttendance/ExportSalaryReport` | Yes | Export salary report to CSV |

### 6.8 `ExamsController` — Route: `/Exams`

| HTTP | Action | Route | Auth Required | Description |
|------|--------|-------|---------------|-------------|
| GET | `Index` | `/Exams/Index` | Yes | List exams with filtering/pagination |
| GET | `Create` | `/Exams/Create` | Yes | Load create exam form |
| POST | `Create` | `/Exams/Create` | Yes | Create new exam |
| GET | `Edit` | `/Exams/Edit/{id}` | Yes | Load edit exam form |
| POST | `Edit` | `/Exams/Edit/{id}` | Yes | Update exam |
| GET | `Details` | `/Exams/Details/{id}` | Yes | Exam details |
| POST | `Delete` | `/Exams/Delete/{id}` | Yes | Delete exam (AJAX) |
| POST | `ToggleStatus` | `/Exams/ToggleStatus/{id}` | Yes | Toggle exam active status (AJAX) |
| GET | `Dashboard` | `/Exams/Dashboard` | Yes | Exam dashboard with statistics |

### 6.9 `FeesController` — Route: `/Fees`

| HTTP | Action | Route | Auth Required | Description |
|------|--------|-------|---------------|-------------|
| GET | `Index` | `/Fees/Index` | Yes | Fee management dashboard |
| GET | `Create` | `/Fees/Create` | Yes | Load create fee structure form |
| POST | `Create` | `/Fees/Create` | Yes | Create new fee structure |
| GET | `Edit` | `/Fees/Edit/{id}` | Yes | Load edit fee structure form |
| POST | `Edit` | `/Fees/Edit/{id}` | Yes | Update fee structure |
| POST | `Delete` | `/Fees/Delete/{id}` | Yes | Delete fee structure (AJAX) |
| POST | `CreateFeeType` | `/Fees/CreateFeeType` | Yes | Create fee type via AJAX |
| GET | `Collections` | `/Fees/Collections` | Yes | List fee collections with filters |
| POST | `GenerateCollection` | `/Fees/GenerateCollection` | Yes | Generate fee collection for a student |
| POST | `GenerateBulkCollection` | `/Fees/GenerateBulkCollection` | Yes | Generate fee collections for all students in a class |
| GET | `Payment` | `/Fees/Payment/{collectionId}` | Yes | Payment page |
| POST | `Payment` | `/Fees/Payment` | Yes | Process payment |
| GET | `Receipt` | `/Fees/Receipt/{transactionId}` | Yes | Payment receipt |
| POST | `CancelPayment` | `/Fees/CancelPayment` | Yes | Cancel payment (AJAX) |
| GET | `Reports` | `/Fees/Reports` | Yes | Fee reports |
| GET | `GetStudentsByClass` | `/Fees/GetStudentsByClass` | No | AJAX: students by class |
| GET | `GetFeeStructuresByClass` | `/Fees/GetFeeStructuresByClass` | No | AJAX: fee structures by class |

### 6.10 `UserManagementController` — Route: `/UserManagement`

| HTTP | Action | Route | Auth Required | Description |
|------|--------|-------|---------------|-------------|
| GET | `Index` | `/UserManagement` | Admin | List all users |
| GET | `Details` | `/UserManagement/Details/{id}` | Admin | User details |
| GET | `Create` | `/UserManagement/Create` | Admin | Redirects to Account/AddUser |
| POST | `Create` | `/UserManagement/Create` | Admin | Create user (duplicate of Account/AddUser) |
| GET | `Edit` | `/UserManagement/Edit/{id}` | Admin | Load edit user form |
| POST | `Edit` | `/UserManagement/Edit/{id}` | Admin | Update user |
| POST | `Delete` | `/UserManagement/Delete/{id}` | Admin | Delete user (AJAX) |
| POST | `ToggleStatus` | `/UserManagement/ToggleStatus/{id}` | Admin | Toggle user active status (AJAX) |
| GET | `Search` | `/UserManagement/Search` | Admin | Search users (AJAX) |
| GET | `Export` | `/UserManagement/Export` | Admin | Export users to CSV |

---

## 7. Service Layer

### 7.1 Service Interface Summary

| Interface | Implementation | Key Methods |
|-----------|---------------|-------------|
| `IUserService` | `UserService` | RegisterUser, LoginUser, LogoutUser, GetAllUsers, GetUserByEmail, IsEmailExists, VerifyUserForReset, ResetPasswordDirect |
| `IStudentService` | `StudentService` | AddStudent, GetAllStudents, GetStudentById, UpdateStudent, DeleteStudent, IsEmailExists, IsRollNumberExists |
| `ITeacherService` | `TeacherService` | AddTeacher, GetAllTeachers, GetTeacherById, UpdateTeacher, DeleteTeacher, IsEmailExists |
| `IAttendanceService` | `AttendanceService` | GetAttendanceSheet, SaveAttendance, GetStudentHistory, GetAttendanceReport, CopyFromPreviousDay |
| `IClassService` | `ClassService` | GetAllClasses, GetClassById, CreateClass, UpdateClass, DeleteClass |
| `IExamService` | `ExamService` | GetAllExams, GetExamById, CreateExam, UpdateExam, DeleteExam, ToggleExamStatus, GenerateUniqueExamCode, GetExamDashboardStats, GetFilteredExams |
| `ITeacherAttendanceService` | `TeacherAttendanceService` | GetDailyAttendance, MarkAttendance, MarkBulkAttendance, GetTeacherWiseAttendance, GetMonthlyReport, CalculateAllSalaries, CalculateTeacherSalary, ApproveAttendance |
| `IFeeService` | `FeeService` | GetAllFeeTypes, GetAllFeeStructures, CreateFeeStructure, GetFeeCollections, GenerateFeeCollection, MakePayment, GetReceipt, CancelPayment, GetFeeReport, GetDashboardData |

### 7.2 Service Registration (Program.cs)

All services are registered as **Scoped** (one instance per HTTP request):

```csharp
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<ITeacherAttendanceService, TeacherAttendanceService>();
builder.Services.AddScoped<IFeeService, FeeService>();
```

---

## 8. View Structure

### 8.1 View Directory Layout

```
Views/
├── _ViewStart.cshtml              — Layout setup (default: _Layout.cshtml)
├── _ViewImports.cshtml            — Using directives & tag helpers
├── Shared/
│   ├── _Layout.cshtml             — Main layout (Bootstrap 5 + Font Awesome 6)
│   ├── Error.cshtml               — Error page
│   └── _ValidationScriptsPartial.cshtml
├── Account/
│   ├── Login.cshtml               — Login form
│   ├── ForgetPassword.cshtml      — Password reset request
│   └── ResetPassword.cshtml       — New password form
├── Home/
│   ├── Dashboard.cshtml           — Main dashboard with cards/charts
│   ├── Index.cshtml               — Home page
│   └── Privacy.cshtml             — Privacy policy
├── Classes/
│   ├── Index.cshtml               — Class list
│   ├── AddClass.cshtml            — Create class form
│   ├── EditClass.cshtml           — Edit class form
│   └── ClassList.cshtml           — Alternative class list
├── Students/
│   ├── StudentList.cshtml         — Student list with search
│   ├── AddStudent.cshtml          — Create student form
│   └── EditStudent.cshtml         — Edit student form
├── Teachers/
│   ├── TeacherList.cshtml         — Teacher list
│   ├── AddTeacher.cshtml          — Create teacher form
│   └── EditTeacher.cshtml         — Edit teacher form
├── StudentAttendance/
│   ├── StudentAttendance.cshtml   — Daily attendance sheet
│   └── AttendanceReport.cshtml    — Attendance report
├── TeacherAttendance/
│   ├── Index.cshtml               — Daily teacher attendance
│   ├── SelectTeacher.cshtml       — Teacher selection
│   ├── MonthlyReport.cshtml       — Monthly report
│   ├── SalaryCalculation.cshtml   — Salary calculation
│   └── Edit.cshtml                — Edit attendance record
├── Exams/
│   ├── Index.cshtml               — Exam list
│   ├── Create.cshtml              — Create exam form
│   ├── Edit.cshtml                — Edit exam form
│   ├── Details.cshtml             — Exam details
│   └── Dashboard.cshtml           — Exam dashboard
├── Fees/
│   ├── Index.cshtml               — Fee structure management
│   ├── Create.cshtml              — Create fee structure
│   ├── Edit.cshtml                — Edit fee structure
│   ├── Collections.cshtml         — Fee collections
│   ├── Payment.cshtml             — Payment form
│   ├── Receipt.cshtml             — Payment receipt
│   └── Reports.cshtml             — Fee reports
├── UserManagement/
│   ├── UserList.cshtml            — User list
│   ├── AddUser.cshtml             — Create user form
│   └── EditUser.cshtml            — Edit user form
└── Pages/
    └── StudentAttendance/
        └── Index.cshtml           — (Alternate path?)
```

---

## 9. Configuration

### 9.1 `Program.cs` — Startup Pipeline

```csharp
// Services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => { ... });
builder.Services.AddHttpContextAccessor();
// Register all services (scoped)

// Middleware Pipeline
app.UseExceptionHandler("/Home/Error");   // Non-Development only
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllerRoute("default", "{controller=Account}/{action=Login}/{id?}");
```

### 9.2 `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=SmartTuitionManagementDB;Username=postgres;Password=Pass@123"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Authentication": {
    "Cookie": {
      "ExpireTimeSpan": "7",
      "SlidingExpiration": true
    }
  }
}
```

### 9.3 `launchSettings.json`

```json
{
  "profiles": {
    "http": { "applicationUrl": "http://0.0.0.0:5070" },
    "https": { "applicationUrl": "https://localhost:7214;http://localhost:5070" }
  }
}
```

### 9.4 NuGet Packages

```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.8" />
<PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="10.0.8" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.8" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.8" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.2" />
```

---

## 10. Route Map

### 10.1 Default Route

```
{controller=Account}/{action=Login}/{id?}
```

### 10.2 All Application Routes

| Pattern | Controller | Action |
|---------|-----------|--------|
| `/` | AccountController | Login |
| `/Account/Login` | AccountController | Login |
| `/Account/Register` | AccountController | Register |
| `/Account/AddUser` | AccountController | AddUser |
| `/Account/Users` | AccountController | Users |
| `/Account/Logout` | AccountController | Logout |
| `/Account/AccessDenied` | AccountController | AccessDenied |
| `/Account/ForgetPassword` | AccountController | ForgetPassword |
| `/Account/ResetPassword` | AccountController | ResetPassword |
| `/Home/Dashboard` | HomeController | Dashboard |
| `/Class/Index` | ClassController | Index |
| `/Class/AddClass` | ClassController | AddClass |
| `/Class/EditClass/{id}` | ClassController | EditClass |
| `/Class/DeleteClass/{id}` | ClassController | DeleteClass |
| `/Class/Details/{id}` | ClassController | Details |
| `/Students/StudentList` | StudentsController | StudentList |
| `/Students/AddStudent` | StudentsController | AddStudent |
| `/Students/EditStudent/{id}` | StudentsController | EditStudent |
| `/Students/DeleteStudent/{id}` | StudentsController | DeleteStudent |
| `/Students/Details/{id}` | StudentsController | Details |
| `/Students/ExportToExcel` | StudentsController | ExportToExcel |
| `/Teachers` | TeachersController | Index |
| `/Teachers/TeacherList` | TeachersController | TeacherList |
| `/Teachers/AddTeacher` | TeachersController | AddTeacher |
| `/Teachers/EditTeacher/{id}` | TeachersController | EditTeacher |
| `/Teachers/DeleteTeacher/{id}` | TeachersController | DeleteTeacher |
| `/StudentAttendance/StudentAttendance` | StudentAttendanceController | StudentAttendance |
| `/StudentAttendance/SaveAttendance` | StudentAttendanceController | SaveAttendance |
| `/StudentAttendance/StudentHistory/{id}` | StudentAttendanceController | StudentHistory |
| `/StudentAttendance/AttendanceReport` | StudentAttendanceController | AttendanceReport |
| `/TeacherAttendance/Index` | TeacherAttendanceController | Index |
| `/TeacherAttendance/MarkAttendance` | TeacherAttendanceController | MarkAttendance |
| `/TeacherAttendance/MarkBulkAttendance` | TeacherAttendanceController | MarkBulkAttendance |
| `/TeacherAttendance/TeacherWise` | TeacherAttendanceController | TeacherWise |
| `/TeacherAttendance/MonthlyReport` | TeacherAttendanceController | MonthlyReport |
| `/TeacherAttendance/SalaryCalculation` | TeacherAttendanceController | SalaryCalculation |
| `/TeacherAttendance/Edit/{id}` | TeacherAttendanceController | Edit |
| `/TeacherAttendance/Delete/{id}` | TeacherAttendanceController | Delete |
| `/TeacherAttendance/Approve/{id}` | TeacherAttendanceController | Approve |
| `/TeacherAttendance/Reject/{id}` | TeacherAttendanceController | Reject |
| `/Exams/Index` | ExamsController | Index |
| `/Exams/Create` | ExamsController | Create |
| `/Exams/Edit/{id}` | ExamsController | Edit |
| `/Exams/Details/{id}` | ExamsController | Details |
| `/Exams/Delete/{id}` | ExamsController | Delete |
| `/Exams/ToggleStatus/{id}` | ExamsController | ToggleStatus |
| `/Exams/Dashboard` | ExamsController | Dashboard |
| `/Fees/Index` | FeesController | Index |
| `/Fees/Create` | FeesController | Create |
| `/Fees/Edit/{id}` | FeesController | Edit |
| `/Fees/Delete/{id}` | FeesController | Delete |
| `/Fees/Collections` | FeesController | Collections |
| `/Fees/Payment` | FeesController | Payment |
| `/Fees/Receipt/{transactionId}` | FeesController | Receipt |
| `/Fees/Reports` | FeesController | Reports |
| `/UserManagement` | UserManagementController | Index |
| `/UserManagement/Details/{id}` | UserManagementController | Details |
| `/UserManagement/Edit/{id}` | UserManagementController | Edit |
| `/UserManagement/Delete/{id}` | UserManagementController | Delete |
| `/UserManagement/ToggleStatus/{id}` | UserManagementController | ToggleStatus |

---

## 11. Key Architecture Notes

### 11.1 Dual Teacher System ⚠️

The project has **two separate teacher tables** that serve different purposes:

| Aspect | `Teachers` (Entities.TeacherEntity) | `Teacher` (Models.Teacher) |
|--------|--------------------------------------|----------------------------|
| **Namespace** | `SmartTuitionManagementSystem.Entities` | `SmartTuitionManagementSystem.Models` |
| **Table Name** | `Teachers` | `Teacher` |
| **Name Field** | `FullName` (single field) | `FirstName` + `LastName` (two fields) |
| **Referenced By** | `Classes.ClassTeacherId` | `TeacherAttendances.TeacherId` |
| **Used For** | Class teacher assignment | Teacher attendance & salary calculation |
| **Status** | Fully migrated | Fully migrated |

**These are separate tables with no relationship between them.** Data must be entered separately into each.

### 11.2 Session-Based Authentication

- No ASP.NET Core Identity used
- Password hashing: SHA256 (not bcrypt/argon2)
- Session stores: `UserId`, `Username`, `UserEmail`, `UserRole`, `UserName`
- Admin check: `HttpContext.Session.GetString("UserRole") == "Admin"`

### 11.3 Fee Entities Not Yet Migrated

The following entities are fully defined in code (entities, DbContext config, services, controllers, views) but **no migration has been created** to add them to the database:
- `FeeTypes` table
- `FeeStructures` table
- `FeeCollections` table
- `PaymentTransactions` table

### 11.4 Soft Delete Pattern

Most entities use soft delete:
- `StudentEntity.IsActive`
- `TeacherEntity.IsActive`
- `ClassEntity.IsActive`
- `FeeTypeEntity.IsActive`
- `FeeStructureEntity.IsActive`
- `FeeCollectionEntity.IsActive`

**Hard delete used for:** `ExamEntity`, `TeacherAttendance`, `PaymentTransactionEntity` (though `IsCancelled` flag exists for transactions)

### 11.5 Concurrency Handling

- `TeacherAttendance` entity has `RowVersion` (`[Timestamp]` attribute) for optimistic concurrency
- No other entities have concurrency tokens

### 11.6 Audit Fields

Standard audit fields across entities:
- `CreatedAt` (DateTime)
- `UpdatedAt` (DateTime?, nullable)
- `CreatedBy` (int?, on TeacherAttendance and Teacher model)
- `UpdatedBy` (int?, on TeacherAttendance and Teacher model)

### 11.7 Missing/Commented-Out Features

- `ClassEntity` has a commented-out `SubjectAssignments` navigation property (referencing a non-existent `SubjectAssignmentEntity`)
- `Grade` column exists in the initial migration for `Students` but was removed from the entity class
- Subject assignment, subject management, and class enrollment features are not fully implemented

---

## 12. Source Code Listings

### 12.1 Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Services;
using SmartTuitionManagementSystem.Services.Interface;
using SmartTuitionManagementSystem.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext for PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register all services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<ITeacherAttendanceService, TeacherAttendanceService>();
builder.Services.AddScoped<IFeeService, FeeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
```

### 12.2 ApplicationDbContext.cs

```csharp
using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;
using TeacherAttendanceSystem.Entities;
using TeacherEntity = SmartTuitionManagementSystem.Entities.TeacherEntity;

namespace SmartTuitionManagementSystem.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ClassEntity> Classes { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<StudentEntity> Students { get; set; }
    public DbSet<TeacherEntity> Teachers { get; set; }
    public DbSet<StudentAttendanceEntity> Attendances { get; set; }
    public DbSet<ExamEntity> Exams { get; set; }
    public DbSet<TeacherAttendance> TeacherAttendances { get; set; }
    
    // Fee Management
    public DbSet<FeeTypeEntity> FeeTypes { get; set; }
    public DbSet<FeeStructureEntity> FeeStructures { get; set; }
    public DbSet<FeeCollectionEntity> FeeCollections { get; set; }
    public DbSet<PaymentTransactionEntity> PaymentTransactions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -- Users --
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("Users");
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // -- Students --
        modelBuilder.Entity<StudentEntity>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email);
            entity.Property(e => e.EnrollmentDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // -- Teachers (Entities) --
        modelBuilder.Entity<TeacherEntity>(entity =>
        {
            entity.ToTable("Teachers");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.HireDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
        });
        
        // -- Student Attendance --
        modelBuilder.Entity<StudentAttendanceEntity>(entity =>
        {
            entity.ToTable("Attendance");
            entity.HasKey(e => e.Id);
            
            entity.HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        // -- Teacher Attendance (Models) --
        modelBuilder.Entity<TeacherAttendance>(entity =>
        {
            entity.ToTable("TeacherAttendances");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TeacherId).IsRequired();
            entity.Property(e => e.Date).IsRequired().HasColumnType("date");
            entity.Property(e => e.Status).IsRequired().HasConversion<int>();
            
            entity.Property(e => e.CheckInTime).HasColumnType("timestamp");
            entity.Property(e => e.CheckOutTime).HasColumnType("timestamp");
            
            entity.Property(e => e.TotalWorkingHours).HasColumnType("decimal(5,2)");
            entity.Property(e => e.OvertimeHours).HasColumnType("decimal(5,2)");
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.IsApproved).HasDefaultValue(false);
            entity.Property(e => e.ApprovedDate).HasColumnType("timestamp");
            entity.Property(e => e.RejectionReason).HasMaxLength(200);
            entity.Property(e => e.DocumentPath).HasMaxLength(500);
            
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnType("timestamp");
            
            entity.HasIndex(e => new { e.TeacherId, e.Date })
                .IsUnique()
                .HasDatabaseName("IX_TeacherAttendance_Unique");
            
            entity.HasIndex(e => e.TeacherId).HasDatabaseName("IX_TeacherAttendance_TeacherId");
            entity.HasIndex(e => e.Date).HasDatabaseName("IX_TeacherAttendance_Date");
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_TeacherAttendance_Status");
            
            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.Attendances)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // -- Fee Types --
        modelBuilder.Entity<FeeTypeEntity>(entity =>
        {
            entity.ToTable("FeeTypes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FeeTypeName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // -- Fee Structures --
        modelBuilder.Entity<FeeStructureEntity>(entity =>
        {
            entity.ToTable("FeeStructures");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(f => f.Class)
                .WithMany()
                .HasForeignKey(f => f.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.FeeType)
                .WithMany(ft => ft.FeeStructures)
                .HasForeignKey(f => f.FeeTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(f => new { f.ClassId, f.FeeTypeId }).IsUnique();
        });

        // -- Fee Collections --
        modelBuilder.Entity<FeeCollectionEntity>(entity =>
        {
            entity.ToTable("FeeCollections");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0);
            entity.Property(e => e.RemainingBalance).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Pending");
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(f => f.Student)
                .WithMany()
                .HasForeignKey(f => f.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.FeeStructure)
                .WithMany(fs => fs.FeeCollections)
                .HasForeignKey(f => f.FeeStructureId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(f => new { f.StudentId, f.FeeStructureId }).IsUnique();
        });

        // -- Payment Transactions --
        modelBuilder.Entity<PaymentTransactionEntity>(entity =>
        {
            entity.ToTable("PaymentTransactions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TransactionReference).HasMaxLength(100);
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(t => t.FeeCollection)
                .WithMany(fc => fc.PaymentTransactions)
                .HasForeignKey(t => t.FeeCollectionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Student)
                .WithMany()
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(t => t.PaymentDate);
        });
    }
}
```

### 12.3 Entity Classes

#### UserEntity.cs

```csharp
namespace SmartTuitionManagementSystem.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Role { get; set; } = "User";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
```

#### TeacherEntity.cs (Entities/ — table: Teachers)

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Entities;

[Table("Teachers")]
public class TeacherEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Address")]
    public string Address { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [Display(Name = "Qualification")]
    public string Qualification { get; set; } = string.Empty;

    [Range(0, 50)]
    [Display(Name = "Years of Experience")]
    public int ExperienceYears { get; set; }

    [MaxLength(200)]
    [Display(Name = "Specialization")]
    public string Specialization { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Hire Date")]
    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999.99)]
    [Display(Name = "Salary")]
    public decimal Salary { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
}
```

#### TeacherEntity.cs (TeacherAttendanceSystem/ — table: Teachers)

```csharp
namespace TeacherAttendanceSystem.Entities;

[Table("Teachers")]
public class TeacherEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Required]
    [MaxLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }
    
    [Phone]
    [MaxLength(20)]
    public string Phone { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal BaseSalary { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PerDayRate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<TeacherAttendanceEntity> Attendances { get; set; }
    
    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";
}
```

#### StudentEntity.cs

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Entities;

[Table("Students")]
public class StudentEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    [Display(Name = "Roll Number")]
    public string RollNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Phone]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Address")]
    public string Address { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime? DateOfBirth { get; set; }

    [MaxLength(100)]
    [Display(Name = "Parent/Guardian Name")]
    public string ParentName { get; set; } = string.Empty;

    [MaxLength(20)]
    [Phone]
    [Display(Name = "Parent Phone")]
    public string ParentPhone { get; set; } = string.Empty;

    [Display(Name = "Enrollment Date")]
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "Class ID")]
    public int ClassId { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ClassId")]
    public virtual ClassEntity? Class { get; set; }
    
    public virtual ICollection<StudentAttendanceEntity> Attendances { get; set; } = new List<StudentAttendanceEntity>();
}
```

#### ClassEntity.cs

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Entities;

[Table("Classes")]
public class ClassEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Class name is required")]
    [MaxLength(50)]
    [Display(Name = "Class Name")]
    public string ClassName { get; set; } = string.Empty;

    [MaxLength(20)]
    [Display(Name = "Class Code")]
    public string ClassCode { get; set; } = string.Empty;

    [MaxLength(10)]
    [Display(Name = "Section")]
    public string Section { get; set; } = string.Empty;

    [NotMapped]
    public string FullClassName => $"{ClassName} {(string.IsNullOrEmpty(Section) ? "" : "- " + Section)}".Trim();

    [Range(1, 100)]
    [Display(Name = "Maximum Capacity")]
    public int MaxCapacity { get; set; } = 40;

    [Display(Name = "Current Strength")]
    public int CurrentStrength { get; set; } = 0;

    [MaxLength(20)]
    [Display(Name = "Room Number")]
    public string RoomNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Class Teacher ID")]
    public int? ClassTeacherId { get; set; }

    [MaxLength(20)]
    [Display(Name = "Academic Year")]
    public string AcademicYear { get; set; } = DateTime.Now.Year.ToString();

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ClassTeacherId")]
    public virtual TeacherEntity? ClassTeacher { get; set; }
    
    public virtual ICollection<StudentEntity> Students { get; set; } = new List<StudentEntity>();
}
```

#### StudentAttendanceEntity.cs

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartTuitionManagementSystem.Entities;

[Table("Attendance")]
public class StudentAttendanceEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int StudentId { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime AttendanceDate { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Present";
    
    [MaxLength(200)]
    public string Remarks { get; set; } = string.Empty;
    
    public DateTime MarkedAt { get; set; } = DateTime.UtcNow;
    
    public int? MarkedBy { get; set; }
    
    [ForeignKey("StudentId")]
    public virtual StudentEntity Student { get; set; } = new StudentEntity();
}
```

#### TeacherAttendanceEntity.cs (TeacherAttendanceSystem)

```csharp
namespace TeacherAttendanceSystem.Entities;

public enum AttendanceStatus
{
    [Display(Name = "Present")] Present = 1,
    [Display(Name = "Absent")] Absent = 2,
    [Display(Name = "Late")] Late = 3,
    [Display(Name = "Half Day")] HalfDay = 4,
    [Display(Name = "Leave")] Leave = 5,
    [Display(Name = "Holiday")] Holiday = 6,
    [Display(Name = "Weekend")] Weekend = 7
}

[Table("TeacherAttendances")]
public class TeacherAttendanceEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required]
    public int TeacherId { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    [Column(TypeName = "date")]
    public DateTime Date { get; set; }
    
    [Required]
    public AttendanceStatus Status { get; set; }
    
    [DataType(DataType.Time)]
    [Column(TypeName = "time")]
    public DateTime? CheckInTime { get; set; }
    
    [DataType(DataType.Time)]
    [Column(TypeName = "time")]
    public DateTime? CheckOutTime { get; set; }
    
    [Column(TypeName = "decimal(5,2)")]
    public decimal? TotalWorkingHours { get; set; }
    
    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string Remarks { get; set; }
    
    public bool IsApproved { get; set; } = false;
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    
    [Timestamp]
    public byte[] RowVersion { get; set; }
    
    [ForeignKey("TeacherId")]
    public virtual TeacherEntity Teacher { get; set; }
}
```

#### Teacher.cs (Models — table: Teacher)

```csharp
namespace SmartTuitionManagementSystem.Models;

public class Teacher
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Qualification { get; set; } = string.Empty;

    [Range(0, 50)]
    public int ExperienceYears { get; set; }

    [MaxLength(200)]
    public string Specialization { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, 999999.99)]
    public decimal Salary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PerDayRate { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    public virtual ICollection<TeacherAttendance> Attendances { get; set; } = new List<TeacherAttendance>();
}
```

#### TeacherAttendance.cs (Models — table: TeacherAttendances)

```csharp
namespace SmartTuitionManagementSystem.Models;

public enum AttendanceStatus
{
    [Display(Name = "Present")] Present = 1,
    [Display(Name = "Absent")] Absent = 2,
    [Display(Name = "Late")] Late = 3,
    [Display(Name = "Half Day")] HalfDay = 4,
    [Display(Name = "Leave")] Leave = 5,
}

[Table("TeacherAttendances")]
public class TeacherAttendance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey("Teacher")]
    public int TeacherId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Column(TypeName = "date")]
    public DateTime Date { get; set; }

    [Required]
    public AttendanceStatus Status { get; set; }

    [DataType(DataType.Time)]
    [Column(TypeName = "time")]
    public DateTime? CheckInTime { get; set; }

    [DataType(DataType.Time)]
    [Column(TypeName = "time")]
    public DateTime? CheckOutTime { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? TotalWorkingHours { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? OvertimeHours { get; set; }

    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string? Remarks { get; set; }

    public bool IsApproved { get; set; } = false;
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? RejectionReason { get; set; }
    public string? DocumentPath { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
```

#### ExamEntity.cs

```csharp
namespace SmartTuitionManagementSystem.Entities;

[Table("Exams")]
public class ExamEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string ExamName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string ExamCode { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string ExamType { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string AcademicYear { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Section { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
```

#### FeeTypeEntity.cs

```csharp
namespace SmartTuitionManagementSystem.Entities;

[Table("FeeTypes")]
public class FeeTypeEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FeeTypeName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<FeeStructureEntity> FeeStructures { get; set; } = new List<FeeStructureEntity>();
}
```

#### FeeStructureEntity.cs

```csharp
namespace SmartTuitionManagementSystem.Entities;

[Table("FeeStructures")]
public class FeeStructureEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int ClassId { get; set; }

    [Required]
    public int FeeTypeId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("ClassId")]
    public virtual ClassEntity? Class { get; set; }

    [ForeignKey("FeeTypeId")]
    public virtual FeeTypeEntity? FeeType { get; set; }

    public virtual ICollection<FeeCollectionEntity> FeeCollections { get; set; } = new List<FeeCollectionEntity>();
}
```

#### FeeCollectionEntity.cs

```csharp
namespace SmartTuitionManagementSystem.Entities;

[Table("FeeCollections")]
public class FeeCollectionEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int FeeStructureId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PaidAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal RemainingBalance { get; set; }

    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("StudentId")]
    public virtual StudentEntity? Student { get; set; }

    [ForeignKey("FeeStructureId")]
    public virtual FeeStructureEntity? FeeStructure { get; set; }

    public virtual ICollection<PaymentTransactionEntity> PaymentTransactions { get; set; } = new List<PaymentTransactionEntity>();
}
```

#### PaymentTransactionEntity.cs

```csharp
namespace SmartTuitionManagementSystem.Entities;

[Table("PaymentTransactions")]
public class PaymentTransactionEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int FeeCollectionId { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountPaid { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(50)]
    public string PaymentMethod { get; set; } = "Cash";

    [MaxLength(100)]
    public string? TransactionReference { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public bool IsCancelled { get; set; } = false;
    public DateTime? CancelledAt { get; set; }
    public int? CancelledBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("FeeCollectionId")]
    public virtual FeeCollectionEntity? FeeCollection { get; set; }

    [ForeignKey("StudentId")]
    public virtual StudentEntity? Student { get; set; }
}
```

### 12.4 Constants

#### AttendanceStatus.cs

```csharp
namespace SmartTuitionManagementSystem.Constants;

public static class AttendanceStatus
{
    public const string Present = "Present";
    public const string Absent = "Absent";
    public const string Leave = "Leave";
    
    public static readonly List<string> All = new List<string>()
    {
        Present,
        Absent,
        Leave
    };
}
```

### 12.5 App Configuration

#### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=SmartTuitionManagementDB;Username=postgres;Password=Pass@123"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Authentication": {
    "Cookie": {
      "ExpireTimeSpan": "7",
      "SlidingExpiration": true
    }
  }
}
```

#### SmartTuitionManagementSystem.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
    <PropertyGroup>
        <TargetFramework>net10.0</TargetFramework>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
    </PropertyGroup>
    <ItemGroup>
        <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.8" />
        <PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="10.0.8" />
        <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.8">
            <PrivateAssets>all</PrivateAssets>
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageReference>
        <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.8">
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
            <PrivateAssets>all</PrivateAssets>
        </PackageReference>
        <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
        <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.2" />
    </ItemGroup>
</Project>
```

---

## Appendix A: Migration Snapshot (Current Database State)

### Existing Tables (3 migrations applied)

| Table | Migration | Records FK References |
|-------|-----------|----------------------|
| `Teachers` | 1 | Referenced by `Classes.ClassTeacherId` |
| `Users` | 1 | — |
| `Classes` | 1 | References `Teachers.Id`, Referenced by `Students.ClassId` |
| `Students` | 1, 3 | References `Classes.Id`, Referenced by `Attendance.StudentId` |
| `Attendance` | 1 | References `Students.Id` |
| `Exams` | 2 | — |
| `Teacher` | 2 | Referenced by `TeacherAttendances.TeacherId` |
| `TeacherAttendances` | 2 | References `Teacher.Id` |

### Code-Only Tables (no migration yet)

| Table | References | Referenced By |
|-------|-----------|---------------|
| `FeeTypes` | — | `FeeStructures.FeeTypeId` |
| `FeeStructures` | `Classes.Id`, `FeeTypes.Id` | `FeeCollections.FeeStructureId` |
| `FeeCollections` | `Students.Id`, `FeeStructures.Id` | `PaymentTransactions.FeeCollectionId` |
| `PaymentTransactions` | `FeeCollections.Id`, `Students.Id` | — |

---

## Appendix B: Key File Index

| File Path | Purpose |
|-----------|---------|
| `SmartTuitionManagementSystem.csproj` | Project file with dependencies |
| `Program.cs` | Application startup, DI, middleware |
| `Data/ApplicationDbContext.cs` | EF Core DbContext with all entity configurations |
| `Constants/AttendanceStatus.cs` | Student attendance status constants |
| `Entities/*.cs` | Database entity classes (11 files) |
| `Models/*.cs` | ViewModel and model classes (18 files) |
| `Controllers/*.cs` | MVC controllers (10 files) |
| `Services/Interface/*.cs` | Service interfaces (8 files) |
| `Services/*.cs` | Service implementations (8 files) |
| `Migrations/*.cs` | EF Core migrations (3 migrations + snapshot) |
| `Views/**/*.cshtml` | Razor views |
| `appsettings.json` | Configuration |
| `Properties/launchSettings.json` | Launch profiles |

---

*End of Dossier — Smart Tuition Management System*

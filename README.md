# 🎨 DrawingBot

DrawingBot is a fullstack technical assignment that allows users to generate drawings on a canvas using natural language prompts.

The system uses an LLM to convert free-text drawing instructions into structured JSON drawing commands, which are rendered on the client canvas and can be saved and loaded from the server.

---

## ✨ Features

- Natural language prompt input (e.g. "Draw a sun")
- Prompt interpretation using an LLM (OpenRouter)
- Client-side canvas rendering
- Supported shapes: rectangle, line, circle
- Action-based Undo / Redo (command history)
- Clear canvas
- Save drawings to server
- Load drawings by ID
- Drawings associated with a user (mock users)

---

## 🧱 Tech Stack

### Frontend
- React
- Vite
- CanvasRenderingContext2D
- Environment-based configuration

### Backend
- ASP.NET Core Web API
- SQLite
- REST API
- JSON validation on server side

---

## 📂 Project Structure

root/
├── client/ # React + Vite frontend
├── server/ # ASP.NET Core backend
├── README.md
└── .gitignore

## ⚙️ Setup Instructions

### 1️⃣ Clone the repository
```bash
git clone <repository-url>
cd DrawingBot
```

### 2️⃣ Backend Setup (.NET)
```bash
cd server
dotnet restore
```
Create a file named appsettings.Development.json:
```bash
{
  "OpenRouter": {
    "ApiKey": "YOUR_API_KEY_HERE",
    "BaseUrl": "https://openrouter.ai/api/v1/chat/completions"
  }
}
```
Run the backend:
```bash
dotnet run
```
The backend runs on:
```bash
https://localhost:44351
```
---

### 3️⃣ Frontend Setup (React)
```bash
cd client
npm install
```
Create a .env file based on .env.example:
```bash
cp .env.example .env
```
Run the frontend:
```bash
npm run dev
```
---

## 🧪 Notes for Reviewers
This project was implemented as a technical assignment.

API keys and secrets are intentionally excluded from the repository.

Example configuration files are provided.

Undo / Redo is implemented using an action-based command history.

User handling is simplified using mock users with GUID identifiers.

Authentication is intentionally not implemented.

## 🎥 Demo
A short demo video demonstrating the application functionality is included with the submission.

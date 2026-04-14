# AI Document Analyzer API

A .NET 8 REST API that uses GPT-4.1 to perform structured risk analysis on any JSON document.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![OpenAI](https://img.shields.io/badge/OpenAI-GPT--4.1-412991?logo=openai)
![License](https://img.shields.io/badge/license-MIT-green)

---

## Features

- Analyze any JSON document with GPT-4.1 — returns a structured risk analysis with executive summary, findings by category, fraud indicators, risk classification by topic, recommended action plans, and improvement suggestions
- Supports 5 document types: `Contract`, `FinancialReport`, `PersonProfile`, `CompanyProfile`, `Generic`
- Interactive chat sessions via the OpenAI Realtime API
- Knowledge base management via OpenAI vector store (auto-cleanup of stale files)
- Token-based authentication via `Token: {guid}` request header
- Consistent JSON response envelope with elapsed time, HTTP status, and structured error details
- Swagger/OpenAPI documentation with Token security definition included

---

## Tech Stack

| Package | Version | Purpose |
|---|---|---|
| ASP.NET Core | 8.0 | Web API framework |
| OpenAI .NET SDK | 2.5.0 | GPT-4.1 completions, Realtime sessions, vector store |
| Newtonsoft.Json | 13.0.3 | JSON serialization / deserialization |
| Swashbuckle.AspNetCore | 6.6.2 | Swagger / OpenAPI documentation |
| Microsoft.Extensions.Http | 8.0.0 | IHttpClientFactory |

---

## Architecture

```
AIDocumentAnalyzer/
├── AIDocumentAnalyzer.sln
├── AIDocumentAnalyzer/             # ASP.NET Core Web API
│   ├── Controllers/
│   │   ├── BaseController.cs       # Response envelope mapping
│   │   ├── AnalyzerController.cs   # POST /api/analyzer
│   │   ├── ChatController.cs       # GET  /api/chat/start
│   │   └── KnowledgeBaseController.cs # POST /api/knowledgebase/update
│   ├── Program.cs                  # DI, middleware, Swagger
│   └── appsettings.json
└── Library/                        # Business logic + data contracts
    ├── BLL/
    │   ├── PromptsBLL.cs           # LLM prompt templates (5 document types)
    │   ├── AuthBLL.cs              # Token validation
    │   ├── AnalyzerBLL.cs          # Orchestrates analysis flow
    │   ├── ChatBLL.cs              # Chat session management
    │   └── KnowledgeBaseBLL.cs     # Vector store management
    ├── DTO/                        # Request/response contracts
    └── Util/                       # OpenAI adapter, exceptions, enums
```

**Controllers** — receive HTTP requests, validate the `Token` header, delegate to the appropriate BLL, and return responses via `BaseController.ValidateReturn()`.

**BLL** — all business logic: builds type-specific prompts, orchestrates OpenAI calls, and handles token validation.

**Util** — adapts the OpenAI .NET SDK for chat completions, Realtime sessions, and vector store operations; defines `AppExceptionUtil` and the `DocumentType` enum.

**DTO** — typed data contracts for all requests and responses; `ResponseDTO` wraps every response with timing, status, and error details.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- An OpenAI API key (GPT-4.1 access required)

### Setup

1. Clone the repository:

   ```bash
   git clone <repo-url>
   cd AIDocumentAnalyzer
   ```

2. Create the development settings file:

   ```bash
   cp AIDocumentAnalyzer/appsettings.Development.json.example AIDocumentAnalyzer/appsettings.Development.json
   ```

3. Edit `AIDocumentAnalyzer/appsettings.Development.json` and fill in your values:

   ```json
   {
     "OpenAI": {
       "ApiKey": "sk-...",
       "VectorStoreId": "vs_..."
     },
     "Auth": {
       "ValidTokens": ["your-guid-here"]
     }
   }
   ```

4. Run the API:

   ```bash
   dotnet run --project AIDocumentAnalyzer/AIDocumentAnalyzer.csproj
   ```

5. Open Swagger UI at `https://localhost:7060/swagger`
   (the port is printed in the terminal when you run the API; default HTTPS port is `7060`, HTTP is `5207`)

---

## Running with Docker

```bash
docker build -t ai-document-analyzer .
docker run -p 8080:8080 \
  -e OpenAI__ApiKey=sk-... \
  -e OpenAI__VectorStoreId=vs_... \
  -e Auth__ValidTokens__0=your-guid-here \
  ai-document-analyzer
```

The API will be available at `http://localhost:8080`.

> **Note:** CORS is currently configured with `AllowAnyOrigin`. This is intentional for local development only — restrict allowed origins before deploying to a production environment.

---

## API Reference

All endpoints require the following header:

```
Token: <your-guid>
```

---

### POST /api/analyzer

Analyze a JSON document and receive a structured risk report.

**Request body:**

```json
{
  "DocumentType": "Contract",
  "DocumentContent": {
    "parties": ["Acme Corp", "Beta Ltd"],
    "value": 500000,
    "currency": "EUR",
    "jurisdiction": "Portugal"
  }
}
```

`DocumentType` must be one of: `Contract`, `FinancialReport`, `PersonProfile`, `CompanyProfile`, `Generic`.  
`DocumentContent` accepts any valid JSON object.

**Response (200 OK):**

```json
{
  "Return": {
    "ExecutiveSummary": {
      "OverallRisk": "Medium",
      "PositivePoints": ["Clear jurisdiction defined"],
      "AttentionPoints": ["Missing termination clause"],
      "AlertPoints": []
    },
    "FindingsByCategory": [
      {
        "Category": "Legal",
        "Observations": ["Governing law is Portugal — review local requirements"]
      }
    ],
    "FraudIndicators": {
      "Disclaimer": "These indicators are based on statistical pattern analysis and do not constitute legal proof.",
      "Points": []
    },
    "RiskClassificationByTopic": [
      { "Topic": "Legal", "Risk": "Medium" }
    ],
    "RecommendedActionPlans": [
      { "Priority": "High", "Actions": ["Add a termination clause"] }
    ],
    "ImprovementSuggestions": {
      "Comment": "Consider adding dispute resolution clauses.",
      "Suggestions": ["Include arbitration clause"]
    }
  },
  "Success": true,
  "Status": "Ok",
  "ElapsedTimeMs": 3200,
  "DateTimeExecution": "2026-04-14T10:45:00Z",
  "HttpStatusCode": 200,
  "Error": null
}
```

`OverallRisk` and `Risk` values: `Low`, `Medium`, `High`, `Critical`.  
`Priority` values: `High`, `Medium`, `Low`, `Monitor`.

---

### GET /api/chat/start?user={name}

Create an interactive chat session using the OpenAI Realtime API.

**Query parameter:** `user` — display name for the session.

**Response (200 OK):** Returns the `clientSecret` string to be used client-side for the Realtime session.

---

### POST /api/knowledgebase/update

Upload a file to the configured OpenAI vector store. The endpoint keeps only the most recent file — older files are removed automatically.

**Request:** `multipart/form-data` with a `file` field containing the document to upload.

**Response (200 OK):** Returns a confirmation message on success.

---

### Error Response

All errors follow the same response envelope:

```json
{
  "Return": null,
  "Success": false,
  "Status": "Error",
  "ElapsedTimeMs": 12,
  "DateTimeExecution": "2026-04-14T10:45:00Z",
  "HttpStatusCode": 401,
  "Error": {
    "FieldName": null,
    "Message": "Invalid or missing token."
  }
}
```

---

## Configuration

| Key | Required | Description |
|---|---|---|
| `OpenAI:ApiKey` | Yes | OpenAI API key (`sk-...`) |
| `OpenAI:VectorStoreId` | Yes (for `/knowledgebase`) | OpenAI vector store ID (`vs_...`) |
| `Auth:ValidTokens` | Yes | JSON array of accepted GUID tokens |

Configuration is loaded from `appsettings.json` and `appsettings.Development.json`. The development file is excluded from source control — use `appsettings.Development.json.example` as the template.

---

## Roadmap

- [ ] Interface abstractions for BLL and Util layers (prerequisite for unit testing)
- [ ] Unit and integration test suite
- [ ] Rate limiting per token
- [ ] Azure Key Vault integration for secret management
- [ ] Background job for periodic vector store refresh
- [ ] Structured logging with Serilog

---

## License

MIT © 2026

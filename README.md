# 🚀 Chatbot Intent Service

Welcome to the Chatbot Intent Service! This project leverages cutting-edge NLP models from Hugging Face to create an intelligent chatbot that understands user sentiment and intent, providing appropriate responses based on the analysis.

## 🌟 Features

- **Sentiment Analysis**: Detects the emotional tone of user messages (positive, negative, neutral).
- **Intent Recognition**: Identifies the intent behind user messages (e.g., greeting, farewell, question).
- **Dynamic Response Generation**: Crafts responses based on the recognized intent and sentiment.
- **Extensible and Modular**: Easily extend functionality with additional NLP models or custom logic.
- **Logging and Monitoring**: Integrated logging for tracking API calls and responses.

## 🛠️ Requirements

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- Hugging Face API Token

## ⚙️ Setup

1. **Clone the repository**:

    ```bash
    git clone https://github.com/yourusername/chatbot-intent-service.git
    cd chatbot-intent-service
    ```

2. **Configure the application**:

    Update the `appsettings.json` file with your Hugging Face API token and model URLs.

    ```json
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "AllowedHosts": "*",
      "HuggingFaceApi": {
        "Token": "YOUR_HUGGING_FACE_API_TOKEN",
        "SentimentModelUrl": "https://api-inference.huggingface.co/models/distilbert-base-uncased-finetuned-sst-2-english",
        "IntentModelUrl": "https://api-inference.huggingface.co/models/distilbert-base-uncased-finetuned-sst-2-english"
      }
    }
    ```

3. **Restore dependencies and build the project**:

    ```bash
    dotnet restore
    dotnet build
    ```

4. **Run the application**:

    ```bash
    dotnet run
    ```

    The application will start and listen on `http://localhost:5000`.

## 📡 API Endpoints

### POST /chat

Analyze the sentiment and intent of the provided message and generate an appropriate response.

- **URL**: `http://localhost:5000/chat`
- **Method**: `POST`
- **Request Body**:

    ```json
    {
        "message": "Hello, how are you?"
    }
    ```

- **Response**:

    ```json
    {
        "response": "Hello! I'm glad to hear that you're doing well. How can I assist you today?",
        "sentiment": "POSITIVE"
    }
    ```

## 🛠️ Example Usage

### Using Postman

1. Open Postman and create a new POST request.
2. Set the request URL to `http://localhost:5000/chat`.
3. Set the request body to raw JSON:

    ```json
    {
        "message": "Hello, how are you?"
    }
    ```

4. Send the request and you should receive a response similar to:

    ```json
    {
        "response": "Hello! I'm glad to hear that you're doing well. How can I assist you today?",
        "sentiment": "POSITIVE"
    }
    ```

### Using Curl

```bash
curl -X POST http://localhost:5000/chat -H "Content-Type: application/json" -d '{"message": "Hello, how are you?"}'

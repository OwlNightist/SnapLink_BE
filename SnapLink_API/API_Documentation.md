# SnapLink Chat API Documentation

## Overview

The SnapLink Chat API provides real-time messaging functionality for the SnapLink application. It supports both direct (1-on-1) and group conversations with real-time message delivery via SignalR.

## Base URL

- **Development**: `https://localhost:7000`
- **Production**: `https://snaplinkapi-g7eubeghazh5byd8.southeastasia-01.azurewebsites.net`

## Authentication

All API endpoints require JWT authentication. Include the JWT token in the Authorization header:

```
Authorization: Bearer <your_jwt_token>
```

The JWT token should contain a `UserId` claim that identifies the authenticated user.

## SignalR Hub

For real-time functionality, connect to the SignalR hub at:
- **Development**: `https://localhost:7000/chatHub`
- **Production**: `https://snaplinkapi-g7eubeghazh5byd8.southeastasia-01.azurewebsites.net/chatHub`

## API Endpoints

### 1. Message Operations

#### Send Message
```http
POST /api/chat/send-message
```

**Request Body:**
```json
{
  "recipientId": 123,
  "content": "Hello! How are you?",
  "messageType": "Text",
  "conversationId": 456
}
```

**Response:**
```json
{
  "success": true,
  "message": "Message sent successfully",
  "messageData": {
    "messageId": 789,
    "senderId": 1,
    "recipientId": 123,
    "conversationId": 456,
    "content": "Hello! How are you?",
    "createdAt": "2024-01-15T10:30:00Z",
    "messageType": "Text",
    "status": "sent",
    "senderName": "John Doe",
    "senderProfileImage": "profile.jpg"
  },
  "conversationId": 456
}
```

#### Get Message by ID
```http
GET /api/chat/messages/{messageId}
```

**Response:**
```json
{
  "messageId": 789,
  "senderId": 1,
  "recipientId": 123,
  "conversationId": 456,
  "content": "Hello! How are you?",
  "createdAt": "2024-01-15T10:30:00Z",
  "messageType": "Text",
  "status": "read",
  "readAt": "2024-01-15T10:35:00Z",
  "senderName": "John Doe",
  "senderProfileImage": "profile.jpg"
}
```

#### Get Conversation Messages
```http
GET /api/chat/conversations/{conversationId}/messages?page=1&pageSize=20
```

**Response:**
```json
{
  "messages": [
    {
      "messageId": 789,
      "senderId": 1,
      "recipientId": 123,
      "conversationId": 456,
      "content": "Hello! How are you?",
      "createdAt": "2024-01-15T10:30:00Z",
      "messageType": "Text",
      "status": "read",
      "senderName": "John Doe",
      "senderProfileImage": "profile.jpg"
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 20,
  "hasMore": false
}
```

#### Mark Message as Read
```http
POST /api/chat/messages/{messageId}/mark-read
```

**Request Body:**
```json
{
  "messageId": 789
}
```

**Response:**
```json
{
  "success": true,
  "message": "Message marked as read"
}
```

#### Delete Message
```http
DELETE /api/chat/messages/{messageId}
```

**Response:**
```json
{
  "success": true,
  "message": "Message deleted successfully"
}
```

### 2. Conversation Operations

#### Create Conversation
```http
POST /api/chat/conversations
```

**Request Body:**
```json
{
  "title": "Project Discussion",
  "type": "Group",
  "participantIds": [1, 2, 3],
  "status": "Active"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Conversation created successfully",
  "conversation": {
    "conversationId": 456,
    "title": "Project Discussion",
    "createdAt": "2024-01-15T10:00:00Z",
    "updatedAt": "2024-01-15T10:00:00Z",
    "status": "Active",
    "type": "Group",
    "participants": [
      {
        "conversationParticipantId": 1,
        "conversationId": 456,
        "userId": 1,
        "joinedAt": "2024-01-15T10:00:00Z",
        "role": "Member",
        "isActive": true,
        "userName": "John Doe",
        "userProfileImage": "profile1.jpg",
        "userFullName": "John Doe"
      }
    ],
    "lastMessage": null,
    "unreadCount": 0
  }
}
```

#### Get Conversation by ID
```http
GET /api/chat/conversations/{conversationId}
```

#### Get User Conversations
```http
GET /api/chat/conversations?page=1&pageSize=20
```

**Response:**
```json
{
  "conversations": [
    {
      "conversationId": 456,
      "title": "Project Discussion",
      "createdAt": "2024-01-15T10:00:00Z",
      "updatedAt": "2024-01-15T10:30:00Z",
      "status": "Active",
      "type": "Group",
      "participants": [...],
      "lastMessage": {
        "messageId": 789,
        "content": "Hello! How are you?",
        "createdAt": "2024-01-15T10:30:00Z"
      },
      "unreadCount": 2
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 20
}
```

#### Update Conversation
```http
PUT /api/chat/conversations/{conversationId}?title=New Title&status=Active
```

#### Delete/Leave Conversation
```http
DELETE /api/chat/conversations/{conversationId}
```

### 3. Participant Operations

#### Add Participant
```http
POST /api/chat/conversations/{conversationId}/participants
```

**Request Body:**
```json
{
  "conversationId": 456,
  "userId": 4,
  "role": "Member"
}
```

#### Remove Participant
```http
DELETE /api/chat/conversations/{conversationId}/participants
```

**Request Body:**
```json
{
  "conversationId": 456,
  "userId": 4
}
```

#### Get Conversation Participants
```http
GET /api/chat/conversations/{conversationId}/participants
```

#### Leave Conversation
```http
POST /api/chat/conversations/{conversationId}/leave
```

### 4. Utility Operations

#### Get Unread Message Count
```http
GET /api/chat/conversations/{conversationId}/unread-count
```

**Response:**
```json
5
```

#### Check if User is Participant
```http
GET /api/chat/conversations/{conversationId}/is-participant
```

**Response:**
```json
true
```

#### Get or Create Direct Conversation
```http
GET /api/chat/direct-conversation?otherUserId=123
```

## SignalR Hub Methods

### Client to Server Methods

#### Register User
```javascript
connection.invoke("RegisterUser", userId);
```

#### Join Conversation
```javascript
connection.invoke("JoinConversation", conversationId);
```

#### Leave Conversation
```javascript
connection.invoke("LeaveConversation", conversationId);
```

#### Send Message to Conversation
```javascript
connection.invoke("SendMessageToConversation", conversationId, messageData);
```

#### Send Message to User
```javascript
connection.invoke("SendMessageToUser", recipientUserId, messageData);
```

### Server to Client Methods

#### Receive Message
```javascript
connection.on("ReceiveMessage", (message) => {
    console.log("New message:", message);
});
```

#### User Registered
```javascript
connection.on("UserRegistered", (userId) => {
    console.log("User registered:", userId);
});
```

#### Joined Conversation
```javascript
connection.on("JoinedConversation", (conversationId) => {
    console.log("Joined conversation:", conversationId);
});
```

#### Left Conversation
```javascript
connection.on("LeftConversation", (conversationId) => {
    console.log("Left conversation:", conversationId);
});
```

#### New Conversation
```javascript
connection.on("NewConversation", (conversation) => {
    console.log("New conversation:", conversation);
});
```

#### Conversation Updated
```javascript
connection.on("ConversationUpdated", (update) => {
    console.log("Conversation updated:", update);
});
```

#### Message Status Changed
```javascript
connection.on("MessageStatusChanged", (messageId, status) => {
    console.log("Message status changed:", messageId, status);
});
```

## JavaScript Client Example

```javascript
// Connect to SignalR Hub
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();

// Start connection
connection.start()
    .then(() => {
        console.log("Connected to chat hub");
        // Register user after connection
        connection.invoke("RegisterUser", currentUserId);
    })
    .catch(err => console.error("Connection failed:", err));

// Listen for messages
connection.on("ReceiveMessage", (message) => {
    // Handle new message
    displayMessage(message);
});

// Send message
async function sendMessage(recipientId, content, conversationId = null) {
    try {
        const response = await fetch('/api/chat/send-message', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${jwtToken}`
            },
            body: JSON.stringify({
                recipientId: recipientId,
                content: content,
                conversationId: conversationId
            })
        });
        
        const result = await response.json();
        if (result.success) {
            // Join conversation if not already joined
            if (result.conversationId) {
                connection.invoke("JoinConversation", result.conversationId);
            }
        }
    } catch (error) {
        console.error("Error sending message:", error);
    }
}

// Get conversations
async function getConversations(page = 1, pageSize = 20) {
    try {
        const response = await fetch(`/api/chat/conversations?page=${page}&pageSize=${pageSize}`, {
            headers: {
                'Authorization': `Bearer ${jwtToken}`
            }
        });
        
        const result = await response.json();
        return result;
    } catch (error) {
        console.error("Error getting conversations:", error);
    }
}
```

## Error Responses

### 401 Unauthorized
```json
{
  "message": "Invalid user token"
}
```

### 400 Bad Request
```json
{
  "success": false,
  "message": "Sender or recipient not found"
}
```

### 404 Not Found
```json
{
  "message": "Message not found"
}
```

## Data Models

### Message Status Values
- `"sent"` - Message has been sent
- `"read"` - Message has been read by recipient

### Conversation Types
- `"Direct"` - 1-on-1 conversation
- `"Group"` - Group conversation

### Conversation Status Values
- `"Active"` - Conversation is active
- `"Archived"` - Conversation is archived
- `"Deleted"` - Conversation is deleted

### Participant Roles
- `"Admin"` - Conversation administrator
- `"Member"` - Regular participant

## Rate Limiting

The API implements rate limiting to prevent abuse. Please respect reasonable usage patterns.

## Support

For technical support or questions about the API, please contact the development team. 
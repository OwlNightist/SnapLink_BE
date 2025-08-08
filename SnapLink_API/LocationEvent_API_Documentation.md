# LocationEvent API Documentation

## Overview
The LocationEvent API provides endpoints for managing promotional events at locations, allowing photographers to apply and users to book within these events.

## Base URL
```
https://your-api-domain.com/api/LocationEvent
```

## Authentication
All endpoints require JWT Bearer token authentication.

## Endpoints

### 🎯 Event Management

#### Create Event
- **POST** `/api/LocationEvent`
- **Description**: Create a new promotional event
- **Request Body**:
```json
{
  "locationId": 1,
  "name": "Spring Festival Photography Event",
  "description": "Join us for a special spring photography event with discounted rates",
  "startDate": "2024-03-15T09:00:00Z",
  "endDate": "2024-03-15T18:00:00Z",
  "discountedPrice": 150.00,
  "originalPrice": 200.00,
  "maxPhotographers": 10,
  "maxBookingsPerSlot": 5
}
```
- **Response**:
```json
{
  "error": 0,
  "message": "Event created successfully",
  "data": {
    "eventId": 1,
    "locationId": 1,
    "name": "Spring Festival Photography Event",
    "description": "Join us for a special spring photography event with discounted rates",
    "startDate": "2024-03-15T09:00:00Z",
    "endDate": "2024-03-15T18:00:00Z",
    "discountedPrice": 150.00,
    "originalPrice": 200.00,
    "maxPhotographers": 10,
    "maxBookingsPerSlot": 5,
    "status": "Draft",
    "createdAt": "2024-01-15T10:00:00Z",
    "updatedAt": "2024-01-15T10:00:00Z",
    "location": {
      "locationId": 1,
      "name": "Central Park",
      "address": "123 Main St"
    },
    "images": [
      {
        "id": 456,
        "url": "https://cdn.example.com/event/1/banner.jpg",
        "eventId": 1,
        "isPrimary": true,
        "caption": "Event Banner",
        "createdAt": "2024-01-15T10:00:00Z"
      }
    ],
    "primaryImage": {
      "id": 456,
      "url": "https://cdn.example.com/event/1/banner.jpg",
      "eventId": 1,
      "isPrimary": true,
      "caption": "Event Banner",
      "createdAt": "2024-01-15T10:00:00Z"
    },
    "approvedPhotographersCount": 0,
    "totalBookingsCount": 0
  }
}
```

#### Get Event by ID
- **GET** `/api/LocationEvent/{eventId}`
- **Description**: Get event details by ID
- **Response**: Event details with counts

#### Get Event Detail
- **GET** `/api/LocationEvent/{eventId}/detail`
- **Description**: Get detailed event information including photographers and bookings
- **Response**: Full event details with navigation properties

#### Get All Events
- **GET** `/api/LocationEvent`
- **Description**: Get all events
- **Response**: List of all events

#### Get Events by Location
- **GET** `/api/LocationEvent/location/{locationId}`
- **Description**: Get events for a specific location
- **Response**: List of events for the location

#### Get Active Events
- **GET** `/api/LocationEvent/active`
- **Description**: Get currently active events
- **Response**: List of active events

#### Get Upcoming Events
- **GET** `/api/LocationEvent/upcoming`
- **Description**: Get upcoming events
- **Response**: List of upcoming events

#### Update Event
- **PUT** `/api/LocationEvent/{eventId}`
- **Description**: Update event details
- **Request Body**:
```json
{
  "name": "Updated Event Name",
  "description": "Updated description",
  "startDate": "2024-03-20T09:00:00Z",
  "endDate": "2024-03-20T18:00:00Z",
  "discountedPrice": 120.00,
  "maxPhotographers": 15
}
```

#### Delete Event
- **DELETE** `/api/LocationEvent/{eventId}`
- **Description**: Delete an event
- **Response**: Success/error message

#### Update Event Status
- **PATCH** `/api/LocationEvent/{eventId}/status`
- **Description**: Update event status (Draft, Open, Active, Closed, Cancelled)
- **Request Body**: `"Open"`

### 📝 Event Applications

#### Apply to Event
- **POST** `/api/LocationEvent/apply`
- **Description**: Photographer applies to participate in an event
- **Request Body**:
```json
{
  "eventId": 1,
  "photographerId": 5,
  "specialRate": 140.00
}
```
- **Response**: Application details

#### Respond to Application
- **POST** `/api/LocationEvent/respond-application`
- **Description**: Location owner approves/rejects photographer application
- **Request Body**:
```json
{
  "eventId": 1,
  "photographerId": 5,
  "status": "Approved",
  "rejectionReason": null
}
```

#### Get Event Applications
- **GET** `/api/LocationEvent/{eventId}/applications`
- **Description**: Get all applications for an event
- **Response**: List of applications

#### Get Photographer Applications
- **GET** `/api/LocationEvent/photographer/{photographerId}/applications`
- **Description**: Get all applications by a photographer
- **Response**: List of applications

#### Withdraw Application
- **DELETE** `/api/LocationEvent/{eventId}/photographer/{photographerId}/withdraw`
- **Description**: Photographer withdraws application
- **Response**: Success/error message

#### Get Approved Photographers
- **GET** `/api/LocationEvent/{eventId}/approved-photographers`
- **Description**: Get approved photographers for an event
- **Response**: List of approved photographers

### 📅 Event Bookings

#### Create Event Booking
- **POST** `/api/LocationEvent/booking`
- **Description**: Create a booking within an event (requires authentication)
- **Request Body**:
```json
{
  "eventId": 1,
  "eventPhotographerId": 3,
  "startDatetime": "2024-03-15T10:00:00Z",
  "endDatetime": "2024-03-15T12:00:00Z",
  "specialRequests": "Outdoor portrait session"
}
```
- **Note**: UserId is automatically extracted from JWT token

#### Get Event Booking
- **GET** `/api/LocationEvent/booking/{eventBookingId}`
- **Description**: Get specific event booking details
- **Response**: Event booking details

#### Get Event Bookings
- **GET** `/api/LocationEvent/{eventId}/bookings`
- **Description**: Get all bookings for an event
- **Response**: List of event bookings

#### Get User Event Bookings
- **GET** `/api/LocationEvent/user/{userId}/bookings`
- **Description**: Get all event bookings for a user
- **Response**: List of user's event bookings

#### Cancel Event Booking
- **DELETE** `/api/LocationEvent/booking/{eventBookingId}`
- **Description**: Cancel an event booking
- **Response**: Success/error message

### 🖼️ Event Images

#### Upload Event Image
- **POST** `/api/image`
- **Description**: Upload an image for an event. Use multipart/form-data.
- **Form Fields**:
  - `file`: Image file (required)
  - `eventId`: Event ID (required when uploading for event)
  - `isPrimary`: boolean (optional)
  - `caption`: string (optional)
- **Example** (curl):
```bash
curl -X POST "https://your-api-domain.com/api/image" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F file=@"./banner.jpg" \
  -F eventId=1 \
  -F isPrimary=true \
  -F caption="Event Banner"
```

#### Get Event Images
- **GET** `/api/image/event/{eventId}`
- **Description**: Get all images of an event

#### Get Primary Event Image
- **GET** `/api/image/event/{eventId}/primary`
- **Description**: Get the primary image of an event

### 📊 Event Statistics and Discovery

#### Get Event Statistics
- **GET** `/api/LocationEvent/{eventId}/statistics`
- **Description**: Get comprehensive event statistics
- **Response**:
```json
{
  "error": 0,
  "message": "Event statistics retrieved successfully",
  "data": {
    "eventId": 1,
    "eventName": "Spring Festival Photography Event",
    "eventStartDate": "2024-03-15T09:00:00Z",
    "eventEndDate": "2024-03-15T18:00:00Z",
    "eventStatus": "Active",
    "totalApplications": 15,
    "approvedApplications": 10,
    "rejectedApplications": 3,
    "pendingApplications": 2,
    "totalBookings": 25,
    "totalRevenue": 3750.00,
    "averageBookingValue": 150.00
  }
}
```

#### Get Event List
- **GET** `/api/LocationEvent/list`
- **Description**: Get event list with counts
- **Response**: List of events with statistics and images

#### Get Events by Status
- **GET** `/api/LocationEvent/status/{status}`
- **Description**: Get events filtered by status
- **Response**: List of events with the specified status

#### Get Events by Date Range
- **GET** `/api/LocationEvent/date-range?startDate=2024-03-01&endDate=2024-03-31`
- **Description**: Get events within a date range
- **Response**: List of events in the date range

#### Search Events
- **GET** `/api/LocationEvent/search?searchTerm=spring`
- **Description**: Search events by name or description
- **Response**: List of matching events

#### Get Featured Events
- **GET** `/api/LocationEvent/featured`
- **Description**: Get featured/active events
- **Response**: List of featured events

#### Get Events Nearby
- **GET** `/api/LocationEvent/nearby?latitude=40.7128&longitude=-74.0060&radiusKm=10`
- **Description**: Get events near a location
- **Response**: List of nearby events

## Error Responses

### Standard Error Format
```json
{
  "error": -1,
  "message": "Error description",
  "data": null
}
```

### Common HTTP Status Codes
- **200**: Success
- **400**: Bad Request (validation errors)
- **401**: Unauthorized (invalid/missing token)
- **404**: Not Found (resource doesn't exist)
- **500**: Internal Server Error

## Business Workflow

### 1. Location Owner Workflow
1. **Create Event** → Status: "Draft"
2. **Update Event** → Add details and pricing
3. **Update Status** → "Open" (for applications)
4. **Review Applications** → Approve/reject photographers
5. **Update Status** → "Active" (for bookings)
6. **Monitor Statistics** → Track performance

### 2. Photographer Workflow
1. **Browse Events** → Search and discover events
2. **Apply to Event** → Submit application with special rate
3. **Wait for Response** → Check application status
4. **Participate** → Once approved, available for bookings

### 3. User Workflow
1. **Discover Events** → Browse active events
2. **View Photographers** → See approved photographers
3. **Make Booking** → Book within event context
4. **Enjoy Discount** → Pay event pricing

## Testing Examples

### Create and Manage Event
```bash
# 1. Create event
curl -X POST "https://your-api-domain.com/api/LocationEvent" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "locationId": 1,
    "name": "Summer Photography Festival",
    "description": "Annual summer photography event",
    "startDate": "2024-06-15T09:00:00Z",
    "endDate": "2024-06-15T18:00:00Z",
    "discountedPrice": 120.00,
    "originalPrice": 180.00,
    "maxPhotographers": 8,
    "maxBookingsPerSlot": 3
  }'

# 2. Update status to Open
curl -X PATCH "https://your-api-domain.com/api/LocationEvent/1/status" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '"Open"'

# 3. Get event details
curl -X GET "https://your-api-domain.com/api/LocationEvent/1/detail" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Photographer Application Process
```bash
# 1. Apply to event
curl -X POST "https://your-api-domain.com/api/LocationEvent/apply" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "eventId": 1,
    "photographerId": 5,
    "specialRate": 110.00
  }'

# 2. Location owner responds
curl -X POST "https://your-api-domain.com/api/LocationEvent/respond-application" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "eventId": 1,
    "photographerId": 5,
    "status": "Approved"
  }'
```

### User Booking Process
```bash
# 1. Get approved photographers
curl -X GET "https://your-api-domain.com/api/LocationEvent/1/approved-photographers" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"

# 2. Create booking
curl -X POST "https://your-api-domain.com/api/LocationEvent/booking" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "eventId": 1,
    "eventPhotographerId": 3,
    "startDatetime": "2024-06-15T10:00:00Z",
    "endDatetime": "2024-06-15T12:00:00Z",
    "specialRequests": "Outdoor portrait session"
  }'
```

## Notes

1. **Authentication**: All endpoints require valid JWT Bearer token
2. **User ID**: For booking endpoints, user ID is automatically extracted from JWT token
3. **Event Status Flow**: Draft → Open → Active → Closed/Cancelled
4. **Application Status Flow**: Applied → Approved/Rejected/Withdrawn
5. **Pricing**: Event pricing takes precedence over photographer's regular rate
6. **Capacity**: Events have limits on photographers and bookings per slot
7. **Validation**: Comprehensive validation for dates, capacity, and business rules

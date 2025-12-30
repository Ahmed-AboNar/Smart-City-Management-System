namespace SmartCity.Application.Features.Dashboard.DTOs;

public record CityStatsDto(
    int TotalCitizens,
    int ActiveComplaints,
    int PendingServiceRequests,
    int TotalMeters,
    List<RecentActivityDto> RecentActivities
);

public record RecentActivityDto(
    string Description,
    DateTime Timestamp,
    string Type // e.g., "Complaint", "ServiceRequest", "IoTAlert"
);

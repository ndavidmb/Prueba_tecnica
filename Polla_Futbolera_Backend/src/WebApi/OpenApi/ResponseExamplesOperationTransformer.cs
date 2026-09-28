using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;

namespace WebApi.OpenApi;

public class ResponseExamplesOperationTransformer : IOpenApiOperationTransformer
{
    private static readonly Dictionary<(string Controller, string Action), Dictionary<string, string>> Examples = new()
    {
        [("Auth", "Register")] = new()
        {
            ["201"] = """
                {
                  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIn0.5f1c9b",
                  "email": "jane.doe@example.com",
                  "role": "Player",
                  "name": "Jane Doe"
                }
                """,
            ["409"] = """{ "message": "Ya existe un usuario con ese email." }"""
        },
        [("Auth", "Login")] = new()
        {
            ["200"] = """
                {
                  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIn0.5f1c9b",
                  "email": "jane.doe@example.com",
                  "role": "Player",
                  "name": "Jane Doe"
                }
                """,
            ["401"] = """{ "message": "Credenciales inválidas." }"""
        },
        [("Bets", "PlaceBet")] = new()
        {
            ["201"] = """
                {
                  "betId": 15,
                  "userId": 1,
                  "matchId": 10,
                  "predictedLocalGoals": 2,
                  "predictedVisitorGoals": 1,
                  "realLocalGoals": null,
                  "realVisitorGoals": null,
                  "pointsEarned": 0,
                  "isExactMatch": false,
                  "isTrendMatch": false
                }
                """,
            ["409"] = """{ "message": "Ya tienes una apuesta registrada para este partido." }"""
        },
        [("Matches", "GetAll")] = new()
        {
            ["200"] = """
                [
                  {
                    "id": 10,
                    "localTeam": { "id": 100, "name": "River Plate" },
                    "visitorTeam": { "id": 200, "name": "Boca Juniors" },
                    "localGoals": null,
                    "visitorGoals": null,
                    "status": 1
                  },
                  {
                    "id": 11,
                    "localTeam": { "id": 101, "name": "Independiente" },
                    "visitorTeam": { "id": 102, "name": "Racing Club" },
                    "localGoals": 3,
                    "visitorGoals": 1,
                    "status": 2
                  }
                ]
                """
        },
        [("AdminMatches", "UpdateResult")] = new()
        {
            ["200"] = """
                {
                  "id": 10,
                  "localTeamId": 100,
                  "visitorTeamId": 200,
                  "localGoals": 3,
                  "visitorGoals": 1,
                  "status": 2
                }
                """,
            ["400"] = """{ "message": "El partido no existe." }"""
        },
        [("Leaderboard", "GetLeaderboard")] = new()
        {
            ["200"] = """
                [
                  { "userId": 1, "userName": "Jane Doe", "totalPoints": 12, "totalBets": 5, "rankPosition": 1 },
                  { "userId": 2, "userName": "John Smith", "totalPoints": 9, "totalBets": 5, "rankPosition": 2 }
                ]
                """
        },
        [("Leaderboard", "GetUserHistory")] = new()
        {
            ["200"] = """
                {
                  "userId": 1,
                  "userName": "Jane Doe",
                  "totalPoints": 6,
                  "bets": [
                    {
                      "betId": 15,
                      "matchId": 10,
                      "localTeamName": "River Plate",
                      "visitorTeamName": "Boca Juniors",
                      "predictedLocalGoals": 2,
                      "predictedVisitorGoals": 1,
                      "realLocalGoals": 3,
                      "realVisitorGoals": 1,
                      "pointsEarned": 1
                    }
                  ]
                }
                """,
            ["404"] = """{ "message": "Usuario no encontrado." }"""
        }
    };

    public Task TransformAsync(
        Microsoft.OpenApi.OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.Description.ActionDescriptor is not ControllerActionDescriptor descriptor)
            return Task.CompletedTask;

        if (!Examples.TryGetValue((descriptor.ControllerName, descriptor.ActionName), out var responseExamples))
            return Task.CompletedTask;

        if (operation.Responses is null)
            return Task.CompletedTask;

        foreach (var (statusCode, exampleJson) in responseExamples)
        {
            if (!operation.Responses.TryGetValue(statusCode, out var response) || response?.Content is null)
                continue;

            if (!response.Content.TryGetValue("application/json", out var mediaType) || mediaType is null)
                continue;

            mediaType.Example = JsonNode.Parse(exampleJson);
        }

        return Task.CompletedTask;
    }
}

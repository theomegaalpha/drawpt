# DrawPT Game Engine

This project contains the core game engine for DrawPT, implementing a scalable and maintainable architecture for managing game state and player interactions.

## Architecture Overview

The game engine follows clean architecture principles and is designed to be horizontally scalable. It uses Redis for distributed state management and event-driven communication between game instances.

### Key Components

1. **Game Engine (`IGameEngine`)**

   - Core game logic implementation
   - Manages game flow and player interactions
   - Handles question generation and answer assessment

2. **State Management (`IGameStateManager`)**

   - Redis-based implementation for distributed state
   - Handles player state, game rounds, and scoring
   - Implements pub/sub for real-time updates

3. **AI Integration (`IAIClient`)**
   - Interface for AI-powered question generation
   - Handles answer assessment and scoring

### Models

- `GameState`: Represents the complete state of a game
- `PlayerState`: Tracks individual player state and scoring
- `GameRound`: Manages rounds, questions, and answers
- `GameQuestion`: Represents a generated question
- `GameAnswer`: Tracks player answers and scoring

## Usage

### Service Registration

```csharp
services.AddGameEngine("your-redis-connection-string");
```

### Basic Game Flow

1. Initialize a game:

```csharp
await gameEngine.InitializeGameAsync(roomCode, configuration);
```

2. Add players:

```csharp
await gameEngine.AddPlayerAsync(roomCode, player);
```

3. Start the game:

```csharp
await gameEngine.StartGameAsync(roomCode);
```

4. Generate questions and handle answers:

```csharp
var question = await gameEngine.GenerateQuestionAsync(roomCode, theme);
var answer = await gameEngine.SubmitAnswerAsync(roomCode, connectionId, guess, isGambling);
```

5. Assess answers and get results:

```csharp
var assessedAnswers = await gameEngine.AssessAnswersAsync(roomCode, roundNumber);
var results = await gameEngine.GetGameResultsAsync(roomCode);
```

## Redis Integration

The game engine uses Redis for:

- Distributed state management
- Real-time event publishing
- Game locking and synchronization
- Player state persistence

### Key Redis Patterns

1. **State Storage**

   - Game state stored as JSON in Redis
   - Key format: `game:{roomCode}`
   - Includes player state, rounds, and scoring

2. **Event Publishing**

   - Pub/sub channels for real-time updates
   - Channel format: `game:{roomCode}`
   - Events include player joins, answers, and scoring

3. **Game Locking**
   - Distributed locks for game operations
   - Key format: `lock:{roomCode}`
   - Ensures consistency across instances

## Extending the Engine

### Adding New Features

1. Define new models in the `Models` namespace
2. Add new methods to `IGameEngine` interface
3. Implement in `GameEngine` class
4. Update state management in `RedisGameStateManager`

### Custom State Management

1. Implement `IGameStateManager` interface
2. Register your implementation in `ServiceCollectionExtensions`
3. Update service registration in your application

## Best Practices

1. **State Management**

   - Always use the state manager for state changes
   - Implement proper locking for concurrent operations
   - Handle Redis connection failures gracefully

2. **Error Handling**

   - Use proper exception handling
   - Log errors with appropriate context
   - Implement retry mechanisms for transient failures

3. **Performance**

   - Minimize Redis operations
   - Use appropriate data structures
   - Implement caching where appropriate

4. **Testing**
   - Mock Redis for unit tests
   - Use integration tests for Redis operations
   - Test concurrent operations thoroughly

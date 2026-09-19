---
name: nano-add-event-handler
description: Add an event handler to a Nano application - a class deriving BaseEventHandler<TEvent> that subscribes to messages published via IEventing.PublishAsync. Use when the user asks to add an event handler, subscriber, or consumer for Nano's Publish/Subscribe eventing to a Nano API, Web, or Console application. Not for Entity Events (automatic per-entity-save publishing, which needs no handler at all) - see AGENTS.md's Entity Events section for that.
---

# Nano add event handler

Adds an event handler to an existing Nano application — a class deriving `BaseEventHandler<TEvent>` that
consumes messages published via `IEventing.PublishAsync`. Read AGENTS.md's `### Publish and Subscribe`
section first — it documents the mechanism in full; this skill is just the file shape.

## Before making any change, determine

1. **Is an eventing provider configured?** Check `Program.cs` for `AddNanoEventing<TProvider>()` (see
   [nano-add-eventing-provider](nano-add-eventing-provider) if not). Per AGENTS.md's ⚠, a handler added
   without a provider configured is a silent no-op — the registration task that subscribes handlers never
   runs, so nothing happens at startup and nothing errors either.
2. **Local or shared event?** If not stated, ask. This decides where the message contract (`TEvent`)
   class lives:
   - **Local** — the event is only ever published and consumed within this same solution. The contract
     class lives directly in this app project. This is the default when in doubt.
   - **Shared** — some other Nano application, in a different solution, will need to publish or subscribe
     to this same event later. The contract class lives in its own publishable sibling project instead,
     so it can be referenced without pulling in this app's other code.
3. **Does the event contract already exist?** Check for an existing class named after the event (local:
   inside this app; shared: in a `{App}.Events` sibling project, if one already exists). If it exists,
   reuse it — don't create a second contract for the same message.
4. **Does this handler need a specific routing key or prefetch override?** Ask only if the same event type
   has (or will have) more than one handler that needs to be selectively targeted, or if this handler's
   processing is heavier than the app-wide default prefetch count. Most handlers need neither.

## Event contract

Only this part differs between local and shared — the handler itself (below) is identical either way.

**Local** — the class lives directly in `{App}/Eventing/` (conventional location, not enforced —
discovered by type):

```csharp
// {App}/Eventing/MyEvent.cs — plain class, no base type required
public class MyEvent
{
    public string Text { get; set; } = null!;
}
```

**Shared** — the class moves out to its own sibling project, `{App}.Events` — same idea as the
`{App}.Models` project AGENTS.md's Solution Structure table already documents, just for event contracts
instead of entities/API clients:

1. Create the `{App}.Events` project (new, if it doesn't exist yet) as a sibling of `{App}` and
   `{App}.Models` — mirror `{App}.Models.csproj`'s packaging metadata (versioning, `GeneratePackageOnBuild`,
   authors, description, etc.) so it can be packed and published the same way.
2. Add it to this solution's `.sln`.
3. Add a `ProjectReference` from `{App}` to `{App}.Events`, so this app can both publish the event and
   host the handler for it.
4. Add a "Publish NuGet" step for it in `.github/workflows/build-and-deploy.yml`, mirroring the existing
   `.Models` pack/push step — this is what lets a different solution consume it later; this skill does not
   touch any other solution itself.

```csharp
// {App}.Events/MyEvent.cs — plain class, no base type required
public class MyEvent
{
    public string Text { get; set; } = null!;
}
```

## Handler class

Always lives in `{App}/Eventing/MyEventHandler.cs`, whether the event it handles is local or shared —
only which project `MyEvent` comes from changes, never where the handler itself lives:

```csharp
public class MyEventHandler : BaseEventHandler<MyEvent>
{
    public override async Task CallbackAsync(MyEvent @event, bool isRedelivered, CancellationToken cancellationToken = default)
    {
        // handle @event; isRedelivered is true if the broker is retrying a previously-failed delivery
    }
}
```

No registration needed — every non-generic `BaseEventHandler<TEvent>` in the entry assembly is discovered
and subscribed automatically at startup, once an eventing provider is configured.

If step 4 (in "Before making any change") identified a real routing/prefetch need, declare these two
static properties on the handler class itself, matching `IEventingHandler`'s member names exactly —
`RegisterEventingHandlersTask` looks them up by name via reflection on your concrete class, so declaring
them is enough; there's no override or `new` keyword involved:

```csharp
public class MyEventHandler : BaseEventHandler<MyEvent>
{
    public static string RoutingKey => "my-routing-key";
    public static ushort OverridePrefetchCount => 10;

    public override async Task CallbackAsync(MyEvent @event, bool isRedelivered, CancellationToken cancellationToken = default) { /* ... */ }
}
```

⚠ The handler class itself must be **non-generic** — an open generic handler is silently skipped during
discovery.

## After making the change

- Show the user the files added (event contract, handler, and for shared events, the new project + sln +
  CI changes).
- For a shared event, remind the user that publishing the NuGet only makes it available — wiring it into
  another solution's app is that app's own separate change, not something this skill does.

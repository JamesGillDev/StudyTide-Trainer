using System.Text;

namespace StudyTideTrainer.Data;

public static class TrainingPackCatalog
{
    public static IReadOnlyList<SeedTopic> GetAllPacks()
    {
        return new List<SeedTopic>
        {
            BuildCSharpDeepDivePack(),
            BuildAspNetCoreInternalsPack(),
            BuildAzureForCloudDevelopersPack(),
            BuildSqlForBackendEngineersPack(),
            BuildGitAndDevOpsEssentialsPack(),
            BuildInterviewBehavioralMasteryPack(),
            BuildSystemDesignForJuniorEngineersPack()
        };
    }

    private static SeedTopic BuildCSharpDeepDivePack()
    {
        return new SeedTopic
        {
            Name = "C# Deep Dive",
            Category = "C#",
            Difficulty = 4,
            Snippets = new List<SeedSnippet>
            {
                Outline(
                    "C# Loops: for and foreach",
                    "csharp,loops,iteration",
                    "Explain when to use for versus foreach in production code.",
                    "Use for when you need indexes or reverse traversal.",
                    "Use foreach for readability when the collection abstraction is enough.",
                    "Avoid mutating the collection while iterating unless you control the algorithm.",
                    "Benchmark hot paths because iterator overhead can matter in tight loops."),

                Outline(
                    "String Interpolation for Readable Output",
                    "csharp,strings,interpolation",
                    "Describe why string interpolation is better than concatenation for diagnostics.",
                    "Interpolation keeps message templates readable and reduces punctuation mistakes.",
                    "Use format specifiers like {value:N2} for consistent numeric output.",
                    "Prefer structured logging placeholders over interpolated logs in high-volume services.",
                    "Use raw string literals for multi-line templates when exact formatting matters."),

                Outline(
                    "Parsing Input with TryParse",
                    "csharp,parsing,tryparse",
                    "Explain safe parsing patterns using TryParse in user-facing APIs.",
                    "TryParse avoids exceptions on expected bad input and keeps control flow explicit.",
                    "Validate range and domain rules after parsing, not before.",
                    "Return useful validation errors so clients can correct payloads quickly.",
                    "Use culture-aware overloads when parsing dates or decimals from global users."),

                Outline(
                    "Stack vs Heap in .NET",
                    "csharp,memory,interview",
                    "Teach stack versus heap in terms an interviewer expects.",
                    "Stack frames hold method-local data with deterministic lifetime.",
                    "Heap allocations survive beyond method scope and are reclaimed by GC.",
                    "Large value types copied by value can be expensive even when stack-allocated.",
                    "Escape analysis and JIT optimizations can blur simple textbook rules."),

                Outline(
                    "Value Types vs Reference Types",
                    "csharp,memory,types",
                    "Explain semantic differences between value and reference types.",
                    "Value types copy data on assignment unless passed by ref or in.",
                    "Reference types copy the pointer, so variables can point to the same object.",
                    "Immutability reduces accidental shared-state bugs for reference types.",
                    "Choose type kind based on semantics first, micro-optimizations second."),

                Outline(
                    "ref and out Parameters",
                    "csharp,ref,out",
                    "Explain practical differences between ref and out parameters.",
                    "ref requires caller initialization and allows read-write updates.",
                    "out requires callee assignment and is ideal for Try-pattern APIs.",
                    "Prefer return types and records when many outputs are needed.",
                    "Document side effects clearly because by-reference APIs are powerful but sharp."),

                Outline(
                    "in Parameters and readonly References",
                    "csharp,in,performance",
                    "Describe why in can reduce copying of large structs.",
                    "in passes a readonly reference, preventing accidental mutation.",
                    "Use in for large immutable structs in tight loops.",
                    "Measure first because JIT may optimize copies away in simple cases.",
                    "Do not use in on tiny structs where readability suffers without benefit."),

                Outline(
                    "Boxing and Unboxing Costs",
                    "csharp,boxing,performance",
                    "Explain boxing costs and how they appear in real code.",
                    "Boxing allocates value types on the heap as object references.",
                    "Unboxing requires type checks and can throw when types mismatch.",
                    "Generics avoid boxing by keeping operations type-safe and specialized.",
                    "Watch for boxing in non-generic collections and interface calls on structs."),

                Outline(
                    "readonly struct for Immutable Value Objects",
                    "csharp,readonly-struct,immutability",
                    "Explain when readonly struct is the correct modeling choice.",
                    "readonly struct communicates immutability intent to readers and compiler.",
                    "It can prevent defensive copies during member access.",
                    "Use for small semantic values like Money, Percentage, and Coordinate.",
                    "Avoid adding mutable fields because it defeats value semantics."),

                Outline(
                    "Garbage Collection Gen 0/1/2",
                    "csharp,gc,memory",
                    "Explain GC generations from a performance troubleshooting angle.",
                    "Most short-lived objects die in Gen 0, which is cheap to collect.",
                    "Objects that survive promotions move toward Gen 2 with higher collection cost.",
                    "Large object heap allocations are expensive and should be minimized.",
                    "Profile allocation rate before blaming GC for latency spikes."),

                Outline(
                    "Delegates: Type-Safe Function Pointers",
                    "csharp,delegates,events",
                    "Define delegates and their role in extensible design.",
                    "Delegates encapsulate method references with strong type safety.",
                    "They enable strategy injection without reflection-heavy code.",
                    "Built-in Action and Func cover most delegate signatures.",
                    "Use explicit delegate types when domain meaning improves readability."),

                Outline(
                    "Multicast Delegates",
                    "csharp,delegates,multicast",
                    "Explain multicast delegates and ordering implications.",
                    "A multicast delegate invokes handlers in subscription order.",
                    "Exceptions in one handler can prevent later handlers from running.",
                    "Use GetInvocationList for controlled fan-out and fault isolation.",
                    "Prefer event abstractions when exposing notifications publicly."),

                Outline(
                    "Events vs Delegates",
                    "csharp,events,encapsulation",
                    "Explain why events are safer than exposing raw delegates.",
                    "Events allow external code to subscribe and unsubscribe only.",
                    "Only the declaring type can raise the event, preserving invariants.",
                    "Raw public delegates can be overwritten or invoked externally.",
                    "Use EventHandler or custom EventArgs for conventional event contracts."),

                Outline(
                    "Action<T> and Func<T>",
                    "csharp,action,func",
                    "Describe Action and Func usage in everyday code.",
                    "Action represents a void-returning callback.",
                    "Func represents a callback that returns a value.",
                    "Use expressive parameter names so lambda meaning is obvious.",
                    "Avoid complex nested lambdas that hide business intent."),

                Outline(
                    "LINQ Deferred Execution",
                    "csharp,linq,deferred",
                    "Explain deferred execution and why it surprises new developers.",
                    "Queries compose lazily until materialization occurs.",
                    "If source data changes before enumeration, results can change too.",
                    "Deferred pipelines are powerful for composition and filtering.",
                    "Materialize once with ToList when determinism is required."),

                Outline(
                    "LINQ Immediate Execution",
                    "csharp,linq,materialization",
                    "Explain when immediate execution is preferred.",
                    "ToList, ToArray, Count, and First trigger immediate query work.",
                    "Materialization prevents repeated expensive query execution.",
                    "Snapshotting data simplifies debugging and repeatable behavior.",
                    "Do not materialize too early in large pipelines without need."),

                Outline(
                    "IQueryable vs IEnumerable",
                    "csharp,linq,efcore",
                    "Contrast IQueryable and IEnumerable in data-heavy apps.",
                    "IQueryable builds expression trees for remote providers like EF Core.",
                    "IEnumerable executes in-memory against already-loaded objects.",
                    "Calling AsEnumerable switches from provider translation to local execution.",
                    "Filter as much as possible before materialization to reduce payload size."),

                Outline(
                    "Expression Trees in Practice",
                    "csharp,expression-trees,linq",
                    "Explain why expression trees matter for ORMs and dynamic filters.",
                    "An expression tree represents code as data for provider translation.",
                    "EF Core turns tree nodes into SQL when supported.",
                    "Custom predicate builders use expression composition for reusable filters.",
                    "Unsupported methods can trigger runtime translation exceptions."),

                Outline(
                    "LINQ Performance Implications",
                    "csharp,linq,performance",
                    "Give a practical LINQ performance checklist.",
                    "Avoid multiple enumeration of expensive sequences.",
                    "Prefer Any over Count > 0 for existence checks.",
                    "Project only needed columns to reduce memory and network usage.",
                    "Use benchmarks for hot paths instead of guessing."),

                Outline(
                    "What async/await Really Does",
                    "csharp,async,state-machine",
                    "Explain async as a compiler-generated state machine.",
                    "async methods return quickly with a Task representing future completion.",
                    "await registers continuations instead of blocking the thread.",
                    "I/O-bound work benefits most because threads stay available.",
                    "Understand generated states to debug exception flow accurately."),

                Outline(
                    "Task vs Thread",
                    "csharp,async,threads",
                    "Compare Task and Thread for interview discussions.",
                    "Thread is a low-level OS scheduling primitive.",
                    "Task is a higher-level abstraction for asynchronous units of work.",
                    "Most server code should compose Tasks, not manually manage threads.",
                    "Use dedicated threads only for specialized long-running scenarios."),

                Outline(
                    "Task.Run in Server Apps",
                    "csharp,taskrun,aspnet",
                    "Explain when Task.Run helps and when it hurts in ASP.NET.",
                    "Task.Run can offload CPU-bound work to the thread pool.",
                    "It does not make synchronous I/O truly asynchronous.",
                    "Overusing Task.Run can increase thread pool pressure.",
                    "Prefer native async APIs end-to-end for web request handlers."),

                Outline(
                    "ConfigureAwait(false)",
                    "csharp,configureawait,async",
                    "Explain ConfigureAwait(false) from a library author perspective.",
                    "It avoids capturing the original synchronization context.",
                    "Library code should often use it to prevent context-related deadlocks.",
                    "ASP.NET Core has no legacy request context capture by default.",
                    "UI apps still require context marshaling for UI-bound updates."),

                Outline(
                    "Deadlocks from Sync-over-Async",
                    "csharp,async,deadlock",
                    "Describe classic deadlocks caused by .Result or .Wait.",
                    "Blocking waits can prevent continuations from resuming on captured contexts.",
                    "This pattern was common in classic ASP.NET and UI frameworks.",
                    "Use async all the way to controller and handler boundaries.",
                    "Convert old synchronous APIs incrementally with clear seams."),

                Outline(
                    "Why async void Is Dangerous",
                    "csharp,async,exceptions",
                    "Explain async void risks in production systems.",
                    "Caller cannot await completion or catch exceptions reliably.",
                    "Use async void only for event handlers that require void signatures.",
                    "Prefer Task-returning methods for composability and testing.",
                    "Unhandled async void exceptions can crash process-level handlers."),

                Outline(
                    "SOLID in Everyday C#",
                    "csharp,solid,design",
                    "Connect SOLID principles to maintainable service code.",
                    "SRP keeps classes focused and easier to unit test.",
                    "OCP encourages extension by adding implementations, not editing core flows.",
                    "DIP drives abstractions around volatile dependencies.",
                    "Apply principles pragmatically; avoid abstraction for tiny code paths."),

                Outline(
                    "Constructor Injection vs Service Locator",
                    "csharp,di,solid",
                    "Explain why constructor injection is preferred in most cases.",
                    "Dependencies are explicit and validated at object creation.",
                    "Service locator hides dependencies and delays failure to runtime.",
                    "Constructor injection improves testability with focused mocks.",
                    "Use factory abstractions when dependencies are dynamic at runtime."),

                Drill(
                    "Live Drill: Reverse a String",
                    "csharp,live-coding,string",
                    "Retype and then implement reverse string without using built-in Reverse helper.",
                    "string ReverseManual(string input)",
                    "Handle null and empty input up front.",
                    "Use two-pointer swaps on a char array for O(n) complexity.",
                    "Rebuild the final string once at the end.",
                    "State complexity and mention Unicode surrogate caveats."),

                Drill(
                    "Live Drill: Validate Parentheses",
                    "csharp,live-coding,stack",
                    "Retype and then implement parentheses validation using a stack.",
                    "bool IsBalanced(string expression)",
                    "Push opening delimiters and pop when closing delimiters arrive.",
                    "Fail fast on mismatched pairs and early close characters.",
                    "End with an empty stack check.",
                    "Explain O(n) time and O(n) worst-case space."),

                Drill(
                    "Live Drill: Find Duplicates in Array",
                    "csharp,live-coding,collections",
                    "Retype and then implement duplicate detection with clear tradeoffs.",
                    "IReadOnlyList<int> FindDuplicates(int[] values)",
                    "Use HashSet for seen values and another set for duplicates.",
                    "Preserve output order only if the interviewer asks.",
                    "Discuss memory tradeoff versus sorting approach.",
                    "Handle null arrays with guard clauses."),

                Drill(
                    "Live Drill: FizzBuzz with Clean Code",
                    "csharp,live-coding,fizzbuzz",
                    "Retype and then implement clean FizzBuzz that is easy to read.",
                    "IEnumerable<string> BuildFizzBuzz(int start, int end)",
                    "Use clear naming and avoid nested if pyramids.",
                    "Extract rule evaluation into a tiny helper for testability.",
                    "Validate boundaries and return predictable output.",
                    "Mention divisibility rules and complexity succinctly."),

                Drill(
                    "Live Drill: Parse CSV Safely",
                    "csharp,live-coding,parsing",
                    "Retype and then implement a safe CSV parser for simple records.",
                    "IReadOnlyList<string[]> ParseCsv(string csv)",
                    "Handle quoted fields containing commas.",
                    "Preserve empty fields and trim only when business rules require.",
                    "Reject malformed quotes with clear error reporting.",
                    "Call out when a library like CsvHelper is preferable."),

                Drill(
                    "Live Drill: Implement Simple LRU Cache",
                    "csharp,live-coding,lru",
                    "Retype and then implement an LRU cache with dictionary plus linked list.",
                    "sealed class LruCache<TKey, TValue>",
                    "Use dictionary for O(1) key lookup and linked list for recency order.",
                    "Move accessed nodes to the head and evict from the tail.",
                    "Keep capacity validation strict to avoid invalid state.",
                    "Discuss thread-safety options after core solution is correct.")
            }
        };
    }

    private static SeedTopic BuildAspNetCoreInternalsPack()
    {
        return new SeedTopic
        {
            Name = "ASP.NET Core Internals",
            Category = ".NET",
            Difficulty = 4,
            Snippets = new List<SeedSnippet>
            {
                Outline(
                    "Request Pipeline: End-to-End Flow",
                    "aspnet,pipeline,internals",
                    "Walk through what happens from HTTP request to response.",
                    "Kestrel accepts the request and builds HttpContext.",
                    "Middleware executes in registration order on the way in.",
                    "Endpoint routing selects the matched handler.",
                    "Response unwinds back through middleware in reverse order."),

                Outline(
                    "Middleware Order Rules",
                    "aspnet,middleware,order",
                    "Explain why middleware ordering is critical.",
                    "UseRouting must run before endpoint execution.",
                    "UseAuthentication should run before UseAuthorization.",
                    "Exception handling should wrap as much of the pipeline as possible.",
                    "Static files should run early to bypass unnecessary work."),

                Outline(
                    "Endpoint Routing Internals",
                    "aspnet,routing,endpoints",
                    "Explain how endpoint routing selects actions.",
                    "Route templates are compiled and matched against incoming paths.",
                    "Constraints narrow matching and reduce ambiguous routes.",
                    "Route values feed model binding for action parameters.",
                    "Keep route design consistent for long-term API clarity."),

                Outline(
                    "DI Lifetimes: Transient, Scoped, Singleton",
                    "aspnet,di,lifetimes",
                    "Explain DI lifetimes with concrete request examples.",
                    "Transient creates a new instance each resolution.",
                    "Scoped reuses one instance per HTTP request.",
                    "Singleton lives for app lifetime and must be thread-safe.",
                    "Never capture scoped services inside singletons directly."),

                Outline(
                    "Dependency Injection Container Behavior",
                    "aspnet,di,container",
                    "Explain how the default container resolves object graphs.",
                    "Constructor injection is deterministic and explicit.",
                    "Circular dependencies are detected and should be redesigned.",
                    "Open generics are supported for patterns like repositories.",
                    "Prefer one composition root in Program.cs for registration clarity."),

                Outline(
                    "HTTP Idempotency",
                    "http,idempotency,rest",
                    "Define idempotent methods with practical examples.",
                    "GET, PUT, and DELETE are idempotent when designed correctly.",
                    "POST is not inherently idempotent unless keys or tokens enforce it.",
                    "Idempotency protects retries during network failures.",
                    "Document idempotency expectations for client teams."),

                Outline(
                    "PUT vs PATCH",
                    "http,put,patch",
                    "Clarify PUT and PATCH behavior in real APIs.",
                    "PUT usually replaces the full representation.",
                    "PATCH applies partial updates to selected fields.",
                    "Validate immutable fields regardless of update method.",
                    "Use ETags or row-version checks to avoid lost updates."),

                Outline(
                    "HTTP Status Codes with Real Meaning",
                    "http,status-codes,api",
                    "Explain status code choices beyond memorization.",
                    "200 for successful reads, 201 for resource creation.",
                    "204 for successful commands with no payload.",
                    "400 for client validation errors, 404 for missing resources.",
                    "500-level responses indicate server-side faults needing investigation."),

                Outline(
                    "REST vs RPC Tradeoffs",
                    "api,rest,rpc",
                    "Compare REST style APIs to RPC style endpoints.",
                    "REST models resources and standard HTTP semantics.",
                    "RPC models actions and can be simpler for command-heavy domains.",
                    "Consistency matters more than ideology in internal systems.",
                    "Align style with client needs and team operational maturity."),

                Outline(
                    "Model Binding: FromBody, FromQuery, FromRoute",
                    "aspnet,model-binding,api",
                    "Explain how ASP.NET Core binds request data to action parameters.",
                    "FromRoute maps URL segments to strongly typed parameters.",
                    "FromQuery binds query string filters and pagination settings.",
                    "FromBody deserializes JSON payloads for complex commands.",
                    "Use explicit attributes on public APIs to avoid ambiguity."),

                Outline(
                    "Validation Attributes",
                    "aspnet,validation,dataannotations",
                    "Explain validation attributes in DTO-driven APIs.",
                    "Use Required, StringLength, Range, and custom attributes for domain rules.",
                    "ModelState captures violations before business logic executes.",
                    "Return clear validation errors for fast client correction.",
                    "Keep DTO validation separate from persistence concerns."),

                Outline(
                    "FluentValidation Awareness",
                    "aspnet,validation,fluentvalidation",
                    "Summarize when FluentValidation is useful even if not required.",
                    "It centralizes validation logic in dedicated validator classes.",
                    "Rules are expressive for conditional and cross-field checks.",
                    "Works well when validation complexity outgrows data annotations.",
                    "Pick one validation approach per project for consistency."),

                Outline(
                    "Global Exception Handling",
                    "aspnet,exceptions,middleware",
                    "Explain centralized exception handling middleware.",
                    "Catch unhandled exceptions once and return standardized error payloads.",
                    "Log exception details with correlation metadata.",
                    "Avoid leaking stack traces to external clients.",
                    "Map known business exceptions to non-500 codes intentionally."),

                Outline(
                    "ILogger and Structured Logging",
                    "aspnet,logging,observability",
                    "Explain structured logging in production support workflows.",
                    "Use message templates with named fields, not string concatenation.",
                    "Log at appropriate levels to reduce noise and cost.",
                    "Include operation identifiers for traceability across services.",
                    "Treat logs as queryable telemetry, not plain text dumps."),

                Outline(
                    "Correlation IDs",
                    "aspnet,logging,correlation",
                    "Explain correlation IDs for distributed troubleshooting.",
                    "Generate or propagate one identifier per request chain.",
                    "Write it into logs, traces, and outbound HTTP headers.",
                    "Correlation drastically reduces mean time to resolve incidents.",
                    "Keep IDs stable across retries where possible."),

                Outline(
                    "Application Insights Basics",
                    "aspnet,application-insights,monitoring",
                    "Summarize practical App Insights usage.",
                    "Collect requests, dependencies, exceptions, and custom events.",
                    "Use sampling for high-traffic services to control costs.",
                    "Set alert thresholds on error rates and latency percentiles.",
                    "Tie telemetry to deployment versions for rollback decisions."),

                Outline(
                    "Options Pattern for Configuration",
                    "aspnet,configuration,options",
                    "Explain safe configuration binding with options objects.",
                    "Bind sections into strongly typed classes for compile-time guidance.",
                    "Validate critical settings at startup to fail fast.",
                    "Use IOptionsSnapshot for per-request freshness in web apps.",
                    "Store secrets outside source control and plain config files."),

                Drill(
                    "Live Drill: Basic CRUD Web API",
                    "aspnet,live-coding,webapi",
                    "Retype and then implement a basic CRUD API with clear status codes.",
                    "app.MapGroup(\"/api/items\")",
                    "Create DTOs and avoid exposing EF entities directly.",
                    "Use GET, GET by id, POST, PUT, and DELETE with predictable responses.",
                    "Validate inputs and return ProblemDetails for errors.",
                    "Add one integration test per endpoint before polishing."),

                Drill(
                    "Live Drill: Pagination Logic",
                    "aspnet,live-coding,pagination",
                    "Retype and then implement reusable pagination parameters and metadata.",
                    "Task<PagedResult<T>> QueryPageAsync(int page, int pageSize)",
                    "Clamp page size to protect the database from large scans.",
                    "Return total count, page number, and page size in the response.",
                    "Use stable ordering before Skip and Take.",
                    "Mention keyset pagination for very large datasets."),

                Outline(
                    "Hosted Services in ASP.NET Core",
                    "aspnet,background-services,hosted",
                    "Explain hosted services and safe background processing.",
                    "IHostedService starts and stops with the application host.",
                    "BackgroundService provides ExecuteAsync for long-running loops.",
                    "Use cancellation tokens and bounded queues to control workload.",
                    "Never block request threads waiting on background jobs.")
            }
        };
    }

    private static SeedTopic BuildAzureForCloudDevelopersPack()
    {
        return new SeedTopic
        {
            Name = "Azure for Cloud Developers",
            Category = "Azure",
            Difficulty = 4,
            Snippets = new List<SeedSnippet>
            {
                Outline(
                    "Regions vs Availability Zones",
                    "azure,regions,availability-zones",
                    "Explain regions and availability zones with resilience implications.",
                    "A region is a geographic area containing one or more datacenters.",
                    "Availability zones isolate power, cooling, and networking failures.",
                    "Zone-redundant services improve uptime for single-region failures.",
                    "Latency and data residency constraints still guide region choice."),

                Outline(
                    "SLA Math for Interviews",
                    "azure,sla,reliability",
                    "Show how to reason about SLA percentages.",
                    "99.9 percent allows about 43 minutes of monthly downtime.",
                    "99.99 percent allows about 4 minutes of monthly downtime.",
                    "Composite architecture SLA depends on weakest required component.",
                    "Design for graceful degradation, not SLA promises alone."),

                Outline(
                    "PaaS vs IaaS vs SaaS",
                    "azure,cloud-models,architecture",
                    "Compare service models with responsibility boundaries.",
                    "IaaS gives VM control but requires patching and base maintenance.",
                    "PaaS abstracts infrastructure so teams focus on application delivery.",
                    "SaaS provides complete software with minimal platform control.",
                    "Choose the highest abstraction that still meets requirements."),

                Outline(
                    "Shared Responsibility Model",
                    "azure,security,governance",
                    "Explain who secures what in cloud environments.",
                    "Cloud provider secures physical datacenters and core managed services.",
                    "Customer secures identity, data classification, and access design.",
                    "Responsibility shifts based on IaaS, PaaS, or SaaS model.",
                    "Assume breach and layer controls, logging, and least privilege."),

                Outline(
                    "Azure App Service Scaling",
                    "azure,app-service,scaling",
                    "Explain vertical and horizontal scaling in App Service.",
                    "Scale up changes instance size for more CPU and memory.",
                    "Scale out adds instances for concurrency and resilience.",
                    "Autoscale rules should follow CPU, queue depth, and response time.",
                    "Warm-up strategies reduce cold-start impact during scale events."),

                Outline(
                    "Deployment Slots",
                    "azure,app-service,deployment-slots",
                    "Explain deployment slots for safer releases.",
                    "Deploy to staging slot and validate before production swap.",
                    "Swap operation minimizes downtime for end users.",
                    "Use slot-specific settings for secrets and environment values.",
                    "Always verify database migration compatibility before swapping."),

                Outline(
                    "Configuration Settings and Secret Handling",
                    "azure,configuration,keyvault",
                    "Explain secure configuration in cloud-hosted apps.",
                    "Store secrets in Key Vault rather than code or plain config files.",
                    "Use managed identity to access Key Vault without embedded credentials.",
                    "Separate configuration by environment using naming conventions.",
                    "Rotate secrets regularly and audit access logs."),

                Outline(
                    "Managed Identity",
                    "azure,identity,managed-identity",
                    "Explain managed identity for service-to-service authentication.",
                    "Azure assigns an identity to the resource automatically.",
                    "Token retrieval happens via Azure metadata endpoints.",
                    "Eliminates hard-coded secrets in deployment pipelines.",
                    "Combine with RBAC for least-privilege resource access."),

                Outline(
                    "Service Principals",
                    "azure,identity,service-principal",
                    "Explain when a service principal is still needed.",
                    "Service principals represent apps or automation outside a single resource.",
                    "They can use certificates or client secrets for auth.",
                    "Secrets need rotation and secure storage discipline.",
                    "Prefer managed identity when compute is hosted in Azure."),

                Outline(
                    "RBAC in Azure",
                    "azure,security,rbac",
                    "Explain RBAC assignment strategy.",
                    "Assign roles at the narrowest scope that satisfies requirements.",
                    "Use built-in roles first, custom roles only when justified.",
                    "Separate deployment rights from runtime operational rights.",
                    "Review stale assignments regularly to reduce privilege creep."),

                Outline(
                    "Azure SQL: DTU vs vCore",
                    "azure,sql,dtu,vcore",
                    "Compare Azure SQL purchasing models.",
                    "DTU bundles compute, memory, and I/O in fixed ratios.",
                    "vCore gives clearer control and mapping to on-prem capacity planning.",
                    "Serverless can reduce cost for intermittent workloads.",
                    "Choose model based on workload predictability and cost governance."),

                Outline(
                    "Azure SQL Indexing and Query Plans",
                    "azure,sql,indexing,query-plan",
                    "Summarize indexing and plan analysis for Azure SQL.",
                    "Index selective columns used by filters and joins.",
                    "Inspect actual execution plans for scans, lookups, and spills.",
                    "Keep statistics up to date for reliable optimizer decisions.",
                    "Measure before adding indexes to avoid write amplification."),

                Outline(
                    "Parameter Sniffing Awareness",
                    "azure,sql,parameter-sniffing",
                    "Explain parameter sniffing in interview-safe terms.",
                    "A cached plan optimized for one parameter can hurt other values.",
                    "Symptoms include unstable latency across similar queries.",
                    "Mitigations include option recompile, optimize for, or query refactoring.",
                    "Investigate plan cache before applying broad fixes."),

                Outline(
                    "Blob Storage Tiers",
                    "azure,storage,blob",
                    "Explain Hot, Cool, and Archive storage tiers.",
                    "Hot tier suits frequently accessed data with higher storage cost.",
                    "Cool tier reduces storage cost for infrequent reads.",
                    "Archive offers lowest storage cost with retrieval delay penalties.",
                    "Lifecycle rules automate movement based on data age."),

                Outline(
                    "Table Storage vs Cosmos DB",
                    "azure,storage,cosmos,table",
                    "Compare Azure Table Storage and Cosmos DB for key-value workloads.",
                    "Table Storage is cost-effective with basic query capabilities.",
                    "Cosmos DB adds global distribution, SLAs, and rich APIs.",
                    "Choose based on latency guarantees, query needs, and scale.",
                    "Partition key design is critical in both systems."),

                Outline(
                    "SAS Tokens",
                    "azure,storage,sas,security",
                    "Explain Shared Access Signatures for delegated access.",
                    "SAS tokens grant scoped permissions for limited time windows.",
                    "Use least privilege and shortest practical expiration.",
                    "Prefer user-delegation SAS when possible for stronger security.",
                    "Log issuance patterns to detect abuse or leaks."),

                Outline(
                    "Storage Access Policies",
                    "azure,storage,policies",
                    "Explain stored access policies for SAS governance.",
                    "Policies centralize expiry and permissions for many SAS tokens.",
                    "You can revoke access by updating or deleting the policy.",
                    "Use naming conventions to map policies to business use cases.",
                    "Document emergency revocation procedures before incidents happen."),

                Outline(
                    "OAuth Basics for Cloud Apps",
                    "azure,oauth,identity",
                    "Explain OAuth roles and token flow basics.",
                    "Resource owner, client, authorization server, and resource server each play a role.",
                    "Access tokens represent delegated permissions for APIs.",
                    "Refresh tokens extend sessions with controlled rotation.",
                    "Use PKCE for public clients to reduce interception risk."),

                Outline(
                    "JWT Structure",
                    "azure,jwt,security",
                    "Explain JWT anatomy and validation steps.",
                    "JWT contains header, payload, and signature sections.",
                    "Validate issuer, audience, expiry, and signature before trust.",
                    "Do not store sensitive secrets in token payloads.",
                    "Short token lifetime limits blast radius after compromise."),

                Outline(
                    "Azure Monitoring Stack",
                    "azure,monitoring,application-insights",
                    "Summarize observability tools in Azure.",
                    "Application Insights collects app telemetry and dependencies.",
                    "Log Analytics supports KQL-based investigation and dashboards.",
                    "Alerts should target user impact signals, not only infrastructure metrics.",
                    "Connect monitoring to incident runbooks for faster response.")
            }
        };
    }

    private static SeedTopic BuildSqlForBackendEngineersPack()
    {
        return new SeedTopic
        {
            Name = "SQL for Backend Engineers",
            Category = "SQL",
            Difficulty = 4,
            Snippets = new List<SeedSnippet>
            {
                Outline(
                    "INNER JOIN vs LEFT JOIN",
                    "sql,joins,querying",
                    "Explain join semantics with practical use cases.",
                    "INNER JOIN returns only matching rows across both tables.",
                    "LEFT JOIN keeps all left-side rows even when right side is missing.",
                    "Use LEFT JOIN when missing related data still has business meaning.",
                    "Always validate row counts to catch accidental fan-out."),

                Outline(
                    "Clustered vs Nonclustered Index",
                    "sql,indexing,clustered",
                    "Explain index types and their storage implications.",
                    "Clustered index defines physical row order for the table.",
                    "Nonclustered index stores key plus row locator.",
                    "Tables have one clustered index but many nonclustered indexes.",
                    "Balance read speed gains against write overhead."),

                Outline(
                    "Composite Index Design",
                    "sql,indexing,composite",
                    "Explain how column order affects composite index usage.",
                    "Leftmost prefix rule determines which predicates can seek.",
                    "Place highly selective and common filter columns first.",
                    "Cover frequent query patterns with INCLUDE columns when useful.",
                    "Avoid redundant indexes with overlapping key patterns."),

                Outline(
                    "Normalization 1NF to 3NF",
                    "sql,normalization,data-modeling",
                    "Explain normalization goals for transactional databases.",
                    "1NF removes repeating groups and enforces atomic values.",
                    "2NF removes partial dependencies on composite keys.",
                    "3NF removes transitive dependencies for cleaner updates.",
                    "Denormalize intentionally only when performance data justifies it."),

                Outline(
                    "Transactions and ACID",
                    "sql,transactions,acid",
                    "Explain ACID properties in practical terms.",
                    "Atomicity means all statements commit or all roll back.",
                    "Consistency enforces valid transitions between states.",
                    "Isolation controls visibility of concurrent transaction changes.",
                    "Durability guarantees committed data survives failures."),

                Outline(
                    "Isolation Levels",
                    "sql,isolation,concurrency",
                    "Explain common isolation levels and anomalies.",
                    "Read Uncommitted can observe dirty data and is rarely acceptable.",
                    "Read Committed prevents dirty reads but not non-repeatable reads.",
                    "Repeatable Read and Serializable increase correctness with locking cost.",
                    "Snapshot isolation avoids locks for reads using row versioning."),

                Outline(
                    "Reading Execution Plans",
                    "sql,query-plan,performance",
                    "Explain how to use execution plans in troubleshooting.",
                    "Start with highest cost operators and cardinality estimates.",
                    "Look for table scans, key lookups, and hash spills.",
                    "Compare estimated versus actual rows to find misestimates.",
                    "Tune query shape before adding speculative indexes."),

                Outline(
                    "Parameterized Queries",
                    "sql,security,performance",
                    "Explain why parameterization matters.",
                    "Parameters prevent SQL injection in dynamic query scenarios.",
                    "They also improve plan cache reuse for stable workloads.",
                    "Use explicit parameter types to avoid implicit conversion scans.",
                    "Dynamic SQL should still validate allowed sort and filter fields."),

                Outline(
                    "Pagination: OFFSET/FETCH vs Keyset",
                    "sql,pagination,performance",
                    "Explain pagination patterns and tradeoffs.",
                    "OFFSET/FETCH is simple but slows on deep pages.",
                    "Keyset pagination scales better with stable sort keys.",
                    "Always enforce deterministic ORDER BY clauses.",
                    "Return pagination metadata so clients can navigate clearly."),

                Outline(
                    "Window Functions Basics",
                    "sql,window-functions,analytics",
                    "Explain when window functions outperform self-joins.",
                    "ROW_NUMBER supports ranking and paging scenarios.",
                    "SUM OVER can compute running totals without grouped collapse.",
                    "PARTITION BY scopes analytics per group.",
                    "Watch memory grants for wide partitions."),

                Outline(
                    "GROUP BY and HAVING",
                    "sql,aggregation,reporting",
                    "Explain GROUP BY and HAVING usage.",
                    "GROUP BY aggregates rows by selected dimensions.",
                    "HAVING filters aggregated groups after grouping step.",
                    "WHERE filters base rows before aggregation.",
                    "Use explicit aliases for readable reporting queries."),

                Outline(
                    "EXISTS vs IN",
                    "sql,subqueries,optimization",
                    "Compare EXISTS and IN for semi-join logic.",
                    "EXISTS short-circuits on first match and often performs well.",
                    "IN can be clear for small static sets.",
                    "Null semantics can differ and cause subtle bugs.",
                    "Benchmark both when optimizer choices vary."),

                Outline(
                    "MERGE and Upsert Safety",
                    "sql,upsert,merge",
                    "Explain safe upsert patterns and MERGE caveats.",
                    "MERGE can be concise but has edge-case behavior across versions.",
                    "Separate UPDATE then INSERT can be easier to reason about.",
                    "Use proper transaction isolation to avoid race conditions.",
                    "Validate unique constraints as final correctness guard."),

                Outline(
                    "Deadlocks and Mitigation",
                    "sql,deadlocks,concurrency",
                    "Explain deadlock basics and mitigation strategy.",
                    "Deadlocks occur when sessions wait on each other cyclically.",
                    "Keep transactions short and access tables in consistent order.",
                    "Use proper indexes to reduce lock duration and scope.",
                    "Capture deadlock graphs for root-cause analysis."),

                Outline(
                    "Covering Index Concept",
                    "sql,indexing,covering",
                    "Define covering indexes and when they help.",
                    "A covering index includes all columns needed by a query.",
                    "It avoids extra lookups to the base table.",
                    "Include non-key columns to reduce key width growth.",
                    "Only cover frequent performance-critical queries."),

                Outline(
                    "SARGable Predicates",
                    "sql,performance,sargable",
                    "Explain SARGability and index seek friendliness.",
                    "Wrap constants, not indexed columns, in expressions.",
                    "Avoid functions on indexed columns in WHERE clauses.",
                    "Use proper data types to avoid implicit conversions.",
                    "SARGable patterns improve both CPU and I/O profiles."),

                Outline(
                    "Foreign Keys and Cascading Rules",
                    "sql,relationships,integrity",
                    "Explain referential integrity for backend systems.",
                    "Foreign keys enforce valid parent-child relationships.",
                    "Cascading deletes simplify cleanup but can hide broad impacts.",
                    "Explicit delete workflows may be safer for critical domains.",
                    "Index foreign key columns for join and delete efficiency."),

                Outline(
                    "CTEs for Readability",
                    "sql,cte,maintainability",
                    "Explain common table expressions as a readability tool.",
                    "CTEs break complex logic into named intermediate steps.",
                    "Recursive CTEs solve hierarchy traversal patterns.",
                    "CTEs are logical query rewrites, not guaranteed materialization.",
                    "Use clear naming to communicate transformation intent."),

                Outline(
                    "Temp Tables vs Table Variables",
                    "sql,tempdb,performance",
                    "Compare temp tables and table variables.",
                    "Temp tables support statistics and often optimize better for larger sets.",
                    "Table variables can be fine for tiny row counts.",
                    "Choose based on cardinality and query complexity.",
                    "Measure tempdb pressure in high-concurrency workloads."),

                Outline(
                    "Backup, Restore, and PITR",
                    "sql,operations,backup",
                    "Explain backup strategy essentials for production databases.",
                    "Use full, differential, and log backups based on RPO goals.",
                    "Point-in-time restore depends on complete log chain integrity.",
                    "Regular restore drills validate backup quality.",
                    "Document recovery runbooks before incidents happen.")
            }
        };
    }

    private static SeedTopic BuildGitAndDevOpsEssentialsPack()
    {
        return new SeedTopic
        {
            Name = "Git & DevOps Essentials",
            Category = "DevOps",
            Difficulty = 3,
            Snippets = new List<SeedSnippet>
            {
                Outline(
                    "Rebase vs Merge",
                    "git,rebase,merge",
                    "Explain tradeoffs between rebase and merge workflows.",
                    "Merge preserves full branch history and context.",
                    "Rebase creates a linear history that is easier to scan.",
                    "Never rebase shared public branches without team agreement.",
                    "Choose strategy based on collaboration model and audit needs."),

                Outline(
                    "Fast-Forward Merges",
                    "git,merge,fast-forward",
                    "Define fast-forward merge behavior.",
                    "Fast-forward moves branch pointer when no divergent commits exist.",
                    "No merge commit is created, keeping history compact.",
                    "Disable fast-forward if you need explicit feature merge markers.",
                    "Use branch protection rules to enforce team policy."),

                Outline(
                    "Cherry-Pick Usage",
                    "git,cherry-pick,workflow",
                    "Explain when cherry-pick is appropriate.",
                    "Cherry-pick applies selected commits onto another branch.",
                    "Useful for urgent hotfix backports across release lines.",
                    "Track duplicated commits carefully to avoid future conflicts.",
                    "Prefer regular merges for long-lived feature synchronization."),

                Outline(
                    "Branch Strategy: GitFlow vs Trunk-Based",
                    "git,branching,strategy",
                    "Compare GitFlow and trunk-based development.",
                    "GitFlow uses long-lived release and develop branches.",
                    "Trunk-based favors short-lived branches and frequent integration.",
                    "Trunk-based reduces merge debt in high-delivery teams.",
                    "Pick strategy that matches release cadence and team discipline."),

                Outline(
                    "Squash Commits",
                    "git,history,squash",
                    "Explain why and when to squash commits.",
                    "Squashing consolidates noisy work-in-progress commits.",
                    "It creates cleaner history for future debugging.",
                    "Keep meaningful commit messages that explain intent and scope.",
                    "Avoid squashing if granular audit trail is required."),

                Outline(
                    "Professional Commit Messages",
                    "git,commits,collaboration",
                    "Explain what makes commit messages useful.",
                    "Start with a concise imperative summary line.",
                    "Describe why the change was needed, not only what changed.",
                    "Reference issue IDs for traceability.",
                    "Good commit messages reduce review and onboarding friction."),

                Outline(
                    "Pull Request Review Checklist",
                    "git,pr,code-review",
                    "Provide a disciplined PR checklist.",
                    "Verify behavior changes are covered by tests.",
                    "Check for breaking changes and migration impact.",
                    "Confirm logging and observability are adequate.",
                    "Reject hidden complexity that lacks business justification."),

                Outline(
                    "What CI Pipelines Actually Do",
                    "devops,ci,pipelines",
                    "Explain CI pipeline purpose to interviewers.",
                    "CI validates code integration continuously.",
                    "Typical stages: restore, build, test, package, publish artifacts.",
                    "Fast feedback prevents defects from compounding.",
                    "Pipeline failures should be treated as team-priority fixes."),

                Outline(
                    "YAML Pipeline Basics",
                    "devops,yaml,pipelines",
                    "Explain YAML pipeline structure.",
                    "Pipelines define stages, jobs, and steps declaratively.",
                    "Variables and templates reduce duplication.",
                    "Conditions allow branch- and environment-specific behavior.",
                    "Version pipeline files alongside code for auditability."),

                Outline(
                    "Build, Test, Deploy Flow",
                    "devops,ci-cd,flow",
                    "Explain canonical build-test-deploy progression.",
                    "Build compiles artifacts and runs static checks.",
                    "Test stage verifies unit and integration behavior.",
                    "Deploy promotes validated artifacts into target environments.",
                    "Immutable artifact promotion improves reproducibility."),

                Outline(
                    "Environment Promotion Strategy",
                    "devops,release,environments",
                    "Explain staged deployment from dev to production.",
                    "Promote the same artifact through QA, staging, and production.",
                    "Use environment-specific config, not rebuilt binaries.",
                    "Require approvals or automated quality gates for sensitive stages.",
                    "Track deployment metadata for incident rollback decisions."),

                Outline(
                    "Rollback and Forward-Fix Decisions",
                    "devops,incident,response",
                    "Explain rollback strategy during failed releases.",
                    "Rollback is fastest when reversible and low-risk.",
                    "Forward-fix may be better when schema changes cannot be reversed quickly.",
                    "Keep database migration strategy compatible with rollback plans.",
                    "Run post-incident review to prevent repeat failures."),

                Outline(
                    "Container vs Virtual Machine",
                    "docker,containers,vm",
                    "Explain architectural differences between containers and VMs.",
                    "Containers share host kernel and package app dependencies.",
                    "VMs virtualize full operating systems with larger overhead.",
                    "Containers start faster and improve deployment consistency.",
                    "VM isolation can still be preferred for certain workloads."),

                Outline(
                    "Dockerfile Layers",
                    "docker,dockerfile,layers",
                    "Explain why Dockerfile ordering matters.",
                    "Each instruction creates a cached image layer.",
                    "Place stable dependency restore steps before frequently changing code copy.",
                    "Smaller layers and fewer invalidations speed up CI builds.",
                    "Use multi-stage builds to reduce runtime image size."),

                Outline(
                    "Why Containers Are Portable",
                    "docker,portability,devops",
                    "Explain container portability claims accurately.",
                    "Images bundle runtime dependencies and startup commands.",
                    "Consistency reduces environment drift across dev and prod.",
                    "Portability still requires compatible host kernel and runtime support.",
                    "External dependencies like storage and networking remain environment-specific."),

                Outline(
                    "Using .dockerignore",
                    "docker,build,optimization",
                    "Explain why .dockerignore improves build quality.",
                    "Exclude bin, obj, git data, and secrets from build context.",
                    "Smaller context speeds docker build and reduces cache churn.",
                    "Prevent accidental credential leakage into images.",
                    "Review ignored paths when build behavior changes unexpectedly."),

                Outline(
                    "Secrets in CI/CD",
                    "devops,secrets,security",
                    "Explain secure secret handling in pipelines.",
                    "Store secrets in managed secret stores, not YAML files.",
                    "Use short-lived tokens where platform supports federation.",
                    "Mask secret values in logs and command output.",
                    "Rotate credentials after incident or role changes."),

                Outline(
                    "Artifact Versioning",
                    "devops,artifacts,release",
                    "Explain artifact versioning for traceability.",
                    "Tag builds with immutable version numbers and commit hashes.",
                    "Include build metadata in deployment records.",
                    "Never redeploy mutable artifacts under same version tag.",
                    "Versioning supports reproducible rollback and audit trails."),

                Outline(
                    "Blue-Green and Canary Releases",
                    "devops,deployment,canary",
                    "Compare blue-green and canary deployment patterns.",
                    "Blue-green switches traffic between two full environments.",
                    "Canary gradually shifts traffic to detect issues early.",
                    "Both approaches require strong observability and rollback automation.",
                    "Choose based on risk tolerance and infrastructure cost."),

                Outline(
                    "Postmortems and Learning Culture",
                    "devops,incident,postmortem",
                    "Explain high-quality incident postmortems.",
                    "Focus on system causes, not individual blame.",
                    "Capture timeline, impact, contributing factors, and action items.",
                    "Track remediation work to completion with clear ownership.",
                    "Use recurring incident patterns to prioritize platform investments.")
            }
        };
    }

    private static SeedTopic BuildInterviewBehavioralMasteryPack()
    {
        return new SeedTopic
        {
            Name = "Interview Behavioral Mastery",
            Category = "Interview",
            Difficulty = 3,
            Snippets = new List<SeedSnippet>
            {
                Outline(
                    "Explain a Difficult Bug You Solved",
                    "interview,behavioral,storytelling",
                    "Retype this as a reusable story skeleton for tough bug questions.",
                    "Situation: state the production impact in one sentence.",
                    "Task: define what success looked like and your ownership scope.",
                    "Action: describe diagnosis steps, hypotheses, and the fix.",
                    "Result: quantify impact and include one lesson learned."),

                Outline(
                    "Tell Me About a Time You Led Under Pressure",
                    "interview,leadership,star",
                    "Structure a high-pressure leadership answer with clear signal.",
                    "Set mission context and stakes without excessive background.",
                    "Explain how you set priorities and delegated under time pressure.",
                    "Highlight communication rhythm you used to align the team.",
                    "Close with measurable outcome and follow-up improvements."),

                Outline(
                    "How Do You Handle Production Incidents?",
                    "interview,incident,response",
                    "Build a concise incident response answer.",
                    "First stabilize user impact, then investigate root cause.",
                    "Use clear incident roles: commander, communications, investigators.",
                    "Maintain timeline notes for later postmortem accuracy.",
                    "Translate incident learning into prevention work items."),

                Outline(
                    "STAR Story: Iraq/Afghanistan Leadership Transfer",
                    "interview,veteran,star",
                    "Translate military leadership into engineering language.",
                    "Situation: describe mission constraints and team composition.",
                    "Task: map military objective to operational engineering equivalent.",
                    "Action: highlight decision-making, risk management, and communication.",
                    "Result: show measurable mission success and transferable skill."),

                Outline(
                    "Translate Military Leadership to Engineering",
                    "interview,veteran,transition",
                    "Explain military experience in a technical team context.",
                    "Mission planning maps to sprint planning and risk registers.",
                    "After-action reviews map to blameless retrospectives.",
                    "Chain-of-command clarity maps to ownership and escalation paths.",
                    "Emphasize collaboration style, not command-and-control stereotypes."),

                Outline(
                    "Explaining PTSD-Related Gaps Professionally",
                    "interview,veteran,career-gap",
                    "Keep gap explanation concise, honest, and future-focused.",
                    "State that you addressed health and readiness responsibly.",
                    "Avoid oversharing medical details in interview settings.",
                    "Pivot quickly to current performance, training, and momentum.",
                    "Practice wording until delivery feels calm and confident."),

                Outline(
                    "Answering Salary Expectations Confidently",
                    "interview,negotiation,salary",
                    "Prepare a direct salary expectation response.",
                    "Anchor to market range, role scope, and location factors.",
                    "Signal flexibility based on total compensation and growth path.",
                    "Avoid giving extreme low numbers that undervalue your skills.",
                    "End by reaffirming interest in fit and impact."),

                Outline(
                    "Why You Are Pivoting to Tech",
                    "interview,transition,motivation",
                    "Craft a pivot narrative that sounds deliberate, not reactive.",
                    "Connect prior mission-driven work to engineering problem-solving.",
                    "Show concrete training and shipped projects as proof.",
                    "Explain why software scale multiplies your impact.",
                    "Keep tone forward-looking and grounded in execution."),

                Outline(
                    "Conflict Resolution Story",
                    "interview,teamwork,conflict",
                    "Prepare a conflict story that demonstrates maturity.",
                    "Describe disagreement on approach, not personal friction.",
                    "Show how you listened, reframed goals, and proposed experiments.",
                    "Document evidence used to settle the decision.",
                    "Conclude with relationship outcome and team learning."),

                Outline(
                    "Ownership Story",
                    "interview,ownership,impact",
                    "Build an ownership answer that proves accountability.",
                    "Pick a case where you owned ambiguity end-to-end.",
                    "Show proactive risk identification and stakeholder updates.",
                    "Explain tradeoffs you made and why.",
                    "Quantify final impact with a metric."),

                Outline(
                    "Failure and Recovery Story",
                    "interview,growth,mindset",
                    "Prepare a failure story that demonstrates growth.",
                    "Choose a real failure with meaningful consequence.",
                    "Take responsibility without blaming teammates.",
                    "Explain corrective actions and systemic changes.",
                    "Share how later outcomes improved because of that lesson."),

                Outline(
                    "Mentoring Story",
                    "interview,mentorship,leadership",
                    "Describe mentoring impact in concrete terms.",
                    "Identify baseline skill gap and learning goal.",
                    "Explain cadence: pairing, feedback loops, and checkpoints.",
                    "Show measurable growth in delivery quality or confidence.",
                    "Mention how mentoring improved your own technical clarity."),

                Outline(
                    "Prioritizing Under Ambiguity",
                    "interview,prioritization,execution",
                    "Explain your prioritization framework under uncertainty.",
                    "Rank work by user impact, risk, and reversibility.",
                    "Break unknowns into fast validation tasks.",
                    "Communicate assumptions and decision deadlines clearly.",
                    "Re-prioritize transparently as new evidence arrives."),

                Outline(
                    "Communicating Tradeoffs to Non-Technical Stakeholders",
                    "interview,communication,stakeholders",
                    "Retype a tradeoff explanation framework.",
                    "Start with business outcome rather than implementation detail.",
                    "Present two options with cost, risk, and timeline.",
                    "Recommend one option and explain why now.",
                    "Confirm decision and ownership in writing after meetings."),

                Outline(
                    "Receiving Feedback Professionally",
                    "interview,feedback,growth",
                    "Prepare a response for feedback-related interview prompts.",
                    "Acknowledge feedback without defensiveness.",
                    "Ask clarifying examples to understand behavior impact.",
                    "Define concrete behavior changes and follow-up checkpoints.",
                    "Show evidence that the change improved outcomes."),

                Outline(
                    "Asking Clarifying Questions in Interviews",
                    "interview,problem-solving,communication",
                    "Demonstrate strong discovery behavior before solving.",
                    "Clarify constraints, scale, and success criteria first.",
                    "Confirm assumptions aloud to reduce rework.",
                    "Prioritize edge cases by likelihood and impact.",
                    "Explain your plan before writing code."),

                Outline(
                    "Live Coding Communication Script",
                    "interview,live-coding,communication",
                    "Use this script while coding under observation.",
                    "Narrate approach, complexity expectations, and test plan.",
                    "Call out tradeoffs before committing to one implementation.",
                    "Pause to validate direction when requirements are ambiguous.",
                    "End by summarizing complexity and potential improvements."),

                Outline(
                    "Questions to Ask the Interviewer",
                    "interview,questions,fit",
                    "Prepare thoughtful reverse-interview questions.",
                    "Ask how success is measured in the first six months.",
                    "Ask about current technical debt and improvement roadmap.",
                    "Ask how incidents are handled and learned from.",
                    "Ask what differentiates top performers on the team."),

                Outline(
                    "30-60-90 Day Plan Response",
                    "interview,onboarding,plan",
                    "Frame your first 90 days with practical milestones.",
                    "First 30: learn domain, systems, and team workflows.",
                    "Next 30: ship scoped improvements and close feedback loops.",
                    "Final 30: own a measurable initiative with cross-team alignment.",
                    "Show how you balance learning speed with delivery quality."),

                Outline(
                    "Confident Closing Statement",
                    "interview,closing,impact",
                    "Retype a concise closing for final interview moments.",
                    "Reinforce role fit with one technical and one behavioral strength.",
                    "Mention excitement about the team problem space specifically.",
                    "Offer to provide additional detail on any concern area.",
                    "Thank the panel and restate your intent to contribute quickly.")
            }
        };
    }

    private static SeedTopic BuildSystemDesignForJuniorEngineersPack()
    {
        return new SeedTopic
        {
            Name = "System Design for Junior Engineers",
            Category = "Architecture",
            Difficulty = 4,
            Snippets = new List<SeedSnippet>
            {
                Outline(
                    "Monolith vs Microservices",
                    "system-design,architecture,services",
                    "Explain monolith and microservice tradeoffs without hype.",
                    "Monoliths simplify deployment and local debugging early on.",
                    "Microservices improve team autonomy at the cost of operational complexity.",
                    "Distributed systems require stronger observability and reliability patterns.",
                    "Start with modular monolith unless clear scaling boundaries demand split."),

                Outline(
                    "Vertical vs Horizontal Scaling",
                    "system-design,scaling,capacity",
                    "Explain scaling choices with cost implications.",
                    "Vertical scaling increases resources on a single node.",
                    "Horizontal scaling adds more nodes for concurrency and resilience.",
                    "Stateful systems are harder to scale horizontally without partitioning.",
                    "Use load testing data to justify scaling decisions."),

                Outline(
                    "Caching Strategies",
                    "system-design,caching,performance",
                    "Explain caching layers and invalidation concerns.",
                    "Use in-memory cache for hot local lookups with short lifetimes.",
                    "Use distributed cache for cross-instance consistency.",
                    "Choose cache-aside or write-through based on update patterns.",
                    "Cache invalidation must be explicit and observable."),

                Outline(
                    "Rate Limiting Basics",
                    "system-design,rate-limiting,api",
                    "Explain why APIs need rate limiting.",
                    "Protect shared resources from abuse and accidental overload.",
                    "Token bucket and sliding window are common algorithms.",
                    "Limit by user, API key, or IP depending on trust boundaries.",
                    "Return clear retry guidance with rate-limit response headers."),

                Outline(
                    "Load Balancers",
                    "system-design,load-balancer,network",
                    "Explain load balancer responsibilities.",
                    "Distribute traffic across healthy instances.",
                    "Perform health checks and remove failing backends.",
                    "L7 balancers can route by host and path.",
                    "Session affinity should be avoided unless state demands it."),

                Outline(
                    "API Gateway Concept",
                    "system-design,api-gateway,edge",
                    "Describe API gateway value in multi-service systems.",
                    "Centralize authentication, throttling, and request shaping.",
                    "Reduce client complexity with one entry point.",
                    "Avoid overloading gateway with deep business logic.",
                    "Keep fallback paths for gateway outages in critical systems."),

                Outline(
                    "Database Indexing Basics",
                    "system-design,database,indexing",
                    "Explain indexing from an application engineer perspective.",
                    "Indexes speed reads by reducing scan work.",
                    "Too many indexes slow writes and increase storage cost.",
                    "Index columns used in WHERE, JOIN, and ORDER BY paths.",
                    "Use query plans to validate index effectiveness."),

                Outline(
                    "Read Replicas and Write Primary",
                    "system-design,database,replication",
                    "Explain primary-replica topology basics.",
                    "Writes go to primary node to preserve authoritative ordering.",
                    "Reads can be offloaded to replicas for scale.",
                    "Replication lag can cause stale reads.",
                    "Choose consistency model based on business tolerance."),

                Outline(
                    "Eventual Consistency Awareness",
                    "system-design,consistency,distributed",
                    "Explain eventual consistency clearly for interviews.",
                    "Distributed writes may converge over time instead of instantly.",
                    "Design UI and workflows to handle temporary stale state.",
                    "Use idempotent consumers and retry logic for asynchronous flows.",
                    "Document consistency guarantees for client teams."),

                Outline(
                    "Queue-Based Async Processing",
                    "system-design,queues,async",
                    "Explain when queues improve system resilience.",
                    "Queues decouple producers from slower consumers.",
                    "Buffering absorbs burst traffic and smooths processing.",
                    "Consumers need idempotency and dead-letter handling.",
                    "Monitor queue depth and age as health indicators."),

                Outline(
                    "Pagination in System Design",
                    "system-design,pagination,api",
                    "Explain pagination approaches for large datasets.",
                    "Offset pagination is simple but degrades on deep pages.",
                    "Cursor or keyset pagination scales better with stable ordering.",
                    "Return continuation tokens for stateless client navigation.",
                    "Define maximum page size to protect backend resources."),

                Outline(
                    "ID Generation Strategies",
                    "system-design,ids,data-modeling",
                    "Explain common ID generation approaches.",
                    "Database identity columns are simple in single-region systems.",
                    "GUIDs support decentralization but can fragment clustered indexes.",
                    "Time-sortable IDs can improve ordering and sharding behavior.",
                    "Choose strategy based on scale, ordering, and uniqueness needs."),

                Outline(
                    "Retries with Exponential Backoff",
                    "system-design,retries,resilience",
                    "Explain safe retry behavior for transient failures.",
                    "Use exponential backoff with jitter to reduce synchronized retries.",
                    "Retry only idempotent operations unless compensated.",
                    "Cap maximum attempts to avoid runaway load.",
                    "Log retry context for downstream troubleshooting."),

                Outline(
                    "Circuit Breaker Pattern",
                    "system-design,resilience,circuit-breaker",
                    "Explain circuit breaker states and purpose.",
                    "Closed state allows normal calls until failure threshold is reached.",
                    "Open state fails fast to protect the system.",
                    "Half-open state probes recovery with limited traffic.",
                    "Pair breakers with fallback behavior and observability."),

                Outline(
                    "Observability Pillars",
                    "system-design,observability,operations",
                    "Summarize logs, metrics, and traces for production systems.",
                    "Logs provide rich event context for investigation.",
                    "Metrics reveal trends and alerting signals.",
                    "Traces map request flow across service boundaries.",
                    "Use all three for fast mean-time-to-diagnosis."),

                Outline(
                    "Why Small Methods Matter",
                    "clean-code,architecture,maintainability",
                    "Explain small methods as a maintainability strategy.",
                    "Small methods reduce cognitive load during code review.",
                    "They improve test granularity and failure localization.",
                    "Extract by business intent, not arbitrary line count.",
                    "Too many tiny wrappers can hide flow if overdone."),

                Outline(
                    "Naming Conventions that Scale",
                    "clean-code,naming,readability",
                    "Explain naming conventions in large teams.",
                    "Names should reveal intent, unit, and context clearly.",
                    "Avoid abbreviations unless domain-standard and obvious.",
                    "Consistency beats personal preference in shared codebases.",
                    "Review names during PRs as part of design quality."),

                Outline(
                    "Guard Clauses and Null Checking",
                    "clean-code,guard-clauses,null-checks",
                    "Explain guard clauses for defensive programming.",
                    "Fail fast on invalid inputs at method boundaries.",
                    "Flatten nested conditionals for clearer happy path.",
                    "Use ArgumentNullException and domain-specific validation errors.",
                    "Guard clauses improve reliability and debugging speed."),

                Outline(
                    "DRY vs Over-Abstraction",
                    "clean-code,dry,abstraction",
                    "Explain DRY as elimination of knowledge duplication.",
                    "Do not abstract prematurely for coincidental similarities.",
                    "Duplicate until patterns stabilize, then extract intentionally.",
                    "Over-abstraction can increase coupling and reduce clarity.",
                    "Prioritize readability over clever generic frameworks."),

                Outline(
                    "YAGNI in Real Projects",
                    "clean-code,yagni,delivery",
                    "Explain YAGNI with practical engineering consequences.",
                    "Build only what current requirements demand.",
                    "Speculative features increase maintenance and test burden.",
                    "Keep architecture extensible without implementing unused paths.",
                    "Use metrics and roadmap certainty before adding complexity."),

                Outline(
                    "Thread Safety: lock vs Monitor",
                    "csharp,thread-safety,concurrency",
                    "Explain synchronization primitives for shared state.",
                    "lock is the idiomatic wrapper around Monitor enter/exit.",
                    "Monitor provides advanced try-enter and timeout control.",
                    "Keep critical sections short to reduce contention.",
                    "Avoid locking on publicly accessible objects."),

                Outline(
                    "Concurrent Collections",
                    "csharp,concurrency,collections",
                    "Explain when to use concurrent collections.",
                    "ConcurrentDictionary supports thread-safe key-value operations.",
                    "BlockingCollection helps producer-consumer workflows.",
                    "ConcurrentQueue and ConcurrentBag fit different access patterns.",
                    "Thread safety does not remove need for invariant design."),

                Outline(
                    "Parallel.ForEach Awareness",
                    "csharp,parallelism,performance",
                    "Explain safe use of Parallel.ForEach.",
                    "Best for CPU-bound independent work items.",
                    "Avoid shared mutable state inside loop bodies.",
                    "Measure overhead because small workloads can run slower.",
                    "Use cancellation tokens for responsive shutdown."),

                Outline(
                    "Background Services Pattern",
                    "aspnet,background-services,architecture",
                    "Explain background service architecture in ASP.NET Core.",
                    "Use BackgroundService for long-running worker loops.",
                    "Inject scoped dependencies through IServiceScopeFactory.",
                    "Persist checkpoints to resume safely after restarts.",
                    "Design graceful shutdown with cancellation and flush logic.")
            }
        };
    }

    private static SeedSnippet Outline(string title, string tags, string prompt, params string[] points)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Interview prompt:");
        builder.AppendLine(prompt);
        builder.AppendLine();
        builder.AppendLine("Retype checklist:");

        foreach (var point in points)
        {
            builder.AppendLine($"- {point}");
        }

        return Snippet(title, tags, builder.ToString());
    }

    private static SeedSnippet Drill(string title, string tags, string objective, string starterSignature, params string[] steps)
    {
        var builder = new StringBuilder();
        builder.AppendLine("Live coding drill:");
        builder.AppendLine(objective);
        builder.AppendLine();
        builder.AppendLine("Starter signature:");
        builder.AppendLine(starterSignature);
        builder.AppendLine();
        builder.AppendLine("Implementation steps:");

        for (var i = 0; i < steps.Length; i++)
        {
            builder.AppendLine($"{i + 1}. {steps[i]}");
        }

        return Snippet(title, tags, builder.ToString());
    }

    private static SeedSnippet Snippet(string title, string tags, string sourceText)
    {
        return new SeedSnippet
        {
            Title = title,
            Tags = tags,
            SourceText = Normalize(sourceText)
        };
    }

    private static string Normalize(string text)
    {
        return text.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
    }
}

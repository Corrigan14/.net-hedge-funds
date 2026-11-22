# .NET and quant tutoring handoff

Last updated: 2026-11-22. Read this file and inspect current source before continuing.
This is a portable learning checkpoint, not a transcript or proof that every input works.

## Purpose and curriculum

Be my private C#/.NET tutor, preparing me for senior .NET interviews with a focus
on hedge fund and quant engineering. I am currently a C# beginner.
Use `docs/100 .NET & Quant Engineering Katas.pdf` as the curriculum, one kata at a time.
We are on Kata 1: a zero-allocation FIX field parser.

## Instructions for the next chat

- Explain one small topic at a time. Ask whether I want more explanation before
  moving to another topic or coding. Do not deliver a large lecture or jump ahead.
- Before every coding task, explain the relevant C# syntax/APIs and business context.
- At the beginning of each kata, list the .NET learning goals and explain the business concepts.
- Prefer TDD: I write and run one test, observe the result, then make the smallest
  implementation change. Coverage for existing behavior can already be green;
  explain that honestly rather than manufacturing a failure.
- I write implementations, tests, and measurements. Give questions and progressively
  more specific hints; provide full solutions only when I ask.
- Preserve my code during reviews. Edit only when explicitly requested; project
  setup is allowed when requested. Inspect current files before judging progress.
- Review test names and assertions as well as implementation. Explain properties
  versus methods and other beginner syntax as it arises.
- Keep feedback direct, patient, and evidence-based. Light dry engineering humor
  is welcome; do not mistake passing tests for proof of universal correctness.
- We switch back and forth between computers and chats. Treat this file as the
  shared learning checkpoint: read it at the start of each new chat and always
  keep it current after meaningful progress, decisions, measurements, or plan
  changes, without waiting for a separate request. Updating this file is authorized;
  that does not authorize edits to my implementation or tests.
- Before ending a working session or handing over to another computer, record the
  exact next step, any unfinished work, and checks actually run. Keep completed
  work, planned work, learner-reported results, and tutor-verified results distinct.
  Do not mark work done merely because it was suggested in chat.
- Before continuing on either computer, pull the latest committed changes and
  inspect the local working tree. If local changes prevent a clean update, preserve
  them and resolve the discrepancy rather than overwriting work. Commit and push
  the updated checkpoint with the relevant work when authorized; never assume an
  uncommitted or unpushed update is available on the other computer.

## Repository and commands

- Remote: `git@github.com:Corrigan14/.net-hedge-funds.git` (HTTPS is also possible;
  inspect `git remote -v` for the actual configuration on each machine).
- Solution: `LearnNet.sln`; projects target `net10.0`.
- Parser: `Examples/ExampleOne.cs`.
- Tests: `Examples.Tests/ExampleOneTests.cs` (xUnit).
- Benchmark runner: `Examples.Benchmarks/Program.cs`.
- Run tests from the repository root: `dotnet test Examples.Tests/Examples.Tests.csproj`.
- Build benchmarks: `dotnet build Examples.Benchmarks/Examples.Benchmarks.csproj -c Release`.
- Once a benchmark exists, run it with:
  `dotnet run --project Examples.Benchmarks/Examples.Benchmarks.csproj -c Release`.
- Git transfers this file only after it is committed and pushed; pull it on the
  other machine. Credentials and local paths are machine-specific. Never put
  tokens or private keys in this file or the repository.

## Current API and scope

```csharp
bool TryGetValue(ReadOnlySpan<byte> fixMessage, int tag, out ReadOnlySpan<byte> value)
```

We use simplified pipe-delimited input: `8=FIX.4.4|9=145|35=D|`.
This is a field lookup, not a complete FIX protocol/session validator. A present
empty value returns `true` with an empty span; an absent tag returns `false` with
an empty span. Business validation of a required value is separate.

The parser scans fields, parses numeric tags with `Utf8Parser.TryParse`, requires
the entire tag to be consumed, and returns a slice of the existing input bytes.
The value slice was corrected to:

```csharp
field.Slice(startIndex + 1, field.Length - (startIndex + 1))
```

## Completed checklist

- [x] Discuss spans as views, `Slice(start, length)`, `IndexOf`, and advancing fields.
- [x] Discuss the `bool`/`out` pattern and complete numeric-tag parsing.
- [x] Explain `u8`, UTF-8 bytes, string encoding, properties versus methods,
  `SequenceEqual`, and expected/actual assertion order.
- [x] Fix the successful-lookup slice bounds.
- [x] Consolidate functional cases into one theory: absent tag, later tag lookup,
  longer value, single-field lookup, and present empty value (five cases).
- [x] Add a separate malformed-field theory: missing `=`, non-numeric tag,
  partially numeric tag, missing tag, missing closing pipe, and empty field (six cases).
- [x] Tutor ran these 11 cases successfully before the allocation test was added.
- [x] Explain allocated versus retained memory and the current-thread allocation counter.
- [x] Learner wrote a 10,000-lookup allocation test and reported it green after
  moving `Encoding.UTF8.GetBytes` outside the measured loop.
- [x] Diagnose the original 480,000-byte result as measurement of repeated encoding
  plus parsing; 48 bytes per iteration was consistent with temporary byte arrays.
- [x] Introduce SOH (Small Object Heap), LOH, stack, CPU, and span backing memory.
  Learner understands that slicing does not allocate another array. Detailed GC
  generations and CPU/cache effects have not yet been taught.
- [x] Add `Examples.Benchmarks` to the solution, reference `Examples`, install
  BenchmarkDotNet 0.15.8, and configure an assembly-discovering benchmark runner.
- [x] Verify the benchmark project builds in Release with zero warnings/errors.

## Current measurement evidence and limits

The allocation test creates the parser and encoded byte array before measurement,
warms up with one lookup, and measures 10,000 synchronous lookups for tag 35.
It asserts a zero allocation delta. The learner reported this green; the tutor
has not rerun that allocation test at this checkpoint. A 1,000-call warm-up was
suggested earlier but is NOT implemented in the inspected source.

There are now 12 test cases in source (11 theory rows and one allocation fact).
No BenchmarkDotNet measurement class/method, MemoryDiagnoser, timing result, or
benchmark allocation result exists yet. The runner alone is setup, not a benchmark.

## Next steps

- [ ] Start here: explain one benchmark invocation, `[Benchmark]`, and
  `[MemoryDiagnoser]` briefly, with the business goal of measuring lookup cost
  without charging each lookup for message encoding/setup. Pause for questions.
- [ ] Guide the learner to write one benchmark class and one measured lookup.
  Reuse prepared parser/input; keep allocations for setup outside the method.
  Make the method's result observable (for example, return the lookup's boolean).
- [ ] Learner runs the benchmark in Release without a debugger; help interpret
  mean time, variation, and allocated bytes per operation. Record actual evidence.
- [ ] Revisit the allocation test's warm-up if needed; do not silently edit it.
- [ ] Discuss measurement limits: one input/path, microbenchmark versus real
  message processing, and why zero allocation does not guarantee low latency.
- [ ] Teach SOH generations/GC, LOH, and CPU/cache implications in separate small
  lessons when the learner wants them, rather than blocking the first benchmark.
- [ ] Agree whether Kata 1 is complete before starting Kata 2 from the PDF.

## Deferred decisions and coverage gaps

- Mixed malformed/valid messages are not covered. Current behavior stops on a
  missing `=`, skips non-numeric tags, and returns immediately when a tag matches.
- Therefore `35=D|broken|` can return a value without examining the malformed tail.
  No policy for whole-message rejection has been implemented or agreed.
- Learner suggested rejecting malformed FIX in a business application. We discussed
  separating full-message validation from field lookup; do not silently expand scope.
- Empty input is not explicitly tested. Complete FIX framing, SOH byte delimiters,
  body length, checksum, required fields, and session handling are outside current scope.

## Resume prompt

> Read handof.md and inspect the current parser, tests, and benchmark project.
> Continue as my tutor from the next unchecked step. Explain one topic at a time,
> let me write the code, and preserve my files unless I explicitly request edits.

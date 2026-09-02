# Poker Chips

Calculates how to divide a case of poker chips evenly among a group of players for a target
starting stack size. Given the chip denominations and amounts available in the case, it computes
how many chips of each denomination each player should receive so every player's stack is as close
as possible to (but never exceeding) the requested target value.

The core chip-allocation algorithm exists in two parallel, functionally identical implementations:

- **C#** (`PokerChips_Library`), used by the Windows Forms application (`PokerChips_Windows`).
- **JavaScript** (`PokerChips_Web`), used by the standalone web page version.

## Solution layout

| Project / folder | Description |
|---|---|
| [PokerChips_Library](PokerChips_Library) | Class library containing the chip-allocation solver (`ChipCalculator`, `PlayerChipsHelper`, `Chip`). See its own [README](PokerChips_Library/README.md) for a detailed write-up of the algorithm. |
| [PokerChips_Windows](PokerChips_Windows) | Windows Forms front end (`MainForm`, `ResultForm`) that lets you enter the case contents, player count, and target stack size, and displays the resulting chip distribution, including a per-denomination and grand total value. |
| [PokerChips_Web](PokerChips_Web) | A dependency-free HTML/JavaScript port (`index.html` / `en/index.html`) with the same solver logic (`PokerChipsSolver.js`) and UI logic (`PokerChips.js`), usable directly in a browser without any build step or server. |
| [TestProject1](TestProject1) | MSTest unit tests (`ChipCalculatorTests`) covering the C# solver, including edge cases such as zero-value chips and invalid input. |

`PokerChips_Windows.slnx` is the Visual Studio solution file tying the C#/.NET projects together;
the web version (`PokerChips_Web`) is not part of the solution and can be opened directly in a
browser.

## The algorithm

Chip allocation is a classic **bounded knapsack** problem: each chip denomination may be used
between 0 and a capped number of times per player, and the solver looks for the achievable sum per
player that is as close as possible to (but not exceeding) the target stack value. See
[PokerChips_Library/README.md](PokerChips_Library/README.md) for a full explanation of the
dynamic-programming approach, including the GCD-based table-shrinking optimization and the
tie-breaking rules used to prefer smaller-denomination chips.

Both the C# (`ChipCalculator.cs`) and JavaScript (`PokerChipsSolver.js`) implementations mirror
each other line-for-line and are expected to produce identical results for the same inputs. The
JavaScript solver can be independently verified against the same test cases used by the C# test
suite via `node PokerChips_Web/verify-solver.js`.

Both implementations reject a case chip that has an amount greater than 0 but a value of 0, since
that combination can never be a legitimate case chip: a value of 0 is only ever expected for
unused/empty case-chip color slots, which always have an amount of 0 too. Both front ends
(Windows Forms and the web UI) validate this before ever calling into the solver.

## Building and running

- Open `PokerChips_Windows.slnx` in Visual Studio (or run `dotnet build`/`dotnet test` from the
  repository root) to build the class library, the Windows Forms app, and the test project.
- Open `PokerChips_Web/index.html` (German) or `PokerChips_Web/en/index.html` (English) directly in
  a browser to use the web version; no build step required.

## Testing

- **C#**: run the `TestProject1` MSTest suite (via Visual Studio's Test Explorer or `dotnet test`).
- **JavaScript**: run `node PokerChips_Web/verify-solver.js` to check the JS solver against the
  same scenarios covered by the C# tests.

## Legacy

[PokerChips_Mobile](PokerChips_Mobile) is an old Windows Mobile / Pocket PC 2003 front end
(`.NET Compact Framework` v2.0, `PlatformFamilyName: PocketPC`), predating today's
`PokerChips_Library`/`PokerChips_Windows` split. It is **not** part of `PokerChips_Windows.slnx`
and is kept only for historical reference; it is not actively built, tested, or maintained.

Notable differences from the current codebase:

- It has its own self-contained, duplicated copy of the chip/amount/value model and allocation
  logic (`Chip.cs`, `MainForm.cs`) instead of depending on `PokerChips_Library` - the shared
  library did not exist yet at the time.
- Naming is in German rather than English (e.g. `Chip.Wert`/`Chip.Anzahl` instead of
  `Chip.Value`/`Chip.Amount`), and the UI/allocation code lives directly in `MainForm.cs` rather
  than being split out into a dedicated solver class.
- It targets the historical `.NET Compact Framework` (`TargetFrameworkVersion v2.0`) via the old,
  non-SDK-style `.csproj` format, rather than the modern SDK-style projects (targeting
  `.NET Framework 4.7.2`/`.NET 10`) used elsewhere in this repository.

If you need to run or modify it, it requires the (long-discontinued) Windows Mobile/Pocket PC
development tooling and is unlikely to build with current versions of Visual Studio.
